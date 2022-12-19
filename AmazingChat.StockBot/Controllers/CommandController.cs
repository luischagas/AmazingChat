using System.Net;
using AmazingChat.Application.Interfaces;
using AmazingChat.Application.Models;
using Microsoft.AspNetCore.Mvc;

namespace AmazingChat.StockBot.Controllers;

[Route("api/[controller]")]
public class CommandController : Controller
{
    private readonly IStockBotService _stockBotService;

    public CommandController(IStockBotService stockBotService)
    {
        _stockBotService = stockBotService;
    }

    [HttpPost]
    public async Task<IActionResult> ProcessCommand([FromBody] CommandViewModel request)
    {
        var result = await _stockBotService.ProcessCommand(request);

        if (result.Success is false)
            return GenerateResponse(HttpStatusCode.BadRequest, result);

        return GenerateResponse(HttpStatusCode.OK, result);
    }

    private IActionResult GenerateResponse(HttpStatusCode statusCode, object result)
    {
        return StatusCode((int) statusCode, result);
    }
}