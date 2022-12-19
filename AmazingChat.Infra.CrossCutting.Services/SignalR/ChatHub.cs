using AmazingChat.Domain.Interfaces.Repositories;
using AmazingChat.Domain.Shared.Models.SignalR;
using AmazingChat.Domain.Shared.Services;
using AmazingChat.Domain.Shared.UnitOfWork;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;

namespace AmazingChat.Infra.CrossCutting.Services.SignalR;

[Authorize]
public class ChatHub : Hub, IChatHub
{
    private readonly IServiceProvider _serviceProvider;

    private readonly IUserRepository _userRepository;

    private readonly IUnitOfWork _unitOfWork;

    protected IHubContext<ChatHub> _context;
    private string ConnectionId => Context.ConnectionId;

    private static readonly List<UserModel> _connections = new();

    private static readonly Dictionary<string, string> _connectionsMap = new();

    public ChatHub(IServiceProvider serviceProvider, IHubContext<ChatHub> context)
    {
        _serviceProvider = serviceProvider;
        _context = context;
        _userRepository = _serviceProvider.GetRequiredService<IUserRepository>();
        _unitOfWork = _serviceProvider.GetRequiredService<IUnitOfWork>();
    }

    public async Task SendMessage(MessageModel messageModel)
    {
        try
        {
            if (messageModel is not null)
                await _context.Clients.Group(messageModel.Room).SendAsync("newMessage", messageModel);
        }
        catch (Exception ex)
        {
            await _context.Clients.All.SendAsync("onError", "You failed to create a message" + ex.Message);
        }
    }

    public async Task SendInfoMessage(MessageModel messageModel)
    {
        try
        {
            if (messageModel is not null)
                await _context.Clients.Group(messageModel.Room).SendAsync("onInfoMessage", messageModel.Message);
        }
        catch (Exception ex)
        {
            await _context.Clients.All.SendAsync("onError", "You failed to send a info message!" + ex.Message);
        }
    }

    public async Task Join(string roomName)
    {
        try
        {
            var user = _connections.FirstOrDefault(u => u.ConnectionId == ConnectionId);

            if (user != null && user.CurrentRoom != roomName)
            {
                if (!string.IsNullOrEmpty(user.CurrentRoom))
                    await Clients.OthersInGroup(user.CurrentRoom).SendAsync("removeUser", user);

                await Leave(user.CurrentRoom);
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

    public async Task Leave(string roomName)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, roomName);
    }

    public IEnumerable<UserModel> GetUsers(string roomName)
    {
        return _connections.Where(u => u.CurrentRoom == roomName).ToList();
    }

    public override async Task OnConnectedAsync()
    {
        try
        {
            var user = await _userRepository.GetByEmail(Context.User?.Identity?.Name ?? "");

            user.UpdateConnectionId(ConnectionId);

            await _unitOfWork.CommitAsync();

            var userViewModel = new UserModel
            {
                Email = user.Email,
                ConnectionId = user.ConnectionId,
                CurrentRoom = ""
            };

            if (_connections.Any(u => u.ConnectionId == ConnectionId) is false)
            {
                _connections.Add(userViewModel);
                _connectionsMap.Add(Context.ConnectionId, userViewModel.Email);
            }

            await Clients.Caller.SendAsync("getProfileInfo", user.Email);
        }
        catch (Exception ex)
        {
            await Clients.Caller.SendAsync("onError", "OnConnected:" + ex.Message);
        }

        await base.OnConnectedAsync();
    }

    public override Task OnDisconnectedAsync(Exception exception)
    {
        try
        {
            var user = _connections.First(u => u.ConnectionId == ConnectionId);

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

    public async Task CreateRoom(Guid roomId, string roomName)
    {
        try
        {
            await _context.Clients.All.SendAsync("addChatRoom", new { id = roomId, name = roomName });
        }
        catch (Exception ex)
        {
            await _context.Clients.All.SendAsync("onError", "You failed to create the chat room!" + ex.Message);
        }
    }

    public async Task RemoveRoom(Guid roomId, string roomName)
    {
        try
        {
            await _context.Clients.All.SendAsync("removeChatRoom", roomId);
            await _context.Clients.Group(roomName).SendAsync("onRoomDeleted", string.Format("Room {0} has been deleted!", roomName));
        }
        catch (Exception ex)
        {
            await _context.Clients.All.SendAsync("onError", "You failed to remove the chat room!" + ex.Message);
        }
    }
}