using AmazingChat.Domain.Shared.Notifications;
using AmazingChat.Domain.Shared.UnitOfWork;
using FluentValidation.Results;

namespace AmazingChat.Application;

public abstract class AppService
{
    private readonly INotifier _notifier;

    private readonly IUnitOfWork _unitOfWork;


    public AppService(IUnitOfWork unitOfWork, INotifier notifier)
    {
        _unitOfWork = unitOfWork;
        _notifier = notifier;
    }


    public async Task<bool> CommitAsync()
    {
        try
        {
            if (await _unitOfWork.CommitAsync())
                return await Task.FromResult(true);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }


        return await Task.FromResult(false);
    }

    protected void Notify(ValidationResult validationResult)
    {
        foreach (var error in validationResult.Errors)
            Notify(error.PropertyName, error.ErrorMessage);
    }

    protected void Notify(string key, string message)
    {
        _notifier.Handle(new Notification(key, message));
    }

    protected List<Notification> GetAllNotifications()
    {
        return _notifier.GetAllNotifications();
    }
}