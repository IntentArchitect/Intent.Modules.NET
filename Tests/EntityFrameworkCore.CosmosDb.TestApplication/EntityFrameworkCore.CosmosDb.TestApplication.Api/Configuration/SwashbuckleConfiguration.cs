using System;
using System.Collections.Generic;
using EntityFrameworkCore.CosmosDb.TestApplication.Api.Filters;
using Intent.RoslynWeaver.Attributes;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerUI;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.AspNetCore.Swashbuckle.SwashbuckleConfiguration", Version = "1.0")]

namespace EntityFrameworkCore.CosmosDb.TestApplication.Api.Configuration
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
                            Title = "EntityFrameworkCore.CosmosDb.TestApplication API"
                        });
                    options.CustomSchemaIds(x => x.FullName);
                    options.OperationFilter<AuthorizeCheckOperationFilter>();

                    var securityScheme = new OpenApiSecurityScheme()
                    {
                        Name = "Authorization",
                        Description = "Enter a Bearer Token into the `Value` field to have it automatically prefixed with `Bearer ` and used as an `Authorization` header value for requests.",
                        In = ParameterLocation.Header,
                        Type = SecuritySchemeType.Http,
                        Scheme = "bearer",
                        BearerFormat = "JWT",
                        Reference = new OpenApiReference
                        {
                            Id = JwtBearerDefaults.AuthenticationScheme,
                            Type = ReferenceType.SecurityScheme
                        }
                    };

                    options.AddSecurityDefinition("Bearer", securityScheme);
                    options.AddSecurityRequirement(
                        new OpenApiSecurityRequirement
                        {
                            { securityScheme, Array.Empty<string>() }
                        });
                });
            return services;
        }

        public static void UseSwashbuckle(this IApplicationBuilder app, IConfiguration configuration)
        {
            app.UseSwagger();
            app.UseSwaggerUI(
                options =>
                {
                    options.RoutePrefix = "swagger";
                    options.SwaggerEndpoint("/swagger/v1/swagger.json", "EntityFrameworkCore.CosmosDb.TestApplication API V1");
                    options.OAuthAppName("EntityFrameworkCore.CosmosDb.TestApplication API");
                    options.EnableDeepLinking();
                    options.DisplayOperationId();
                    options.DefaultModelsExpandDepth(2);
                    options.DefaultModelRendering(ModelRendering.Model);
                    options.DocExpansion(DocExpansion.List);
                    options.ShowExtensions();
                    options.EnableFilter(string.Empty);
                    options.OAuthScopeSeparator(" ");
                });
        }
    }
}