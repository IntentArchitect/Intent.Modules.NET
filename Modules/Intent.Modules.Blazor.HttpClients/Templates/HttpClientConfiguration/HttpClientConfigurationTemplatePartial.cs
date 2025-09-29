using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.Modules.Common.Configuration;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.Constants;
using Intent.Modules.Contracts.Clients.Http.Shared;
using Intent.Modules.Contracts.Clients.Shared;
using Intent.Modules.Integration.HttpClients.Shared;
using Intent.Modules.Integration.HttpClients.Shared.Templates;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;
using static Intent.Modules.Constants.Roles;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.Blazor.HttpClients.Templates.HttpClientConfiguration
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class HttpClientConfigurationTemplate : CSharpTemplateBase<IList<IServiceProxyModel>>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.Blazor.HttpClients.HttpClientConfiguration";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public HttpClientConfigurationTemplate(IOutputTarget outputTarget, IList<IServiceProxyModel> model = null) : base(TemplateId, outputTarget, model)
        {
            if (model.Any(RequiresAuthorization))
            {
                AddNugetDependency("Microsoft.AspNetCore.Components.WebAssembly.Authentication", "6.0.20");
            }

            HostingSettingsCreatedEvent hostSettings = null;
            ExecutionContext.EventDispatcher.Subscribe<HostingSettingsCreatedEvent>(x =>
            {
                hostSettings = x;
            });


            var uniqueApplicationNames = new HashSet<string>();
            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddUsing("Microsoft.Extensions.DependencyInjection")
                .AddAssemblyAttribute("[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]")
                .AddClass($"HttpClientConfiguration", @class =>
                {
                    @class.Static();

                    @class.AddMethod("void", "AddHttpClients", method =>
                    {
                        method.Static();
                        method.AddParameter(UseType("Microsoft.Extensions.DependencyInjection.IServiceCollection"), "services", p => p.WithThisModifier());
                        method.AddParameter(UseType("Microsoft.Extensions.Configuration.IConfiguration"), "configuration");

                        foreach (var proxy in Model)
                        {
                            method.AddMethodChainStatement("services", chain =>
                            {
                                var applicationName = GetApplicationName(proxy);

                                uniqueApplicationNames.Add(applicationName);

                                chain.AddChainStatement(new CSharpInvocationStatement($"AddHttpClient<{this.GetServiceContractName(proxy)}, {this.GetHttpClientName(proxy)}>")
                                    .AddArgument(new CSharpLambdaBlock("http")
                                        .AddStatement($"http.BaseAddress = GetUrl(configuration, \"{applicationName}\");")
                                    )
                                    .WithoutSemicolon()
                                );

                                if (RequiresAuthorization(proxy))
                                {
                                    var authorizationMessageHandlerTypeName = UseType("Microsoft.AspNetCore.Components.WebAssembly.Authentication.AuthorizationMessageHandler");

                                    chain.AddChainStatement(new CSharpInvocationStatement("AddHttpMessageHandler")
                                        .AddArgument(new CSharpLambdaBlock("sp")
                                            .AddStatement(@$"return sp.GetRequiredService<{authorizationMessageHandlerTypeName}>()
                        .ConfigureHandler(
                            authorizedUrls: new[] {{ GetUrl(configuration, ""{applicationName}"").AbsoluteUri }});")
                                        )
                                        .WithoutSemicolon()
                                    );
                                }
                            });
                        }
                    });

                    @class.AddMethod(UseType("System.Uri"), "GetUrl", method =>
                    {
                        method.Private().Static();

                        method.AddParameter(UseType("Microsoft.Extensions.Configuration.IConfiguration"), "configuration");
                        method.AddParameter("string", "applicationName");

                        method.AddStatement("var url = configuration.GetValue<Uri?>($\"Urls:{applicationName}\");");

                        method.AddStatement(
                            $"return url ?? throw new {UseType("System.Exception")}($\"Configuration key \\\"Urls:{{applicationName}}\\\" is not set\");",
                            s => s.SeparatedFromPrevious());
                    });
                })
                .AfterBuild(file =>
                {
                    var template = file.Template as HttpClientConfigurationTemplate;

                    foreach (var applicationName in uniqueApplicationNames)
                    {
                        var proxy = template.Model.FirstOrDefault(m => template.GetApplicationName(m) == applicationName);
                        var proxyUrl = template.GetProxyUrl(proxy);

                        if (string.IsNullOrWhiteSpace(proxyUrl))
                        {
                            proxyUrl = $"https://localhost:{hostSettings.SslPort}/";
                        }

                        this.ApplyAppSetting($"Urls:{applicationName}", hostSettings != null ? proxyUrl : "", null, Frontend.Blazor);
                        this.ApplyAppSetting($"Urls:{applicationName}", hostSettings != null ? proxyUrl : "", null, "Startup");
                    }
                });
        }

        public string GetProxyUrl(IServiceProxyModel proxy)
        {
            var url = string.Empty;

            var package = proxy.InternalElement?.Package;
            // this if for if the service is not defined in a folder, then the proxy.InternalElement is null. In which 
            // case we revert to trying to get the package from the first element on the proxy
            package = package is null && proxy.Endpoints.Any() ? proxy.Endpoints[0].InternalElement?.Package : package;

            if (package == null)
            {
                return url;
            }

            return ProxyUrlHelper.GetProxyApplicationtUrl(package, ExecutionContext);
        }

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

        public override RoslynMergeConfig ConfigureRoslynMerger() => ToFullyManagedUsingsMigration.GetConfig(Id, 2);

        private static bool RequiresAuthorization(IServiceProxyModel model)
        {
            return model.Endpoints.Any(x => x.RequiresAuthorization);
        }

        public string GetApplicationName(IServiceProxyModel model)
        {
            return string.Concat(model.Endpoints[0].InternalElement.Package.Name
                .RemoveSuffix(".Services")
                .Split('.')
                .Select(x => x.ToCSharpIdentifier()));
        }
    }
}