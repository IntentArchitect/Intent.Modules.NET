using System.Reflection;
using Intent.RoslynWeaver.Attributes;
using Wolverine.CQRS.TestApplication.Application.Common.Exceptions;
using Wolverine.CQRS.TestApplication.Application.Common.Interfaces;
using Wolverine.CQRS.TestApplication.Application.Common.Security;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Wolverine.AuthorizationMiddleware", Version = "1.0")]

namespace Wolverine.CQRS.TestApplication.Infrastructure.Dispatch.Middleware
{
    public class AuthorizationMiddleware
    {
        public async Task BeforeAsync(
            Envelope envelope,
            ICurrentUserService currentUserService,
            CancellationToken cancellationToken)
        {
            await AuthorizeAsync(envelope.Message, currentUserService);
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