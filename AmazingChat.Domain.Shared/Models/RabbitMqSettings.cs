namespace AmazingChat.Domain.Shared.Models;

public class RabbitMqSettings
{
    public string HostName { get; set; }

    public int Port { get; set; }

    public string UserName { get; set; }

    public string Password { get; set; }

    public RabbitMqSettingsQueue Queue { get; set; }
}

public class RabbitMqSettingsQueue
{
    public string Name { get; set; }

    public bool IsDurable { get; set; }

    public bool IsExclusive { get; set; }

    public bool IsAutoDeleted { get; set; }

    public string RoutingKey { get; set; }

    public IDictionary<string, object> Arguments { get; set; }

    public bool HasDeadLetter { get; set; }

    public RabbitMqSettingsExchange Exchange { get; set; }
}

public class RabbitMqSettingsExchange
{
    public string Name { get; set; }

    public string Type { get; set; }

    public bool IsDurable { get; set; }

    public bool IsAutoDeleted { get; set; }

    public IDictionary<string, object> Arguments { get; set; }
}