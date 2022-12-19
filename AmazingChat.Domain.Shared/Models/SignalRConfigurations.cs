namespace AmazingChat.Domain.Shared.Models;

public class SignalRConfigurations
{
    public string BaseUrl { get; set; }
    public string QueryString { get; set; }
    public string UrlStockBot { get; set; }
    public List<string> AllowedCommands { get; set; }
    public string StockUser { get; set; }
    
}