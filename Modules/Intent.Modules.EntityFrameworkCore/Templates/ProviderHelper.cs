using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.EntityFrameworkCore.Settings;
using Intent.Modules.Metadata.RDBMS.Settings;
using Intent.Utils;
using NuGet.Versioning;
using Serilog.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intent.Modules.EntityFrameworkCore.Templates
{
    internal class ProviderHelper
    {
        private static Dictionary<string, NuGetVersion> _sqlLiteMinumumRequirements = new Dictionary<string, NuGetVersion> { 
            { "Intent.Redis.Om.Repositories", new NuGetVersion("1.0.12-pre.0") },
            { "Intent.FastEndpoints.Dispatch.Services", new NuGetVersion("1.0.3-pre.0") },
            { "Intent.Eventing.Solace", new NuGetVersion("2.0.7-pre.0") },
            { "Intent.Eventing.MassTransit", new NuGetVersion("7.0.5-pre.0") },
            { "Intent.Eventing.MassTransit.RequestResponse", new NuGetVersion("1.1.8-pre.0") },
            { "Intent.Eventing.Kafka", new NuGetVersion("1.0.0-beta.25") },
            { "Intent.Eventing.AzureServiceBus", new NuGetVersion("1.1.3-pre.0") },
            { "Intent.Dapr.AspNetCore.Pubsub", new NuGetVersion("3.0.5-pre.0") },
            { "Intent.AzureFunctions.AzureServiceBus", new NuGetVersion("1.0.5-pre.0") },
            { "Intent.AzureFunctions.AzureEventGrid", new NuGetVersion("1.1.2-pre.0") },
            { "Intent.AspNetCore.Grpc", new NuGetVersion("1.0.0-beta.7") },
            { "Intent.AspNetCore.Controllers.Dispatch.ServiceContract", new NuGetVersion("5.2.15-pre.0") },
            { "Intent.Application.MediatR.Behaviours", new NuGetVersion("4.3.7-pre.0") },
        };

        internal static void ValidateModuleSetupForProvider(ICSharpFileBuilderTemplate template)
        {
            if (template.ExecutionContext.GetSettings().GetDatabaseSettings().DatabaseProvider().IsSqlLite())
            {
                var belowMinVersion = template.ExecutionContext.GetInstalledModules().Where(m => _sqlLiteMinumumRequirements.TryGetValue(m.ModuleId, out var version) && new NuGetVersion(m.Version) < version);
                if (belowMinVersion.Any())
                {
                    Logging.Log.Failure($"SQL Lite requires modules updates: {string.Join(",", belowMinVersion.Select(x => $"({x.ModuleId} {_sqlLiteMinumumRequirements[x.ModuleId]})") )}");
                }
            }
        }
    }
}
