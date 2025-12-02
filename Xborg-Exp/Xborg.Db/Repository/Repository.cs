using DogsHouse.Db.Entities;
using Microsoft.EntityFrameworkCore;

namespace DogsHouse.Db.Repository
{

    public class Repository<T> : IRepository<T> where T : Entity
    {
        private readonly DogsHouseDbContext _context;
        private readonly DbSet<T> _dbSet;

        public Repository(DogsHouseDbContext context)
        {
            _context = context;
            _dbSet = _context.Set<T>();
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return _dbSet.AsQueryable();
        }
        public async Task AddAsync(T entity)
        {
            await _dbSet.AddAsync(entity);
            await _context.SaveChangesAsync();
        }
    }
}
