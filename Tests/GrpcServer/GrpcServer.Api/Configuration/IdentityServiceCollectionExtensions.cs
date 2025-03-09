using System;
using System.Diagnostics.CodeAnalysis;
using Intent.RoslynWeaver.Attributes;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection.Extensions;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.AspNetCore.Identity.IdentityServiceCollectionExtensions", Version = "1.0")]

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Adaptation of <see href="https://github.com/dotnet/aspnetcore/blob/6a7bcda42de7b98196b38924cc354216eba57c9b/src/Identity/Core/src/IdentityServiceCollectionExtensions.cs"/>
/// which configures Identity services without also adding Cookie authentication.
/// </summary>
public static class IdentityServiceCollectionExtensions
{
    /// <summary>
    /// Adds the default identity system configuration for the specified User and Role types.
    /// </summary>
    /// <typeparam name="TUser">The type representing a User in the system.</typeparam>
    /// <typeparam name="TRole">The type representing a Role in the system.</typeparam>
    /// <param name="services">The services available in the application.</param>
    /// <returns>An <see cref="IdentityBuilder"/> for creating and configuring the identity system.</returns>
    public static IdentityBuilder AddIdentityWithoutCookieAuth<TUser, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] TRole>(
        this IServiceCollection services)
        where TUser : class
        where TRole : class
        => services.AddIdentityWithoutCookieAuth<TUser, TRole>(setupAction: null!);

    /// <summary>
    /// Adds and configures the identity system for the specified User and Role types.
    /// </summary>
    /// <typeparam name="TUser">The type representing a User in the system.</typeparam>
    /// <typeparam name="TRole">The type representing a Role in the system.</typeparam>
    /// <param name="services">The services available in the application.</param>
    /// <param name="setupAction">An action to configure the <see cref="IdentityOptions"/>.</param>
    /// <returns>An <see cref="IdentityBuilder"/> for creating and configuring the identity system.</returns>
    public static IdentityBuilder AddIdentityWithoutCookieAuth<TUser, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] TRole>(
        this IServiceCollection services,
        Action<IdentityOptions> setupAction)
        where TUser : class
        where TRole : class
    {
        // Hosting doesn't add IHttpContextAccessor by default
        services.AddHttpContextAccessor();
        // Identity services
        services.TryAddScoped<IUserValidator<TUser>, UserValidator<TUser>>();
        services.TryAddScoped<IPasswordValidator<TUser>, PasswordValidator<TUser>>();
        services.TryAddScoped<IPasswordHasher<TUser>, PasswordHasher<TUser>>();
        services.TryAddScoped<ILookupNormalizer, UpperInvariantLookupNormalizer>();
        services.TryAddScoped<IRoleValidator<TRole>, RoleValidator<TRole>>();
        // No interface for the error describer so we can add errors without rev'ing the interface
        services.TryAddScoped<IdentityErrorDescriber>();
        services.TryAddScoped<ISecurityStampValidator, SecurityStampValidator<TUser>>();
        services.TryAddScoped<ITwoFactorSecurityStampValidator, TwoFactorSecurityStampValidator<TUser>>();
        services.TryAddScoped<IUserClaimsPrincipalFactory<TUser>, UserClaimsPrincipalFactory<TUser, TRole>>();
        services.TryAddScoped<IUserConfirmation<TUser>, DefaultUserConfirmation<TUser>>();
        services.TryAddScoped<UserManager<TUser>>();
        services.TryAddScoped<SignInManager<TUser>>();
        services.TryAddScoped<RoleManager<TRole>>();

        if (setupAction != null)
        {
            services.Configure(setupAction);
        }

        return new IdentityBuilder(typeof(TUser), typeof(TRole), services);
    }
}