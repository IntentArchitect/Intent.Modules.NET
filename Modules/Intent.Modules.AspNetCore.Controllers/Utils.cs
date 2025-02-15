﻿using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modules.AspNetCore.Controllers.Templates;
using Intent.Modules.AspNetCore.Controllers.Templates.Controller;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.Metadata.WebApi.Models;

namespace Intent.Modules.AspNetCore.Controllers;

public static class Utils
{

    private static readonly Dictionary<string, ResponseCodeLookup> ResponseCodeLookups = new() 
        { 
            { "200 (Ok)", new ResponseCodeLookup("200", "Status200OK", resultExpression => $"Ok({resultExpression})") },
            { "201 (Created)", new ResponseCodeLookup("201", "Status201Created", resultExpression => string.IsNullOrEmpty(resultExpression) ? "Created(string.Empty, null)" : $"Created(string.Empty, {resultExpression})") },
            { "202 (Accepted)", new ResponseCodeLookup("202", "Status202Accepted", resultExpression => $"Accepted({resultExpression})") },
            { "203 (Non-Authoritative Information)", new ResponseCodeLookup("203", "Status203NonAuthoritative", resultExpression => $"StatusCode(203{(string.IsNullOrEmpty(resultExpression) ? "" : $", {resultExpression}")})") },
            { "204 (No Content)", new ResponseCodeLookup("204", "Status204NoContent", _ => "NoContent()") },
            { "205 (Reset Content)", new ResponseCodeLookup("205", "Status205ResetContent", resultExpression => $"StatusCode(205{(string.IsNullOrEmpty(resultExpression) ? "" : $", {resultExpression}")})") },
            { "206 (Partial Content)", new ResponseCodeLookup("206", "Status206PartialContent", resultExpression => $"StatusCode(206{(string.IsNullOrEmpty(resultExpression) ? "" : $", {resultExpression}")})") },
            { "207 (Multi-Status)", new ResponseCodeLookup("207", "Status207MultiStatus", resultExpression => $"StatusCode(207{(string.IsNullOrEmpty(resultExpression) ? "" : $", {resultExpression}")})") },
            { "208 (Already Reported)", new ResponseCodeLookup("208", "Status208AlreadyReported", resultExpression => $"StatusCode(208{(string.IsNullOrEmpty(resultExpression) ? "" : $", {resultExpression}")})") },
            { "226 (IM Used)", new ResponseCodeLookup("226", "Status226IMUsed", resultExpression => $"StatusCode(226{(string.IsNullOrEmpty(resultExpression) ? "" : $", {resultExpression}")})") }
        };

    internal record ResponseCodeLookup(string Code, string StatusCodesEnumValue, Func<string?, string> ReturnOperation);

    public static bool ShouldBeJsonResponseWrapped(this ICSharpTemplate template, IControllerOperationModel operationModel)
    {
        var isWrappedReturnType = operationModel.MediaType == HttpMediaType.ApplicationJson;
        var returnsCollection = operationModel.ReturnType?.IsCollection == true;
        var returnsString = operationModel.ReturnType.HasStringType();
        var returnsPrimitive = template.GetTypeInfo(operationModel.ReturnType!).IsPrimitive &&
                               !returnsCollection;

        return isWrappedReturnType && (returnsPrimitive || returnsString);
    }

    private static ResponseCodeLookup? GetResponseCodeLookup(this IControllerOperationModel operation)
    {
        if (!operation.InternalElement.HasStereotype("Http Settings"))
        {
            return null;
        }

        var settings = operation.InternalElement.GetStereotype("Http Settings");
        if (!settings.TryGetProperty("Success Response Code", out var property))
        {
            return null;
        }

        //Not specified use the default
        if (string.IsNullOrEmpty(property.Value))
        {
            return null;
        }

        return ResponseCodeLookups.GetValueOrDefault(property.Value);
    }

    internal static string GetSuccessResponseCode(this IControllerOperationModel operation, string defaultValue)
    {
        var lookup = operation.GetResponseCodeLookup();
        if (lookup == null)
        {
            return defaultValue;
        }
        return lookup.Code;
    }


    internal static string GetSuccessResponseCodeEnumValue(this IControllerOperationModel operation, string defaultValue)
    {
        var lookup = operation.GetResponseCodeLookup();
        if (lookup == null)
        {
            return defaultValue;
        }
        return lookup.StatusCodesEnumValue;
    }

    private static string GetSuccessResponseCodeOperation(this IControllerOperationModel operation, string defaultValue, string? resultExpression)
    {
        var lookup = operation.GetResponseCodeLookup();
        if (lookup == null)
        {
            return defaultValue;
        }
        return lookup.ReturnOperation(resultExpression);
    }

    public static CSharpStatement GetReturnStatement(this ControllerTemplate template, IControllerOperationModel operationModel)
    {
        if (FileTransferHelper.IsFileDownloadOperation( operationModel))
        {
            var dtoInfo = FileTransferHelper.GetDownloadTypeInfo(operationModel);
            return @$"if (result == null)
            {{
                return NotFound();
            }}
            return File(result.{dtoInfo.StreamField}{(string.IsNullOrEmpty(dtoInfo.ContentTypeField) ? ", \"application/octet-stream\"" : $", result.{dtoInfo.ContentTypeField} ?? \"application/octet-stream\"" )}{(string.IsNullOrEmpty(dtoInfo.FileNameField) ? "" : $", result.{dtoInfo.FileNameField}")});";
        }
        var hasReturnType = operationModel.ReturnType != null;

        var resultExpression = default(string);
        if (hasReturnType)
        {
            resultExpression = template.ShouldBeJsonResponseWrapped(operationModel)
                ? $"new {template.GetJsonResponseName()}<{template.GetTypeName(operationModel.ReturnType)}>(result)"
            : "result";
        }


        string? returnExpression;
        switch (operationModel.Verb)
        {
            case HttpVerb.Get:
            case HttpVerb.Patch:
            case HttpVerb.Put:
                returnExpression = $"{operationModel.GetSuccessResponseCodeOperation(hasReturnType ? $"Ok({resultExpression})" : "NoContent()", resultExpression)}";
                break;
            case HttpVerb.Delete:
                returnExpression = $"{operationModel.GetSuccessResponseCodeOperation(hasReturnType ? $"Ok({resultExpression})" : "Ok()", resultExpression)}";
                break;
            case HttpVerb.Post:
                switch (operationModel.Parameters.Count)
                {
                    case 1:
                        // Aggregate
                        {
                            var getByIdOperation = template.Model.Operations.FirstOrDefault(x => x.Verb == HttpVerb.Get &&
                                x.ReturnType is { IsCollection: false } &&
                                x.Parameters.Count == 1 &&
                                string.Equals(x.Parameters[0].Name, "id", StringComparison.OrdinalIgnoreCase));
                            if (getByIdOperation != null && operationModel.ReturnType?.Element.Name is "guid" or "long" or "int" or "string")
                            {
                                returnExpression = $"CreatedAtAction(nameof({getByIdOperation.Name}), new {{ id = result }}, {resultExpression})";

                                // if multitenancy configured, and it's a http post, then add the tenant to the parameters
                                if(IsRouteMultiTenancyConfigured(template, operationModel) && operationModel.Verb == HttpVerb.Post)
                                {
                                    returnExpression = $"CreatedAtAction(nameof({getByIdOperation.Name}), new {{ id = result, tenant = tenantInfo.Id }}, {resultExpression})";
                                }
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
                            var getByIdOperation = template.Model.Operations.FirstOrDefault(x => x.Verb == HttpVerb.Get &&
                                x.ReturnType is { IsCollection: false } &&
                                x.Parameters.Count == 2 &&
                                string.Equals(x.Parameters[0].Name, operationModel.Parameters[0].Name, StringComparison.OrdinalIgnoreCase) &&
                                string.Equals(x.Parameters[1].Name, "id", StringComparison.OrdinalIgnoreCase));
                            if (getByIdOperation != null && operationModel.ReturnType?.Element.Name is "guid" or "long" or "int" or "string")
                            {
                                var aggregateIdParameter = getByIdOperation.Parameters[0].Name.ToCamelCase();
                                returnExpression = $"CreatedAtAction(nameof({getByIdOperation.Name}), new {{ {aggregateIdParameter} = {aggregateIdParameter}, id = result }}, {resultExpression})";

                                // if multitenancy configured, and it's a http post, then add the tenant to the parameters
                                if (IsRouteMultiTenancyConfigured(template, operationModel) && operationModel.Verb == HttpVerb.Post)
                                {
                                    returnExpression = $"CreatedAtAction(nameof({getByIdOperation.Name}), new {{ {aggregateIdParameter} = {aggregateIdParameter}, id = result, tenant = tenantInfo.Id }}, {resultExpression})";
                                }
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
                returnExpression ??= $"{operationModel.GetSuccessResponseCodeOperation(hasReturnType ? $"Created(string.Empty, {resultExpression})" : "Created(string.Empty, null)", resultExpression)}";
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        if (operationModel.ReturnType != null &&
            operationModel.CanReturnNotFound() &&
            !template.GetTypeInfo(operationModel.ReturnType).IsPrimitive)
        {
            returnExpression = $"result == null ? NotFound() : {returnExpression}";
        }

        return $"return {returnExpression};";
    }

    public static bool IsRouteMultiTenancyConfigured(this ControllerTemplate template, IControllerOperationModel operation)
    {
        // only apply if the strategy is route strategy
        var tenancyStrategy = template.ExecutionContext.GetSettings().GetSetting("41ae5a02-3eb2-42a6-ade2-322b3c1f1115", "e15fe0fb-be28-4cc5-8b85-37a07b7ca160");
        if (tenancyStrategy is null || tenancyStrategy.Value != "route")
        {
            return false;
        }

        // get the route parameter
        var routeParamValue = template.ExecutionContext.GetSettings().GetSetting("41ae5a02-3eb2-42a6-ade2-322b3c1f1115", "c8ff4af6-68b6-4e31-a291-43ada6a0008a");

        // if the method is not using the route parameter in the URL path
        return operation.Route?.Contains($"{{{routeParamValue?.Value ?? string.Empty}}}") == true;
    }

    public static bool CanReturnNotFound(this IControllerOperationModel operation)
    {
        if (operation.ReturnType?.IsNullable == true)
        {
            return false;
        }

        if (operation.Verb == HttpVerb.Get &&
            operation.ReturnType?.IsCollection != true &&
            !CSharpTypeIsCollection(operation.ReturnType) &&
            operation.Parameters.Any())
        {
            return true;
        }

        return operation.Parameters.Any(x => x.Name.EndsWith("id", StringComparison.OrdinalIgnoreCase) ||
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
            value = false;
            return false;
        }

        value = openApiSettings.GetProperty<bool>("Ignore");
        return true;
    }
}