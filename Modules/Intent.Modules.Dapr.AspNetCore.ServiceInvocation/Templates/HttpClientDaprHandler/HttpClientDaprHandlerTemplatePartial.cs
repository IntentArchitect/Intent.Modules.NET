using System;
using System.Collections.Generic;
using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.Dapr.AspNetCore.ServiceInvocation.Templates.HttpClientDaprHandler
{
    [IntentManaged(Mode.Fully, Body = Mode.Ignore, Signature = Mode.Ignore)]
    public partial class HttpClientDaprHandlerTemplate : CSharpTemplateBase<object>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.Dapr.AspNetCore.ServiceInvocation.HttpClientDaprHandlerTemplate";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public HttpClientDaprHandlerTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            AddNugetDependency(AspNetCore.NugetPackages.DaprClient(outputTarget));
            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddUsing("System.Linq")
                .AddUsing("System.Reflection")
                .AddUsing("System.Net.Http.Headers")
                .AddUsing("Dapr.Client")
                .AddUsing("Microsoft.Extensions.DependencyInjection")
                .AddClass($"HttpClientDaprExtensions", @class =>
                {
                    @class.Static();
                    @class.AddMethod("IHttpClientBuilder", "ConfigureForDapr", method =>
                    {
                        method
                            .Static()
                            .AddParameter("IHttpClientBuilder", "builder", p => p.WithThisModifier())
                            .AddStatement(@"builder.ConfigureHttpClient(http => http.DefaultRequestHeaders.UserAgent.Add(UserAgent()));
                builder.AddHttpMessageHandler(services =>
                {
                    return new InvocationHandler();
                });
                return builder;");
                    });
                    @class.AddMethod("ProductInfoHeaderValue", "UserAgent", method =>
                    {
                        method
                            .Static()
                            .Private()
                            .AddStatements(@"
                                var assembly = typeof(DaprClient).Assembly;
                                string assemblyVersion = assembly
                                    .GetCustomAttributes<AssemblyInformationalVersionAttribute>()
                                    .FirstOrDefault()?
                                    .InformationalVersion;

                                return new ProductInfoHeaderValue(""dapr-sdk-dotnet"", $""v{assemblyVersion}"");".ConvertToStatements());
                    });
                });
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