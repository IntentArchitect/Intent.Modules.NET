using System;
using System.Linq;
using Intent.Engine;
using Intent.Modules.Blazor.HttpClients.Templates;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Plugins;
using Intent.Modules.Constants;
using Intent.Plugins.FactoryExtensions;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.FactoryExtension", Version = "1.0")]

namespace Intent.Modules.Blazor.HttpClients.FactoryExtensions
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class WebAssemblyProgramIntegrationFactoryExtension : FactoryExtensionBase
    {
        public override string Id => "Intent.Blazor.HttpClients.WebAssemblyProgramIntegrationFactoryExtension";

        [IntentManaged(Mode.Ignore)]
        public override int Order => 0;

        protected override void OnAfterTemplateRegistrations(IApplication application)
        {
            base.OnAfterTemplateRegistrations(application);
            RegisterHttpClients(application);
        }

        private void RegisterHttpClients(IApplication application)
        {
            var template = application.FindTemplateInstance<ICSharpFileBuilderTemplate>("Intent.Blazor.WebAssembly.ProgramTemplate");
            if (template is null)
            {
                return;
            }

            template.CSharpFile.OnBuild(file =>
            {
                template.UseType(template.GetHttpClientConfigurationName());

                var @class = file.Classes.First();
                var mainMethod = @class.FindMethod("Main");
                if (mainMethod == null)
                    return;
                var lastStatement = mainMethod.FindStatement(s => s.HasMetadata("run-builder"));
                if (lastStatement == null)
                    return;
                lastStatement.InsertAbove("builder.Services.AddHttpClients(builder.Configuration);");
            });
        }
    }
}