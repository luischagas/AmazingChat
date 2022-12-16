using AmazingChat.Application.Models;
using AmazingChat.Domain.Entities;
using AmazingChat.Domain.Interfaces.Repositories;
using AmazingChat.Infra.CrossCutting.Services.SignalR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace AmazingChat.UI.Controllers;

public class RoomController : BaseController
{
    private readonly ILogger<RoomController> _logger;
    private readonly IHubContext<ChatHub> _hubContext;
    private readonly IRoomRepository _roomRepository;
    private readonly IUserRepository _userRepository;

    public RoomController(ILogger<RoomController> logger, IHubContext<ChatHub> hubContext, IRoomRepository roomRepository, IUserRepository userRepository)
    {
        _logger = logger;
        _hubContext = hubContext;
        _roomRepository = roomRepository;
        _userRepository = userRepository;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<RoomViewModel>>> Get()
    {
        var rooms =  await _roomRepository.GetAllAsync();

        var roomsViewModel = new List<RoomViewModel>();

        foreach (var room in rooms)
        {
            roomsViewModel.Add(new RoomViewModel()
            {
                Id = room.Id,
                Name = room.Name
            });
        }

        return Ok(roomsViewModel);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Room>> Get(Guid id)
    {
        var room = await _roomRepository.GetAsync(id);
        
        if (room is null)
            return NotFound();

        var roomViewModel = new RoomViewModel()
        {
            Id = room.Id,
            Name = room.Name
        };
        
        return Ok(roomViewModel);
    }

    [HttpPost]
    public async Task<ActionResult<Room>> Create(RoomViewModel roomViewModel)
    {
        var existentRoom = await _roomRepository.GetByName(roomViewModel.Name);
         
        if (existentRoom is not null)
            return BadRequest("Invalid room name or room already exists");

        var room = new Room(roomViewModel.Name);

        await _roomRepository.AddAsync(room);

        await _hubContext.Clients.All.SendAsync("addChatRoom", new { id = room.Id, name = room.Name });

        return CreatedAtAction(nameof(Get), new { id = room.Id }, new { id = room.Id, name = room.Name });
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Edit(Guid id, RoomViewModel roomViewModel)
    {
        var room = await _roomRepository.GetByName(roomViewModel.Name);
        
        if (room is null)
            return NotFound();
        
        if (room.Name == roomViewModel.Name && room.Id != id)
            return BadRequest("Invalid room name or room already exists");
        
        room.UpdateName(roomViewModel.Name);
        
        _roomRepository.Update(room);

        await _hubContext.Clients.All.SendAsync("updateChatRoom", new { id = room.Id, room.Name });

        return Ok();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var room = await _roomRepository.GetAsync(id);

        if (room == null)
            return NotFound();

        _roomRepository.Delete(room);

        await _hubContext.Clients.All.SendAsync("removeChatRoom", room.Id);
        await _hubContext.Clients.Group(room.Name).SendAsync("onRoomDeleted", string.Format("Room {0} has been deleted.\nYou are moved to the first available room!", room.Name));

        return Ok();
    }
}