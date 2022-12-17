namespace AmazingChat.Infra.CrossCutting.Services.SignalR.Models;

public class MessageModel
{
    public Guid Id { get; set; }
    public string? Message { get; set; }
    public DateTime Timestamp { get; set; }
    public string? User { get; set; }
    public string? Room { get; set; }
}