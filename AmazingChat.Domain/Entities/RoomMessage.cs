using FluentValidation;

namespace AmazingChat.Domain.Entities;

public class RoomMessage : Entity<RoomMessage>
{
    public RoomMessage(string message, Guid roomId, Guid userId)
    {
        Message = message;
        RoomId = roomId;
        UserId = userId;
        Timestamp = DateTime.Now;
    }
    
    protected RoomMessage()
    {
    }
    
    public string Message { get; }
    
    public DateTime Timestamp { get; private set; }

    public Guid RoomId { get; private set; }
    
    public Room Room { get; private set; }
    
    public Guid UserId { get; private set; }
    
    public User User { get; private set; }
    
    public override bool IsValid()
    {
        ValidateMessage();
        ValidateUserId();
        ValidateRoomId();
        
        AddErrors(Validate(this));

        return ValidationResult.IsValid;
    }
    
    private void ValidateMessage()
    {
        RuleFor(d => d.Message)
            .NotEmpty()
            .WithMessage("Message is required");
    }
    
    private void ValidateUserId()
    {
        RuleFor(d => d.UserId)
            .NotEmpty()
            .WithMessage("User is required");
    }
    
    private void ValidateRoomId()
    {
        RuleFor(d => d.RoomId)
            .NotEmpty()
            .WithMessage("Room is required");
    }
}