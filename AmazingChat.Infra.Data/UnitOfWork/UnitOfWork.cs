using AmazingChat.Domain.Shared.UnitOfWork;
using AmazingChat.Infra.Data.Context;

namespace AmazingChat.Infra.Data.UnitOfWork;

public class UnitOfWork : IUnitOfWork
{
    private readonly AmazingChatContext _context;
    private bool _disposed;

    public UnitOfWork(AmazingChatContext context)
    {
        _context = context;
        _disposed = false;
    }

    public bool Commit()
    {
        bool result;

        result = _context.SaveChanges() > 0;

        return result;
    }

    public async Task<bool> CommitAsync()
    {
        return await _context.SaveChangesAsync() > 0;
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                _context.Dispose();
            }
        }

        _disposed = true;
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
}