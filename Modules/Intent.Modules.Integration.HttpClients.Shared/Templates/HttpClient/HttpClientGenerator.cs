using System.Collections.Generic;
using System.Linq;
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
using Intent.Modules.Metadata.WebApi.Models;

namespace Intent.Modules.Integration.HttpClients.Shared.Templates.HttpClient;

public class HttpClientGenerator
{
    private readonly CSharpTemplateBase<ServiceProxyModel> _template;
    private readonly string _httpClientRequestExceptionTemplateId;
    private readonly string _jsonResponseTemplateId;
    private readonly IReadOnlyCollection<IHttpEndpointModel> _endpoints;

    private HttpClientGenerator(
        CSharpTemplateBase<ServiceProxyModel> template,
        string httpClientRequestExceptionTemplateId,
        string jsonResponseTemplateId)
    {
        _template = template;
        _httpClientRequestExceptionTemplateId = httpClientRequestExceptionTemplateId;
        _jsonResponseTemplateId = jsonResponseTemplateId;
        _endpoints = template.Model.GetMappedEndpoints().ToArray();
    }

    public static CSharpFile CreateCSharpFile(
        CSharpTemplateBase<ServiceProxyModel> template,
        string httpClientRequestExceptionTemplateId,
        string jsonResponseTemplateId)
    {
        return new HttpClientGenerator(template, httpClientRequestExceptionTemplateId, jsonResponseTemplateId).Create();
    }

    private CSharpFile Create()
    {
        _template.AddNugetDependency(NuGetPackages.MicrosoftExtensionsHttp);
        _template.AddNugetDependency(NuGetPackages.MicrosoftAspNetCoreWebUtilities);

        _template.AddTypeSource(ServiceContractTemplate.TemplateId);
        _template.AddTypeSource(DtoContractTemplate.TemplateId).WithCollectionFormat("List<{0}>");
        _template.SetDefaultCollectionFormatter(CSharpCollectionFormatter.Create("List<{0}>"));

        return new CSharpFile(_template.GetNamespace(), _template.GetFolderPath())
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
            .AddClass($"{_template.Model.Name.RemoveSuffix("Client")}HttpClient", @class =>
            {
                @class
                    .ImplementsInterface(_template.GetServiceContractName())
                    .AddField("JsonSerializerOptions", "_serializerOptions", f => f.PrivateReadOnly())
                    .AddConstructor(constructor => constructor
                        .AddParameter("HttpClient", "httpClient", p => p.IntroduceReadonlyField())
                        .AddStatement(@"_serializerOptions = new JsonSerializerOptions()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        };"));

                foreach (var endpoint in _endpoints)
                {
                    var inputsBySource = endpoint.Inputs
                        .GroupBy(x => x.Source)
                        .Where(x => x.Key != null)
                        .ToDictionary(x => x.Key, x => x.ToArray());

                    @class.AddMethod(GetReturnType(endpoint), $"{endpoint.Name.ToPascalCase().RemoveSuffix("Async")}Async", method =>
                    {
                        method.Async();

                        foreach (var input in endpoint.Inputs)
                        {
                            method.AddParameter(_template.GetTypeName(input.TypeReference), input.Name.ToParameterName());
                        }

                        method.AddParameter("CancellationToken", "cancellationToken", parameter => parameter.WithDefaultValue("default"));

                        // We're leveraging the C# $"" notation to actually take leverage of the parameters
                        // that are meant to be Route-based.
                        method.AddStatement($"var relativeUri = $\"{endpoint.Route}\";");

                        if (inputsBySource.TryGetValue(HttpInputSource.FromQuery, out var queryParams))
                        {
                            method.AddStatement("var queryParams = new Dictionary<string, string>();", s => s.SeparatedFromPrevious());

                            foreach (var queryParameter in queryParams)
                            {
                                method.AddStatement($"queryParams.Add(\"{queryParameter.Name.ToCamelCase()}\", {GetParameterValueExpression(queryParameter)});");
                            }

                            method.AddStatement("relativeUri = QueryHelpers.AddQueryString(relativeUri, queryParams);");
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
                                .AddStatement($"throw await {_template.GetTypeName(_httpClientRequestExceptionTemplateId)}.Create(_httpClient.BaseAddress, request, response, cancellationToken).ConfigureAwait(false);")
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
                                var returnsPrimitive = _template.GetTypeInfo(endpoint.ReturnType).IsPrimitive &&
                                                       !returnsCollection;

                                usingContentStreamBlock.SeparatedFromPrevious();

                                if (isWrappedReturnType && (returnsPrimitive || returnsString))
                                {
                                    usingContentStreamBlock.AddStatement($"var wrappedObj = await JsonSerializer.DeserializeAsync<{_template.GetTypeName(_jsonResponseTemplateId)}<{_template.GetTypeName(endpoint.ReturnType)}>>(contentStream, _serializerOptions, cancellationToken).ConfigureAwait(false);");
                                    usingContentStreamBlock.AddStatement("return wrappedObj.Value;");
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
                                    usingContentStreamBlock.AddStatement($"return {_template.GetTypeName(endpoint.ReturnType)}.Parse(str);");
                                }
                                else
                                {
                                    usingContentStreamBlock.AddStatement($"return await JsonSerializer.DeserializeAsync<{_template.GetTypeName(endpoint.ReturnType)}>(contentStream, _serializerOptions, cancellationToken).ConfigureAwait(false);");
                                }
                            });
                        });
                    });
                }

                @class.AddMethod("void", "Dispose");
            });
    }

    private string GetReturnType(IHttpEndpointModel endpoint)
    {
        return endpoint.ReturnType?.Element == null
            ? "Task"
            : $"Task<{_template.GetTypeName(endpoint.ReturnType)}>";
    }

    private static string GetParameterValueExpression(IHttpEndpointInputModel input)
    {
        return !input.TypeReference.HasStringType()
            ? $"{input.Name.ToParameterName()}.ToString()"
            : input.Name.ToParameterName();
    }
}