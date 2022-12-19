using AmazingChat.Application.Interfaces;

namespace AmazingChat.Application.Common;

public class AppServiceResponse<T> : IAppServiceResponse where T : class
{
    public AppServiceResponse(T data, string message, bool success)
    {
        Data = data;
        Message = message;
        Success = success;
    }

    protected AppServiceResponse()
    {
    }

    public T Data { get; set; }
    public string Message { get; set; }
    public bool Success { get; set; }
}