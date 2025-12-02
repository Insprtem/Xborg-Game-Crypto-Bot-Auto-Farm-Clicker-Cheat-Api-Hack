using Microsoft.EntityFrameworkCore;
using Moq;

namespace DogHouse.Tests.Utillities
{
    public class MockUtillity
    {
        public static Mock<DbSet<T>> GetMockedDbSet<T>(List<T> list) where T : class
        {
            var queryable = list.AsQueryable();

            var dbSet = new Mock<DbSet<T>>();

            dbSet.As<IQueryable<T>>().Setup(m => m.Provider).Returns(queryable.Provider);
            dbSet.As<IQueryable<T>>().Setup(m => m.Expression).Returns(queryable.Expression);
            dbSet.As<IQueryable<T>>().Setup(m => m.ElementType).Returns(queryable.ElementType);
            dbSet.As<IQueryable<T>>().Setup(m => m.GetEnumerator()).Returns(() => queryable.GetEnumerator());

            return dbSet;
        }
    }
}
