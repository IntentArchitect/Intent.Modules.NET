using System.Threading;
using Intent.Engine;
using Intent.Modules.Blazor.Api;
using Intent.Modules.Blazor.Authentication.Settings;
using Intent.Modules.Blazor.Authentication.Templates.Templates.Client.PersistentAuthenticationStateProvider;
using Intent.Modules.Blazor.Settings;
using Intent.Modules.Blazor.Templates.Templates.Client.Program;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.Plugins;
using Intent.Plugins.FactoryExtensions;
using Intent.RoslynWeaver.Attributes;
using Intent.Utils;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.FactoryExtension", Version = "1.0")]

namespace Intent.Modules.Blazor.Authentication.FactoryExtensions
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class ClientAddAuthentication : FactoryExtensionBase
    {
        public override string Id => "Intent.Blazor.Authentication.ClientAddAuthentication";

        [IntentManaged(Mode.Ignore)]
        public override int Order => 0;

        protected override void OnAfterTemplateRegistrations(IApplication application)
        {
            var program = application.FindTemplateInstance<IBlazorProgramTemplate>(ProgramTemplate.TemplateId);

            if (program == null)
            {
                Logging.Log.Warning("Unable to install authentication. Program class could not be found.");
                return;
            }

            program.AddUsing("Microsoft.AspNetCore.Components.Authorization");

            program.CSharpFile.AfterBuild(_ =>
            {
                if (!program.ExecutionContext.GetSettings().GetBlazor().RenderMode().IsInteractiveServer())
                {
                    program.ProgramFile.ConfigureMainStatementsBlock(main =>
                    {
                        main.FindStatement(x => x.HasMetadata("run-builder"))
                            ?.InsertAbove(new CSharpMethodChainStatement("builder.Services.AddCascadingAuthenticationState()").SeparatedFromNext())
                            ?.InsertAbove(new CSharpMethodChainStatement($"builder.Services.AddSingleton<AuthenticationStateProvider, {program.GetTypeName(PersistentAuthenticationStateProviderTemplate.TemplateId)}>()").SeparatedFromNext())
                            ?.InsertAbove(new CSharpMethodChainStatement($"builder.Services.AddApiAuthorization()").SeparatedFromNext());
                    });
                }
            });

        }
    }
}
