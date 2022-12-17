using AmazingChat.Domain.Shared.Notifications;

namespace AmazingChat.Domain.Notification;

public class Notifier : INotifier
{
    #region Fields

    private List<Shared.Notifications.Notification> _notifications;

    #endregion Fields

    #region Constructors

    public Notifier()
    {
        _notifications = new List<Shared.Notifications.Notification>();
    }

    #endregion Constructors

    #region Methods

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

    #endregion Methods
}