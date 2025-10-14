using System;
using System.Linq;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modelers.Services.Api;
using Intent.Modelers.Services.CQRS.Api;
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
using Intent.Modules.Metadata.WebApi.Models;
using Intent.RoslynWeaver.Attributes;

namespace Intent.Modules.Integration.HttpClients.Shared.Templates.HttpClient;

public abstract class HttpClientTemplateBase : CSharpTemplateBase<IServiceProxyModel>, ICSharpFileBuilderTemplate
{
    private readonly string _pagedResultTemplateId;

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
        _pagedResultTemplateId = pagedResultTemplateId;

        AddNugetDependency(NuGetPackages.MicrosoftExtensionsHttp(outputTarget));
        AddNugetDependency(NuGetPackages.MicrosoftAspNetCoreWebUtilities(outputTarget));

        SetDefaultCollectionFormatter(CSharpCollectionFormatter.CreateList());
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
                var addSerializerOptions = false;

                if (model.UnderlyingModel != null)
                {
                    @class.AddMetadata("model", model.UnderlyingModel);
                }
                @class
                    .ImplementsInterface(GetTypeName(serviceContractTemplateId, Model))
                    .AddConstructor(constructor =>
                    {
                        // _serializerOptions/JsonSerializerOptions only added if required as set by "addSerializerOptions" below
                        constructor
                            .AddParameter("HttpClient", "httpClient", p => p.IntroduceReadonlyField());
                    });

                @class.AddField("string", "JSON_MEDIA_TYPE", field =>
                {
                    field.PrivateConstant("\"application/json\"");
                });

                foreach (var endpoint in Model.Endpoints)
                {
                    var inputsBySource = endpoint.Inputs
                        .GroupBy(x => x.Source)
                        .Where(x => x.Key != null)
                        .ToDictionary(x => x.Key!.Value, x => x.ToArray());

                    @class.AddMethod(GetReturnType(endpoint), $"{endpoint.Name.ToPascalCase().RemoveSuffix("Async")}Async", method =>
                    {
                        method.Async();
                        // Doing it this way because endpoints don't have any context of the source operation, only the mapped-to elements (i.e. commands/queries/etc).
                        if (Model.UnderlyingModel is ServiceProxyModel serviceProxyModel && serviceProxyModel.Operations.Any())
                        {
                            var operationModel = serviceProxyModel.Operations.Single(x => x.Mapping?.ElementId == endpoint.Id);
                            method.RepresentsModel(operationModel);
                        }

                        string? parameterName;
                        string? bodyPayload = null;
                        if (model.CreateParameterPerInput)
                        {
                            foreach (var input in endpoint.Inputs)
                            {
                                parameterName = input.Name.ToParameterName();
                                method.AddParameter(GetTypeName(input.TypeReference), parameterName);
                            }

                            parameterName = null;
                        }
                        else
                        {
                            var fields = endpoint.InternalElement.ChildElements.Where(x => x.IsDTOFieldModel()).ToArray();

                            switch (fields.Length)
                            {
                                case 0:
                                    parameterName = null;
                                    break;
                                case 1:
                                    parameterName = fields[0].Name.ToParameterName();
                                    method.AddParameter(GetTypeName(fields[0].TypeReference), parameterName);
                                    bodyPayload = $"new {{ {fields[0].Name.ToPascalCase()} = {parameterName} }}";
                                    break;
                                default:
                                    parameterName = endpoint.InternalElement.SpecializationTypeId switch
                                    {
                                        CommandModel.SpecializationTypeId => "command",
                                        QueryModel.SpecializationTypeId => "query",
                                        _ => endpoint.InternalElement.Name.ToParameterName()

                                    };
                                    method.AddParameter(GetTypeName(endpoint.InternalElement), parameterName);
                                    break;
                            }
                        }
                        
                        method.AddParameter("CancellationToken", "cancellationToken", parameter => parameter.WithDefaultValue("default"));

                        var endpointRoute = endpoint.Route;
                        foreach (var input in endpoint.Inputs)
                        {
                            endpointRoute = endpointRoute.Replace($"{{{input.Name}}}", $"{{{GetSourceExpression(parameterName, endpoint, input)}}}", StringComparison.OrdinalIgnoreCase);
                        }

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
                                    AddDtoQueryParams(method, GetSourceExpression(parameterName, endpoint, queryParameter), dto);
                                }
                                else if (queryParameter.TypeReference.IsCollection)
                                {
                                    // if there is only one parameter which is a collection then keep it "index", otherwise rename
                                    var variableName = queryParams.Count(q => q.TypeReference.IsCollection) == 1 ? "index" : $"{queryParameter.Name}Index";
                                    
                                    method.AddStatement($"var {variableName} = 0;");
                                    method.AddForEachStatement("element", GetSourceExpression(parameterName, endpoint, queryParameter), block =>
                                    {
                                        block.AddStatement($@"queryParams.Add($""{queryParameter.QueryStringName ?? GetSourceExpression(parameterName, endpoint, queryParameter)}[{{{variableName}++}}]"", element.ToString());");
                                    });
                                }
                                else
                                {
                                    method.AddStatement(
                                        $"queryParams.Add(\"{queryParameter.QueryStringName ?? queryParameter.Name.ToCamelCase()}\"," +
                                        $" {GetParameterValueExpression(
                                            template: this,
                                            input: queryParameter,
                                            sourceExpression: GetSourceExpression(parameterName, endpoint, queryParameter))});");
                                }
                            }

                            method.AddStatement($"relativeUri = {UseType("Microsoft.AspNetCore.WebUtilities.QueryHelpers")}.AddQueryString(relativeUri, queryParams);");
                        }

                        method.AddStatement($"using var httpRequest = new HttpRequestMessage(HttpMethod.{endpoint.Verb}, relativeUri);");
                        method.AddStatement("httpRequest.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(JSON_MEDIA_TYPE));");

                        if (inputsBySource.TryGetValue(HttpInputSource.FromHeader, out var headerParams))
                        {
                            foreach (var headerParameter in headerParams)
                            {
                                if (string.IsNullOrWhiteSpace(headerParameter.HeaderName))
                                {
                                    throw new Exception($"Header parameter '{headerParameter.Name}' is missing a Header Name.");
                                }

                                if (headerParameter.HeaderName.Equals("content-type", StringComparison.OrdinalIgnoreCase))
                                {
                                    continue;
                                }

                                if (headerParameter.HeaderName.Equals("content-length", StringComparison.OrdinalIgnoreCase))
                                {
                                    // if file endpoint upload, it will be handled further down
                                    if (FileTransferHelper.IsFileUploadOperation(endpoint))
                                    {
                                        continue;
                                    }

                                    AddRequestContentLength(true, method, GetSourceExpression(parameterName, endpoint, headerParameter), headerParameter.TypeReference.IsNullable);
                                    continue;
                                }

                                if (headerParameter.TypeReference.IsNullable)
                                {
                                    method.AddIfStatement($"{GetSourceExpression(parameterName, endpoint, headerParameter)} != null", stmt =>
                                    {
                                        stmt.AddStatement($"httpRequest.Headers.Add(\"{headerParameter.HeaderName}\", {GetSourceExpression(parameterName, endpoint, headerParameter)}{(!headerParameter.TypeReference.HasStringType() ? ".ToString()" : "")});");
                                    });
                                }
                                else
                                {
                                    method.AddStatement($"httpRequest.Headers.Add(\"{headerParameter.HeaderName}\", {GetSourceExpression(parameterName, endpoint, headerParameter)}{(!headerParameter.TypeReference.HasStringType() ? ".ToString()" : "")});");
                                }
                            }
                        }

                        if (FileTransferHelper.IsFileUploadOperation(endpoint))
                        {
                            var fieldInfo = FileTransferHelper.GetUploadTypeInfo(endpoint);
                            string pathPrefix = fieldInfo.DtoPropertyName == null ? "" : $"{fieldInfo.DtoPropertyName}.";
                            Func<string, string> fieldNameFormatter = (fieldName) => fieldName;
                            if (fieldInfo.DtoPropertyName != null)
                            {
                                fieldNameFormatter = fieldName => Model.CreateParameterPerInput
                                    ? $"{pathPrefix}{fieldName.ToPascalCase()}"
                                    : $"{pathPrefix}{parameterName}.{fieldName.ToPascalCase()}";
                            }
                            method.AddStatement($"httpRequest.Content = new StreamContent({fieldNameFormatter(fieldInfo.StreamField)});");

                            if (fieldInfo.HasContentType())
                            {
                                method.AddStatement($"httpRequest.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue({fieldNameFormatter(fieldInfo.ContentTypeField!)} ?? \"application/octet-stream\");");
                            }
                            else
                            {
                                method.AddStatement($"httpRequest.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(\"application/octet-stream\");");
                            }
                            if (fieldInfo.HasContentLength())
                            {
                                // don't add the httpRequest.Content check here, as it will always be instantiated 
                                AddRequestContentLength(false, method, fieldInfo.ContentLengthField.ToParameterName(), true);
                            }

                            if (fieldInfo.HasFilename())
                            {
                                method.AddIfStatement($"{fieldNameFormatter(fieldInfo.FileNameField!)} != null", stmt =>
                                {
                                    stmt.AddStatement($"httpRequest.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue(\"form-data\") {{FileName = {fieldNameFormatter(fieldInfo.FileNameField!)} }};");
                                });
                            }
                        }
                        else if (inputsBySource.TryGetValue(HttpInputSource.FromBody, out var bodyParams))
                        {
                            var bodyParam = bodyParams.Single();

                            var payload = bodyPayload ?? GetSourceExpression(parameterName, endpoint, bodyParam);
                            method.AddStatement($"var content = JsonSerializer.Serialize({payload}, _serializerOptions);", s => s.SeparatedFromPrevious());

                            // Changed to UTF8 as Default can be sketchy:
                            // https://learn.microsoft.com/en-us/dotnet/api/system.text.encoding.default?view=net-7.0&devlangs=csharp&f1url=%3FappId%3DDev16IDEF1%26l%3DEN-US%26k%3Dk(System.Text.Encoding.Default)%3Bk(DevLang-csharp)%26rd%3Dtrue
                            method.AddStatement("httpRequest.Content = new StringContent(content, Encoding.UTF8 , JSON_MEDIA_TYPE);");
                            addSerializerOptions = true;
                        }
                        else if (inputsBySource.TryGetValue(HttpInputSource.FromForm, out var formParams))
                        {
                            method.AddStatement("var formVariables = new List<KeyValuePair<string, string>>();", s => s.SeparatedFromPrevious());

                            foreach (var formParameter in formParams)
                            {
                                method.AddStatement($"formVariables.Add(new KeyValuePair<string, string>(\"{formParameter.Name.ToPascalCase()}\", " +
                                                    $"{GetParameterValueExpression(this, formParameter, GetSourceExpression(parameterName, endpoint, formParameter))}));");
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

                            if (FileTransferHelper.IsFileDownloadOperation(endpoint))
                            {
                                var fields = FileTransferHelper.GetDownloadTypeInfo(endpoint);

                                usingResponseBlock.AddStatement($"var memoryStream = new {UseType("System.IO.MemoryStream")}();");
                                usingResponseBlock.AddStatement($"var responseStream  = await response.Content.{GetReadAsStreamAsyncMethodCall()};");
                                usingResponseBlock.AddStatement($"await responseStream.{GetCopyToAsyncMethodCall()};");
                                usingResponseBlock.AddStatement("memoryStream.Seek(0, SeekOrigin.Begin);");


                                var invocation = new CSharpInvocationStatement($"return {GetTypeName(endpoint.ReturnType)}", $"Create");
                                invocation.AddArgument("memoryStream");
                                if (fields.HasFilename())
                                {
                                    invocation.AddArgument($"{fields.FileNameField.ToParameterName()}: response.Content.Headers.ContentDisposition?.FileName");
                                }
                                if (fields.HasContentType())
                                {
                                    invocation.AddArgument($"{fields.ContentTypeField.ToParameterName()}: response.Content.Headers.ContentType?.MediaType ?? \"\"");
                                }
                                usingResponseBlock.AddStatement(invocation, s => s.SeparatedFromPrevious());
                            }
                            else
                            {
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
                                        addSerializerOptions = true;
                                    }
                                    else if (!isWrappedReturnType && returnsString && !returnsCollection)
                                    {
                                        usingContentStreamBlock.AddStatement($"var str = await new {UseType("System.IO.StreamReader")}(contentStream).{GetReadToEndMethodCall()}.ConfigureAwait(false);");
                                        usingContentStreamBlock.AddIfStatement("str.StartsWith('\"') || str.StartsWith('\\'')", stmt =>
                                        {
                                            if (outputTarget.GetProject().GetLanguageVersion().Major >= 8)
                                            {
                                                stmt.AddStatement("str = str[1..^1];");
                                            }
                                            else
                                            {
                                                stmt.AddStatement("str = str.Substring(1, str.Length - 2);");
                                            }
                                        });
                                        usingContentStreamBlock.AddStatement("return str;");
                                    }
                                    else if (!isWrappedReturnType && returnsPrimitive)
                                    {
                                        usingContentStreamBlock.AddStatement($"var str = await new {UseType("System.IO.StreamReader")}(contentStream).{GetReadToEndMethodCall()}.ConfigureAwait(false);");
                                        usingContentStreamBlock.AddIfStatement("str.StartsWith('\"') || str.StartsWith('\\'')", stmt =>
                                        {
                                            if (outputTarget.GetProject().GetLanguageVersion().Major >= 8)
                                            {
                                                stmt.AddStatement("str = str[1..^1];");
                                            }
                                            else
                                            {
                                                stmt.AddStatement("str = str.Substring(1, str.Length - 2);");
                                            }
                                        });

                                        var nonNullableType = GetTypeInfo(endpoint.ReturnType).WithIsNullable(false);
                                        if (endpoint.ReturnType.IsNullable)
                                        {
                                            usingContentStreamBlock.AddIfStatement("string.IsNullOrEmpty(str)", ifs => ifs.AddStatement("return null;"));
                                            usingContentStreamBlock.AddStatement($"return {UseType(nonNullableType)}.Parse(str);");
                                        }
                                        else
                                        {
                                            usingContentStreamBlock.AddStatement($"return {UseType(nonNullableType)}.Parse(str);");
                                        }
                                    }
                                    else
                                    {
                                        usingContentStreamBlock.AddStatement($"return {SuppressNullable($"await JsonSerializer.DeserializeAsync<{GetTypeName(endpoint.ReturnType)}>(contentStream, _serializerOptions, cancellationToken).ConfigureAwait(false)")};");
                                        addSerializerOptions = true;
                                    }
                                });
                            }
                        });
                    });
                }

                if (addSerializerOptions)
                {
                    @class.AddField("JsonSerializerOptions", "_serializerOptions", f => f.PrivateReadOnly());

                    var jsonSettingsBlock = new CSharpObjectInitializerBlock("_serializerOptions = new JsonSerializerOptions")
                            .AddInitStatement("PropertyNamingPolicy", "JsonNamingPolicy.CamelCase")
                            .WithSemicolon();
                    if (this.SerializeEnumsAsStrings(model.InternalElement))
                    {
                        AddUsing("System.Text.Json.Serialization");
                        jsonSettingsBlock.AddInitStatement("Converters", "{ new JsonStringEnumConverter() }");
                    }

                    @class.Constructors.First().AddStatement(jsonSettingsBlock);
                }

                @class.AddMethod("void", "Dispose", method =>
                {
                    method.AddInvocationStatement("Dispose", inv =>
                    {
                        inv.AddArgument("true");
                    });

                    method.AddInvocationStatement($"{UseType("System.GC")}.SuppressFinalize", inv =>
                    {
                        inv.AddArgument("this");
                    });
                });

                @class.AddMethod("void", "Dispose", method =>
                {
                    method.Virtual();
                    method.Protected();

                    method.AddParameter("bool", "disposing");

                    method.AddStatement("// Class cleanup goes here");
                });
            });
    }

    private string GetSourceExpression(string? methodParameterName, IHttpEndpointModel endpoint, IHttpEndpointInputModel input)
    {
        var hasSingleFieldChild = endpoint.InternalElement.ChildElements.Count(x => x.IsDTOFieldModel()) == 1;
        if (!Model.CreateParameterPerInput && hasSingleFieldChild)
        {
            return methodParameterName!;
        }

        return Model.CreateParameterPerInput || endpoint.InternalElement.Id == input.TypeReference.ElementId
            ? input.Name.ToParameterName()
            : $"{methodParameterName!}.{input.Name.ToPascalCase()}";
    }

    public override void AfterTemplateRegistration()
    {
        base.AfterTemplateRegistration();
        PagedResultTypeSource.ApplyTo(this, _pagedResultTemplateId);
    }

    private static void AddRequestContentLength(bool addcontentNullablityCheck, CSharpClassMethod method, string headerParameterName, bool isNullable)
    {
        var statementText = $"httpRequest.Content.Headers.ContentLength = {headerParameterName};";
        var ifStatement = addcontentNullablityCheck ? "httpRequest.Content != null" : "";

        if (isNullable)
        {
            ifStatement = ifStatement == string.Empty ? $"{headerParameterName} != null" : $"&& {headerParameterName} != null";
        }

        method.AddIfStatement(ifStatement, stmt =>
        {
            stmt.AddStatement(statementText);
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

    private string GetCopyToAsyncMethodCall()
    {
        return OutputTarget switch
        {
            _ when OutputTarget.GetProject().TargetFramework().StartsWith("netstandard") => "CopyToAsync(memoryStream)",
            _ => "CopyToAsync(memoryStream, cancellationToken)"
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

    private object GetParameterValueExpression(ICSharpFileBuilderTemplate template, DTOFieldModel field)
    {
        return !field.TypeReference.HasStringType()
            ? ConvertToString(template, field.Name.ToPascalCase(), field.TypeReference)
            : field.Name.ToPascalCase();
    }


    private static string GetParameterValueExpression(ICSharpFileBuilderTemplate template, IHttpEndpointInputModel input, string sourceExpression)
    {
        return !input.TypeReference.HasStringType()
            ? ConvertToString(template, sourceExpression, input.TypeReference)
            : sourceExpression;
    }

    private static string ConvertToString(ICSharpFileBuilderTemplate template, string variableName, ITypeReference typeRef)
    {
        string conversion;
        if (typeRef.HasBoolType())
        {
            conversion = $"ToString().ToLowerInvariant()";
        }
        else if (typeRef.HasDecimalType())
        {
            template.AddUsing("System.Globalization");
            conversion = $"ToString(CultureInfo.InvariantCulture)";
        }
        else if (typeRef.HasDateType() || typeRef.HasDateTimeOffsetType() || typeRef.HasDateTimeType())
        {
            conversion = $"ToString(\"o\")";
        }
        else if (typeRef.HasGuidType())
        {
            conversion = $"ToString(\"D\")";
        }
        else if (typeRef.Element.Name == "TimeSpan")
        {
            conversion = $"ToString(\"c\")";
        }
        else
        {
            conversion = $"ToString()";
        }
        return $"{variableName}{(typeRef.IsNullable ? "?" : "")}.{conversion}";
    }

    private void AddDtoQueryParams(CSharpClassMethod method, string dtoVar, DTOModel dto)
    {
        if (dto == null || string.IsNullOrEmpty(dtoVar))
        {
            return;
        }

        var block = new CSharpStatementBlock();
        foreach (var field in dto.Fields)
        {
            ProcessQueryField(block, dtoVar, field);
        }

        if (block.Statements.Count > 0)
        {
            method.AddStatement($"ArgumentNullException.ThrowIfNull({dtoVar});");
            method.AddStatements(block.Statements);
        }
    }

    private void ProcessQueryField(CSharpStatementBlock block, string dtoVar, DTOFieldModel field)
    {
        var fieldAccess = $"{dtoVar}.{field.Name.ToPascalCase()}";
        var fieldName = field.Name.ToCamelCase();

        // Check if this field is itself a DTO - handle nested DTOs recursively
        if (field.TypeReference.Element.IsDTOModel())
        {
            var nestedDto = field.TypeReference.Element.AsDTOModel();
            foreach (var nestedField in nestedDto.Fields)
            {
                ProcessNestedQueryField(block, fieldAccess, fieldName, nestedField);
            }
        }
        else if (field.TypeReference.IsCollection)
        {
            // Handle collection fields - check if collection is not null
            var variableName = $"{field.Name.ToLocalVariableName()}Index";
            block.AddIfStatement($"{fieldAccess} != null", collBlock =>
            {
                collBlock.AddStatement($"var {variableName} = 0;");
                collBlock.AddForEachStatement("element", fieldAccess, elementBlock =>
                {
                    elementBlock.AddStatement($@"queryParams.Add($""{fieldName}[{{{variableName}++}}]"", element.ToString());");
                });
            });
        }
        else if (field.TypeReference.IsNullable)
        {
            // Handle nullable fields - check if field is not null
            var valueExpression = !field.TypeReference.HasStringType()
                ? ConvertToString(this, fieldAccess, field.TypeReference)
                : fieldAccess;
            block.AddIfStatement($"{fieldAccess} != null", nullCheckBlock =>
            {
                nullCheckBlock.AddStatement($"queryParams.Add(\"{fieldName}\", {valueExpression});");
            });
        }
        else
        {
            // Handle non-nullable fields - no null check needed
            var valueExpression = !field.TypeReference.HasStringType()
                ? ConvertToString(this, fieldAccess, field.TypeReference)
                : fieldAccess;
            block.AddStatement($"queryParams.Add(\"{fieldName}\", {valueExpression});");
        }
    }

    private void ProcessNestedQueryField(CSharpStatementBlock block, string parentFieldAccess, string parentFieldName, DTOFieldModel field)
    {
        var fieldAccess = $"{parentFieldAccess}.{field.Name.ToPascalCase()}";
        var fieldAccessWithNullable = $"{parentFieldAccess}?.{field.Name.ToPascalCase()}";
        var fieldName = $"{parentFieldName}.{field.Name.ToCamelCase()}";

        // Check if this nested field is itself a DTO - handle deeply nested DTOs recursively
        if (field.TypeReference.Element.IsDTOModel())
        {
            var nestedDto = field.TypeReference.Element.AsDTOModel();
            block.AddIfStatement($"{fieldAccessWithNullable} != null", nestedBlock =>
            {
                foreach (var nestedField in nestedDto.Fields)
                {
                    ProcessNestedQueryField(nestedBlock, fieldAccess, fieldName, nestedField);
                }
            });
        }
        else if (field.TypeReference.IsCollection)
        {
            // Handle collection fields in nested DTOs
            var variableName = $"{field.Name.ToLocalVariableName()}Index";
            block.AddIfStatement($"{fieldAccessWithNullable} != null", collBlock =>
            {
                collBlock.AddStatement($"var {variableName} = 0;");
                collBlock.AddForEachStatement("element", fieldAccess, elementBlock =>
                {
                    elementBlock.AddStatement($@"queryParams.Add($""{fieldName}[{{{variableName}++}}]"", element.ToString());");
                });
            });
        }
        else if (field.TypeReference.IsNullable)
        {
            // Handle nullable fields in nested DTOs
            var valueExpression = !field.TypeReference.HasStringType()
                ? ConvertToString(this, fieldAccess, field.TypeReference)
                : fieldAccess;
            block.AddIfStatement($"{fieldAccessWithNullable} != null", nullCheckBlock =>
            {
                nullCheckBlock.AddStatement($"queryParams.Add(\"{fieldName}\", {valueExpression});");
            });
        }
        else
        {
            // Handle non-nullable fields in nested DTOs
            var valueExpression = !field.TypeReference.HasStringType()
                ? ConvertToString(this, fieldAccess, field.TypeReference)
                : fieldAccess;
            block.AddStatement($"queryParams.Add(\"{fieldName}\", {valueExpression});");
        }
    }
}
