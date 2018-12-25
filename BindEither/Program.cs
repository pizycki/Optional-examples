using System;
using Optional;
using Optional.Linq;

namespace BindEither
{
    public class Error
    {
        public Error(string message)
        {
            Message = message;
        }

        public string Message { get; }
    }

    public class LogFootprintGenerator
    {
        public Option<string, Error> Generate() =>
            from id in GetRequestingUserId()
                        .WithException(new Error("User ID is missing."))
            from d in GetUserDepartmentName(id)
                        .WithException(new Error("User depertment is missing."))
            select $"{id}-{d}-{ActualTimeUtc:yyyy-mm-dd}";

        private Option<int> GetRequestingUserId() => 123.Some();
        private Option<string> GetUserDepartmentName(int userId) => "ABC".Some();
        private DateTime ActualTimeUtc => DateTime.UtcNow;
    }

    class Program
    {
        static void Main(string[] args)
        {
            var footprintGenerator = new LogFootprintGenerator();
            Option<string, Error> footprint = footprintGenerator.Generate();
            footprint.Match(
                x => Console.WriteLine(footprint),
                err => Console.WriteLine(err.Message));
        }
    }
}
