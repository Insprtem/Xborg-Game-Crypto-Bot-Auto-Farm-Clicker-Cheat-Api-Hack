using DogsHouse.Db.Entities;
using DogsHouseApp.Models;

namespace DogsHouseApp.Services
{
    public interface IDogService
    {
        Task AddDogAsync(DogModel dogModel);

        Task<bool> IsDogNameAlreadyExistsAsync(string name);

        Task<IEnumerable<Dog>> GetDogsAsync(string attribute, string order, int? pageNumber = null, int? pageSize = null);
    }
}
