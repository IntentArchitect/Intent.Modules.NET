using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.AspNetCore.Permissions", Version = "1.0")]

namespace SecurityConfig.Tests.Application.Security
{
    public static class Permissions
    {
        public const string RoleAdmin = "Admin";
        public const string RoleProduct = "Product";
        public const string PolicyAdmin = "Admin";
        public const string PolicyCustomer = "Customer";

        public static IEnumerable<string> All()
        {
            yield return RoleAdmin;
            yield return RoleProduct;
            yield return PolicyAdmin;
            yield return PolicyCustomer;
        }

        public static IEnumerable<string> Roles()
        {
            yield return RoleAdmin;
            yield return RoleProduct;
        }

        public static IEnumerable<string> Policies()
        {
            yield return PolicyAdmin;
            yield return PolicyCustomer;
        }
    }
}