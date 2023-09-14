using Intent.Engine;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common;
using Intent.Modules.Common.Plugins;
using Intent.Modules.Common.Templates;
using Intent.Plugins.FactoryExtensions;
using Intent.RoslynWeaver.Attributes;
using System.Linq;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Blazor.HttpClients.AccountController.Templates.AccountServiceInterface;
using Intent.Modules.Blazor.HttpClients.AccountController.Templates.AccountServiceHttpClient;
using Intent.Modules.Blazor.HttpClients.AccountController.Templates;
using static Intent.Modules.Constants.Roles;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.FactoryExtension", Version = "1.0")]

namespace Intent.Modules.Blazor.HttpClients.AccountController.FactoryExtensions
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class HttpClientConfigurationFactoryExtension : FactoryExtensionBase
    {
        public override string Id => "Intent.Blazor.HttpClients.AccountController.HttpClientConfigurationFactoryExtension";

        [IntentManaged(Mode.Ignore)]
        public override int Order => 0;

        protected override void OnAfterTemplateRegistrations(IApplication application)
        {
            var template = application.FindTemplateInstance<ICSharpFileBuilderTemplate>(TemplateDependency.OnTemplate("Intent.Blazor.HttpClients.HttpClientConfiguration"));
            if (template != null)
            {
                template.CSharpFile.OnBuild(file =>
                {
                    template.AddNugetDependency(NuGetPackages.MicrosoftAspNetCoreComponentsWebAssemblyAuthentication);
                    template.AddTypeSource(AccountServiceInterfaceTemplate.TemplateId);
                    template.AddTypeSource(AccountServiceHttpClientTemplate.TemplateId);
                    var @class = file.Classes.First();
                    var method = @class.FindMethod("AddHttpClients");
                    method.AddMethodChainStatement("services", chain =>
                    {
                        var applicationName = "STSApplication";

                        ((IntentTemplateBase)template).ApplyAppSetting($"Urls:{applicationName}", "", null, Frontend.Blazor);


                        chain.AddChainStatement(new CSharpInvocationStatement($"AddHttpClient<{template.GetAccountServiceInterfaceTemplateName()}, {template.GetAccountServiceHttpClientTemplateName()}>")
                            .AddArgument(new CSharpLambdaBlock("http")
                                .AddStatement($"http.BaseAddress = GetUrl(configuration, \"{applicationName}\");")
                            )
                            .WithoutSemicolon()
                        );

                        var authorizationMessageHandlerTypeName = template.UseType("Microsoft.AspNetCore.Components.WebAssembly.Authentication.AuthorizationMessageHandler");

                        chain.AddChainStatement(new CSharpInvocationStatement("AddHttpMessageHandler")
                            .AddArgument(new CSharpLambdaBlock("sp")
                                .AddStatement(@$"return sp.GetRequiredService<{authorizationMessageHandlerTypeName}>()
                    .ConfigureHandler(
                        authorizedUrls: new[] {{ GetUrl(configuration, ""{applicationName}"").AbsoluteUri }});")
                            )
                            .WithoutSemicolon()
                        );
                    });
                });
            }
        }
    }
}