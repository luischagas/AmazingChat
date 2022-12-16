using FluentValidation;
using FluentValidation.Results;

namespace AmazingChat.Domain.Entities;

public abstract class Entity<T> : AbstractValidator<T>
    where T : Entity<T>
{
    #region Constructors

    protected Entity()
    {
        Id = Guid.NewGuid();
        IsDeleted = false;
        CreatedOn = DateTime.Now;
        ValidationResult = new ValidationResult();
    }

    #endregion Constructors

    #region Properties

    public Guid Id { get; protected set; }

    public DateTime CreatedOn { get; protected set; }

    public bool IsDeleted { get; protected set; }

    public ValidationResult ValidationResult { get; protected set; }

    #endregion Properties

    #region Methods

    public abstract bool IsValid();

    public void Delete()
    {
        IsDeleted = true;
    }

    protected void AddErrors(ValidationResult validateResult)
    {
        foreach (var error in validateResult.Errors)
        {
            ValidationResult.Errors.Add(error);
        }
    }

    #endregion Methods
}