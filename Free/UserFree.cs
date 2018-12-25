using System;
using Optional;
using static Optional.Option;

namespace Free
{
    public class UserId
    {
        public UserId(int value)
        {
            Value = value;
        }

        public int Value { get; }
    }

    public class CreateUserRequest
    {
        public string Name { get; set; }
    }

    public class UserEntity
    {
        public UserId Id { get; set; }
        public string Name { get; set; }
    }


    public interface IUsers
    {
        UserId Create(CreateUserRequest request);

        UserEntity Get(UserId userId);
    }

    public abstract class UserFree<Out>
    {
        public class Return : UserFree<Out>
        {
            public Return(Out value)
            {
                Value = value;
            }

            public Out Value { get; }
        }

        public class CreateUser : UserFree<Out>
        {
            public CreateUser(CreateUserRequest request, Func<UserId, UserFree<Out>> next)
            {
                Request = request;
                Next = next;
            }

            public CreateUserRequest Request { get; }
            public Func<UserId, UserFree<Out>> Next { get; }
        }

        public class GetUser : UserFree<Out>
        {
            public GetUser(UserId userId, Func<UserEntity, UserFree<Out>> next)
            {
                UserId = userId;
                Next = next;
            }

            public UserId UserId { get; }
            public Func<UserEntity, UserFree<Out>> Next { get; }
        }
    }

    public static class UserFreeExtensions
    {
        public static UserFree<Out> Bind<In, Out>(this UserFree<In> m, Func<In, UserFree<Out>> f)
        {
            switch (m)
            {
                case UserFree<In>.Return r:      return f(r.Value);
                case UserFree<In>.CreateUser cu: return new UserFree<Out>.CreateUser(cu.Request, id => cu.Next(id).Bind(f));
                case UserFree<In>.GetUser gu:    return new UserFree<Out>.GetUser(gu.UserId, u => gu.Next(u).Bind(f));
                default:                         throw new ArgumentOutOfRangeException(m.GetType().FullName);
            }
        }

        public static UserFree<B> Select<A, B>(this UserFree<A> ma, Func<A, B> f) =>
            ma.Bind(a => new UserFree<B>.Return(f(a)));

        public static UserFree<C> SelectMany<A, B, C>(this UserFree<A> ma, Func<A, UserFree<B>> bind, Func<A, B, C> project) =>
            ma.Bind(a => bind(a).Select(b => project(a, b)));
    }

    public class Error
    {
        public Error(string message)
        {
            Message = message;
        }

        public string Message { get; }
    }

    public class Env
    {
        public Option<int, Error> RunDbCommand(string sql) =>
            Some(42).WithException(new Error("Failed to create user in database"));

        public Option<UserEntity, Error> SelectDatabase(string sql) =>
            None<UserEntity, Error>(new Error("User not found in database."));
    }

    public static class UserFreeInterpreter
    {
        public static Option<(T, Env), Error> Interpret<T>(UserFree<T> m, Env env)
        {
            switch (m)
            {
                case UserFree<T>.Return r:      return (r.Value, env).Some<(T, Env), Error>();
                case UserFree<T>.CreateUser cu: return CreateUser(cu, env);
                case UserFree<T>.GetUser gu:    return GetUser(gu, env);
                default:                        throw new ArgumentOutOfRangeException(m.GetType().FullName);
            }
        }

        private static Option<(T, Env), Error> GetUser<T>(UserFree<T>.GetUser gu, Env env) =>
            env.SelectDatabase($"SELECT * FROM [Users] WHERE ID = {gu.UserId}")
               .FlatMap(user => Interpret(gu.Next(user), env));

        private static Option<(T, Env), Error> CreateUser<T>(UserFree<T>.CreateUser cu, Env env) =>
            env.RunDbCommand($"INSERT INTO [Users] ('Name') VALUES ({cu.Request.Name});")
               .Map(id => new UserId(id))
               .FlatMap(userId => Interpret(cu.Next(userId), env));
    }


}
