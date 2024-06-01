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
        Task AddObjectAsync(T obj);
        Task<IEnumerable<T>> GetAllObjectsAsync();
        Task SaveObjects(List<T> objs);
        DbSet<T> GetDbSet();
        Task<T> GetObject_ByFilterAsync(Expression<Func<T, bool>> filter);
        Task<T> GetObject_ByKEYAsync(string id);
        Task UpdateObjectAsync(T obj);
        Task DeleteObjectAsync(T obj);
    }
}
