using System.Reflection;
using Intent.RoslynWeaver.Attributes;
using Mediator;
using TestMediator.Publisher.Application.Common.Exceptions;
using TestMediator.Publisher.Application.Common.Interfaces;
using TestMediator.Publisher.Application.Common.Security;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Mediator.AuthorizationBehaviour", Version = "1.0")]

namespace TestMediator.Publisher.Application.Common.Behaviours
{
    public class AuthorizationBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IMessage
    {
        private readonly ICurrentUserService _currentUserService;

        public AuthorizationBehaviour(ICurrentUserService currentUserService)
        {
            _currentUserService = currentUserService;
        }

        public async ValueTask<TResponse> Handle(
            TRequest request,
            MessageHandlerDelegate<TRequest, TResponse> next,
            CancellationToken cancellationToken)
        {
            var authorizeAttributes = request.GetType().GetCustomAttributes<AuthorizeAttribute>();

            foreach (var authorizeAttribute in authorizeAttributes)
            {
                // Must be an authenticated user
                if (await _currentUserService.GetAsync() is null)
                {
                    throw new UnauthorizedAccessException();
                }

                // Role-based authorization
                if (!string.IsNullOrWhiteSpace(authorizeAttribute.Roles))
                {
                    var authorized = false;
                    var roles = authorizeAttribute.Roles.Split(",").Select(x => x.Trim());

                    foreach (var role in roles)
                    {
                        var isInRole = await _currentUserService.IsInRoleAsync(role);
                        if (isInRole)
                        {
                            authorized = true;
                            break;
                        }
                    }

                    // Must be a member of at least one role in roles
                    if (!authorized)
                    {
                        throw new ForbiddenAccessException();
                    }
                }

                // Policy-based authorization
                if (!string.IsNullOrWhiteSpace(authorizeAttribute.Policy))
                {
                    var authorized = false;
                    var policies = authorizeAttribute.Policy.Split(",").Select(x => x.Trim());

                    foreach (var policy in policies)
                    {
                        var isAuthorized = await _currentUserService.AuthorizeAsync(policy);
                        if (isAuthorized)
                        {
                            authorized = true;
                            break;
                        }
                    }

                    // Must be authorized by at least one policy
                    if (!authorized)
                    {
                        throw new ForbiddenAccessException();
                    }
                }
            }

            // User is authorized / authorization not required
            return await next(request, cancellationToken);
        }
    }
}