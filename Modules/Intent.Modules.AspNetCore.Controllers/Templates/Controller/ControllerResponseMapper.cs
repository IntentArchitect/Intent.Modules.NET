using System;
using System.Collections.Generic;
using Intent.Metadata.Models;
using Intent.Modules.FastEndpoints.Intent.Modules.ApiHosting.Shared.Endpoints;

namespace Intent.Modules.AspNetCore.Controllers.Templates.Controller;

internal class ControllerResponseMapper : ResponseMapperBase
{
    private readonly Dictionary<string, ControllerResponseCodeLookup> _mapper;
    
    public ControllerResponseMapper()
    {
        _mapper = new Dictionary<string, ControllerResponseCodeLookup>
        {
            { SuccessResponseCodeKey.Ok, new ControllerResponseCodeLookup("200", "StatusCodes.Status200OK", resultExpression => $"Ok({resultExpression})") },
            { SuccessResponseCodeKey.Created, new ControllerResponseCodeLookup("201", "StatusCodes.Status201Created", resultExpression => string.IsNullOrEmpty(resultExpression) ? "Created(string.Empty, null)" : $"Created(string.Empty, {resultExpression})") },
            { SuccessResponseCodeKey.Accepted, new ControllerResponseCodeLookup("202", "StatusCodes.Status202Accepted", resultExpression => $"Accepted({resultExpression})") },
            { SuccessResponseCodeKey.NonAuthInfo, new ControllerResponseCodeLookup("203", "StatusCodes.Status203NonAuthoritative", resultExpression => $"StatusCode(203{(string.IsNullOrEmpty(resultExpression) ? "" : $", {resultExpression}")})") },
            { SuccessResponseCodeKey.NoContent, new ControllerResponseCodeLookup("204", "StatusCodes.Status204NoContent", _ => "NoContent()") },
            { SuccessResponseCodeKey.ResetContent, new ControllerResponseCodeLookup("205", "StatusCodes.Status205ResetContent", resultExpression => $"StatusCode(205{(string.IsNullOrEmpty(resultExpression) ? "" : $", {resultExpression}")})") },
            { SuccessResponseCodeKey.PartialContent, new ControllerResponseCodeLookup("206", "StatusCodes.Status206PartialContent", resultExpression => $"StatusCode(206{(string.IsNullOrEmpty(resultExpression) ? "" : $", {resultExpression}")})") },
            { SuccessResponseCodeKey.MultiStatus, new ControllerResponseCodeLookup("207", "StatusCodes.Status207MultiStatus", resultExpression => $"StatusCode(207{(string.IsNullOrEmpty(resultExpression) ? "" : $", {resultExpression}")})") },
            { SuccessResponseCodeKey.AlreadyReported, new ControllerResponseCodeLookup("208", "StatusCodes.Status208AlreadyReported", resultExpression => $"StatusCode(208{(string.IsNullOrEmpty(resultExpression) ? "" : $", {resultExpression}")})") },
            { SuccessResponseCodeKey.ImUsed, new ControllerResponseCodeLookup("226", "StatusCodes.Status226IMUsed", resultExpression => $"StatusCode(226{(string.IsNullOrEmpty(resultExpression) ? "" : $", {resultExpression}")})") }
        };
    }
    
    public string GetResponseStatusCodeEnum(IElement operation, string defaultEnum)
    {
        return ((ControllerResponseCodeLookup?)GetResponseCodeLookup(operation))?.StatusCodeEnum ?? defaultEnum;
    }
    
    public string GetSuccessResponseCodeOperation(IElement operation, string defaultValue, string? resultExpression)
    {
        var lookup = (ControllerResponseCodeLookup?)GetResponseCodeLookup(operation);
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

internal record ControllerResponseCodeLookup(string Code, string StatusCodeEnum, Func<string?, string> ReturnOperation) : ResponseCodeLookup(Code);