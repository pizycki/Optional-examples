using System;
using System.Collections.Generic;
using System.Linq;
using Optional;
using Optional.Unsafe;

namespace Match
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

        public Option<User> GetUser(int userId)
        {
            return _users.SingleOrDefault(x => x.Id == userId).SomeNotNull();
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            var userRepo = new UserRepository();
            Option<User> user = userRepo.GetUser(42);


            //// #1 
            //if (user.HasValue)
            //    user.MatchSome(x => Console.WriteLine("My name is: " + x.Name));
            //else
            //    user.MatchNone(() => Console.WriteLine("No user has been found with given ID"));

            //// #2
            //user.Match(
            //    x => Console.WriteLine("My name is: " + x.Name),
            //    () => Console.WriteLine("No user has been found with given ID"));

            //// #3
            //var message = user.Match(
            //    x => "My name is: " + x.Name,
            //    () => "No user has been found with given ID");
            //Console.WriteLine(message);

            //// #4
            //var message = 
            //    user.Map(x => "My name is: " + x.Name)
            //        .ValueOr("No user has been found with given ID");
            //Console.WriteLine(message);

        }
    }
}
