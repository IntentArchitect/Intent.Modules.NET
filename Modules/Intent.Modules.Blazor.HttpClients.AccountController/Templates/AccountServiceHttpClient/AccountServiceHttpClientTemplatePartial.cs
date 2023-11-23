using System;
using System.Collections.Generic;
using Intent.Engine;
using Intent.Modules.Blazor.HttpClients.AccountController.Templates.AccountServiceInterface;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.Blazor.HttpClients.AccountController.Templates.AccountServiceHttpClient
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class AccountServiceHttpClientTemplate : CSharpTemplateBase<object>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.Blazor.HttpClients.AccountController.AccountServiceHttpClientTemplate";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public AccountServiceHttpClientTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            AddNugetDependency(NuGetPackages.MicrosoftExtensionsHttp);

            AddTypeSource(AccountServiceInterfaceTemplate.TemplateId);

            CSharpFile = new CSharpFile(this.GetNamespace("AccountService"), this.GetFolderPath("AccountService"))
                .AddUsing("System.Net.Http")
                .AddUsing("System.Net.Http.Headers")
                .AddUsing("System.Text")
                .AddUsing("System.Text.Json")
                .AddUsing("System.Threading")
                .AddUsing("System.Threading.Tasks")
                .AddClass("AccountServiceHttpClient", @class =>
                {

                    @class.ImplementsInterface(this.GetAccountServiceInterfaceTemplateName())
                        .AddField("JsonSerializerOptions", "_serializerOptions", f => f.PrivateReadOnly())
                        .AddConstructor(constructor => constructor
                            .AddParameter("HttpClient", "httpClient", p => p.IntroduceReadonlyField())
                            .AddStatement(@"_serializerOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            };"));

                    @class.AddMethod("Task", "Register", method =>
                    {
                        const string relativeUri = "api/Account/Register";

                        method.Async()
                            .AddParameter("RegisterDto", "dto")
                            .AddParameter("CancellationToken", "cancellationToken", p => p.WithDefaultValue("default"));

                        method.AddStatement($"const string relativeUri = \"{relativeUri}\";");

                        method.AddStatement("var request = new HttpRequestMessage(HttpMethod.Post, relativeUri);", s => s.SeparatedFromPrevious());
                        method.AddStatement("request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(\"application/json\"));");

                        method.AddStatement("var content = JsonSerializer.Serialize(dto, _serializerOptions);", s => s.SeparatedFromPrevious());
                        method.AddStatement("request.Content = new StringContent(content, Encoding.Default, \"application/json\");");

                        method.AddStatementBlock("using (var response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false))", usingResponseBlock =>
                        {
                            usingResponseBlock.SeparatedFromPrevious();
                            usingResponseBlock.AddStatementBlock("if (!response.IsSuccessStatusCode)", s => s
                                .AddStatement($"throw await {GetTypeName(ExceptionTemplateId)}.Create(_httpClient.BaseAddress!, request, response, cancellationToken).ConfigureAwait(false);")
                            );
                        });
                    });

                    @class.AddMethod("Task<TokenResultDto>", "Login", method =>
                    {
                        const string relativeUri = "api/Account/Login";
                        const string returnType = "TokenResultDto";

                        method.Async()
                            .AddParameter("LoginDto", "dto")
                            .AddParameter("CancellationToken", "cancellationToken", p => p.WithDefaultValue("default"));

                        method.AddStatement($"const string relativeUri = \"{relativeUri}\";");

                        method.AddStatement("var request = new HttpRequestMessage(HttpMethod.Post, relativeUri);", s => s.SeparatedFromPrevious());
                        method.AddStatement("request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(\"application/json\"));");

                        method.AddStatement("var content = JsonSerializer.Serialize(dto, _serializerOptions);", s => s.SeparatedFromPrevious());
                        method.AddStatement("request.Content = new StringContent(content, Encoding.Default, \"application/json\");");

                        method.AddStatementBlock("using (var response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false))", usingResponseBlock =>
                        {
                            usingResponseBlock.SeparatedFromPrevious();
                            usingResponseBlock.AddStatementBlock("if (!response.IsSuccessStatusCode)", s => s
                                .AddStatement($"throw await {GetTypeName(ExceptionTemplateId)}.Create(_httpClient.BaseAddress!, request, response, cancellationToken).ConfigureAwait(false);")
                            );
                            usingResponseBlock.AddStatementBlock("await using (var contentStream = await response.Content.ReadAsStreamAsync(cancellationToken).ConfigureAwait(false))", usingContentStreamBlock =>
                            {
                                usingContentStreamBlock.SeparatedFromPrevious();
                                usingContentStreamBlock.AddStatement($"return (await JsonSerializer.DeserializeAsync<{returnType}>(contentStream, _serializerOptions, cancellationToken).ConfigureAwait(false))!;");
                            });
                        });
                    });

                    @class.AddMethod("Task<TokenResultDto>", "Refresh", method =>
                    {
                        const string relativeUri = "api/Account/Refresh";
                        const string returnType = "TokenResultDto";

                        method.Async()
                            .AddParameter("string", "refreshToken")
                            .AddParameter("CancellationToken", "cancellationToken", p => p.WithDefaultValue("default"));

                        method.AddStatement($"const string relativeUri = \"{relativeUri}\";");

                        method.AddStatement("var request = new HttpRequestMessage(HttpMethod.Post, relativeUri);", s => s.SeparatedFromPrevious());
                        method.AddStatement("request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(\"application/json\"));");

                        method.AddStatement(new CSharpObjectInitializerBlock("var dto = new RefreshTokenDto")
                            .AddInitStatement("RefreshToken", "refreshToken")
                            .WithSemicolon()
                            .SeparatedFromPrevious());
                        method.AddStatement("var content = JsonSerializer.Serialize(dto, _serializerOptions);");
                        method.AddStatement("request.Content = new StringContent(content, Encoding.Default, \"application/json\");");

                        method.AddStatementBlock("using (var response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false))", usingResponseBlock =>
                        {
                            usingResponseBlock.SeparatedFromPrevious();
                            usingResponseBlock.AddStatementBlock("if (!response.IsSuccessStatusCode)", s => s
                                .AddStatement($"throw await {GetTypeName(ExceptionTemplateId)}.Create(_httpClient.BaseAddress!, request, response, cancellationToken).ConfigureAwait(false);")
                            );

                            usingResponseBlock.AddStatementBlock("await using (var contentStream = await response.Content.ReadAsStreamAsync(cancellationToken).ConfigureAwait(false))", usingContentStreamBlock =>
                            {
                                usingContentStreamBlock.SeparatedFromPrevious();
                                usingContentStreamBlock.AddStatement($"return (await JsonSerializer.DeserializeAsync<{returnType}>(contentStream, _serializerOptions, cancellationToken).ConfigureAwait(false))!;");
                            });
                        });
                    });

                    @class.AddMethod("Task", "ConfirmEmail", method =>
                    {
                        const string relativeUri = "api/Account/ConfirmEmail";

                        method.Async()
                            .AddParameter("ConfirmEmailDto", "dto")
                            .AddParameter("CancellationToken", "cancellationToken", p => p.WithDefaultValue("default"));

                        method.AddStatement($"const string relativeUri = \"{relativeUri}\";");

                        method.AddStatement("var request = new HttpRequestMessage(HttpMethod.Post, relativeUri);", s => s.SeparatedFromPrevious());
                        method.AddStatement("request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(\"application/json\"));");

                        method.AddStatement("var content = JsonSerializer.Serialize(dto, _serializerOptions);", s => s.SeparatedFromPrevious());
                        method.AddStatement("request.Content = new StringContent(content, Encoding.Default, \"application/json\");");

                        method.AddStatementBlock("using (var response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false))", usingResponseBlock =>
                        {
                            usingResponseBlock.SeparatedFromPrevious();
                            usingResponseBlock.AddStatementBlock("if (!response.IsSuccessStatusCode)", s => s
                                .AddStatement($"throw await {GetTypeName(ExceptionTemplateId)}.Create(_httpClient.BaseAddress!, request, response, cancellationToken).ConfigureAwait(false);")
                            );
                        });
                    });

                    @class.AddMethod("Task", "Logout", method =>
                    {
                        const string relativeUri = "api/Account/Logout";

                        method.Async()
                            .AddParameter("CancellationToken", "cancellationToken", p => p.WithDefaultValue("default"));

                        method.AddStatement($"const string relativeUri = \"{relativeUri}\";");

                        method.AddStatement("var request = new HttpRequestMessage(HttpMethod.Post, relativeUri);", s => s.SeparatedFromPrevious());
                        method.AddStatement("request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(\"application/json\"));");

                        method.AddStatementBlock("using (var response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false))", usingResponseBlock =>
                        {
                            usingResponseBlock.SeparatedFromPrevious();
                            usingResponseBlock.AddStatementBlock("if (!response.IsSuccessStatusCode)", s => s
                                .AddStatement($"throw await {GetTypeName(ExceptionTemplateId)}.Create(_httpClient.BaseAddress!, request, response, cancellationToken).ConfigureAwait(false);")
                            );
                        });

                    });

                    @class.AddMethod("void", "Dispose");
                });

            CSharpFile.AddClass("TokenResultDto", dto =>
            {
                dto.AddProperty("string?", "AuthenticationToken");
                dto.AddProperty("string?", "RefreshToken");
            });

            CSharpFile.AddClass("RegisterDto", dto =>
            {
                dto.AddProperty("string?", "Email");
                dto.AddProperty("string?", "Password");
            });

            CSharpFile.AddClass("LoginDto", dto =>
            {
                dto.AddProperty("string?", "Email");
                dto.AddProperty("string?", "Password");
            });

            CSharpFile.AddClass("ConfirmEmailDto", dto =>
            {
                dto.AddProperty("string?", "UserId");
                dto.AddProperty("string?", "Code");
            });

            CSharpFile.AddClass("RefreshTokenDto", dto =>
            {
                dto.AddProperty("string?", "RefreshToken");
            });
        }

        public override bool CanRunTemplate()
        {
            var template = ExecutionContext.FindTemplateInstance<IClassProvider>("Intent.Blazor.HttpClients.HttpClientRequestException");
            //Idea here is we can use this for different techs , this is Blazer implementation
            if (template != null)
            {
                ExceptionTemplateId = template.Id;
            }
            return template != null;
        }

        public string ExceptionTemplateId { get; set; }

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