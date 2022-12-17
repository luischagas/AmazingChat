namespace AmazingChat.Domain.Shared.Notifications;

public interface INotifier
{
    List<Notification> GetAllNotifications();

    void Handle(Notification notification);

    bool HasNotification();
}