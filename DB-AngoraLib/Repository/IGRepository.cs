using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DB_AngoraLib.Repository
{
    public interface IGRepository<T>
    {
        Task<IEnumerable<T>> GetAllObjectsAsync();
        Task AddObjectAsync(T obj);
        Task DeleteObjectAsync(T obj);
        Task UpdateObjectAsync(T obj);
        Task<T> GetObjectByIdAsync(int id);
        Task SaveObjects(List<T> objs);
    }
}
