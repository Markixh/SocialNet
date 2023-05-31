using SocialNet.Data.Models;

namespace SocialNet.Data.Repositories
{
    public class GenerateUsers
    {
        public readonly string[] maleNames = new string[] { "Александр", "Игорь", "Андрей", "Сергей", "Евгений", "Алексей"};
        public readonly string[] femaleNames = new string[] { "Анна", "Мария", "Елена" };
        public readonly string[] lastNames = new string[] { "Тестов", "Марков", "Иванов", "Еремеев" };

        public List<User> Populate(int count)
        {
            var users = new List<User>();
            for (int i = 1; i < count; i++)
            {
                string firstName;
                var rand = new Random();

                var male = rand.Next(0, 2);

                var lastName = lastNames[rand.Next(0, lastNames.Length - 1)];
                if (male == 0)
                {
                    firstName = maleNames[rand.Next(0, maleNames.Length - 1)];
                }
                else
                {
                    lastName = lastName + "a";
                    firstName = femaleNames[rand.Next(0, femaleNames.Length - 1)];
                }

                var item = new User()
                {
                    FirstName = firstName,
                    LastName = lastName,
                    BirthDate = DateTime.Now.AddDays(-rand.Next(1, (DateTime.Now - DateTime.Now.AddYears(-25)).Days)),
                    Email = "test" + rand.Next(0, 1204) + "@test.com",
                };

                item.UserName = item.Email;
                item.Image = "https://via.placeholder.com/500";

                users.Add(item);
            }

            return users;
        }
    }
}
