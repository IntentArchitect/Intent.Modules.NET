using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Intent.RoslynWeaver.Attributes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.AspNetCore.Swashbuckle.Security.AuthorizeCheckOperationFilter", Version = "1.0")]

namespace AdvancedMappingCrud.RichDomain.ServiceModel.Tests.Api.Filters
{
    public class AuthorizeCheckOperationFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            if (!HasAuthorize(context))
            {
                return;
            }
            operation.Security.Add(new OpenApiSecurityRequirement
            {
                [new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    }
                }] = Array.Empty<string>()
            });
        }

        private bool HasAuthorize(OperationFilterContext context)
        {
            if (context.MethodInfo.GetCustomAttributes(true).OfType<AuthorizeAttribute>().Any())
            {
                return true;
            }
            return context.MethodInfo.DeclaringType != null
                && context.MethodInfo.DeclaringType.GetCustomAttributes(true).OfType<AuthorizeAttribute>().Any();
        }
    }
}