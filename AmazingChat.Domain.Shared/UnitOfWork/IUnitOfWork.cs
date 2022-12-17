namespace AmazingChat.Domain.Shared.UnitOfWork;

public interface IUnitOfWork : IDisposable
{
    Task<bool> CommitAsync();
}