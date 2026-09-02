using System.Threading;
using Intent.Engine;
using Intent.Modules.Blazor.Api;
using Intent.Modules.Blazor.Authentication.Settings;
using Intent.Modules.Blazor.Authentication.Templates.Templates.Client.PersistentAuthenticationStateProvider;
using Intent.Modules.Blazor.Settings;
using Intent.Modules.Blazor.Templates.Templates.Client.Program;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
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
                    var httpClients = application.FindTemplateInstance<ICSharpFileBuilderTemplate>("Intent.Blazor.HttpClients.HttpClientConfiguration");

                    program.ProgramFile.ConfigureMainStatementsBlock(main =>
                    {
                        main.FindStatement(x => x.HasMetadata("run-builder"))
                            ?.InsertAbove(new CSharpMethodChainStatement("builder.Services.AddCascadingAuthenticationState()").SeparatedFromNext())
                            ?.InsertAbove(new CSharpMethodChainStatement($"builder.Services.AddSingleton<AuthenticationStateProvider, {program.GetTypeName(PersistentAuthenticationStateProviderTemplate.TemplateId)}>()").SeparatedFromNext())
                            ?.InsertAbove(new CSharpMethodChainStatement($"builder.Services.AddApiAuthorization()").SeparatedFromNext());

                        // The server strips the AuthorizationMessageHandler that AddHttpClients used to
                        // attach, because AddHttpClients is shared by both hosts and that handler is
                        // WebAssembly-only. Re-attach it here so the browser keeps exactly the behaviour
                        // it had before. Gated on the same eagerly-set metadata the server uses, so the
                        // two hosts can never disagree: skipped when there are no service proxies, when
                        // none of them requires authorization, or when Intent.Blazor.HttpClients is an
                        // older version that emits no such method to call.
                        if (httpClients?.CSharpFile.HasMetadata("api-authorization-handler") == true)
                        {
                            // AddApiAuthorizationHandler is an extension method on the generated
                            // HttpClientConfiguration class, which only happens to share this file's namespace
                            // in the two-project WebAssembly layout - MinimalHostingModel puts it elsewhere.
                            program.AddUsing(httpClients.CSharpFile.Namespace);

                            var authorizationMessageHandlerTypeName = program.UseType("Microsoft.AspNetCore.Components.WebAssembly.Authentication.AuthorizationMessageHandler");

                            main.FindStatement(x => x.HasMetadata("run-builder"))
                                ?.InsertAbove(new CSharpMethodChainStatement($"builder.Services.AddApiAuthorizationHandler(builder.Configuration, (sp, urls) => sp.GetRequiredService<{authorizationMessageHandlerTypeName}>().ConfigureHandler(authorizedUrls: urls))").SeparatedFromNext());
                        }
                    });
                }
            });
        }
    }
}
