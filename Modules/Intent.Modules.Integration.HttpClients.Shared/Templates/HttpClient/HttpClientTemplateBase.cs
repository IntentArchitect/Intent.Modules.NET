using System;
using System.Linq;
using Intent.Engine;
using Intent.Modelers.Services.Api;
using Intent.Modelers.Types.ServiceProxies.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.CSharp.TypeResolvers;
using Intent.Modules.Common.CSharp.VisualStudio;
using Intent.Modules.Common.Templates;
using Intent.Modules.Common.VisualStudio;
using Intent.Modules.Contracts.Clients.Shared;
using Intent.Modules.Contracts.Clients.Shared.Templates.PagedResult;
using Intent.Modules.Integration.HttpClients.Shared.Templates.Adapters;
using Intent.Modules.Metadata.WebApi.Models;
using Intent.RoslynWeaver.Attributes;

namespace Intent.Modules.Integration.HttpClients.Shared.Templates.HttpClient;

public abstract class HttpClientTemplateBase : CSharpTemplateBase<IServiceProxyModel>, ICSharpFileBuilderTemplate
{
    protected HttpClientTemplateBase(
        string templateId,
        IOutputTarget outputTarget,
        ServiceProxyModel model,
        string httpClientRequestExceptionTemplateId,
        string jsonResponseTemplateId,
        string serviceContractTemplateId,
        string dtoContractTemplateId,
        string enumContractTemplateId,
        string pagedResultTemplateId)
    : this(
          templateId, 
          outputTarget, 
          new ServiceProxyModelAdapter(model), 
          httpClientRequestExceptionTemplateId,
            jsonResponseTemplateId,
            serviceContractTemplateId,
            dtoContractTemplateId,
            enumContractTemplateId,
            pagedResultTemplateId)
    {
    }

    protected HttpClientTemplateBase(
        string templateId,
        IOutputTarget outputTarget,
        IServiceProxyModel model,
        string httpClientRequestExceptionTemplateId,
        string jsonResponseTemplateId,
        string serviceContractTemplateId,
        string dtoContractTemplateId,
        string enumContractTemplateId,
        string pagedResultTemplateId)
        : base(templateId, outputTarget, model)
    {
        var endpoints = Model.GetMappedEndpoints().ToArray();

        AddNugetDependency(NuGetPackages.MicrosoftExtensionsHttp);
        AddNugetDependency(NuGetPackages.MicrosoftAspNetCoreWebUtilities);

        SetDefaultCollectionFormatter(CSharpCollectionFormatter.CreateList());
        PagedResultTypeSource.ApplyTo(this, pagedResultTemplateId);
        AddTypeSource(serviceContractTemplateId);
        AddTypeSource(dtoContractTemplateId);
        AddTypeSource(enumContractTemplateId);

        CSharpFile = new CSharpFile(
                @namespace: this.GetNamespace(),
                relativeLocation: this.GetFolderPath())
            .AddUsing("System.Net.Http")
            .AddUsing("System.Net.Http.Headers")
            .AddUsing("System.Text")
            .AddUsing("System.Text.Json")
            .AddUsing("System.Threading")
            .AddUsing("System.Threading.Tasks")
            .AddAssemblyAttribute("[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]")
            .IntentManagedFully()
            .AddClass($"{Model.Name.RemoveSuffix("Http", "Client")}HttpClient", @class =>
            {
                if (model.UnderlyingModel != null)
                {
                    @class.AddMetadata("model", model.UnderlyingModel);
                }
                @class
                    .ImplementsInterface(GetTypeName(serviceContractTemplateId, Model))
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
                            method.AddStatement($"var queryParams = new {UseType("System.Collections.Generic.Dictionary")}<string, string?>();", s => s.SeparatedFromPrevious());

                            foreach (var queryParameter in queryParams)
                            {
                                if (queryParameter.TypeReference.Element.IsDTOModel())
                                {
                                    var dto = queryParameter.TypeReference.Element.AsDTOModel();
                                    foreach (var field in dto.Fields)
                                    {
                                        method.AddStatement($"queryParams.Add(\"{field.Name.ToCamelCase()}\", {queryParameter.Name.ToCamelCase()}.{GetParameterValueExpression(field)});");
                                    }
                                }
                                else if (queryParameter.TypeReference.IsCollection)
                                {
                                    method.AddStatement("var index = 0;");
                                    method.AddForEachStatement("element", queryParameter.Name.ToCamelCase(), block =>
                                    {
                                        block.AddStatement($@"queryParams.Add($""{queryParameter.QueryStringName ?? queryParameter.Name.ToCamelCase()}[{{index++}}]"", element.ToString());");
                                    });
                                }
                                else
                                {
                                    method.AddStatement($"queryParams.Add(\"{queryParameter.QueryStringName ?? queryParameter.Name.ToCamelCase()}\", {GetParameterValueExpression(queryParameter)});");
                                }
                            }

                            method.AddStatement($"relativeUri = {UseType("Microsoft.AspNetCore.WebUtilities.QueryHelpers")}.AddQueryString(relativeUri, queryParams);");
                        }

                        method.AddStatement($"var httpRequest = new HttpRequestMessage(HttpMethod.{endpoint.Verb}, relativeUri);");
                        method.AddStatement("httpRequest.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(\"application/json\"));");

                        foreach (var headerParameter in inputsBySource.TryGetValue(HttpInputSource.FromHeader, out var headerParams)
                                     ? headerParams
                                     : Enumerable.Empty<IHttpEndpointInputModel>())
                        {
                            method.AddStatement($"httpRequest.Headers.Add(\"{headerParameter.HeaderName}\", {headerParameter.Name.ToParameterName()});");
                        }

                        if (inputsBySource.TryGetValue(HttpInputSource.FromBody, out var bodyParams))
                        {
                            var bodyParam = bodyParams.Single();

                            method.AddStatement($"var content = JsonSerializer.Serialize({bodyParam.Name.ToParameterName()}, _serializerOptions);", s => s.SeparatedFromPrevious());
                            // Changed to UTF8 as Default can be sketchy:
                            // https://learn.microsoft.com/en-us/dotnet/api/system.text.encoding.default?view=net-7.0&devlangs=csharp&f1url=%3FappId%3DDev16IDEF1%26l%3DEN-US%26k%3Dk(System.Text.Encoding.Default)%3Bk(DevLang-csharp)%26rd%3Dtrue
                            method.AddStatement("httpRequest.Content = new StringContent(content, Encoding.UTF8 , \"application/json\");");
                        }
                        else if (inputsBySource.TryGetValue(HttpInputSource.FromForm, out var formParams))
                        {
                            method.AddStatement("var formVariables = new List<KeyValuePair<string, string>>();", s => s.SeparatedFromPrevious());

                            foreach (var formParameter in formParams)
                            {
                                method.AddStatement($"formVariables.Add(new KeyValuePair<string, string>(\"{formParameter.Name.ToPascalCase()}\", {GetParameterValueExpression(formParameter)}));");
                            }

                            method.AddStatement("var content = new FormUrlEncodedContent(formVariables);");
                            method.AddStatement("httpRequest.Content = content;");
                        }

                        method.AddStatementBlock("using (var response = await _httpClient.SendAsync(httpRequest, HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false))", usingResponseBlock =>
                        {
                            usingResponseBlock.SeparatedFromPrevious();

                            usingResponseBlock.AddStatementBlock("if (!response.IsSuccessStatusCode)", s => s
                                .AddStatement($"throw await {GetTypeName(httpClientRequestExceptionTemplateId)}.Create(_httpClient.BaseAddress!, httpRequest, response, cancellationToken).ConfigureAwait(false);")
                            );

                            if (endpoint.ReturnType?.Element == null)
                            {
                                return;
                            }

                            if (endpoint.ReturnType.IsNullable)
                            {
                                usingResponseBlock.AddStatementBlock($"if (response.StatusCode == {UseType("System.Net.HttpStatusCode")}.NoContent || response.Content.Headers.ContentLength == 0)", s => s
                                    .AddStatement("return default;")
                                );
                            }

                            usingResponseBlock.AddStatementBlock($"using (var contentStream = await response.Content.{GetReadAsStreamAsyncMethodCall()}.ConfigureAwait(false))", usingContentStreamBlock =>
                            {
                                var isWrappedReturnType = endpoint.MediaType == HttpMediaType.ApplicationJson;
                                var returnsCollection = endpoint.ReturnType.IsCollection;
                                var returnsString = endpoint.ReturnType.HasStringType();
                                var returnsPrimitive = GetTypeInfo(endpoint.ReturnType).IsPrimitive &&
                                                       !returnsCollection;

                                string SuppressNullable(string expression) => endpoint.ReturnType.IsNullable ? expression : $"({expression})!";

                                usingContentStreamBlock.SeparatedFromPrevious();

                                if (isWrappedReturnType && (returnsPrimitive || returnsString))
                                {
                                    usingContentStreamBlock.AddStatement($"var wrappedObj = {SuppressNullable($"await JsonSerializer.DeserializeAsync<{GetTypeName(jsonResponseTemplateId)}<{GetTypeName(endpoint.ReturnType)}>>(contentStream, _serializerOptions, cancellationToken).ConfigureAwait(false)")};");
                                    usingContentStreamBlock.AddStatement("return wrappedObj!.Value;");
                                }
                                else if (!isWrappedReturnType && returnsString && !returnsCollection)
                                {
                                    usingContentStreamBlock.AddStatement($"var str = await new {UseType("System.IO.StreamReader")}(contentStream).{GetReadToEndMethodCall()}.ConfigureAwait(false);");
                                    usingContentStreamBlock.AddIfStatement("str.StartsWith(@\"\"\"\") || str.StartsWith(\"'\")", stmt => 
                                    {
                                        stmt.AddStatement("str = str.Substring(1, str.Length - 2);");
                                    });
                                    usingContentStreamBlock.AddStatement("return str;");
                                }
                                else if (!isWrappedReturnType && returnsPrimitive)
                                {
                                    usingContentStreamBlock.AddStatement($"var str = await new {UseType("System.IO.StreamReader")}(contentStream).{GetReadToEndMethodCall()}.ConfigureAwait(false);");
                                    usingContentStreamBlock.AddIfStatement("str.StartsWith(@\"\"\"\") || str.StartsWith(\"'\")", stmt => 
                                    {
                                        stmt.AddStatement("str = str.Substring(1, str.Length - 2);");
                                    });
                                    usingContentStreamBlock.AddStatement($"return {GetTypeName(endpoint.ReturnType)}.Parse(str);");
                                }
                                else
                                {
                                    usingContentStreamBlock.AddStatement($"return {SuppressNullable($"await JsonSerializer.DeserializeAsync<{GetTypeName(endpoint.ReturnType)}>(contentStream, _serializerOptions, cancellationToken).ConfigureAwait(false)")};");
                                }
                            });
                        });
                    });
                }

                @class.AddMethod("void", "Dispose");
            });
    }

    private string GetReadToEndMethodCall()
    {
        return OutputTarget switch
        {
            _ when OutputTarget.GetProject().IsNetApp(5) => "ReadToEndAsync()",
            _ when OutputTarget.GetProject().IsNetApp(6) => "ReadToEndAsync()",
            _ when OutputTarget.GetProject().TargetFramework().StartsWith("netstandard") => "ReadToEndAsync()",
            _ => "ReadToEndAsync(cancellationToken)"
        }; 
    }

    private string GetReadAsStreamAsyncMethodCall()
    {
        return OutputTarget switch
        {
            _ when OutputTarget.GetProject().TargetFramework().StartsWith("netstandard") => "ReadAsStreamAsync()",
            _ => "ReadAsStreamAsync(cancellationToken)"
        };
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

    public override RoslynMergeConfig ConfigureRoslynMerger() => ToFullyManagedUsingsMigration.GetConfig(Id, 2);

    private string GetReturnType(IHttpEndpointModel endpoint)
    {
        return endpoint.ReturnType?.Element == null
            ? "Task"
            : $"Task<{GetTypeName(endpoint.ReturnType)}>";
    }

    private object GetParameterValueExpression(DTOFieldModel field)
    {
        return !field.TypeReference.HasStringType()
            ? $"{field.Name.ToPascalCase()}.ToString()"
            : field.Name.ToPascalCase();
    }


    private static string GetParameterValueExpression(IHttpEndpointInputModel input)
    {
        return !input.TypeReference.HasStringType()
            ? $"{input.Name.ToParameterName()}.ToString()"
            : input.Name.ToParameterName();
    }
}