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
        public virtual async Task<IEnumerable<T>> GetAllObjectsAsync()
        {
            using (var context = new DB_AngoraContext())
            {
                return await context.Set<T>().AsNoTracking().ToListAsync();
            }
        }

        public async Task SaveObjects(List<T> objs)
        {
            using (var context = new DB_AngoraContext())
            {
                foreach (T obj in objs)
                {
                    context.Set<T>().Add(obj);
                    context.SaveChanges();
                }

                context.SaveChanges();
            }
        }

        public async Task<T> GetObjectAsync(Expression<Func<T, bool>> filter)
        {
            using (var context = new DB_AngoraContext())
            {
                return await context.Set<T>().Where(filter).FirstOrDefaultAsync();
            }
        }

        public virtual async Task AddObjectAsync(T obj)
        {
            using (var context = new DB_AngoraContext())
            {
                context.Set<T>().Add(obj);
                await context.SaveChangesAsync();
            }
        }

        public async Task<T> GetObjectByIdAsync(int id)
        {
            using (var context = new DB_AngoraContext())
            {
                return await context.Set<T>().FindAsync(id);
            }
        }        

        public async Task UpdateObjectAsync(T obj)
        {
            using (var context = new DB_AngoraContext())
            {
                context.Set<T>().Update(obj);
                await context.SaveChangesAsync();
            }
        }

        public async Task DeleteObjectAsync(T obj)
        {
            using (var context = new DB_AngoraContext())
            {
                context.Set<T>().Remove(obj);
                await context.SaveChangesAsync();
            }
        }
    }
}
