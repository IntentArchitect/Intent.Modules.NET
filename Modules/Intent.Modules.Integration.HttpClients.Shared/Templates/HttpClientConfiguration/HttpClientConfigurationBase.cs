﻿using Intent.Engine;
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
using System.Net.Http.Headers;
using Intent.Modules.Integration.HttpClients.Shared.Templates.Adapters;

namespace Intent.Modules.Integration.HttpClients.Shared.Templates.HttpClientConfiguration
{
    public abstract class HttpClientConfigurationBase : CSharpTemplateBase<IList<IServiceProxyModel>>, ICSharpFileBuilderTemplate
    {
        public HttpClientConfigurationBase(
            string templateId,
            IOutputTarget outputTarget,
            IList<ServiceProxyModel> model,
            string serviceContractTemplateId,
            string httpClientTemplateId,
            Action<CSharpLambdaBlock, IServiceProxyModel, ICSharpFileBuilderTemplate> configureHttpClient
            ) : this(
                    templateId,
                    outputTarget,
                    model.Select(x => new ServiceProxyModelAdapter(x)).Cast<IServiceProxyModel>().ToList(),
                    serviceContractTemplateId,
                    httpClientTemplateId,
                    configureHttpClient
                )
        {
        }

        private HttpClientConfigurationBase(
            string templateId,
            IOutputTarget outputTarget,
            IList<IServiceProxyModel> model,
            string serviceContractTemplateId,
            string httpClientTemplateId,
            Action<CSharpLambdaBlock, IServiceProxyModel, ICSharpFileBuilderTemplate > configureHttpClient
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
                            method.AddMethodChainStatement("services", statement =>
                            {
                                statement
                                    .SeparatedFromPrevious()
                                    .AddMetadata("model", proxy);

                                statement
                                    .AddChainStatement(new CSharpInvocationStatement($"AddHttpClient<{GetTypeName(serviceContractTemplateId, proxy)}, {GetTypeName(httpClientTemplateId, proxy)}>")
                                        .AddArgument(new CSharpLambdaBlock("http"), a =>
                                        {
                                            var options = (CSharpLambdaBlock)a;
                                            configureHttpClient(options, proxy, this);
                                        }).WithoutSemicolon());
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
