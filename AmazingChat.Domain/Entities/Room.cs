using FluentValidation;

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

    public IEnumerable<RoomMessage> Messages => _messages;
    
    public void AddRoomMessage(RoomMessage roomMessage)
    {
        if (roomMessage.IsValid())
            _messages.Add(roomMessage);

        AddErrors(roomMessage.ValidationResult);
    }
    
    public override bool IsValid()
    {
        ValidateName();
        
        AddErrors(Validate(this));

        return ValidationResult.IsValid;
    }
    
    private void ValidateName()
    {
        RuleFor(d => d.Name)
            .NotEmpty()
            .WithMessage("Name is required");
    }
}