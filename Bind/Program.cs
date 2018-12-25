using System;
using Optional;
using Optional.Linq;
using Optional.Unsafe;

namespace Bind
{
    public class FootprintGenerator
    {
        public Option<string> Generate() =>
            from _userId in GetRequestingUserId()
            from _userDept in GetUserDepartmentName()
            select $"{_userId}-{_userDept}-{ActualTimeUtc:yyyy-mm-dd}";

        private Option<int> GetRequestingUserId() => 123.Some();
        private Option<string> GetUserDepartmentName() => "ABC".Some();
        private DateTime ActualTimeUtc => DateTime.UtcNow;
    }

    class Program
    {
        static void Main(string[] args)
        {
        }
    }

}
