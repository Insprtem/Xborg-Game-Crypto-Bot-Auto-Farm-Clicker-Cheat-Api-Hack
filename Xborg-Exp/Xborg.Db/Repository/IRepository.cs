using DogsHouse.Db.Entities;
using System.Linq.Expressions;

namespace DogsHouse.Db.Repository
{
    public interface IRepository<TEntity> where TEntity : Entity
    {
        Task<IEnumerable<TEntity>> GetAllAsync();
        Task AddAsync(TEntity entity);
    }
}
