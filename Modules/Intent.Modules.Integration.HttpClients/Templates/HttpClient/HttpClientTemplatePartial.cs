using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Modelers.Types.ServiceProxies.Api;
using Intent.Modules.Application.Contracts.Clients;
using Intent.Modules.Application.Contracts.Clients.Templates;
using Intent.Modules.Application.Contracts.Clients.Templates.DtoContract;
using Intent.Modules.Application.Contracts.Clients.Templates.ServiceContract;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.CSharp.TypeResolvers;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;
using OperationModel = Intent.Modelers.Services.Api.OperationModel;
using ParameterModel = Intent.Modelers.Services.Api.ParameterModel;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.Integration.HttpClients.Templates.HttpClient
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class HttpClientTemplate : CSharpTemplateBase<ServiceProxyModel>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.Integration.HttpClients.HttpClient";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public HttpClientTemplate(IOutputTarget outputTarget, ServiceProxyModel model) : base(TemplateId, outputTarget, model)
        {
            ServiceMetadataQueries.Validate(this, model);

            AddNugetDependency(NuGetPackages.MicrosoftExtensionsHttp);
            AddNugetDependency(NuGetPackages.MicrosoftAspNetCoreWebUtilities);

            AddTypeSource(ServiceContractTemplate.TemplateId);
            AddTypeSource(DtoContractTemplate.TemplateId).WithCollectionFormat("List<{0}>");
            SetDefaultCollectionFormatter(CSharpCollectionFormatter.Create("List<{0}>"));

            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddUsing("System")
                .AddUsing("System.Collections.Generic")
                .AddUsing("System.IO")
                .AddUsing("System.Linq")
                .AddUsing("System.Net")
                .AddUsing("System.Net.Http")
                .AddUsing("System.Net.Http.Headers")
                .AddUsing("System.Text")
                .AddUsing("System.Text.Json")
                .AddUsing("System.Threading")
                .AddUsing("System.Threading.Tasks")
                .AddUsing("Microsoft.AspNetCore.WebUtilities")
                .IntentManagedFully()
                .AddClass($"{Model.Name.RemoveSuffix("Client")}HttpClient", @class =>
                {
                    @class
                        .ImplementsInterface(this.GetServiceContractName())
                        .AddField("JsonSerializerOptions", "_serializerOptions", f => f.PrivateReadOnly())
                        .AddConstructor(constructor => constructor
                            .AddParameter("HttpClient", "httpClient", p => p.IntroduceReadonlyField())
                            .AddStatement(@"_serializerOptions = new JsonSerializerOptions()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        }"));

                    foreach (var operation in Model.MappedService.Operations.Where(ContractMetadataQueries.IsAbleToReference))
                    {
                        @class.AddMethod(GetReturnType(operation), GetOperationName(operation), method =>
                        {
                            method.Async();

                            foreach (var parameter in operation.Parameters)
                            {
                                method.AddParameter(GetTypeName(parameter.TypeReference), parameter.Name.ToParameterName());
                            }

                            method.AddParameter("CancellationToken", "cancellationToken", parameter => parameter.WithDefaultValue("default"));

                            // We're leveraging the C# $"" notation to actually take leverage of the parameters
                            // that are meant to be Route-based.
                            method.AddStatement($"var relativeUri = $\"{GetRelativeUri(operation)}\";");

                            if (HasQueryParameter(operation))
                            {
                                method.AddStatement("var queryParams = new Dictionary<string, string>();", s => s.SeparatedFromPrevious());

                                foreach (var queryParameter in GetQueryParameters(operation))
                                {
                                    method.AddStatement($"queryParams.Add(\"{queryParameter.Name.ToCamelCase()}\", {GetParameterValueExpression(queryParameter)});");
                                }

                                method.AddStatement("relativeUri = QueryHelpers.AddQueryString(relativeUri, queryParams);");
                            }

                            method.AddStatement($"var request = new HttpRequestMessage({GetHttpVerb(operation)}, relativeUri);");
                            method.AddStatement("request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(\"application/json\"));");

                            foreach (var headerParameter in GetHeaderParameters(operation))
                            {
                                method.AddStatement($"request.Headers.Add(\"{headerParameter.HeaderName}\", {headerParameter.Parameter.Name.ToParameterName()});");
                            }

                            if (HasBodyParameter(operation))
                            {
                                method.AddStatement($"var content = JsonSerializer.Serialize({GetBodyParameterName(operation)}, _serializerOptions);", s => s.SeparatedFromPrevious());
                                method.AddStatement("request.Content = new StringContent(content, Encoding.Default, \"application/json\");");
                            }
                            else if (HasFormUrlEncodedParameter(operation))
                            {
                                method.AddStatement("var formVariables = new List<KeyValuePair<string, string>>();", s => s.SeparatedFromPrevious());

                                foreach (var formParameter in GetFormUrlEncodedParameters(operation))
                                {
                                    method.AddStatement($"formVariables.Add(new KeyValuePair<string, string>(\"{formParameter.Name.ToPascalCase()}\", {GetParameterValueExpression(formParameter)}));");
                                }

                                method.AddStatement("var content = new FormUrlEncodedContent(formVariables);");
                                method.AddStatement("request.Content = content;");
                            }

                            method.AddStatementBlock("using (var response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false))", usingResponseBlock =>
                            {
                                usingResponseBlock.SeparatedFromPrevious();

                                usingResponseBlock.AddStatementBlock("if (!response.IsSuccessStatusCode)", s => s
                                    .AddStatement($"throw await {this.GetHttpClientRequestExceptionName()}.Create(_httpClient.BaseAddress, request, response, cancellationToken).ConfigureAwait(false);")
                                );

                                if (HasResponseType(operation))
                                {
                                    usingResponseBlock.AddStatement(new CSharpStatementBlock("if (response.StatusCode == HttpStatusCode.NoContent || response.Content.Headers.ContentLength == 0)"), s => ((CSharpStatementBlock)s)
                                        .AddStatement("return default;")
                                    );

                                    usingResponseBlock.AddStatementBlock("using (var contentStream = await response.Content.ReadAsStreamAsync(cancellationToken).ConfigureAwait(false))", usingContentStreamBlock =>
                                    {
                                        usingContentStreamBlock.SeparatedFromPrevious();

                                        if (HasWrappedReturnType(operation) && (IsReturnTypePrimitive(operation) || operation.ReturnType.HasStringType()))
                                        {
                                            usingContentStreamBlock.AddStatement($"var wrappedObj = await JsonSerializer.DeserializeAsync<{this.GetJsonResponseName()}<{GetTypeName(operation.ReturnType)}>>(contentStream, _serializerOptions, cancellationToken).ConfigureAwait(false);");
                                            usingContentStreamBlock.AddStatement("return wrappedObj.Value;");
                                        }
                                        else if (!HasWrappedReturnType(operation) && operation.ReturnType.HasStringType() && !operation.ReturnType.IsCollection)
                                        {
                                            usingContentStreamBlock.AddStatement("var str = await new StreamReader(contentStream).ReadToEndAsync().ConfigureAwait(false);");
                                            usingContentStreamBlock.AddStatement("if (str != null && (str.StartsWith(@\"\"\"\") || str.StartsWith(\"'\"))) { str = str.Substring(1, str.Length - 2); }");
                                            usingContentStreamBlock.AddStatement("return str;");
                                        }
                                        else if (!HasWrappedReturnType(operation) && IsReturnTypePrimitive(operation))
                                        {
                                            usingContentStreamBlock.AddStatement("var str = await new StreamReader(contentStream).ReadToEndAsync().ConfigureAwait(false);");
                                            usingContentStreamBlock.AddStatement("if (str != null && (str.StartsWith(@\"\"\"\") || str.StartsWith(\"'\"))) { str = str.Substring(1, str.Length - 2); };");
                                            usingContentStreamBlock.AddStatement($"return {GetTypeName(operation.ReturnType)}.Parse(str);");
                                        }
                                        else
                                        {
                                            usingContentStreamBlock.AddStatement($"return await JsonSerializer.DeserializeAsync<{GetTypeName(operation.ReturnType)}>(contentStream, _serializerOptions, cancellationToken).ConfigureAwait(false);");
                                        }
                                    });
                                }
                            });
                        });
                    }

                    @class.AddMethod("void", "Dispose");
                });
        }

        [IntentManaged(Mode.Fully)]
        public CSharpFile CSharpFile { get; }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        protected override CSharpFileConfig DefineFileConfig() => CSharpFile.GetConfig();

        [IntentManaged(Mode.Fully)]
        public override string TransformText()
        {
            return CSharpFile.ToString();
        }

        private string GetReturnType(OperationModel operation)
        {
            if (operation.ReturnType == null)
            {
                return "Task";
            }

            return $"Task<{GetTypeName(operation.ReturnType)}>";
        }

        private static string GetOperationName(OperationModel operation)
        {
            return $"{operation.Name.ToPascalCase().RemoveSuffix("Async")}Async";
        }

        private static string GetRelativeUri(OperationModel operation)
        {
            var relativeUri = ServiceMetadataQueries.GetRelativeUri(operation);
            return relativeUri;
        }

        private bool HasQueryParameter(OperationModel operation)
        {
            return ServiceMetadataQueries.GetQueryParameters(this, operation).Any();
        }

        private IReadOnlyCollection<ParameterModel> GetQueryParameters(OperationModel operation)
        {
            return ServiceMetadataQueries.GetQueryParameters(this, operation);
        }

        private static string GetHttpVerb(OperationModel operation)
        {
            return $"HttpMethod.{ServiceMetadataQueries.GetHttpVerb(operation)}";
        }

        private static IReadOnlyCollection<ServiceMetadataQueries.HeaderParameter> GetHeaderParameters(OperationModel operation)
        {
            return ServiceMetadataQueries.GetHeaderParameters(operation);
        }

        private bool HasBodyParameter(OperationModel operation)
        {
            return ServiceMetadataQueries.GetBodyParameter(this, operation) != null;
        }

        private string GetBodyParameterName(OperationModel operation)
        {
            return ServiceMetadataQueries.GetBodyParameter(this, operation).Name.ToParameterName();
        }

        private static bool HasFormUrlEncodedParameter(OperationModel operation)
        {
            return ServiceMetadataQueries.GetFormUrlEncodedParameters(operation).Any();
        }

        private static IReadOnlyCollection<ParameterModel> GetFormUrlEncodedParameters(OperationModel operation)
        {
            return ServiceMetadataQueries.GetFormUrlEncodedParameters(operation);
        }

        private static bool HasResponseType(OperationModel operation)
        {
            return operation.ReturnType != null;
        }

        private static bool HasWrappedReturnType(OperationModel operationModel)
        {
            return ServiceMetadataQueries.HasJsonWrappedReturnType(operationModel);
        }

        private bool IsReturnTypePrimitive(OperationModel operation)
        {
            return GetTypeInfo(operation.ReturnType).IsPrimitive && !operation.ReturnType.IsCollection;
        }

        private static string GetParameterValueExpression(ParameterModel parameter)
        {
            return !parameter.TypeReference.HasStringType()
                ? $"{parameter.Name.ToParameterName()}.ToString()"
                : parameter.Name.ToParameterName();
        }
    }
}