using System.Text.RegularExpressions;
using AmazingChat.Application.Common;
using AmazingChat.Application.Interfaces;
using AmazingChat.Application.Models;
using AmazingChat.Domain.Entities;
using AmazingChat.Domain.Interfaces.Notifier;
using AmazingChat.Domain.Interfaces.Repositories;
using AmazingChat.Domain.Notification;
using AmazingChat.Domain.Shared;
using AmazingChat.Infra.CrossCutting.Services.SignalR;
using AmazingChat.Infra.CrossCutting.Services.SignalR.Models;
using Microsoft.AspNetCore.Http;

namespace AmazingChat.Application.Services;

public class MessageService : AppService, IMessageService
{
    #region Fields

    private readonly IRoomRepository _roomRepository;
    private readonly IRoomMessageRepository _roomMessageRepository;
    private readonly IUserRepository _userRepository;
    private readonly IHttpContextAccessor _httpContextAccessor;


    private readonly ChatHub _hubContext;

    #endregion

    #region Constructors

    public MessageService(IUnitOfWork unitOfWork,
        INotifier notifier,
        IRoomRepository roomRepository,
        ChatHub hubContext,
        IRoomMessageRepository roomMessageRepository,
        IUserRepository userRepository,
        IHttpContextAccessor httpContextAccessor)
        : base(unitOfWork, notifier)
    {
        _roomRepository = roomRepository;
        _hubContext = hubContext;
        _roomMessageRepository = roomMessageRepository;
        _userRepository = userRepository;
        _httpContextAccessor = httpContextAccessor;
    }

    #endregion Constructors

    #region Public Methods

    public async Task<IAppServiceResponse> Create(MessageViewModel request)
    {
        var user = _httpContextAccessor.HttpContext?.User;

        var userData = await _userRepository.GetByEmail(user?.Identity?.Name ?? "");

        if (userData == null)
        {
            Notify("Users", "User not found");

            return await Task.FromResult(new AppServiceResponse<ICollection<Notification>>(GetAllNotifications(), "Error to create Message", false));
        }

        var room = await _roomRepository.GetByName(request.Room);

        if (room == null)
        {
            Notify("Rooms", "Room not found");

            return await Task.FromResult(new AppServiceResponse<ICollection<Notification>>(GetAllNotifications(), "Error to create Message", false));
        }

        var message = new RoomMessage(Regex.Replace(request.Message, @"<.*?>", string.Empty), room.Id, userData.Id);

        if (message.IsValid())
        {
            await _roomMessageRepository.AddAsync(message);

            await _hubContext.SendMessage(userData.ConnectionId, new MessageModel { Id = message.Id, Message = message.Message, Room = room.Name, Timestamp = message.Timestamp, User = userData.Email });
        }
        else
        {
            Notify(room.ValidationResult);

            return await Task.FromResult(new AppServiceResponse<ICollection<Notification>>(GetAllNotifications(), "Error to Create Message", false));
        }

        if (await CommitAsync())
            return await Task.FromResult(new AppServiceResponse<MessageViewModel>(new MessageViewModel { Id = message.Id, Message = message.Message, Timestamp = message.Timestamp, Room = room.Name, User = userData.Email }, "Message Created Successfully", true));

        return await Task.FromResult(new AppServiceResponse<ICollection<Notification>>(GetAllNotifications(), "Error Creating Message", false));
    }

    public async Task<IAppServiceResponse> GetAll()
    {
        var messages = await _roomMessageRepository.GetAllDetailedAsync();

        var messagesViewModel = new List<MessageViewModel>();

        foreach (var message in messages)
        {
            messagesViewModel.Add(new MessageViewModel
            {
                Id = message.Id,
                Message = message.Message,
                Timestamp = message.Timestamp,
                Room = message.Room.Name,
                User = message.User.Email
            });
        }

        if (messagesViewModel.Any() is false)
            return await Task.FromResult(new AppServiceResponse<List<MessageViewModel>>(messagesViewModel, "No Messages Found", true));

        return await Task.FromResult(new AppServiceResponse<List<MessageViewModel>>(messagesViewModel, "Messages obtained successfully", true));
    }

    public async Task<IAppServiceResponse> GetByRoom(string roomName)
    {
        var room = await _roomRepository.GetByName(roomName);

        if (room == null)
        {
            Notify("Rooms", "Room not found");

            return await Task.FromResult(new AppServiceResponse<ICollection<Notification>>(GetAllNotifications(), "Error to obtain Room", false));
        }

        var messages = await _roomMessageRepository.GetAllByRoomAsync(room.Id);

        var messagesViewModel = new List<MessageViewModel>();

        foreach (var message in messages)
        {
            messagesViewModel.Add(new MessageViewModel
            {
                Id = message.Id,
                Message = message.Message,
                Timestamp = message.Timestamp,
                Room = message.Room.Name,
                User = message.User.Email
            });
        }

        return await Task.FromResult(new AppServiceResponse<List<MessageViewModel>>(messagesViewModel, "Messages obtained successfully", true));
    }

    #endregion Public Methods
}