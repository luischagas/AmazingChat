using System.Text.RegularExpressions;
using AmazingChat.Application.Models;
using AmazingChat.Domain.Interfaces.Repositories;
using AmazingChat.Infra.Data.Context;
using Microsoft.AspNetCore.SignalR;

namespace AmazingChat.Infra.CrossCutting.Services.SignalR;

public class ChatHub : Hub
    {
        private readonly IUserRepository _userRepository;
        private string Connectionid => Context.ConnectionId;

        private static readonly List<UserModel> _connections = new();
        
        private static readonly Dictionary<string, string> _connectionsMap = new();

        public ChatHub(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task SendMessage(string receiverName, string message)
        {
            if (_connectionsMap.TryGetValue(receiverName, out string userId))
            {
                var sender = _connections.First(u => u.ConnectionId == Connectionid);

                if (!string.IsNullOrEmpty(message.Trim()))
                {
                    var messageViewModel = new MessageModel()
                    {
                        Message = Regex.Replace(message, @"<.*?>", string.Empty),
                        User = sender.Name,
                        Room = "",
                        Timestamp = DateTime.Now
                    };
                    
                    await Clients.Client(userId).SendAsync("newMessage", messageViewModel);
                    await Clients.Caller.SendAsync("newMessage", messageViewModel);
                }
            }
        }

        public async Task JoinRoom(string roomName)
        {
            try
            {
                var user = _connections.FirstOrDefault(u => u.ConnectionId == Connectionid);
                
                if (user != null && user.CurrentRoom != roomName)
                {
                    if (!string.IsNullOrEmpty(user.CurrentRoom))
                        await Clients.OthersInGroup(user.CurrentRoom).SendAsync("removeUser", user);
                    
                    await LeaveRoom(user.CurrentRoom);
                    await Groups.AddToGroupAsync(Context.ConnectionId, roomName);
                    user.CurrentRoom = roomName;
                    
                    await Clients.OthersInGroup(roomName).SendAsync("addUser", user);
                }
            }
            catch (Exception ex)
            {
                await Clients.Caller.SendAsync("onError", "You failed to join the chat room!" + ex.Message);
            }
        }

        public async Task LeaveRoom(string roomName)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, roomName);
        }

        public IEnumerable<UserModel> GetUsers(string roomName)
        {
            return _connections.Where(u => u.CurrentRoom == roomName).ToList();
        }

        public override Task OnConnectedAsync()
        {
            try
            {
                var user =  _userRepository.GetByConnectionId(Connectionid).GetAwaiter().GetResult();

                var userViewModel = new UserModel
                {
                    Name = user.Name,
                    ConnectionId = user.ConnectionId,
                    CurrentRoom = ""
                };

                if (_connections.Any(u => u.ConnectionId == Connectionid) is false)
                {
                    _connections.Add(userViewModel);
                    _connectionsMap.Add(userViewModel.Name, Context.ConnectionId);
                }

                Clients.Caller.SendAsync("user", user.Name, user.ConnectionId);
            }
            catch (Exception ex)
            {
                Clients.Caller.SendAsync("onError", "OnConnected:" + ex.Message);
            }
            
            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
            try
            {
                var user = _connections.First(u => u.ConnectionId == Connectionid);
                
                _connections.Remove(user);
                
                Clients.OthersInGroup(user.CurrentRoom).SendAsync("removeUser", user);

                _connectionsMap.Remove(user.ConnectionId);
            }
            catch (Exception ex)
            {
                Clients.Caller.SendAsync("onError", "OnDisconnected: " + ex.Message);
            }

            return base.OnDisconnectedAsync(exception);
        }
    }