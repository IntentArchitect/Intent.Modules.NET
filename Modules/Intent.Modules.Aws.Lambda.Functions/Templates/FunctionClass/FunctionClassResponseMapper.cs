using System.Collections.Generic;
using Intent.Modules.FastEndpoints.Intent.Modules.ApiHosting.Shared.Endpoints;

namespace Intent.Modules.Aws.Lambda.Functions.Templates.FunctionClass;

internal class FunctionClassResponseMapper : ResponseMapperBase
{
    private readonly Dictionary<string, ResponseCodeLookup> _mapper;
    
    public FunctionClassResponseMapper()
    {
        _mapper = new Dictionary<string, ResponseCodeLookup>
        {
            { SuccessResponseCodeKey.Ok, new ResponseCodeLookup("200") },
            { SuccessResponseCodeKey.Created, new ResponseCodeLookup("201") },
            { SuccessResponseCodeKey.Accepted, new ResponseCodeLookup("202") },
            { SuccessResponseCodeKey.NonAuthInfo, new ResponseCodeLookup("203") },
            { SuccessResponseCodeKey.NoContent, new ResponseCodeLookup("204") },
            { SuccessResponseCodeKey.ResetContent, new ResponseCodeLookup("205") },
            { SuccessResponseCodeKey.PartialContent, new ResponseCodeLookup("206") },
            { SuccessResponseCodeKey.MultiStatus, new ResponseCodeLookup("207") },
            { SuccessResponseCodeKey.AlreadyReported, new ResponseCodeLookup("208") },
            { SuccessResponseCodeKey.ImUsed, new ResponseCodeLookup("226") }
        };
    }
    
    protected override ResponseCodeLookup? GetResponseCodeLookup(string key)
    {
        return _mapper.GetValueOrDefault(key);
    }
}
