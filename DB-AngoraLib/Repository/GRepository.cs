using DB_AngoraLib.EF_DbContext;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DB_AngoraLib.Repository
{
    public class GRepository<T> : IGRepository<T> where T : class
    {
        private readonly DB_AngoraContext _dbContext;

        public GRepository(DB_AngoraContext dbContext)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        public DbSet<T> GetDbSet()
        {
            return _dbContext.Set<T>();
        }

        public virtual async Task<IEnumerable<T>> GetAllObjectsAsync()
        {
            return await _dbContext.Set<T>().AsNoTracking().ToListAsync();
        }

        public async Task SaveObjects(List<T> objs)
        {
            foreach (T obj in objs)
            {
                _dbContext.Set<T>().Add(obj);
            }

            await _dbContext.SaveChangesAsync();
        }

        public async Task<T> GetObjectAsync(Expression<Func<T, bool>> filter)
        {
            return await _dbContext.Set<T>().Where(filter).FirstOrDefaultAsync();
        }

        public virtual async Task AddObjectAsync(T obj)
        {
            try
            {
                _dbContext.Set<T>().Add(obj);
                await _dbContext.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                // Log the error or rethrow, or handle as necessary
                throw new Exception("An error occurred while adding the object to the database.", ex);
            }
        }
        
        public async Task<T> GetObjectByStringIdAsync(string id)
        {
            return await _dbContext.Set<T>().FindAsync(id);
        }

        public async Task UpdateObjectAsync(T obj)
        {
            _dbContext.Set<T>().Update(obj);
            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteObjectAsync(T obj)
        {
            _dbContext.Set<T>().Remove(obj);
            await _dbContext.SaveChangesAsync();
        }
    }
}
