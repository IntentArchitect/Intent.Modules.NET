using System.Linq;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.HotChocolate.GraphQL.Models;

namespace Intent.Modules.HotChocolate.GraphQL.Templates
{
    internal static class AuthorizationHelper
    {
        public static void AddAuthorizeAttributes(ICSharpTemplate template, IGraphQLResolverModel resolver, CSharpClassMethod method)
        {
            foreach (var securityModel in resolver.SecurityModels ?? [])
            {
                var policies = securityModel.Policies.Count > 0
                    ? securityModel.Policies
                    : [null];

                foreach (var policy in policies)
                {
                    method.AddAttribute(template.UseType("HotChocolate.Authorization.Authorize"), attr =>
                    {
                        if (securityModel.Roles.Count > 0)
                        {
                            attr.AddArgument($"Roles = new [] {{ {string.Join(", ", securityModel.Roles.Select(role => $"\"{role}\""))} }}");
                        }

                        if (policy != null)
                        {
                            attr.AddArgument($"Policy = \"{policy}\"");
                        }
                    });
                }
            }
        }
    }
}
