using System;
using System.Collections.Generic;
using System.Linq;

namespace NullRefEx
{
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class UserRepository
    {
        IReadOnlyCollection<User> _users = new List<User>
        {
            new User
            {
                Id = 42,
                Name = "Paweł Iżycki"
            },
        };

        public User GetUser(int userId)
        {
            return _users.SingleOrDefault(x => x.Id == userId);
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            var userRepo = new UserRepository();
            User user = userRepo.GetUser(42);
            Console.WriteLine("My name is: " + user.Name);
        }
    }

}
