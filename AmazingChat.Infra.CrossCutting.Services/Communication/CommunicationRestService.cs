using AmazingChat.Domain.Shared.Services;
using RestSharp;

namespace AmazingChat.Infra.CrossCutting.Services.Communication;

public class CommunicationRestService : ICommunicationRestService
{
    #region Public Methods

    public RestResponse SendRequest(string url, string path, Method method, object body = null)
    {
        var client = new RestClient($"{url}");

        var request = new RestRequest(path, method);

        if (body is not null)
            request.AddJsonBody(body);
        
        var result = client.Execute(request);

        return result;
    }

    #endregion Public Methods
}