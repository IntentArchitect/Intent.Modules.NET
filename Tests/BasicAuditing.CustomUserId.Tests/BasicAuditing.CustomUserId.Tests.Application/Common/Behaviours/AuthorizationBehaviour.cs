using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using BasicAuditing.CustomUserId.Tests.Application.Common.Exceptions;
using BasicAuditing.CustomUserId.Tests.Application.Common.Interfaces;
using BasicAuditing.CustomUserId.Tests.Application.Common.Security;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.Behaviours.AuthorizationBehaviour", Version = "1.0")]

namespace BasicAuditing.CustomUserId.Tests.Application.Common.Behaviours
{
    public class AuthorizationBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : notnull
    {
        private readonly ICurrentUserService _currentUserService;

        public AuthorizationBehaviour(ICurrentUserService currentUserService)
        {
            _currentUserService = currentUserService;
        }

        public async Task<TResponse> Handle(
            TRequest request,
            RequestHandlerDelegate<TResponse> next,
            CancellationToken cancellationToken)
        {
            var authorizeAttributes = request.GetType().GetCustomAttributes<AuthorizeAttribute>().ToList();

            if (authorizeAttributes.Count > 0)
            {
                // Must be authenticated user
                if (_currentUserService.UserId is null)
                {
                    throw new UnauthorizedAccessException();
                }

                // Role-Based authorization
                await RoleBasedAuthenticationAsync(authorizeAttributes);

                // Policy-based authorization
                await PolicyBasedAuthenticationAsync(authorizeAttributes);
            }

            // User is authorized / authorization not required
            return await next();
        }

        public async Task RoleBasedAuthenticationAsync(List<AuthorizeAttribute> authorizeAttributes)
        {
            var authorizeAttributesWithRoles = authorizeAttributes.Where(a => !string.IsNullOrWhiteSpace(a.Roles)).ToList();

            if (authorizeAttributesWithRoles.Count > 0)
            {
                foreach (var roles in authorizeAttributesWithRoles.Select(a => a.Roles.Split(',')))
                {
                    var authorized = false;
                    foreach (var role in roles)
                    {
                        var isInRole = await _currentUserService.IsInRoleAsync(role.Trim());
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
            }
        }

        public async Task PolicyBasedAuthenticationAsync(List<AuthorizeAttribute> authorizeAttributes)
        {
            var authorizeAttributesWithPolicies = authorizeAttributes.Where(a => !string.IsNullOrWhiteSpace(a.Policy)).ToList();

            if (authorizeAttributesWithPolicies.Count > 0)
            {
                foreach (var policy in authorizeAttributesWithPolicies.Select(a => a.Policy))
                {
                    var authorized = await _currentUserService.AuthorizeAsync(policy);

                    if (!authorized)
                    {
                        throw new ForbiddenAccessException();
                    }
                }
            }
        }
    }
}