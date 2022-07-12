using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.DependencyInjection;
using Intent.Modules.Common.CSharp.Templates;
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

        private void HandleServiceConfigurationRequest(ServiceConfigurationRequest request)
        {
            _serviceConfigurations.Add(request);
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        protected override CSharpFileConfig DefineFileConfig()
        {
            return new CSharpFileConfig(
                className: $"Startup",
                @namespace: $"{this.GetNamespace()}",
                relativeLocation: $"{this.GetFolderPath()}");
        }

        private string GetServiceConfigurationStatementList()
        {
            var statementList = new List<string>();

            statementList.AddRange(_serviceConfigurations
                .OrderBy(o => o.Priority)
                .Select(s => $"builder.Services.{s.ExtensionMethodName}({(s.SupplyConfiguration ? "configuration" : string.Empty)});"));

            const string newLine = @"
            ";
            return string.Join(newLine, statementList);
        }
    }
}