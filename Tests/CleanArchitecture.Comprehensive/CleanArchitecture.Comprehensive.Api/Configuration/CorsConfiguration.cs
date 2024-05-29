using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.AspNetCore.Cors.CorsConfiguration", Version = "1.0")]

namespace CleanArchitecture.Comprehensive.Api.Configuration
{
    public static class CorsConfiguration
    {
        public static IServiceCollection ConfigureCors(this IServiceCollection services, IConfiguration configuration)
        {
            var corsPolicies = configuration.GetSection("CorsPolicies").Get<CorsPolicies>();
            if (corsPolicies == null)
            {
                return services;
            }

            services
                .AddCors(options =>
                {
                    if (corsPolicies.Default != null)
                    {
                        options.AddDefaultPolicy(policy => ApplyOptions(policy, corsPolicies.Default));
                    }

                    foreach (var (name, policyOptions) in corsPolicies.Named ?? new Dictionary<string, PolicyOptions>())
                    {
                        options.AddPolicy(name, policy => ApplyOptions(policy, policyOptions));
                    }
                });

            return services;
        }

        private static void ApplyOptions(CorsPolicyBuilder policy, PolicyOptions options)
        {
            policy
                .WithOrigins(options.Origins ?? Array.Empty<string>())
                .WithMethods(options.Methods ?? Array.Empty<string>())
                .WithHeaders(options.Headers ?? Array.Empty<string>())
                .WithExposedHeaders(options.ExposedHeaders ?? Array.Empty<string>());

            if (options.AllowCredentials)
            {
                policy.AllowCredentials();
            }

            if (options.PreflightMaxAge != null)
            {
                policy.SetPreflightMaxAge(options.PreflightMaxAge.Value);
            }
        }

        private class CorsPolicies
        {
            public PolicyOptions? Default { get; set; }
            public Dictionary<string, PolicyOptions>? Named { get; set; }
        }
        private class PolicyOptions
        {
            public string[]? Origins { get; set; }
            public string[]? Methods { get; set; }
            public string[]? Headers { get; set; }
            public string[]? ExposedHeaders { get; set; }
            public bool AllowCredentials { get; set; }
            public TimeSpan? PreflightMaxAge { get; set; }
        }
    }
}