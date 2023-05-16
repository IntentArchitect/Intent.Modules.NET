using System;
using System.Collections.Generic;
using Finbuckle.SharedDatabase.TestApplication.Api.Filters;
using Intent.RoslynWeaver.Attributes;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerUI;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.AspNetCore.Swashbuckle.SwashbuckleConfiguration", Version = "1.0")]

namespace Finbuckle.SharedDatabase.TestApplication.Api.Configuration
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
                            Title = "Finbuckle.SharedDatabase.TestApplication API"
                        });
                    options.OperationFilter<AuthorizeCheckOperationFilter>();
                    options.CustomSchemaIds(x => x.FullName);
                    options.OperationFilter<TenantHeaderOperationFilter>();
                });
            return services;
        }

        public static void UseSwashbuckle(this IApplicationBuilder app)
        {
            app.UseSwagger();
            app.UseSwaggerUI(
                options =>
                {
                    options.RoutePrefix = "swagger";
                    options.SwaggerEndpoint("/swagger/v1/swagger.json", "Finbuckle.SharedDatabase.TestApplication API V1");
                    options.OAuthAppName("Finbuckle.SharedDatabase.TestApplication API");
                    options.EnableDeepLinking();
                    options.DisplayOperationId();
                    options.DefaultModelsExpandDepth(2);
                    options.DefaultModelRendering(ModelRendering.Model);
                    options.DocExpansion(DocExpansion.List);
                    options.ShowExtensions();
                    options.EnableFilter(string.Empty);
                });
        }
    }
}