using AmazingChat.Domain.Interfaces.Notifier;
using AmazingChat.Domain.Notification;
using AmazingChat.Domain.Shared;
using FluentValidation.Results;

namespace AmazingChat.Application;

public abstract class AppService
{
    #region Fields

    private readonly INotifier _notifier;

    private readonly IUnitOfWork _unitOfWork;

    #endregion Fields

    #region Constructors

    public AppService(IUnitOfWork unitOfWork, INotifier notifier)
    {
        _unitOfWork = unitOfWork;
        _notifier = notifier;
    }

    #endregion Constructors

    #region Methods

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

    #endregion Methods
}