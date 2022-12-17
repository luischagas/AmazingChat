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
public class MessageController : BaseController
{
    private readonly ILogger<MessageController> _logger;
    private readonly IMessageService _messageService;

    public MessageController(ILogger<MessageController> logger, IMessageService messageService)
    {
        _logger = logger;
        _messageService = messageService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<MessageViewModel>>> Get()
    {
        var result = await _messageService.GetAll();

        var messages = result as AppServiceResponse<List<MessageViewModel>>;

        return Ok(messages?.Data);
    }

    [HttpGet("Room/{roomName}")]
    public async Task<IActionResult> GetMessages(string roomName)
    {
        var result = await _messageService.GetByRoom(roomName);

        if (result.Success is false)
            return NotFound();

        var messages = result as AppServiceResponse<List<MessageViewModel>>;

        return Ok(messages?.Data);
    }

    [HttpPost]
    public async Task<ActionResult<Room>> Create([FromBody] MessageViewModel messageViewModel)
    {
        var result = await _messageService.Create(messageViewModel);

        if (result.Success is false)
            return BadRequest();

        var messageCreated = result as AppServiceResponse<MessageViewModel>;

        return CreatedAtAction(nameof(Get), new { id = messageCreated?.Data.Id }, messageViewModel);
    }
}