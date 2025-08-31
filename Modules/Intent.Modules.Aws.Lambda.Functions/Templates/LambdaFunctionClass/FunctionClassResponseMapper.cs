using System;
using System.Collections.Generic;
using Intent.Metadata.Models;
using Intent.Modules.ApiHosting.Shared.Endpoints;
using Intent.Modules.Aws.Lambda.Functions.Api;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Metadata.WebApi.Models;

namespace Intent.Modules.Aws.Lambda.Functions.Templates.LambdaFunctionClass;

internal class FunctionClassResponseMapper : ResponseMapperBase
{
    private readonly Dictionary<string, EndpointResponseCodeLookup> _mapper;
    
    public FunctionClassResponseMapper()
    {
        _mapper = new Dictionary<string, EndpointResponseCodeLookup>
        {
            { 
                SuccessResponseCodeKey.Ok, 
                new EndpointResponseCodeLookup("200", (_, hasValue, resultExpression) => hasValue ? $"HttpResults.Ok({resultExpression})" : "HttpResults.Ok()") 
            },
            {
                SuccessResponseCodeKey.Created, 
                new EndpointResponseCodeLookup("201", (_, hasValue, resultExpression) => hasValue ? $"HttpResults.Created(body: {resultExpression})" : "HttpResults.Created()")
            },
            {
                SuccessResponseCodeKey.Accepted, 
                new EndpointResponseCodeLookup("202", (_, hasValue, resultExpression) => hasValue ? $"HttpResults.Accepted({resultExpression})" : "HttpResults.Accepted()")
            },
            {
                SuccessResponseCodeKey.NonAuthInfo, 
                new EndpointResponseCodeLookup("203", (template, hasValue, resultExpression) => hasValue ? $"HttpResults.NewResult({GetHttpStatusCode(template)}.NonAuthoritativeInformation, {resultExpression})" : $"HttpResults.NewResult({GetHttpStatusCode(template)}.NonAuthoritativeInformation)")
            },
            {
                SuccessResponseCodeKey.NoContent, 
                new EndpointResponseCodeLookup("204", (template, _, _) => $"HttpResults.NewResult({GetHttpStatusCode(template)}.NoContent)")
            },
            {
                SuccessResponseCodeKey.ResetContent, 
                new EndpointResponseCodeLookup("205", (template, hasValue, resultExpression) => hasValue ? $"HttpResults.NewResult({GetHttpStatusCode(template)}.ResetContent, {resultExpression})" : $"HttpResults.NewResult({GetHttpStatusCode(template)}.ResetContent)")
            },
            {
                SuccessResponseCodeKey.PartialContent, 
                new EndpointResponseCodeLookup("206", (template, hasValue, resultExpression) => hasValue ? $"HttpResults.NewResult({GetHttpStatusCode(template)}.PartialContent, {resultExpression})" : $"HttpResults.NewResult({GetHttpStatusCode(template)}.PartialContent)")
            },
            {
                SuccessResponseCodeKey.MultiStatus, 
                new EndpointResponseCodeLookup("207", (template, hasValue, resultExpression) => hasValue ? $"HttpResults.NewResult({GetHttpStatusCode(template)}.MultiStatus, {resultExpression})" : $"HttpResults.NewResult({GetHttpStatusCode(template)}.MultiStatus)")
            },
            {
                SuccessResponseCodeKey.AlreadyReported, 
                new EndpointResponseCodeLookup("208", (template, hasValue, resultExpression) => hasValue ? $"HttpResults.NewResult({GetHttpStatusCode(template)}.AlreadyReported, {resultExpression})" : $"HttpResults.NewResult({GetHttpStatusCode(template)}.AlreadyReported)")
            },
            {
                SuccessResponseCodeKey.ImUsed, 
                new EndpointResponseCodeLookup("226", (template, hasValue, resultExpression) => hasValue ? $"HttpResults.NewResult({GetHttpStatusCode(template)}.IMUsed, {resultExpression})" : $"HttpResults.NewResult({GetHttpStatusCode(template)}.IMUsed)")
            }
        };
    }

    private static string GetHttpStatusCode(ICSharpTemplate template)
    {
        return template.UseType("System.Net.HttpStatusCode");
    }
    
    protected override ResponseCodeLookup? GetResponseCodeLookup(string key)
    {
        return _mapper.GetValueOrDefault(key);
    }
    
    public string GetSuccessResponseCodeOperation(ICSharpTemplate template, ILambdaFunctionModel operation, string? resultExpression, DefaultResultExpression defaultResultExpressionFunc)
    {
        ArgumentNullException.ThrowIfNull(operation);
        ArgumentNullException.ThrowIfNull(defaultResultExpressionFunc);
        
        var hasReturnType = operation.ReturnType is not null;
        var lookup = (EndpointResponseCodeLookup?)GetResponseCodeLookup(operation.InternalElement);
        if (lookup == null)
        {
            return defaultResultExpressionFunc(hasReturnType, operation.Verb, resultExpression);
        }

        return lookup.ResultExpressionFunc(template, hasReturnType, resultExpression);
    }
}

internal delegate string DefaultResultExpression(bool hasReturnType, HttpVerb verb, string? resultExpression);
internal delegate string ResponseCodeResultExpression(ICSharpTemplate template, bool hasReturnType, string? resultExpression);

internal record EndpointResponseCodeLookup(string Code, ResponseCodeResultExpression ResultExpressionFunc) : ResponseCodeLookup(Code);