namespace AmazingChat.Domain.Shared.Models.SignalR;

public class MessageModel
{
    public Guid Id { get; set; }
    public string? Message { get; set; }
    public DateTime Timestamp { get; set; }
    public string? User { get; set; }
    public string? Room { get; set; }
}