namespace AmazingChat.Domain.Interfaces.Notifier;

public interface INotifier
{
    List<Domain.Notification.Notification> GetAllNotifications();

    void Handle(Domain.Notification.Notification notification);

    bool HasNotification();
}