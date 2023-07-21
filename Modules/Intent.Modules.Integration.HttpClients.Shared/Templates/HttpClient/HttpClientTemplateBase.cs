using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Modelers.Types.ServiceProxies.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.CSharp.TypeResolvers;
using Intent.Modules.Common.Templates;
using Intent.Modules.Contracts.Clients.Shared;
using Intent.Modules.Metadata.WebApi.Models;
using Intent.RoslynWeaver.Attributes;

namespace Intent.Modules.Integration.HttpClients.Shared.Templates.HttpClient;

public abstract class HttpClientTemplateBase : CSharpTemplateBase<ServiceProxyModel>, ICSharpFileBuilderTemplate
{
    protected HttpClientTemplateBase(
        string templateId,
        IOutputTarget outputTarget,
        ServiceProxyModel model,
        string httpClientRequestExceptionTemplateId,
        string jsonResponseTemplateId,
        string serviceContractTemplateId,
        string dtoContractTemplateId,
        IEnumerable<string> additionalFolderParts = null)
        : base(templateId, outputTarget, model)
    {
        var serviceContractTemplateId1 = serviceContractTemplateId;
        var endpoints = Model.GetMappedEndpoints().ToArray();
        var additionalFolderPartsAsArray = additionalFolderParts?.ToArray() ?? Array.Empty<string>();

        AddNugetDependency(NuGetPackages.MicrosoftExtensionsHttp);
        AddNugetDependency(NuGetPackages.MicrosoftAspNetCoreWebUtilities);

        AddTypeSource(serviceContractTemplateId1);
        AddTypeSource(dtoContractTemplateId).WithCollectionFormat("List<{0}>");
        SetDefaultCollectionFormatter(CSharpCollectionFormatter.Create("List<{0}>"));

        CSharpFile = new CSharpFile(this.GetNamespace(additionalFolderPartsAsArray), this.GetFolderPath(additionalFolderPartsAsArray))
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
            .IntentManagedFully()
            .AddClass($"{Model.Name.RemoveSuffix("Client")}HttpClient", @class =>
            {
                @class
                    .ImplementsInterface(GetTypeName(serviceContractTemplateId1, Model))
                    .AddField("JsonSerializerOptions", "_serializerOptions", f => f.PrivateReadOnly())
                    .AddConstructor(constructor => constructor
                        .AddParameter("HttpClient", "httpClient", p => p.IntroduceReadonlyField())
                        .AddStatement(@"_serializerOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        };"));

                foreach (var endpoint in endpoints)
                {
                    var inputsBySource = endpoint.Inputs
                        .GroupBy(x => x.Source)
                        .Where(x => x.Key != null)
                        .ToDictionary(x => x.Key, x => x.ToArray());

                    @class.AddMethod(GetReturnType(endpoint), $"{endpoint.Name.ToPascalCase().RemoveSuffix("Async")}Async", method =>
                    {
                        method.Async();

                        var endpointRoute = endpoint.Route;
                        foreach (var input in endpoint.Inputs)
                        {
                            var parameterName = input.Name.ToParameterName();
                            method.AddParameter(GetTypeName(input.TypeReference), parameterName);

                            endpointRoute = endpointRoute.Replace($"{{{parameterName}}}", $"{{{parameterName}}}", StringComparison.OrdinalIgnoreCase);
                        }

                        method.AddParameter("CancellationToken", "cancellationToken", parameter => parameter.WithDefaultValue("default"));

                        // We're leveraging the C# $"" notation to actually take leverage of the parameters
                        // that are meant to be Route-based.
                        method.AddStatement($"var relativeUri = $\"{endpointRoute}\";");

                        if (inputsBySource.TryGetValue(HttpInputSource.FromQuery, out var queryParams))
                        {
                            method.AddStatement("var queryParams = new Dictionary<string, string>();", s => s.SeparatedFromPrevious());

                            foreach (var queryParameter in queryParams)
                            {
                                method.AddStatement($"queryParams.Add(\"{queryParameter.Name.ToCamelCase()}\", {GetParameterValueExpression(queryParameter)});");
                            }

                            method.AddStatement($"relativeUri = {UseType("Microsoft.AspNetCore.WebUtilities.QueryHelpers")}.AddQueryString(relativeUri, queryParams);");
                        }

                        method.AddStatement($"var request = new HttpRequestMessage(HttpMethod.{endpoint.Verb}, relativeUri);");
                        method.AddStatement("request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(\"application/json\"));");

                        foreach (var headerParameter in inputsBySource.TryGetValue(HttpInputSource.FromHeader, out var headerParams)
                                     ? headerParams
                                     : Enumerable.Empty<IHttpEndpointInputModel>())
                        {
                            method.AddStatement($"request.Headers.Add(\"{headerParameter.HeaderName}\", {headerParameter.Name.ToParameterName()});");
                        }

                        if (inputsBySource.TryGetValue(HttpInputSource.FromBody, out var bodyParams))
                        {
                            var bodyParam = bodyParams.Single();

                            method.AddStatement($"var content = JsonSerializer.Serialize({bodyParam.Name.ToParameterName()}, _serializerOptions);", s => s.SeparatedFromPrevious());
                            method.AddStatement("request.Content = new StringContent(content, Encoding.Default, \"application/json\");");
                        }
                        else if (inputsBySource.TryGetValue(HttpInputSource.FromForm, out var formParams))
                        {
                            method.AddStatement("var formVariables = new List<KeyValuePair<string, string>>();", s => s.SeparatedFromPrevious());

                            foreach (var formParameter in formParams)
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
                                .AddStatement($"throw await {GetTypeName(httpClientRequestExceptionTemplateId)}.Create(_httpClient.BaseAddress!, request, response, cancellationToken).ConfigureAwait(false);")
                            );

                            if (endpoint.ReturnType?.Element == null)
                            {
                                return;
                            }

                            usingResponseBlock.AddStatementBlock("if (response.StatusCode == HttpStatusCode.NoContent || response.Content.Headers.ContentLength == 0)", s => s
                                .AddStatement("return default;")
                            );

                            usingResponseBlock.AddStatementBlock("using (var contentStream = await response.Content.ReadAsStreamAsync(cancellationToken).ConfigureAwait(false))", usingContentStreamBlock =>
                            {
                                var isWrappedReturnType = endpoint.MediaType == HttpMediaType.ApplicationJson;
                                var returnsCollection = endpoint.ReturnType.IsCollection;
                                var returnsString = endpoint.ReturnType.HasStringType();
                                var returnsPrimitive = GetTypeInfo(endpoint.ReturnType).IsPrimitive &&
                                                       !returnsCollection;

                                usingContentStreamBlock.SeparatedFromPrevious();

                                if (isWrappedReturnType && (returnsPrimitive || returnsString))
                                {
                                    usingContentStreamBlock.AddStatement($"var wrappedObj = await JsonSerializer.DeserializeAsync<{GetTypeName(jsonResponseTemplateId)}<{GetTypeName(endpoint.ReturnType)}>>(contentStream, _serializerOptions, cancellationToken).ConfigureAwait(false);");
                                    usingContentStreamBlock.AddStatement("return wrappedObj!.Value;");
                                }
                                else if (!isWrappedReturnType && returnsString && !returnsCollection)
                                {
                                    usingContentStreamBlock.AddStatement("var str = await new StreamReader(contentStream).ReadToEndAsync().ConfigureAwait(false);");
                                    usingContentStreamBlock.AddStatement("if (str != null && (str.StartsWith(@\"\"\"\") || str.StartsWith(\"'\"))) { str = str.Substring(1, str.Length - 2); }");
                                    usingContentStreamBlock.AddStatement("return str;");
                                }
                                else if (!isWrappedReturnType && returnsPrimitive)
                                {
                                    usingContentStreamBlock.AddStatement("var str = await new StreamReader(contentStream).ReadToEndAsync().ConfigureAwait(false);");
                                    usingContentStreamBlock.AddStatement("if (str != null && (str.StartsWith(@\"\"\"\") || str.StartsWith(\"'\"))) { str = str.Substring(1, str.Length - 2); };");
                                    usingContentStreamBlock.AddStatement($"return {GetTypeName(endpoint.ReturnType)}.Parse(str);");
                                }
                                else
                                {
                                    usingContentStreamBlock.AddStatement($"return await JsonSerializer.DeserializeAsync<{GetTypeName(endpoint.ReturnType)}>(contentStream, _serializerOptions, cancellationToken).ConfigureAwait(false);");
                                }
                            });
                        });
                    });
                }

                @class.AddMethod("void", "Dispose");
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

    private string GetReturnType(IHttpEndpointModel endpoint)
    {
        var typeInfo = GetTypeInfo(endpoint.ReturnType);
        var typeName = UseType(typeInfo);
        if (!typeInfo.IsPrimitive)
        {
            typeName = $"{typeName}?";
        }

        return endpoint.ReturnType?.Element == null
            ? "Task"
            : $"Task<{typeName}>";
    }

    private static string GetParameterValueExpression(IHttpEndpointInputModel input)
    {
        return !input.TypeReference.HasStringType()
            ? $"{input.Name.ToParameterName()}.ToString()"
            : input.Name.ToParameterName();
    }
}