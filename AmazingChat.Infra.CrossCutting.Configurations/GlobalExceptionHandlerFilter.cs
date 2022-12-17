using System.Net;
using AmazingChat.Application.Common;
using AmazingChat.Domain.Shared.Notifications;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace AmazingChat.Infra.CrossCutting.Identity;

public class GlobalExceptionHandlerFilter: IExceptionFilter
{
    #region Private Fields

    private readonly INotifier _notifier;

    #endregion Private Fields

    #region Public Constructors

    public GlobalExceptionHandlerFilter(
        INotifier notifier)
    {
        _notifier = notifier;
    }

    #endregion Public Constructors

    #region Public Methods

    public void OnException(ExceptionContext context)
    {
        _notifier.Handle(new Notification("Oops!", "We have encountered a failure while trying to perform this operation at the moment"));

        var errorResponse = new AppServiceResponse<ICollection<Notification>>(_notifier.GetAllNotifications(), "Unexpected Error", false);

        context.Result = new ObjectResult(errorResponse)
        {
            StatusCode = (int)HttpStatusCode.InternalServerError
        };
    }

    #endregion Public Methods
}