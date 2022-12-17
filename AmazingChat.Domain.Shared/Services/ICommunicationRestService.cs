using RestSharp;

namespace AmazingChat.Domain.Shared.Services;

public interface ICommunicationRestService
{
    #region Public Methods

    RestResponse SendRequest(string url, string path, Method method, object body = null);

    #endregion Public Methods
}