using System.Text.RegularExpressions;
using AmazingChat.Application.Common;
using AmazingChat.Application.Interfaces;
using AmazingChat.Application.Models;
using AmazingChat.Domain.Entities;
using AmazingChat.Domain.Enums;
using AmazingChat.Domain.Interfaces.Repositories;
using AmazingChat.Domain.Shared.Models;
using AmazingChat.Domain.Shared.Notifications;
using AmazingChat.Domain.Shared.Services;
using AmazingChat.Domain.Shared.UnitOfWork;
using AmazingChat.Infra.CrossCutting.Services.SignalR;
using AmazingChat.Infra.CrossCutting.Services.SignalR.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using RestSharp;

namespace AmazingChat.Application.Services;

public class MessageService : AppService, IMessageService
{
    private readonly IRoomRepository _roomRepository;
    private readonly IRoomMessageRepository _roomMessageRepository;
    private readonly IUserRepository _userRepository;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly SignalRConfigurations _signalRConfigurations;
    private readonly ICommunicationRestService _communicationRestService;
    private readonly ChatHub _hubContext;
    
    public MessageService(IUnitOfWork unitOfWork,
        INotifier notifier,
        IRoomRepository roomRepository,
        ChatHub hubContext,
        IRoomMessageRepository roomMessageRepository,
        IUserRepository userRepository,
        IHttpContextAccessor httpContextAccessor,
        IOptions<SignalRConfigurations> signalRConfigurations,
        ICommunicationRestService communicationRestService)
        : base(unitOfWork, notifier)
    {
        _roomRepository = roomRepository;
        _hubContext = hubContext;
        _roomMessageRepository = roomMessageRepository;
        _userRepository = userRepository;
        _httpContextAccessor = httpContextAccessor;
        _communicationRestService = communicationRestService;
        _signalRConfigurations = signalRConfigurations.Value;
    }


    public async Task<IAppServiceResponse> Create(MessageViewModel request)
    {
        var email = string.IsNullOrEmpty(request.User) is false ? request.User : _httpContextAccessor.HttpContext?.User?.Identity?.Name;

        var userData = await _userRepository.GetByEmail(email ?? "");

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

        if (message.Message.Contains('/'))
        {
            var resultProcessCommand = await ProcessCommandMessage(message, room.Name, userData.Email);

            if (resultProcessCommand.result)
                return await Task.FromResult(new AppServiceResponse<MessageViewModel>(new MessageViewModel { Id = message.Id, Message = message.Message, Timestamp = message.Timestamp, Room = room.Name, User = userData.Email }, "Command Sent Successfully", true));

            if (resultProcessCommand.type == ETypeErrorProcessCommandMessage.ErrorProcess)
                return await Task.FromResult(new AppServiceResponse<ICollection<Notification>>(GetAllNotifications(), "Error to process Command", false));

            return await Task.FromResult(new AppServiceResponse<ICollection<Notification>>(GetAllNotifications(), "Error Sending Command", false));
        }

        if (message.IsValid())
        {
            if (userData.Email == _signalRConfigurations.StockUser)
            {
                await _hubContext.SendMessage(new MessageModel { Id = message.Id, Message = message.Message, Room = room.Name, Timestamp = message.Timestamp, User = userData.Email });

                return await Task.FromResult(new AppServiceResponse<MessageViewModel>(new MessageViewModel { Id = message.Id, Message = message.Message, Timestamp = message.Timestamp, Room = room.Name, User = userData.Email }, "Message Created Successfully", true));
            }

            await _roomMessageRepository.AddAsync(message);
        }
        else
        {
            Notify(room.ValidationResult);

            return await Task.FromResult(new AppServiceResponse<ICollection<Notification>>(GetAllNotifications(), "Error to Create Message", false));
        }

        if (await CommitAsync())
        {
            await _hubContext.SendMessage(new MessageModel { Id = message.Id, Message = message.Message, Room = room.Name, Timestamp = message.Timestamp, User = userData.Email });

            return await Task.FromResult(new AppServiceResponse<MessageViewModel>(new MessageViewModel { Id = message.Id, Message = message.Message, Timestamp = message.Timestamp, Room = room.Name, User = userData.Email }, "Message Created Successfully", true));
        }


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

    private async Task<(bool result, ETypeErrorProcessCommandMessage type)> ProcessCommandMessage(RoomMessage message, string roomName, string userEmail)
    {
        var messageModel = new MessageModel { Id = message.Id, Room = roomName, Timestamp = message.Timestamp, User = userEmail };

        var commandAllowed = false;

        foreach (var allowedCommand in _signalRConfigurations.AllowedCommands)
        {
            var regex = new Regex($"(?<={allowedCommand}).*").Matches(message.Message);

            if (regex.Any())
                commandAllowed = true;
        }

        if (commandAllowed)
        {
            messageModel.Message = "Wait! Command in processing";

            await _hubContext.SendInfoMessage(messageModel);

            var splitCommand = message.Message.Split("=");

            var result = _communicationRestService.SendRequest(_signalRConfigurations.UrlStockBot, "Command", Method.Post, new CommandViewModel
            {
                Room = roomName,
                Command = splitCommand[1]
            });

            if (result.IsSuccessful)
                return (true, ETypeErrorProcessCommandMessage.Success);

            messageModel.Message = "Error to process Command";

            await _hubContext.SendInfoMessage(messageModel);

            return (false, ETypeErrorProcessCommandMessage.ErrorProcess);
        }

        messageModel.Message = "Command Invalid";

        await _hubContext.SendInfoMessage(messageModel);

        return (true, ETypeErrorProcessCommandMessage.CommandInvalid);
    }
}