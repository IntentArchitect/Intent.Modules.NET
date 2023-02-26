using System.Collections.Generic;
using System.Linq;
using System.Text;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modules.Application.Contracts.Clients.Templates;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.DependencyInjection;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.Dapr.AspNetCore.ServiceInvocation.Templates.HttpClient;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.Dapr.AspNetCore.ServiceInvocation.Templates.DaprConfiguration
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class DaprConfigurationTemplate : CSharpTemplateBase<object>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.Dapr.AspNetCore.ServiceInvocation.DaprConfiguration";
        private readonly bool _canRunTemplate;

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public DaprConfigurationTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            var templateInstances = ExecutionContext.FindTemplateInstances<HttpClientTemplate>(TemplateDependency.OfType<HttpClientTemplate>())
                .ToArray();

            _canRunTemplate = templateInstances.Any();

            AddNugetDependency(NuGetPackages.DaprAspNetCore);
            ExecutionContext.GetApplicationConfig("3d4af4d8-a5ce-44e5-b461-87b9bb07d1cd");
            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .IntentManagedFully()
                .AddUsing("Dapr.Client")
                .AddUsing("Microsoft.Extensions.DependencyInjection")
                .AddClass("DaprConfiguration", @class =>
                {
                    @class.Static();
                    @class.AddMethod("void", "AddDaprServices", method =>
                    {
                        method
                            .Static()
                            .AddParameter("IServiceCollection", "services", p => p.WithThisModifier());

                        foreach (var template in templateInstances)
                        {
                            var applicationName = ExecutionContext.GetDaprApplicationName(template.Model.InternalElement.MappedElement.ApplicationId);
                            var serviceContractName = this.GetServiceContractName(template.Model);
                            var serviceName = GetTypeName(template);

                            method.AddStatement($"services.AddSingleton<{serviceContractName}, {serviceName}>(" +
                                                $"_ => new {serviceName}(DaprClient.CreateInvokeHttpClient(\"{applicationName}\")));");
                        }
                    });
                });
        }

        public override void BeforeTemplateExecution()
        {
            base.BeforeTemplateExecution();

            if (!CanRunTemplate())
            {
                return;
            }

            ExecutionContext.EventDispatcher.Publish(ServiceConfigurationRequest
                .ToRegister("AddDaprServices")
                .HasDependency(this));
        }

        public override bool CanRunTemplate() => _canRunTemplate && base.CanRunTemplate();

        [IntentManaged(Mode.Fully)]
        public CSharpFile CSharpFile { get; }

        [IntentManaged(Mode.Fully)]
        protected override CSharpFileConfig DefineFileConfig()
        {
            return CSharpFile.GetConfig();
        }

        [IntentManaged(Mode.Fully)]
        public override string TransformText()
        {
            return CSharpFile.ToString();
        }
    }
}