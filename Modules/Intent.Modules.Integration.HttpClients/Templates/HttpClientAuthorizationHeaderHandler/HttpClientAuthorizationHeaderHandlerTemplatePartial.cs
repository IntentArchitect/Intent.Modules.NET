using System;
using System.Collections.Generic;
using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.CSharp.VisualStudio;
using Intent.Modules.Common.Templates;
using Intent.Modules.Integration.HttpClients.Settings;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.Integration.HttpClients.Templates.HttpClientAuthorizationHeaderHandler
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class HttpClientAuthorizationHeaderHandlerTemplate : CSharpTemplateBase<object>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.Integration.HttpClients.HttpClientAuthorizationHeaderHandler";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public HttpClientAuthorizationHeaderHandlerTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            string nullableChar = outputTarget.GetProject().NullableEnabled ? "?" : "";

            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddUsing("System")
                .AddUsing("System.Net.Http")
                .AddUsing("System.Threading")
                .AddUsing("System.Threading.Tasks")
                .AddUsing("Microsoft.AspNetCore.Http")
                .AddUsing("Microsoft.Extensions.Primitives")
                .AddUsing("Microsoft.Extensions.DependencyInjection")
                .IntentManagedFully()
                .AddClass("HttpClientAuthorizationHeaderHandler", @class => @class
                    .WithBaseType("DelegatingHandler")
                    .AddConstructor(c => c
                        .AddParameter(this.GetAuthorizationHeaderProviderInterfaceName(), "authorizationHeaderProvider", param => param.IntroduceReadonlyField())
                    )
                    .AddMethod($"Task<HttpResponseMessage>", "SendAsync", m => m
                        .Protected()
                        .Async()
                        .Override()
                        .AddParameter("HttpRequestMessage", "request")
                        .AddParameter("CancellationToken", "cancellationToken")
                        .AddStatements(@"SetHeader(request, ""Authorization"", _authorizationHeaderProvider.GetAuthorizationHeader());           

            // Call the base SendAsync method to continue the request
            return await base.SendAsync(request, cancellationToken);".ConvertToStatements()))
                    .AddMethod($"void", "SetHeader", m => m
                        .Private()
                        .AddParameter("HttpRequestMessage", "request")
                        .AddParameter("string", "key")
                        .AddParameter($"string?", "value")
                        .AddStatements(@"
            var hasContent = request.Content != null;

            if (!request.Headers.TryAddWithoutValidation(key, value) && hasContent)
            {
                request.Content!.Headers.TryAddWithoutValidation(key, value);
            }".ConvertToStatements())))
                .AddClass("HttpClientAuthorizationHeaderHandlerExtensions", @class => @class
                    .Static()
                    .AddMethod("IHttpClientBuilder", "AddAuthorizationHeader", method => method
                        .Static()
                        .AddParameter("IHttpClientBuilder", "builder", p => p.WithThisModifier())
                            .AddStatements($@"
            builder.AddHttpMessageHandler(services =>
            {{
                return new HttpClientAuthorizationHeaderHandler(services.GetRequiredService<{this.GetAuthorizationHeaderProviderInterfaceName()}>());
            }});
            return builder;".ConvertToStatements())

                    )
                )
            ;
        }

        public override bool CanRunTemplate()
        {
            return base.CanRunTemplate() && ExecutionContext.Settings.GetIntegrationHttpClientSettings().AuthorizationSetup().IsAuthorizationHeaderProvider();
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