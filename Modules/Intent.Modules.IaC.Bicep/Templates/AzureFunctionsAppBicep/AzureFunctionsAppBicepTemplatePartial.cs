using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Configuration;
using Intent.Modules.Common.Templates;
using Intent.Modules.Constants;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.ProjectItemTemplate.Partial", Version = "1.0")]

namespace Intent.Modules.IaC.Bicep.Templates.AzureFunctionsAppBicep
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    partial class AzureFunctionsAppBicepTemplate : IntentTemplateBase<object>
    {
        private readonly List<InfrastructureRegisteredEvent> _infrastructureEvents = [];
        private readonly List<AppSettingRegistrationRequest> _appSettingsRequests = [];
        private readonly Dictionary<string, string> _appSettingsRegistration = new();

        [IntentManaged(Mode.Fully)]
        public const string TemplateId = "Intent.IaC.Bicep.AzureFunctionsAppBicepTemplate";

        [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
        public AzureFunctionsAppBicepTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            ExecutionContext.EventDispatcher.Subscribe<AppSettingRegistrationRequest>(Handle);
            ExecutionContext.EventDispatcher.Subscribe<InfrastructureRegisteredEvent>(Handle);
        }

        private void Handle(InfrastructureRegisteredEvent @event)
        {
            _infrastructureEvents.Add(@event);
        }

        private void Handle(AppSettingRegistrationRequest request)
        {
            _appSettingsRequests.Add(request);
        }

        public override bool CanRunTemplate()
        {
            return ExecutionContext.FindTemplateInstances<ITemplate>(TemplateRoles.Distribution.AzureFunctions.AzureFunctionEndpoint).Any();
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public override ITemplateFileConfig GetTemplateFileConfig()
        {
            return new TemplateFileConfig(
                fileName: $"azure_functions",
                fileExtension: "bicep"
            );
        }

        private string GetAzureFunctionsAppSettings(string padding)
        {
            var input = new Dictionary<string, string>();
            input["APPINSIGHTS_INSTRUMENTATIONKEY"] = "appInsights.properties.InstrumentationKey";
            input["AzureWebJobsStorage"] = "storageConnectionString";
            foreach (var request in _appSettingsRequests)
            {
                input.TryAdd(request.Key, request.Value.ToString() ?? "");
            }
            
            var sb = new StringBuilder();

            sb.AppendLine("[");
            
            foreach (var entry in input)
            {
                sb.AppendLine($"{padding}  {{");
                sb.AppendLine($"{padding}    name:  '{entry.Key}'");
                sb.AppendLine($"{padding}    value: {GetAppSettingsValue(entry.Key, entry.Value)}");
                sb.AppendLine($"{padding}  }}");
            }

            sb.Append($"{padding}]");
            
            return sb.ToString();
        }

        private string GetAppSettingsValue(string key, string value)
        {
            return _appSettingsRegistration.TryGetValue(key, out var v) ? v : $"'{value}'";
        }

        private string GetScriptParameters()
        {
            var sb = new StringBuilder();

            var sanitizedAppName = ExecutionContext.GetApplicationConfig().Name.ToKebabCase();
            sb.AppendLine($$"""param functionAppName string = '{{sanitizedAppName}}-${uniqueString(resourceGroup().id)}'""");
            
            

            return sb.ToString();
        }
    }
}