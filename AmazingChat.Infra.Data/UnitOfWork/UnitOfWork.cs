using AmazingChat.Domain.Shared;
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
        if (!this._disposed)
        {
            if (disposing)
            {
                _context.Dispose();
            }
        }
        this._disposed = true;
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
}