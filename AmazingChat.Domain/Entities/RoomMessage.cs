namespace AmazingChat.Domain.Entities;

public class RoomMessage : Entity<RoomMessage>
{
    public RoomMessage(string message, Guid roomId)
    {
        Message = message;
        RoomId = roomId;
    }
    
    protected RoomMessage()
    {
    }
    
    public string Message { get; }
    
    public DateTime Timestamp { get; private set; }

    public Guid RoomId { get; private set; }
    
    public Room Room { get; private set; }
    
    public override bool IsValid()
    {
        throw new NotImplementedException();
    }
}