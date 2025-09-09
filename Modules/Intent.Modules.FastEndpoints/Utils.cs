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

    public static CSharpStatement? GetReturnStatement(this EndpointTemplate template, IEndpointModel endpointModel, EndpointResponseMapper responseMapper)
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
                responseStatement = GetResponseCodeStatement(endpointModel, defaultResponseExpression, resultExpression, responseMapper);
                break;
            case HttpVerb.Delete:
                defaultResponseExpression = hasReturnType ? $"TypedResults.Ok({resultExpression})" : "TypedResults.Ok()";
                responseStatement = GetResponseCodeStatement(endpointModel, defaultResponseExpression, resultExpression, responseMapper);
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
                        responseStatement = new CSharpInvocationStatement($"Send.CreatedAtAsync<{template.GetEndpointTemplateName(getByIdOperation)}>")
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
                        responseStatement = new CSharpInvocationStatement($"Send.CreatedAtAsync<{template.GetEndpointTemplateName(getByIdOperation)}>")
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
                    responseStatement = GetResponseCodeStatement(endpointModel, defaultResponseExpression, resultExpression, responseMapper);
                }
                
                break;
            default:
                throw new ArgumentOutOfRangeException($"Unknown verb: {endpointModel.Verb}");
        }
        
        return responseStatement;
    }
    
    private static CSharpStatement GetResponseCodeStatement(IEndpointModel endpointModel, string defaultResponseExpression, string? resultExpression, EndpointResponseMapper responseMapper)
    {
        CSharpStatement responseStatement;
        var packagedResultExpression = responseMapper.GetSuccessResponseCodeOperation(endpointModel.InternalElement, defaultResponseExpression, resultExpression);
        if (packagedResultExpression is not null)
        {
            responseStatement = new CSharpAwaitExpression(new CSharpInvocationStatement("Send.ResultAsync")
                .AddArgument(packagedResultExpression));
            responseStatement.AddMetadata("response", "SendResultAsync");
        }
        else if (!string.IsNullOrEmpty(resultExpression))
        {
            responseStatement = new CSharpAwaitExpression(new CSharpInvocationStatement("Send.ResponseAsync")
                .AddArgument(resultExpression)
                .AddArgument(responseMapper.GetResponseCode(endpointModel.InternalElement, "204"))
                .AddArgument("ct"));
            responseStatement.AddMetadata("response", "SendAsync");
        }
        else
        {
            responseStatement = new CSharpAwaitExpression(new CSharpInvocationStatement("Send.ResultAsync")
                .AddArgument($"TypedResults.StatusCode({responseMapper.GetResponseCode(endpointModel.InternalElement, "204")})"));
            responseStatement.AddMetadata("response", "SendResultAsync");
        }
    
        return responseStatement;
    }
}