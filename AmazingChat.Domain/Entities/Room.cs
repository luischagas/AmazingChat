namespace AmazingChat.Domain.Entities;

public class Room : Entity<Room>
{
    private IList<RoomMessage> _messages;
    
    public Room(string name)
    {
        Name = name;
    }
    
    protected Room()
    {
    }

    public string Name { get; private set; }

    public Guid UserId { get; private set; }
    
    public User User { get; private set; }

    public ICollection<RoomMessage> Messages => _messages;

    public void UpdateName(string name)
    {
        Name = name;
    }
    
    public void AddRoomMessage(RoomMessage roomMessage)
    {
        if (roomMessage.IsValid())
            _messages.Add(roomMessage);

        AddErrors(roomMessage.ValidationResult);
    }
    
    public override bool IsValid()
    {
        throw new NotImplementedException();
    }
}