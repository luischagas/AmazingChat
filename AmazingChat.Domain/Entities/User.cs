using FluentValidation;

namespace AmazingChat.Domain.Entities;

public class User : Entity<User>
{
    private IList<RoomMessage> _messages;

    public User(Guid id, string email)
    {
        Id = id;
        Email = email;
    }

    protected User()
    {
        
    }
    
    public string Email { get; private set; }

    public string? ConnectionId { get; private set; }

    public IEnumerable<RoomMessage> Messages => _messages;
    
    public override bool IsValid()
    {
        ValidateEmail();
        
        AddErrors(Validate(this));

        return ValidationResult.IsValid;
    }
    
    private void ValidateEmail()
    {
        RuleFor(d => d.Email)
            .NotEmpty()
            .WithMessage("Email is required");
    }

    public void UpdateConnectionId(string connectionId)
    {
        ConnectionId = connectionId;
    }

    public void AddRoomMessage(RoomMessage roomMessage)
    {
        if (roomMessage.IsValid())
            _messages.Add(roomMessage);

        AddErrors(roomMessage.ValidationResult);
    }
}