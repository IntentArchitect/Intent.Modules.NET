using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Modelers.Services.Api;
using Intent.Modelers.Types.ServiceProxies.Api;
using Intent.Modules.Application.Contracts.Clients.Templates.ServiceContract;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Configuration;
using Intent.Modules.Common.CSharp.DependencyInjection;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.Integration.HttpClients.Shared.Templates;
using Intent.Modules.Integration.HttpClients.Stubs.Templates.HttpClientStub;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.Integration.HttpClients.Stubs.Templates.StubHttpClientConfiguration
{
    [IntentManaged(Mode.Ignore)]
    public class StubHttpClientConfigurationTemplate : CSharpTemplateBase<IList<IServiceProxyModel>>, ICSharpFileBuilderTemplate
    {
        [IntentManaged(Mode.Fully)]
        public const string TemplateId = "Intent.Integration.HttpClients.Stubs.StubHttpClientConfiguration";

        private const string UseStubKey = "UseStub";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public StubHttpClientConfigurationTemplate(IOutputTarget outputTarget, IList<IServiceProxyModel> model)
            : base(TemplateId, outputTarget, model)
        {
            AddNugetDependency(NugetPackages.MicrosoftExtensionsConfigurationBinder(OutputTarget));
            AddNugetDependency(NugetPackages.MicrosoftExtensionsDependencyInjectionAbstractions(OutputTarget));
            AddTypeSource(ServiceContractTemplate.TemplateId);
            AddTypeSource(HttpClientStubTemplate.TemplateId);

            CSharpFile = new CSharpFile($"{this.GetNamespace()}.Configuration", "Configuration")
                .AddUsing("Microsoft.Extensions.Configuration")
                .AddUsing("Microsoft.Extensions.DependencyInjection")
                .AddUsing("Microsoft.Extensions.DependencyInjection.Extensions")
                .AddClass("StubHttpClientConfiguration", @class =>
                {
                    @class.Static();
                    @class.AddMethod("IServiceCollection", "AddStubHttpClients", method =>
                    {
                        method.Static()
                            .AddParameter("this IServiceCollection", "services")
                            .AddParameter("IConfiguration", "configuration");

                        foreach (var proxy in Model.DistinctBy(x => x.Id))
                        {
                            var contractName = GetTypeName(ServiceContractTemplate.TemplateId, proxy);
                            var stubName = GetTypeName(HttpClientStubTemplate.TemplateId, proxy);
                            var groupName = proxy.GetGroupName();
                            var serviceName = proxy.Name.ToPascalCase();

                            method.AddIfStatement($"UseStubHttpClient(configuration, \"{groupName}\", \"{serviceName}\")", @if =>
                            {
                                @if.AddStatement($"services.RemoveAll<{contractName}>();");
                                @if.AddStatement($"services.AddTransient<{contractName}, {stubName}>();");
                            });
                        }

                        method.AddStatement("return services;");
                    });

                    @class.AddMethod("bool", "UseStubHttpClient", method =>
                    {
                        method.Private()
                            .Static()
                            .AddParameter("IConfiguration", "configuration")
                            .AddParameter("string", "groupName")
                            .AddParameter("string", "serviceName");

                        method.AddReturn($$"""configuration.GetValue<bool?>($"{{HttpClientSettingsHelper.HttpClientsSection}}:{serviceName}:{{UseStubKey}}") ?? configuration.GetValue<bool?>($"{{HttpClientSettingsHelper.HttpClientsSection}}:{groupName}:{{UseStubKey}}") ?? false""");
                    });
                });
        }

        public override void BeforeTemplateExecution()
        {
            // Register the per-group UseStub application setting (defaults to false).
            foreach (var groupName in Model.Select(x => x.GetGroupName()).Distinct())
            {
                ExecutionContext.EventDispatcher.Publish(
                    new AppSettingRegistrationRequest(
                        HttpClientSettingsHelper.GetConfigKey(groupName, UseStubKey),
                        false));
            }

            // Emit `services.AddStubHttpClients(configuration);` into the composition root. A high priority
            // ensures it is ordered after AddInfrastructure, so the real registrations exist to be replaced.
            ExecutionContext.EventDispatcher.Publish(
                ServiceConfigurationRequest.ToRegister("AddStubHttpClients", ServiceConfigurationRequest.ParameterType.Configuration)
                    .WithPriority(100)
                    .HasDependency(this));
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
    }
}
