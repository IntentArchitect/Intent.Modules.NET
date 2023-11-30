using Intent.Engine;
using Intent.Modelers.Types.ServiceProxies.Api;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Templates;
using System;
using System.Collections.Generic;
using System.Text;
using Intent.Modules.Common.CSharp.Configuration;
using Intent.Modules.Common.CSharp.DependencyInjection;
using Intent.Modules.Common.Templates;
using System.Linq;
using Intent.Modules.Metadata.WebApi.Models;

namespace Intent.Modules.Integration.HttpClients.Shared.Templates.HttpClientConfiguration
{
    public abstract class HttpClientConfigurationBase : CSharpTemplateBase<IList<ServiceProxyModel>>, ICSharpFileBuilderTemplate
    {
        public HttpClientConfigurationBase(
            string templateId,
            IOutputTarget outputTarget,
            IList<ServiceProxyModel> model,
            string serviceContractTemplateId,
            string httpClientTemplateId,
            Action<CSharpLambdaBlock, ServiceProxyModel> configureHttpClient
            ) : base(templateId, outputTarget, model)
        {
            AddNugetDependency(NuGetPackages.IdentityModelAspNetCore);
            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddUsing("System")
                .AddUsing("Microsoft.Extensions.Configuration")
                .AddUsing("Microsoft.Extensions.DependencyInjection")
                .AddClass("HttpClientConfiguration", @class =>
                {
                    @class.Static();
                    @class.AddMethod("void", "AddHttpClients", method =>
                    {
                        method
                            .Static()
                            .AddParameter("IServiceCollection", "services", p => p.WithThisModifier())
                            .AddParameter("IConfiguration", "configuration")
                            ;

                        foreach (var proxy in model)
                        {
                            method.AddInvocationStatement($"services.AddHttpClient<{GetTypeName(serviceContractTemplateId, proxy)}, {GetTypeName(httpClientTemplateId, proxy)}>", invocation =>
                            {
                                invocation.SeparatedFromPrevious();
                                invocation.AddMetadata("model", proxy);
                                invocation.AddArgument(new CSharpLambdaBlock("http"), a =>
                                {
                                    var options = (CSharpLambdaBlock)a;
                                    configureHttpClient(options, proxy);
                                });
                            });
                        }
                    });
                });

        }

        public override void BeforeTemplateExecution()
        {
            ExecutionContext.EventDispatcher.Publish(ServiceConfigurationRequest
                .ToRegister("AddHttpClients", ServiceConfigurationRequest.ParameterType.Configuration)
                .ForConcern("Infrastructure")
                .HasDependency(this));

            ExecutionContext.EventDispatcher.Publish(new AppSettingRegistrationRequest("IdentityClients:default:Address", "https://localhost:{sts_port}/connect/token"));
            ExecutionContext.EventDispatcher.Publish(new AppSettingRegistrationRequest("IdentityClients:default:ClientId", "clientId"));
            ExecutionContext.EventDispatcher.Publish(new AppSettingRegistrationRequest("IdentityClients:default:ClientSecret", "secret"));
            ExecutionContext.EventDispatcher.Publish(new AppSettingRegistrationRequest("IdentityClients:default:Scope", "api"));
        }


        public CSharpFile CSharpFile { get; }

        protected override CSharpFileConfig DefineFileConfig()
        {
            return new CSharpFileConfig(
                className: $"HttpClientConfiguration",
                @namespace: $"{this.GetNamespace()}",
                relativeLocation: $"{this.GetFolderPath()}");
        }

        public override string TransformText() => CSharpFile.ToString();
    }
}
