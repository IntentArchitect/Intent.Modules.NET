using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Identity.ForbiddenAccessException", Version = "1.0")]

namespace Intent.Modules.NET.Tests.Application.Core.Common.Exceptions
{
    public class ForbiddenAccessException : Exception
    {
        public ForbiddenAccessException()
            : base() { }

        public ForbiddenAccessException(string message)
            : base(message)
        {
        }

        public ForbiddenAccessException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}