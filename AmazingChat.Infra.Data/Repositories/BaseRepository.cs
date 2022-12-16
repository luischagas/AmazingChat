using System.Data;
using AmazingChat.Domain.Interfaces.Repositories;
using AmazingChat.Infra.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace AmazingChat.Infra.Data.Repositories;

 public abstract class BaseRepository<TEntity> : IBaseRepository<TEntity> where TEntity : class
    {
        private readonly AmazingChatContext _db;

        public BaseRepository(AmazingChatContext db)
        {
            _db = db;
        }

        public async Task<TEntity> GetAsync(Guid id)
        {
            return await _db.Set<TEntity>().FindAsync(id);
        }
        
        public async Task<IEnumerable<TEntity>> GetAllAsync()
        {
            return await _db.Set<TEntity>().ToListAsync();
        }

        public async Task<TEntity> AddAsync(TEntity obj)
        {
            var result = await _db.Set<TEntity>().AddAsync(obj);

           await _db.SaveChangesAsync();

            return result.Entity;
        }

        public void Update(TEntity obj)
        {
            _db.Set<TEntity>().Update(obj);
        }

        public void Delete(TEntity obj)
        {
            _db.Set<TEntity>().Remove(obj);
        }
    }