#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.FastEndpoints.Templates;
using Intent.Modules.FastEndpoints.Templates.Endpoint;
using Intent.Modules.Metadata.WebApi.Models;

namespace Intent.Modules.FastEndpoints;

internal static class Utils
{
    private static readonly Dictionary<string, ResponseCodeLookup> _responseCodeLookup = new()
    {
        { "200 (Ok)", new ResponseCodeLookup("200", "Status200OK", resultExpression => $"Ok({resultExpression})") },
        {
            "201 (Created)",
            new ResponseCodeLookup("201", "Status201Created",
                resultExpression => string.IsNullOrEmpty(resultExpression) ? $"Created(string.Empty, null)" : $"Created(string.Empty, {resultExpression})")
        },
        { "202 (Accepted)", new ResponseCodeLookup("202", "Status202Accepted", resultExpression => $"Accepted({resultExpression})") },
        {
            "203 (Non-Authoritative Information)",
            new ResponseCodeLookup("203", "Status203NonAuthoritative",
                resultExpression => $"StatusCode(203{(string.IsNullOrEmpty(resultExpression) ? "" : $", {resultExpression}")})")
        },
        { "204 (No Content)", new ResponseCodeLookup("204", "Status204NoContent", resultExpression => $"NoContent()") },
        {
            "205 (Reset Content)",
            new ResponseCodeLookup("205", "Status205ResetContent", resultExpression => $"StatusCode(205{(string.IsNullOrEmpty(resultExpression) ? "" : $", {resultExpression}")})")
        },
        {
            "206 (Partial Content)",
            new ResponseCodeLookup("206", "Status206PartialContent",
                resultExpression => $"StatusCode(206{(string.IsNullOrEmpty(resultExpression) ? "" : $", {resultExpression}")})")
        },
        {
            "207 (Multi-Status)",
            new ResponseCodeLookup("207", "Status207MultiStatus", resultExpression => $"StatusCode(207{(string.IsNullOrEmpty(resultExpression) ? "" : $", {resultExpression}")})")
        },
        {
            "208 (Already Reported)",
            new ResponseCodeLookup("208", "Status208AlreadyReported",
                resultExpression => $"StatusCode(208{(string.IsNullOrEmpty(resultExpression) ? "" : $", {resultExpression}")})")
        },
        {
            "226 (IM Used)",
            new ResponseCodeLookup("226", "Status226IMUsed", resultExpression => $"StatusCode(226{(string.IsNullOrEmpty(resultExpression) ? "" : $", {resultExpression}")})")
        }
    };

    internal record ResponseCodeLookup(string Code, string StatusCodesEnumValue, Func<string?, string> ReturnOperation);

    public static bool ShouldBeJsonResponseWrapped(this ICSharpTemplate template, IEndpointModel endpointModel)
    {
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

        if (!_responseCodeLookup.TryGetValue(property.Value, out var result))
        {
            return null;
        }

        return result;
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

    private static string GetSuccessResponseCodeOperation(this IEndpointModel endpointModel, string defaultValue, string? resultExpression)
    {
        var lookup = endpointModel.GetResponseCodeLookup();
        if (lookup == null)
        {
            return defaultValue;
        }

        return lookup.ReturnOperation(resultExpression);
    }

    public static CSharpStatement GetReturnStatement(this EndpointTemplate template, IEndpointModel endpointModel)
    {
        // if (FileTransferHelper.IsFileDownloadOperation( endpointModel))
        // {
        //     var dtoInfo = FileTransferHelper.GetDownloadTypeInfo(endpointModel);
        //     return @$"if (result == null)
        //     {{
        //         return NotFound();
        //     }}
        //     return File(result.{dtoInfo.StreamField}{(string.IsNullOrEmpty(dtoInfo.ContentTypeField) ? ", \"application/octet-stream\"" : $", result.{dtoInfo.ContentTypeField} ?? \"application/octet-stream\"" )}{(string.IsNullOrEmpty(dtoInfo.FileNameField) ? "" : $", result.{dtoInfo.FileNameField}")});";
        // }
        var hasReturnType = endpointModel.ReturnType != null;

        var resultExpression = default(string);
        if (hasReturnType)
        {
            resultExpression = template.ShouldBeJsonResponseWrapped(endpointModel)
                ? $"new {template.GetJsonResponseTemplateName()}<{template.GetTypeName(endpointModel.ReturnType)}>(result)"
                : "result";
        }


        string? returnExpression;
        switch (endpointModel.Verb)
        {
            case HttpVerb.Get:
            case HttpVerb.Patch:
            case HttpVerb.Put:
                returnExpression = $"{endpointModel.GetSuccessResponseCodeOperation(hasReturnType ? $"Ok({resultExpression})" : "NoContent()", resultExpression)}";
                break;
            case HttpVerb.Delete:
                returnExpression = $"{endpointModel.GetSuccessResponseCodeOperation(hasReturnType ? $"Ok({resultExpression})" : "Ok()", resultExpression)}";
                break;
            case HttpVerb.Post:
                switch (endpointModel.Parameters.Count)
                {
                    case 1:
                        // Aggregate
                    {
                        var getByIdOperation = template.Model.Container.Endpoints
                            .FirstOrDefault(x => x.Verb == HttpVerb.Get &&
                                                 x.ReturnType is { IsCollection: false } &&
                                                 x.Parameters.Count == 1 &&
                                                 string.Equals(x.Parameters[0].Name, "id",
                                                     StringComparison.OrdinalIgnoreCase));
                        if (getByIdOperation != null && endpointModel.ReturnType?.Element.Name is "guid" or "long" or "int" or "string")
                        {
                            returnExpression = $"CreatedAtAction(nameof({getByIdOperation.Name}), new {{ id = result }}, {resultExpression})";
                        }
                        else
                        {
                            returnExpression = null;
                        }
                    }
                        break;
                    case 2:
                        // Owned composite
                    {
                        var getByIdOperation = template.Model.Container.Endpoints
                            .FirstOrDefault(x => x.Verb == HttpVerb.Get &&
                                                 x.ReturnType is { IsCollection: false } &&
                                                 x.Parameters.Count == 2 &&
                                                 string.Equals(x.Parameters[0].Name, endpointModel.Parameters[0].Name, StringComparison.OrdinalIgnoreCase) &&
                                                 string.Equals(x.Parameters[1].Name, "id", StringComparison.OrdinalIgnoreCase));
                        if (getByIdOperation != null && endpointModel.ReturnType?.Element.Name is "guid" or "long" or "int" or "string")
                        {
                            var aggregateIdParameter = getByIdOperation.Parameters[0].Name.ToParameterName();
                            returnExpression =
                                $"CreatedAtAction(nameof({getByIdOperation.Name}), new {{ {aggregateIdParameter} = {aggregateIdParameter}, id = result }}, {resultExpression})";
                        }
                        else
                        {
                            returnExpression = null;
                        }
                    }
                        break;
                    default:
                        returnExpression = null;
                        break;
                }

                returnExpression ??=
                    $"{endpointModel.GetSuccessResponseCodeOperation(hasReturnType ? $"Created(string.Empty, {resultExpression})" : "Created(string.Empty, null)", resultExpression)}";
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        if (endpointModel.ReturnType != null &&
            endpointModel.CanReturnNotFound() &&
            !template.GetTypeInfo(endpointModel.ReturnType).IsPrimitive)
        {
            returnExpression = $"result == null ? NotFound() : {returnExpression}";
        }

        return $"return {returnExpression};";
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