using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.AspNetCore.Permissions", Version = "1.0")]

namespace CleanArchitecture.Comprehensive.Application.Security
{
    public static class Permissions
    {
        public const string RoleAdmin = "Admin";
        public const string RoleOne = "One";
        public const string RoleTwo = "Two";

        public static IEnumerable<string> All()
        {
            yield return RoleAdmin;
            yield return RoleOne;
            yield return RoleTwo;
        }

        public static IEnumerable<string> Roles()
        {
            yield return RoleAdmin;
            yield return RoleOne;
            yield return RoleTwo;
        }

        public static IEnumerable<string> Policies()
        {
            yield break;
        }
    }
}