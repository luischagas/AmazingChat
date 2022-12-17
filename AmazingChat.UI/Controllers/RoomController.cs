using AmazingChat.Application.Common;
using AmazingChat.Application.Interfaces;
using AmazingChat.Application.Models;
using AmazingChat.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AmazingChat.UI.Controllers;

[Authorize]
[Route("[controller]")]
[ApiController]
public class RoomController : BaseController
{
    private readonly ILogger<RoomController> _logger;
    private readonly IRoomService _roomService;

    public RoomController(ILogger<RoomController> logger, IRoomService roomService)
    {
        _logger = logger;
        _roomService = roomService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<RoomViewModel>>> Get()
    {
        var result = await _roomService.GetAll();

        var rooms = result as AppServiceResponse<List<RoomViewModel>>;

        return Ok(rooms?.Data);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Room>> Get(Guid id)
    {
        var result = await _roomService.Get(id);

        if (result.Success is false)
            return NotFound();

        var room = result as AppServiceResponse<RoomViewModel>;

        return Ok(room?.Data);
    }

    [HttpPost]
    public async Task<ActionResult<Room>> Create([FromBody] RoomViewModel roomViewModel)
    {
        var result = await _roomService.Create(roomViewModel);

        if (result.Success is false)
            return BadRequest();

        var roomCreated = result as AppServiceResponse<RoomViewModel>;

        return CreatedAtAction(nameof(Get), new { id = roomCreated?.Data.Id }, new { id = roomCreated?.Data.Id, name = roomCreated?.Data.Name });
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await _roomService.Remove(id);

        if (result.Success is false)
            return BadRequest();

        return Ok();
    }
}