using DogsHouse.Db.Entities;
using Microsoft.EntityFrameworkCore;

namespace DogsHouse.Db
{
    public class DogsHouseDbContext : DbContext
    {
        private readonly string _connectionString;

        public DogsHouseDbContext() { }

        public DogsHouseDbContext(string connectionString)
        {
            _connectionString = connectionString;
        }

        #region DbSets
        public DbSet<Dog> Dogs { get; set; }
        #endregion

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(_connectionString);
                optionsBuilder.UseLazyLoadingProxies();

            }
        }
    }
}
