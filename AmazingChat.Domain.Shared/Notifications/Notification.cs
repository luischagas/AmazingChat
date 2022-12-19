namespace AmazingChat.Domain.Shared.Notifications;

public class Notification
{
    public Notification(string key, string message)
    {
        Key = key;
        Message = message;
    }
    
    public string Key { get; }

    public string Message { get; }
}