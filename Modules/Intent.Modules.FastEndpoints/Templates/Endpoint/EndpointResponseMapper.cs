using System;
using System.Collections.Generic;
using Intent.Metadata.Models;
using Intent.Modules.ApiHosting.Shared.Endpoints;

namespace Intent.Modules.FastEndpoints.Templates.Endpoint;

internal class EndpointResponseMapper : ResponseMapperBase
{
    private readonly Dictionary<string, EndpointResponseCodeLookup> _mapper;
    
    public EndpointResponseMapper()
    {
        _mapper = new Dictionary<string, EndpointResponseCodeLookup>
        {
            { SuccessResponseCodeKey.Ok, new EndpointResponseCodeLookup("200", "StatusCodes.Status200OK", resultExpression => $"TypedResults.Ok({resultExpression})") },
            { SuccessResponseCodeKey.Created, new EndpointResponseCodeLookup("201", "StatusCodes.Status201Created", resultExpression => string.IsNullOrEmpty(resultExpression) ? $"TypedResults.Created(string.Empty, (string)null)" : $"TypedResults.Created(string.Empty, {resultExpression})") },
            { SuccessResponseCodeKey.Accepted, new EndpointResponseCodeLookup("202", "StatusCodes.Status202Accepted", resultExpression => $"TypedResults.Accepted({(string.IsNullOrEmpty(resultExpression) ? "string.Empty" : resultExpression)})") },
            { SuccessResponseCodeKey.NonAuthInfo, new EndpointResponseCodeLookup("203", "StatusCodes.Status203NonAuthoritative", resultExpression => null) },
            { SuccessResponseCodeKey.NoContent, new EndpointResponseCodeLookup("204", "StatusCodes.Status204NoContent", resultExpression => $"TypedResults.NoContent()") },
            { SuccessResponseCodeKey.ResetContent, new EndpointResponseCodeLookup("205", "StatusCodes.Status205ResetContent", resultExpression => null) },
            { SuccessResponseCodeKey.PartialContent, new EndpointResponseCodeLookup("206", "StatusCodes.Status206PartialContent", resultExpression => null) },
            { SuccessResponseCodeKey.MultiStatus, new EndpointResponseCodeLookup("207", "StatusCodes.Status207MultiStatus", resultExpression => null) },
            { SuccessResponseCodeKey.AlreadyReported, new EndpointResponseCodeLookup("208", "StatusCodes.Status208AlreadyReported", resultExpression => null) },
            { SuccessResponseCodeKey.ImUsed, new EndpointResponseCodeLookup("226", "StatusCodes.Status226IMUsed", resultExpression => null) }
        };
    }
    
    public string GetResponseStatusCodeEnum(IElement operation, string defaultEnum)
    {
        return ((EndpointResponseCodeLookup?)GetResponseCodeLookup(operation))?.StatusCodeEnum ?? defaultEnum;
    }
    
    public string? GetSuccessResponseCodeOperation(IElement operation, string defaultValue, string? resultExpression)
    {
        var lookup = (EndpointResponseCodeLookup?)GetResponseCodeLookup(operation);
        if (lookup == null)
        {
            return defaultValue;
        }
        return lookup.ReturnOperation(resultExpression);
    }
    
    protected override ResponseCodeLookup? GetResponseCodeLookup(string key)
    {
        return _mapper.GetValueOrDefault(key);
    }
}

internal record EndpointResponseCodeLookup(string Code, string StatusCodeEnum, Func<string?, string?> ReturnOperation) : ResponseCodeLookup(Code);