namespace AmazingChat.Infra.CrossCutting.Services.SignalR.Models;

public class UserModel
{
    public string Email { get; set; }
    public string ConnectionId { get; set; }
    public string CurrentRoom { get; set; }
}