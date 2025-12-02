using DogsHouse.Db.Entities;

namespace DogHouse.Tests.Utillities
{
    public class DogsCollectionUtillity
    {
        public List<Dog> Dogs = new List<Dog>();

        public Dog Dog1;
        public Dog Dog2;

        public DogsCollectionUtillity()
        {
            Dog1 = new Dog
            {
                Name = "TestName1",
                Color = "TestColor1",
                TailLength = 1,
                Weight = 1
            };
            Dogs.Add(Dog1);

            Dog2 = new Dog
            {
                Name = "TestName2",
                Color = "TestColor2",
                TailLength = 2,
                Weight = 2
            };
            Dogs.Add(Dog2);
        }
    }
}
