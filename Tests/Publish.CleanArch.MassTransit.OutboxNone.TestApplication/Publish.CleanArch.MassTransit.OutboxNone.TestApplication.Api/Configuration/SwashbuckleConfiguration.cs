using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Publish.CleanArch.MassTransit.OutboxNone.TestApplication.Api.Filters;
using Swashbuckle.AspNetCore.SwaggerUI;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.AspNetCore.Swashbuckle.SwashbuckleConfiguration", Version = "1.0")]

namespace Publish.CleanArch.MassTransit.OutboxNone.TestApplication.Api.Configuration
{
    public static class SwashbuckleConfiguration
    {
        public static IServiceCollection ConfigureSwagger(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSwaggerGen(
                options =>
                {
                    options.SwaggerDoc(
                        "v1",
                        new OpenApiInfo
                        {
                            Version = "v1",
                            Title = "Publish.CleanArch.MassTransit.OutboxNone.TestApplication API",
                        });
                    options.OperationFilter<AuthorizeCheckOperationFilter>();
                    options.AddSecurityDefinition("ApiToken", new OpenApiSecurityScheme()
                    {
                        Name = "Authorization",
                        Description = "Enter the Bearer Authorization string as following: `Bearer Generated-JWT-Token`",
                        In = ParameterLocation.Header,
                        Type = SecuritySchemeType.ApiKey,
                        Scheme = "Bearer",
                    });
                });
            return services;
        }

        public static void UseSwashbuckle(this IApplicationBuilder app)
        {
            app.UseSwagger(
                options =>
                {
                });
            app.UseSwaggerUI(
                options =>
                {
                    options.RoutePrefix = "swagger";
                    options.SwaggerEndpoint("/swagger/v1/swagger.json", "Publish.CleanArch.MassTransit.OutboxNone.TestApplication API V1");
                    options.OAuthAppName("Publish.CleanArch.MassTransit.OutboxNone.TestApplication API");
                    options.EnableDeepLinking();
                    options.DisplayOperationId();
                    options.DefaultModelsExpandDepth(-1);
                    options.DefaultModelsExpandDepth(2);
                    options.DefaultModelRendering(ModelRendering.Model);
                    options.DocExpansion(DocExpansion.List);
                    options.ShowExtensions();
                    options.EnableFilter(string.Empty);
                    options.OAuthScopeSeparator(" ");
                    options.OAuthScopeSeparator(" ");
                });
        }
    }
}