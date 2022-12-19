using AmazingChat.Domain.Shared.Notifications;

namespace AmazingChat.Domain.Notification;

public class Notifier : INotifier
{
    private readonly List<Shared.Notifications.Notification> _notifications;


    public Notifier()
    {
        _notifications = new List<Shared.Notifications.Notification>();
    }


    public List<Shared.Notifications.Notification> GetAllNotifications()
    {
        return _notifications;
    }

    public void Handle(Shared.Notifications.Notification notificacao)
    {
        _notifications.Add(notificacao);
    }

    public bool HasNotification()
    {
        return _notifications.Any();
    }
}