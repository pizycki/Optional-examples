using Optional;
using static System.Console;

namespace Free
{
    class Program
    {
        static void Main(string[] args)
        {
            UserFree<UserEntity> flow = from userId in new UserFree<UserId>.CreateUser(new CreateUserRequest { Name = "Paweł Iżycki" },
                                                                                       id => new UserFree<UserId>.Return(id))
                                        from userEntity in new UserFree<UserEntity>.GetUser(userId,
                                                                                            u => new UserFree<UserEntity>.Return(u))
                                        select userEntity;

            var env = new Env(); // File system, Database, 3rd systems

            Option<(UserEntity user, Env), Error> result = UserFreeInterpreter.Interpret(flow, env);

            result
                .Map(r => r.user)
                .Match(u => WriteLine($"User name is {u.Name}"),
                       err => WriteLine($"Error!: {err.Message}"));

        }
    }
}
