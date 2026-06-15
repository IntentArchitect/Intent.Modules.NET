using System.Reflection;
using FluentValidation;
using Intent.RoslynWeaver.Attributes;
using Wolverine;
using Wolverine.CQRS.TestApplication.Application.Common.Exceptions;
using Wolverine.CQRS.TestApplication.Application.Common.Interfaces;
using Wolverine.CQRS.TestApplication.Application.Common.Security;

[assembly: DefaultIntentManaged(Mode.Fully)]

namespace Wolverine.CQRS.TestApplication.Application.Common.Behaviours
{
    public class AuthorizationMiddleware
    {
        public Task BeforeAsync(Envelope envelope, ICurrentUserService currentUserService, CancellationToken cancellationToken)
        {
            return AuthorizeAsync(envelope.Message, currentUserService);
        }

        private static async Task AuthorizeAsync(object request, ICurrentUserService currentUserService)
        {
            var authorizeAttributes = request.GetType().GetCustomAttributes<AuthorizeAttribute>();

            foreach (var authorizeAttribute in authorizeAttributes)
            {
                if (await currentUserService.GetAsync() is null)
                {
                    throw new UnauthorizedAccessException();
                }

                if (!string.IsNullOrWhiteSpace(authorizeAttribute.Roles))
                {
                    var authorized = false;
                    var roles = authorizeAttribute.Roles.Split(',').Select(x => x.Trim());

                    foreach (var role in roles)
                    {
                        if (await currentUserService.IsInRoleAsync(role))
                        {
                            authorized = true;
                            break;
                        }
                    }

                    if (!authorized)
                    {
                        throw new ForbiddenAccessException();
                    }
                }

                if (!string.IsNullOrWhiteSpace(authorizeAttribute.Policy))
                {
                    var authorized = false;
                    var policies = authorizeAttribute.Policy.Split(',').Select(x => x.Trim());

                    foreach (var policy in policies)
                    {
                        if (await currentUserService.AuthorizeAsync(policy))
                        {
                            authorized = true;
                            break;
                        }
                    }

                    if (!authorized)
                    {
                        throw new ForbiddenAccessException();
                    }
                }
            }
        }
    }
}
