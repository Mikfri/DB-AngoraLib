using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DB_AngoraLib.Repository
{
    public interface IGRepository<T> where T : class
    {
        DbSet<T> GetDbSet();
        Task<IEnumerable<T>> GetAllObjectsAsync();
        Task SaveObjects(List<T> objs);
        Task<T> GetObjectAsync(Expression<Func<T, bool>> filter); //todo: overvej at istedet at benytte GetObjectByIdAsync
        Task AddObjectAsync(T obj);
        Task<T> GetObjectByIdAsync(int id);
        Task UpdateObjectAsync(T obj);
        Task DeleteObjectAsync(T obj);
    }
}
