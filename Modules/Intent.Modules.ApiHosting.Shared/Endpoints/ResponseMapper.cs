using System;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;

namespace Intent.Modules.FastEndpoints.Intent.Modules.ApiHosting.Shared.Endpoints;

internal record ResponseCodeLookup(string Code);

internal static class SuccessResponseCodeKey
{
    public const string Ok = "200 (Ok)";
    public const string Created = "201 (Created)";
    public const string Accepted = "202 (Accepted)";
    public const string NonAuthInfo = "203 (Non-Authoritative Information)";
    public const string NoContent = "204 (No Content)";
    public const string ResetContent = "205 (Reset Content)";
    public const string PartialContent = "206 (Partial Content)";
    public const string MultiStatus = "207 (Multi-Status)";
    public const string AlreadyReported = "208 (Already Reported)";
    public const string ImUsed = "226 (IM Used)";
}

internal abstract class ResponseMapperBase
{
    protected ResponseMapperBase()
    {
    }

    public string GetResponseCode(IElement operation, string defaultValue)
    {
        return GetResponseCodeLookup(operation)?.Code ?? defaultValue;
    }
    
    protected abstract ResponseCodeLookup? GetResponseCodeLookup(string key);
    
    protected ResponseCodeLookup? GetResponseCodeLookup(IElement operation)
    {
        if (!operation.HasStereotype("Http Settings"))
        {
            return null;
        }
        
        var httpSettingsElement = operation.GetStereotype("Http Settings");
        
        if (!httpSettingsElement.TryGetProperty("Success Response Code", out var property))
        {
            return null;
        }
        
        //Not specified use the default
        if (string.IsNullOrEmpty(property.Value))
        {
            return null;
        }
        
        return GetResponseCodeLookup(property.Value);
    }
}