using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Subscribe.GooglePubSub.TestApplication.Api.Filters;
using Swashbuckle.AspNetCore.SwaggerUI;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.AspNetCore.Swashbuckle.SwashbuckleConfiguration", Version = "1.0")]

namespace Subscribe.GooglePubSub.TestApplication.Api.Configuration
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
                            Title = "Subscribe.GooglePubSub.TestApplication API"
                        });
                    options.OperationFilter<AuthorizeCheckOperationFilter>();
                    options.CustomSchemaIds(x => x.FullName);
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
                    options.SwaggerEndpoint("/swagger/v1/swagger.json", "Subscribe.GooglePubSub.TestApplication API V1");
                    options.OAuthAppName("Subscribe.GooglePubSub.TestApplication API");
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