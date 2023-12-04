using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.CSharp.VisualStudio;
using Intent.Templates;
using System;
using System.Collections.Generic;
using System.Text;

namespace Intent.Modules.Integration.HttpClients.Shared.Templates.HttpClientHeaderDelegatingHandler
{
    public abstract class HttpClientHeaderHandlerTemplateBase : CSharpTemplateBase<object>, ICSharpFileBuilderTemplate
    {
        protected HttpClientHeaderHandlerTemplateBase(string templateId, IOutputTarget outputTarget, object model = null) : base(templateId, outputTarget, model)
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
                .AddClass("HttpClientHeaderHandler", @class => @class
                    .WithBaseType("DelegatingHandler")
                    .ImplementsInterface("IHeaderConfiguration")
                    .AddField($"HttpRequestMessage{nullableChar}", "_request", f => f.Private())
                    .AddConstructor(c => c
                        .AddParameter("Action<IHeaderConfiguration>", "configureHeaders", param => param.IntroduceReadonlyField())
                        .AddParameter("IHttpContextAccessor", "httpContextAccessor", param => param.IntroduceReadonlyField())
                    )
                    .AddMethod($"Task<HttpResponseMessage>", "SendAsync", m => m
                        .Protected()
                        .Async()
                        .Override()
                        .AddParameter("HttpRequestMessage", "request")
                        .AddParameter("CancellationToken", "cancellationToken")
                        .AddStatements(@"
            _request = request;
            _configureHeaders(this);

            // Call the base SendAsync method to continue the request
            return await base.SendAsync(request, cancellationToken);".ConvertToStatements()))
                    .AddMethod($"void", "AddFromHeader", m => m
                        .AddParameter("string", "headerName")
                        .AddStatements(@"
            if (_httpContextAccessor.HttpContext == null)
            {
                return;
            }
            if (_httpContextAccessor.HttpContext!.Request.Headers.TryGetValue(headerName, out var value))
            {
                SetHeader(_request!, headerName, value);
            }".ConvertToStatements()))
                    .AddMethod($"void", "AddFromSession", m => m
                        .AddParameter("string", "sessionKey")
                        .AddParameter("string", "headerName")
                        .AddStatements(@"
            if (_httpContextAccessor.HttpContext == null)
            {
                return;
            }
            SetHeader(_request!, headerName, _httpContextAccessor.HttpContext!.Session.GetString(sessionKey));".ConvertToStatements())
                    )
                    .AddMethod($"void", "SetHeader", m => m
                        .Private()
                        .AddParameter("HttpRequestMessage", "request")
                        .AddParameter("string", "key")
                        .AddParameter($"string{nullableChar}", "value")
                        .AddStatements(@"
            if (string.IsNullOrEmpty(value))
            {
                return;
            }
            var hasContent = request.Content != null;

            if (!request.Headers.TryGetValues(key, out var _) &&
                !(hasContent && request.Content!.Headers.TryGetValues(key, out var _)))
            {
                if (!request.Headers.TryAddWithoutValidation(key, value) && hasContent)
                {
                    request.Content!.Headers.TryAddWithoutValidation(key, value);
                }
            }".ConvertToStatements()))
                    .AddMethod($"void", "SetHeader", m => m
                        .Private()
                        .AddParameter("HttpRequestMessage", "request")
                        .AddParameter("string", "key")
                        .AddParameter($"StringValues", "stringValues")
                        .AddStatements(@"
            if (StringValues.IsNullOrEmpty(stringValues))
            {
                return;
            }
            var hasContent = request.Content != null;

            if (!request.Headers.TryGetValues(key, out var _) &&
                !(hasContent && request.Content!.Headers.TryGetValues(key, out var _)))
            {
                if (stringValues.Count == 1)
                {
                    var value = stringValues.ToString();
                    if (!request.Headers.TryAddWithoutValidation(key, value) && hasContent)
                    {
                        request.Content!.Headers.TryAddWithoutValidation(key, value);
                    }
                }
                else
                {
                    var values = stringValues.ToArray();
                    if (!request.Headers.TryAddWithoutValidation(key, (System.Collections.Generic.IEnumerable<string?>)values) && hasContent)
                    {
                        request.Content!.Headers.TryAddWithoutValidation(key, (System.Collections.Generic.IEnumerable<string?>)values);
                    }
                }
            }".ConvertToStatements())
                    )
                )
            .AddInterface("IHeaderConfiguration", @interface => @interface
                .AddMethod("void", "AddFromHeader", method => method
                    .AddParameter("string", "headerName")
                )
                .AddMethod("void", "AddFromSession", method => method
                    .AddParameter("string", "sessionKey")
                    .AddParameter("string", "headerName")
                )
            )
            .AddClass("HttpClientExtensions", @class => @class
                .Static()
                .AddMethod("IHttpClientBuilder", "AddHeaders", method => method
                    .Static()
                    .AddParameter("IHttpClientBuilder", "builder", p => p.WithThisModifier())
                    .AddParameter("Action<IHeaderConfiguration>", "configureHeaders")
                        .AddStatements(@"
            builder.AddHttpMessageHandler(services =>
            {
                return new HttpClientHeaderHandler(configureHeaders, services.GetRequiredService<IHttpContextAccessor>());
            });
            return builder;".ConvertToStatements())

                    )
                )               
            ;
        }

        public CSharpFile CSharpFile { get; }

        protected override CSharpFileConfig DefineFileConfig() => new CSharpFileConfig("HttpClientHeaderHandler", CSharpFile.Namespace, CSharpFile.RelativeLocation);

        public override string TransformText() => CSharpFile.ToString();
    }
}