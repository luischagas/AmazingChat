namespace AmazingChat.Application.Interfaces;

public interface IAppServiceResponse
{
    string Message { get; set; }

    bool Success { get; set; }
}