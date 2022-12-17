namespace AmazingChat.Domain.Shared;

public interface IUnitOfWork : IDisposable
{
    Task<bool> CommitAsync();
}