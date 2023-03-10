using AmazingChat.Application.Common;
using AmazingChat.Application.Interfaces;
using AmazingChat.Application.Models;
using AmazingChat.Domain.Entities;
using AmazingChat.Domain.Interfaces.Repositories;
using AmazingChat.Domain.Shared.Notifications;
using AmazingChat.Domain.Shared.Services;
using AmazingChat.Domain.Shared.UnitOfWork;
using AmazingChat.Infra.CrossCutting.Services.SignalR;

namespace AmazingChat.Application.Services;

public class RoomService : AppService, IRoomService
{
    private readonly IRoomRepository _roomRepository;
    private readonly IChatHub _hub;

    public RoomService(IUnitOfWork unitOfWork,
        INotifier notifier,
        IRoomRepository roomRepository,
        IChatHub hub)
        : base(unitOfWork, notifier)
    {
        _roomRepository = roomRepository;
        _hub = hub;
    }


    public async Task<IAppServiceResponse> Create(RoomViewModel request)
    {
        var existentRoom = await _roomRepository.GetByName(request.Name);

        if (existentRoom is not null)
        {
            Notify("Rooms", "Room existent");

            return await Task.FromResult(new AppServiceResponse<ICollection<Notification>>(GetAllNotifications(), "Error to Create Room", false));
        }

        var room = new Room(request.Name);

        if (room.IsValid())
        {
            await _roomRepository.AddAsync(room);
        }
        else
        {
            Notify(room.ValidationResult);

            return await Task.FromResult(new AppServiceResponse<ICollection<Notification>>(GetAllNotifications(), "Error to Create Room", false));
        }

        if (await CommitAsync())
        {
            await _hub.CreateRoom(room.Id, room.Name);

            return await Task.FromResult(new AppServiceResponse<RoomViewModel>(new RoomViewModel { Id = room.Id, Name = room.Name }, "Room Created Successfully", true));
        }


        return await Task.FromResult(new AppServiceResponse<ICollection<Notification>>(GetAllNotifications(), "Error Creating Order", false));
    }

    public async Task<IAppServiceResponse> GetAll()
    {
        var rooms = await _roomRepository.GetAllAsync();

        var roomsViewModel = new List<RoomViewModel>();

        foreach (var room in rooms)
        {
            roomsViewModel.Add(new RoomViewModel
            {
                Id = room.Id,
                Name = room.Name
            });
        }

        if (roomsViewModel.Any() is false)
            return await Task.FromResult(new AppServiceResponse<List<RoomViewModel>>(roomsViewModel, "No Rooms Found", true));

        return await Task.FromResult(new AppServiceResponse<List<RoomViewModel>>(roomsViewModel, "Rooms obtained successfully", true));
    }

    public async Task<IAppServiceResponse> Get(Guid id)
    {
        var room = await _roomRepository.GetAsync(id);

        if (room is null)
        {
            Notify("Rooms", "Room not found");

            return await Task.FromResult(new AppServiceResponse<ICollection<Notification>>(GetAllNotifications(), "Error to obtain Room", false));
        }

        var roomViewModel = new RoomViewModel
        {
            Id = room.Id,
            Name = room.Name
        };

        return await Task.FromResult(new AppServiceResponse<RoomViewModel>(roomViewModel, "Room obtained successfully", true));
    }

    public async Task<IAppServiceResponse> Remove(Guid id)
    {
        var room = await _roomRepository.GetAsync(id);

        if (room is null)
        {
            Notify("Rooms", "Room not found");

            return await Task.FromResult(new AppServiceResponse<ICollection<Notification>>(GetAllNotifications(), "Error to Remove Room", false));
        }

        if (room.IsValid())
        {
            _roomRepository.Delete(room);
        }
        else
        {
            Notify(room.ValidationResult);

            return await Task.FromResult(new AppServiceResponse<ICollection<Notification>>(GetAllNotifications(), "Error to Remove Room", false));
        }

        if (await CommitAsync())
        {
            await _hub.RemoveRoom(room.Id, room.Name);
            
            return await Task.FromResult(new AppServiceResponse<RoomViewModel>(new RoomViewModel { Id = room.Id, Name = room.Name }, "Room Removed Successfully", true));
        }

        return await Task.FromResult(new AppServiceResponse<ICollection<Notification>>(GetAllNotifications(), "Error to Remove Room", false));
    }
}