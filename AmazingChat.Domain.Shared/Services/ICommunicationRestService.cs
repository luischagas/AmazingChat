using RestSharp;

namespace AmazingChat.Domain.Shared.Services;

public interface ICommunicationRestService
{
    RestResponse SendRequest(string url, string path, Method method, object body = null);
}