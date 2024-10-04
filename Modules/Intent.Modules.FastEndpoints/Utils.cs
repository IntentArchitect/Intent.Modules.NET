#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.CSharp.VisualStudio;
using Intent.Modules.FastEndpoints.Templates;
using Intent.Modules.FastEndpoints.Templates.Endpoint;
using Intent.Modules.Metadata.WebApi.Models;

namespace Intent.Modules.FastEndpoints;

internal static class Utils
{
    private static readonly Dictionary<string, ResponseCodeLookup> _responseCodeLookup = new()
    {
        {
            "200 (Ok)", 
            new ResponseCodeLookup("200", "Status200OK", resultExpression => $"TypedResults.Ok({resultExpression})")
        },
        {
            "201 (Created)",
            new ResponseCodeLookup("201", "Status201Created", resultExpression => string.IsNullOrEmpty(resultExpression) ? $"TypedResults.Created(string.Empty, (string)null)" : $"TypedResults.Created(string.Empty, {resultExpression})")
        },
        {
            "202 (Accepted)", 
            new ResponseCodeLookup("202", "Status202Accepted", resultExpression => $"TypedResults.Accepted({(string.IsNullOrEmpty(resultExpression) ? "string.Empty" : resultExpression)})")
        },
        {
            "203 (Non-Authoritative Information)",
            new ResponseCodeLookup("203", "Status203NonAuthoritative", resultExpression => null)
        },
        {
            "204 (No Content)", 
            new ResponseCodeLookup("204", "Status204NoContent", resultExpression => $"TypedResults.NoContent()")
        },
        {
            "205 (Reset Content)",
            new ResponseCodeLookup("205", "Status205ResetContent", resultExpression => null)
        },
        {
            "206 (Partial Content)",
            new ResponseCodeLookup("206", "Status206PartialContent", resultExpression => null)
        },
        {
            "207 (Multi-Status)",
            new ResponseCodeLookup("207", "Status207MultiStatus", resultExpression => null)
        },
        {
            "208 (Already Reported)", 
            new ResponseCodeLookup("208", "Status208AlreadyReported", resultExpression => null)
        },
        {
            "226 (IM Used)",
            new ResponseCodeLookup("226", "Status226IMUsed", resultExpression => null)
        }
    };

    internal record ResponseCodeLookup(string Code, string StatusCodesEnumValue, Func<string?, string?> ReturnOperation);

    public static bool ShouldBeJsonResponseWrapped(this ICSharpTemplate template, IEndpointModel endpointModel)
    {
        if (endpointModel.ReturnType is null)
        {
            return false;
        }
        
        var isWrappedReturnType = endpointModel.MediaType == HttpMediaType.ApplicationJson;
        var returnsCollection = endpointModel.ReturnType?.IsCollection == true;
        var returnsString = endpointModel.ReturnType.HasStringType();
        var returnsPrimitive = template.GetTypeInfo(endpointModel.ReturnType).IsPrimitive &&
                               !returnsCollection;

        return isWrappedReturnType && (returnsPrimitive || returnsString);
    }

    private static ResponseCodeLookup? GetResponseCodeLookup(this IEndpointModel endpointModel)
    {
        if (!endpointModel.InternalElement.HasStereotype("Http Settings"))
        {
            return null;
        }

        var settings = endpointModel.InternalElement.GetStereotype("Http Settings");
        if (!settings.TryGetProperty("Success Response Code", out var property))
        {
            return null;
        }

        //Not specified use the default
        if (string.IsNullOrEmpty(property.Value))
        {
            return null;
        }

        return _responseCodeLookup.GetValueOrDefault(property.Value);
    }

    internal static string GetSuccessResponseCode(this IEndpointModel endpointModel, string defaultValue)
    {
        var lookup = endpointModel.GetResponseCodeLookup();
        if (lookup == null)
        {
            return defaultValue;
        }
    
        return lookup.Code;
    }


    internal static string GetSuccessResponseCodeEnumValue(this IEndpointModel endpointModel, string defaultValue)
    {
        var lookup = endpointModel.GetResponseCodeLookup();
        if (lookup == null)
        {
            return defaultValue;
        }

        return lookup.StatusCodesEnumValue;
    }

    private static string? GetSuccessResponseCodeOperation(this IEndpointModel endpointModel, string defaultValue, string? resultExpression)
    {
        var lookup = endpointModel.GetResponseCodeLookup();
        if (lookup == null)
        {
            return defaultValue;
        }
    
        return lookup.ReturnOperation(resultExpression);
    }

    public static CSharpStatement? GetReturnStatement(this EndpointTemplate template, IEndpointModel endpointModel)
    {
        template.AddUsing("Microsoft.AspNetCore.Http");

        var resultExpression = default(string);
        var hasReturnType = endpointModel.ReturnType is not null;
        if (hasReturnType)
        {
            resultExpression = template.ShouldBeJsonResponseWrapped(endpointModel)
                ? $"new {template.GetJsonResponseTemplateName()}<{template.GetTypeName(endpointModel.ReturnType)}>(result)"
                : "result";
        }

        string defaultResponseExpression;
        CSharpStatement? responseStatement;
        
        switch (endpointModel.Verb)
        {
            case HttpVerb.Get:
            case HttpVerb.Patch:
            case HttpVerb.Put:
                defaultResponseExpression = hasReturnType ? $"TypedResults.Ok({resultExpression})" : "TypedResults.NoContent()";
                responseStatement = GetResponseCodeStatement(endpointModel, defaultResponseExpression, resultExpression);
                break;
            case HttpVerb.Delete:
                defaultResponseExpression = hasReturnType ? $"TypedResults.Ok({resultExpression})" : "TypedResults.Ok()";
                responseStatement = GetResponseCodeStatement(endpointModel, defaultResponseExpression, resultExpression);
                break;
            case HttpVerb.Post:
                responseStatement = null;
                if (endpointModel.Parameters.Count == 1) // Aggregate Entity
                {
                    bool GetByIdForAggregateEntity(IEndpointModel x) =>
                        x.Verb == HttpVerb.Get && 
                        x.ReturnType is { IsCollection: false } && 
                        x.Parameters.Count == 1 &&
                        string.Equals(x.Parameters[0].Name, "id", StringComparison.OrdinalIgnoreCase);

                    var getByIdOperation = template.Model.Container.Endpoints.FirstOrDefault(GetByIdForAggregateEntity);
                    if (getByIdOperation != null && endpointModel.ReturnType?.Element.Name is "guid" or "long" or "int" or "string")
                    {
                        responseStatement = new CSharpInvocationStatement($"SendCreatedAtAsync<{template.GetEndpointTemplateName(getByIdOperation)}>")
                            .AddArgument("new { id = result }")
                            .AddArgument(resultExpression)
                            .AddArgument("cancellation: ct");
                        responseStatement = new CSharpAwaitExpression(responseStatement);
                        responseStatement.AddMetadata("response", "SendCreatedAtAsync");
                    }
                }
                else if (endpointModel.Parameters.Count == 2) // Owned Composite Entity
                {
                    bool GetByIdForOwnedCompositeEntity(IEndpointModel x) =>
                        x.Verb == HttpVerb.Get && 
                        x.ReturnType is { IsCollection: false } && 
                        x.Parameters.Count == 2 &&
                        string.Equals(x.Parameters[0].Name, endpointModel.Parameters[0].Name, StringComparison.OrdinalIgnoreCase) &&
                        string.Equals(x.Parameters[1].Name, "id", StringComparison.OrdinalIgnoreCase);

                    var getByIdOperation = template.Model.Container.Endpoints.FirstOrDefault(GetByIdForOwnedCompositeEntity);
                    if (getByIdOperation != null && endpointModel.ReturnType?.Element.Name is "guid" or "long" or "int" or "string")
                    {
                        var aggregateIdParameter = getByIdOperation.Parameters[0].Name.ToParameterName();
                        responseStatement = new CSharpInvocationStatement($"SendCreatedAtAsync<{template.GetEndpointTemplateName(getByIdOperation)}>")
                            .AddArgument($"new {{ {aggregateIdParameter} = req.{aggregateIdParameter.ToPropertyName()},  id = result }}")
                            .AddArgument(resultExpression)
                            .AddArgument("cancellation: ct");
                        responseStatement = new CSharpAwaitExpression(responseStatement);
                        responseStatement.AddMetadata("response", "SendCreatedAtAsync");
                    }
                }

                if (responseStatement is null)
                {
                    var nullableSymbol = template.OutputTarget.GetProject().NullableEnabled ? "?" : string.Empty;
                    defaultResponseExpression = hasReturnType ? $"TypedResults.Created(string.Empty, {resultExpression})" : $"TypedResults.Created(string.Empty, (string{nullableSymbol})null)";
                    responseStatement = GetResponseCodeStatement(endpointModel, defaultResponseExpression, resultExpression);
                }
                
                break;
            default:
                throw new ArgumentOutOfRangeException($"Unknown verb: {endpointModel.Verb}");
        }
        
        return responseStatement;
    }

    private static CSharpStatement GetResponseCodeStatement(IEndpointModel endpointModel, string defaultResponseExpression, string? resultExpression)
    {
        CSharpStatement responseStatement;
        var packagedResultExpression = endpointModel.GetSuccessResponseCodeOperation(defaultResponseExpression, resultExpression);
        if (packagedResultExpression is not null)
        {
            responseStatement = new CSharpAwaitExpression(new CSharpInvocationStatement("SendResultAsync")
                .AddArgument(packagedResultExpression));
            responseStatement.AddMetadata("response", "SendResultAsync");
        }
        else if (!string.IsNullOrEmpty(resultExpression))
        {
            responseStatement = new CSharpAwaitExpression(new CSharpInvocationStatement("SendAsync")
                .AddArgument(resultExpression)
                .AddArgument(endpointModel.GetSuccessResponseCode("204"))
                .AddArgument("ct"));
            responseStatement.AddMetadata("response", "SendAsync");
        }
        else
        {
            responseStatement = new CSharpAwaitExpression(new CSharpInvocationStatement("SendResultAsync")
                .AddArgument($"TypedResults.StatusCode({endpointModel.GetSuccessResponseCode("204")})"));
            responseStatement.AddMetadata("response", "SendResultAsync");
        }

        return responseStatement;
    }

    public static bool CanReturnNotFound(this IEndpointModel endpointModel)
    {
        if (endpointModel.ReturnType?.IsNullable == true)
        {
            return false;
        }

        if (endpointModel.Verb == HttpVerb.Get &&
            endpointModel.ReturnType?.IsCollection != true &&
            !CSharpTypeIsCollection(endpointModel.ReturnType) &&
            endpointModel.Parameters.Any())
        {
            return true;
        }

        return endpointModel.Parameters.Any(x => x.Name.EndsWith("id", StringComparison.OrdinalIgnoreCase) ||
                                             x.Name.StartsWith("id", StringComparison.OrdinalIgnoreCase));

        static bool CSharpTypeIsCollection(ITypeReference? typeReference)
        {
            if (typeReference?.Element is not IElement element)
            {
                return false;
            }

            var stereotype = element.GetStereotype("C#");
            if (stereotype == null)
            {
                return false;
            }

            return stereotype.GetProperty<bool>("Is Collection");
        }
    }

    public static bool TryGetIsIgnoredForApiExplorer(this IElement? element, out bool value)
    {
        var openApiSettings = element?.GetStereotype("OpenAPI Settings");
        if (openApiSettings == null)
        {
            value = default;
            return false;
        }

        value = openApiSettings.GetProperty<bool>("Ignore");
        return true;
    }
}