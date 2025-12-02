using DogsHouse.Db.Entities;
using DogsHouse.Db.Repository;
using DogsHouseApp.Models;

namespace DogsHouseApp.Services
{
    public class DogService : IDogService
    {
        private readonly IRepository<Dog> _repository;

        public DogService(IRepository<Dog> repository)
        {
            _repository = repository;
        }

        public async Task AddDogAsync(DogModel dogModel)
        {
            if (await IsDogNameAlreadyExistsAsync(dogModel.Name))
            {
                throw new ArgumentException($"Dog with name {dogModel.Name} is already exist");
            }

            if (dogModel.Weight <= 0)
            {
                throw new ArgumentException("Weight is not valid");
            }

            if (dogModel.TailLength <= 0)
            {
                throw new ArgumentException("TailLength is not valid");
            }

            var dog = new Dog()
            {
                Name = dogModel.Name,
                Color = dogModel.Color,
                TailLength = dogModel.TailLength,
                Weight = dogModel.Weight
            };

            await _repository.AddAsync(dog);
        }

        public async Task<IEnumerable<Dog>> GetDogsAsync(string attribute = null, string order = null, int? pageNumber = null, int? pageSize = null)
        {
            var dogs = await _repository.GetAllAsync();

            if (!string.IsNullOrEmpty(attribute) && !string.IsNullOrEmpty(order))
            {
                dogs = Sort(dogs, attribute, order);
            }

            if (pageNumber.HasValue && pageSize.HasValue && pageNumber >= 1 && pageSize > 0)
            {
                dogs = GetPaginatedDogs(dogs, pageNumber.Value, pageSize.Value);
            }

            return dogs.ToList();
        }

        public async Task<bool> IsDogNameAlreadyExistsAsync(string name)
        {
            var dogs = await _repository.GetAllAsync();

            return dogs.Any(dog => dog.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
        }

        private IEnumerable<Dog> GetPaginatedDogs(IEnumerable<Dog> dogs, int pageNumber, int pageSize)
        {
            var result = dogs.Skip((pageNumber - 1) * pageSize).Take(pageSize);

            return result;
        }

        private IEnumerable<Dog> Sort(IEnumerable<Dog> dogs, string attribute, string order)
        {
            var isDesc = order == "desc";

            Func<Dog, object> keySelector = null;

            switch (attribute.ToLower())
            {
                case "name":
                    keySelector = new Func<Dog, string>(m => m.Name);
                    break;
                case "color":
                    keySelector = new Func<Dog, string>(m => m.Color);
                    break;
                case "taillength":
                    keySelector = new Func<Dog, object>(m => m.TailLength);
                    break;
                case "weight":
                    keySelector = new Func<Dog, object>(m => m.Weight);
                    break;
                default:
                    break;
            }

            if (keySelector != null)
            {
                dogs = isDesc
                    ? dogs.OrderByDescending(keySelector)
                    : dogs.OrderBy(keySelector);
            }

            return dogs;
        }
    }
}
