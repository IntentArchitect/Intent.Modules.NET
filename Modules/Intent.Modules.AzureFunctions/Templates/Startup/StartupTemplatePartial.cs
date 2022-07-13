using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.DependencyInjection;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.CSharp.TypeResolvers;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.AzureFunctions.Templates.Startup
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    partial class StartupTemplate : CSharpTemplateBase<object>
    {
        public const string TemplateId = "Intent.AzureFunctions.Startup";

        private readonly IList<ServiceConfigurationRequest> _serviceConfigurations =
            new List<ServiceConfigurationRequest>();

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public StartupTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            ExecutionContext.EventDispatcher.Subscribe<ServiceConfigurationRequest>(HandleServiceConfigurationRequest);
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        protected override CSharpFileConfig DefineFileConfig()
        {
            return new CSharpFileConfig(
                className: $"Startup",
                @namespace: $"{this.GetNamespace()}",
                relativeLocation: $"{this.GetFolderPath()}");
        }

        public override void BeforeTemplateExecution()
        {
            foreach (var request in GetRelevantServiceConfigurationRequests())
            {
                this.AddTypeSource(request.SourceConfigurationTemplate.Id);
                if (this.GetTypeInfo(request.SourceConfigurationTemplate.Id) is CSharpResolvedTypeInfo typeInfo)
                {
                    this.AddUsing(typeInfo.Namespace);
                }
            }
        }

        private void HandleServiceConfigurationRequest(ServiceConfigurationRequest request)
        {
            _serviceConfigurations.Add(request);
        }

        private IEnumerable<ServiceConfigurationRequest> GetRelevantServiceConfigurationRequests()
        {
            return _serviceConfigurations
                .Where(p => !p.IsHandled)
                .OrderBy(o => o.Priority)
                .ToArray();
        }

        private string GetServiceConfigurationStatementList()
        {
            var statementList = new List<string>();

            statementList.AddRange(GetRelevantServiceConfigurationRequests()
                .Select(s => $"builder.Services.{s.ExtensionMethodName}({GetExtensionMethodParameterList(s)});"));

            const string newLine = @"
            ";
            return string.Join(newLine, statementList);
        }

        private string GetExtensionMethodParameterList(ServiceConfigurationRequest request)
        {
            if (request.ExtensionMethodParameterList?.Any() != true)
            {
                return string.Empty;
            }

            var paramList = new List<string>();

            foreach (var param in request.ExtensionMethodParameterList)
            {
                switch (param)
                {
                    case ServiceConfigurationParameterType.Configuration:
                        paramList.Add("configuration");
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(
                            paramName: nameof(request.ExtensionMethodParameterList),
                            actualValue: param,
                            message: "Type specified in parameter list is not known or supported");
                }
            }

            return string.Join(", ", paramList);
        }
    }
}