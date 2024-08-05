using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Abstractions;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Enums;
using Microsoft.OpenApi.Models;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.AzureFunctions.OpenApi.OpenApiConfigurarationTemplate", Version = "1.0")]

namespace AzureFunctions.TestApplication.Api.Configuration
{
    public class OpenApiConfigurationOptions : IOpenApiConfigurationOptions
    {
        public OpenApiConfigurationOptions()
        {
            Info = new OpenApiInfo
            {
                Title = "AzureFunctions.TestApplication API",
                Version = "1.0.0"
            };
        }

        public OpenApiInfo Info { get; set; }
        public List<OpenApiServer> Servers { get; set; } = new();
        public OpenApiVersionType OpenApiVersion { get; set; } = OpenApiVersionType.V3;
        public List<IDocumentFilter> DocumentFilters { get; set; }
        public bool IncludeRequestingHostName { get; set; } = false;
        public bool ForceHttp { get; set; } = true;
        public bool ForceHttps { get; set; } = false;
    }
}