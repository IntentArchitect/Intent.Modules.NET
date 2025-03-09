using System.Linq;
using Intent.Engine;
using Intent.Modules.AspNetCore.Grpc.Templates.GrpcConfiguration;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.AppStartup;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.Plugins;
using Intent.Modules.Common.Templates;
using Intent.Plugins.FactoryExtensions;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.FactoryExtension", Version = "1.0")]

namespace Intent.Modules.AspNetCore.Grpc.FactoryExtensions
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class GrpcFactoryExtension : FactoryExtensionBase
    {
        public override string Id => "Intent.AspNetCore.Grpc.GrpcFactoryExtension";

        [IntentManaged(Mode.Ignore)]
        public override int Order => 0;

        protected override void OnAfterTemplateRegistrations(IApplication application)
        {
            var template = application.FindTemplateInstance<IAppStartupTemplate>(IAppStartupTemplate.RoleName);
            ((IntentTemplateBase)template).AddTemplateDependency(GrpcConfigurationTemplate.TemplateId);

            template.CSharpFile.AfterBuild(_ =>
            {
                var startup = template.StartupFile;

                startup.AddServiceConfiguration(ctx => $"{ctx.Services}.ConfigureGrpc();");
                startup.ConfigureEndpoints((statements, ctx) =>
                {
                    statements.AddStatement($"{ctx.App}.MapGrpcServices();");
                });
            });

        }
    }
}