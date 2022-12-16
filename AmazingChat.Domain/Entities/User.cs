namespace AmazingChat.Domain.Entities;

public class User : Entity<User>
{
    private IList<Room> _rooms;
    
    private IList<RoomMessage> _messages;
    
    public string Name { get; private set; }

    public string ConnectionId { get; private set; }

    public IEnumerable<Room> Rooms => _rooms;
    
    public ICollection<RoomMessage> Messages => _messages;
    
    public override bool IsValid()
    {
        throw new NotImplementedException();
    }
    
    public void AddRoom(Room room)
    {
        if (room.IsValid())
            _rooms.Add(room);

        AddErrors(room.ValidationResult);
    }
    
    public void AddRoomMessage(RoomMessage roomMessage)
    {
        if (roomMessage.IsValid())
            _messages.Add(roomMessage);

        AddErrors(roomMessage.ValidationResult);
    }
}