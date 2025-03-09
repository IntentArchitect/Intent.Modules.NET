using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.AspNetCore.Permissions", Version = "1.0")]

namespace GrpcServer.Application.Security
{
    public static class Permissions
    {
        public const string SomeRole = "SomeRole";

        public static IEnumerable<string> All()
        {
            yield return SomeRole;
        }

        public static IEnumerable<string> Roles()
        {
            yield return SomeRole;
        }

        public static IEnumerable<string> Policies()
        {
            yield break;
        }
    }
}