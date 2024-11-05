using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Modules.AspNetCore.MultiTenancy.Swashbuckle.TenantHeaderOperationFilter", Version = "1.0")]

namespace MassTransitFinbuckle.Test.Api.Filters
{
    public class TenantHeaderOperationFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            operation.Parameters ??= new List<OpenApiParameter>();

            operation.Parameters.Add(new OpenApiParameter
            {
                Name = "X-Tenant-Identifier",
                In = ParameterLocation.Header,
                Description = "Tenant Id",
                Required = false
            });
        }
    }
}