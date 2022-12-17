namespace AmazingChat.Application.Interfaces;

public interface IAppServiceResponse
{
    #region Properties

    string Message { get; set; }

    bool Success { get; set; }

    #endregion Properties
}