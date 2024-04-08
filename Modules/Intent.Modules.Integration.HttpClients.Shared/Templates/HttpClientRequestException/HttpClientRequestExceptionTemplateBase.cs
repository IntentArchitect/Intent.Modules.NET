using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.CSharp.VisualStudio;
using Intent.Modules.Common.VisualStudio;
using System.Reflection.PortableExecutable;

namespace Intent.Modules.Integration.HttpClients.Shared.Templates.HttpClientRequestException
{
    public abstract class HttpClientRequestExceptionTemplateBase : CSharpTemplateBase<object>, ICSharpFileBuilderTemplate
    {
        protected HttpClientRequestExceptionTemplateBase(string templateId, IOutputTarget outputTarget, object model = null) : base(templateId, outputTarget, model)
        {
            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddUsing("System")
                .AddUsing("System.Collections.Generic")
                .AddUsing("System.Linq")
                .AddUsing("System.Net")
                .AddUsing("System.Net.Http")
                .AddUsing("System.Threading")
                .AddUsing("System.Threading.Tasks")
                .AddUsing("Intent.RoslynWeaver.Attributes")
                .IntentManagedFully()
                .AddClass("HttpClientRequestException", @class => @class
                    .WithBaseType("Exception")
                    .AddProperty("ProblemDetailsWithErrors?", "ProblemDetails", prop => prop
                        .PrivateSetter()
                    )
                    .AddConstructor(c => c
                        .AddParameter("Uri", "requestUri", param => param.IntroduceProperty(prop => prop.PrivateSetter()))
                        .AddParameter("HttpStatusCode", "statusCode", param => param.IntroduceProperty(prop => prop.PrivateSetter()))
                        .AddParameter("IReadOnlyDictionary<string, IEnumerable<string>>", "responseHeaders", param => param.IntroduceProperty(prop => prop.PrivateSetter()))
                        .AddParameter("string?", "reasonPhrase", param => param.IntroduceProperty(prop => prop.PrivateSetter()))
                        .AddParameter("string", "responseContent", param => param.IntroduceProperty(prop => prop.PrivateSetter()))
                        .CallsBase(b => b
                            .AddArgument("GetMessage(requestUri, statusCode, reasonPhrase, responseContent)")
                        )
                        .AddStatements(new[]
                        {
                            $"if (responseContent is not null &&",
                            $"    responseHeaders.TryGetValue(\"Content-Type\", out var contentTypeValues) &&",
                            $"    contentTypeValues.FirstOrDefault() == \"application/problem+json; charset=utf-8\")",
                            $"{{",
                            $"    ProblemDetails = System.Text.Json.JsonSerializer.Deserialize<ProblemDetailsWithErrors>(responseContent);",
                            $"}}",
                        })
                    )
                    .AddMethod($"Task<{@class.Name}>", "Create", m => m
                        .Static()
                        .Async()
                        .AddParameter("Uri", "baseAddress")
                        .AddParameter("HttpRequestMessage", "request")
                        .AddParameter("HttpResponseMessage", "response")
                        .AddParameter("CancellationToken", "cancellationToken")
                        .AddStatements(new[]
                        {
                            "var fullRequestUri = new Uri(baseAddress, request.RequestUri!);",
                            $"var content = await response.Content.{GetReadAsStringAsyncMethodCall()}.ConfigureAwait(false);",
                            "",
                            "var headers = response.Headers.ToDictionary(k => k.Key, v => v.Value);",
                            "var contentHeaders = response.Content.Headers.ToDictionary(k => k.Key, v => v.Value); ",
                            "var allHeaders = headers",
                            "    .Concat(contentHeaders)",
                            "    .GroupBy(kvp => kvp.Key)",
                            "    .ToDictionary(group => group.Key, group => group.Last().Value);",
                            "",
                            $"return new {@class.Name}(fullRequestUri, response.StatusCode, allHeaders, response.ReasonPhrase, content);",
                        })
                    )
                    .AddMethod("string", "GetMessage", m => m
                        .Private()
                        .Static()
                        .AddParameter("Uri", "requestUri")
                        .AddParameter("HttpStatusCode", "statusCode")
                        .AddParameter("string?", "reasonPhrase")
                        .AddParameter("string", "responseContent")
                        .AddStatement("var message = $\"Request to {requestUri} failed with status code {(int)statusCode} {reasonPhrase}.\";")
                        .AddStatementBlock("if (!string.IsNullOrWhiteSpace(responseContent))", sb => sb
                            .AddStatement("message += \" See content for more detail.\";")
                        )
                        .AddStatement("return message;")
                    )
                );
        }

        private string GetReadAsStringAsyncMethodCall()
        {
            return OutputTarget switch
            {
                _ when OutputTarget.GetProject().TargetFramework().StartsWith("netstandard") => "ReadAsStringAsync()",
                _ => "ReadAsStringAsync(cancellationToken)"
            };
        }


        public CSharpFile CSharpFile { get; }

        protected override CSharpFileConfig DefineFileConfig() => CSharpFile.GetConfig();

        public override string TransformText() => CSharpFile.ToString();
    }
}