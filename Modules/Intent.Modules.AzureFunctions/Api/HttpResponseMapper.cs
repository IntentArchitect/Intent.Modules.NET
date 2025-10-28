using Intent.Metadata.Models;
using Intent.Modules.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intent.Modules.AzureFunctions.Api
{
    public class HttpResponseMapper : ResponseMapperBase
    {
        private readonly Dictionary<string, EndPointResponseCodeLookup> _mapper;

        public HttpResponseMapper()
        {
            _mapper = new Dictionary<string, EndPointResponseCodeLookup>
        {
            { SuccessResponseCodeKey.Ok, new EndPointResponseCodeLookup("200", "OK", resultExpression => string.IsNullOrEmpty(resultExpression) ? "OkResult()" : $"OkObjectResult({resultExpression})") },
            { SuccessResponseCodeKey.Created, new EndPointResponseCodeLookup("201", "Created", resultExpression => string.IsNullOrEmpty(resultExpression) ? "CreatedResult(string.Empty, null)" : $"CreatedResult(string.Empty, {resultExpression})") },
            { SuccessResponseCodeKey.Accepted, new EndPointResponseCodeLookup("202", "Accepted", resultExpression => $"AcceptedResult(string.Empty,{resultExpression})") },
            { SuccessResponseCodeKey.NonAuthInfo, new EndPointResponseCodeLookup("203", "NonAuthoritativeInformation", resultExpression => $"ObjectResult({resultExpression}) {{ StatusCode = 203 }}") },
            { SuccessResponseCodeKey.NoContent, new EndPointResponseCodeLookup("204", "NoContent", _ => "NoContentResult()") },
            { SuccessResponseCodeKey.ResetContent, new EndPointResponseCodeLookup("205", "ResetContent", resultExpression => $"ObjectResult({resultExpression}) {{ StatusCode = 205 }}") },
            { SuccessResponseCodeKey.PartialContent, new EndPointResponseCodeLookup("206", "PartialContent", resultExpression => $"ObjectResult({resultExpression}) {{ StatusCode = 206 }}") },
            { SuccessResponseCodeKey.MultiStatus, new EndPointResponseCodeLookup("207", "MultiStatus", resultExpression => $"ObjectResult({resultExpression}) {{ StatusCode = 207 }}") },
            { SuccessResponseCodeKey.AlreadyReported, new EndPointResponseCodeLookup("208", "AlreadyReported", resultExpression => $"ObjectResult({resultExpression}) {{ StatusCode = 208 }}") },
            { SuccessResponseCodeKey.ImUsed, new EndPointResponseCodeLookup("226", "IMUsed", resultExpression => $"ObjectResult({resultExpression}) {{ StatusCode = 226 }}") }
        };
        }

        public string GetResponseStatusCodeEnum(IElement operation, string defaultEnum)
        {
            return ((EndPointResponseCodeLookup?)GetResponseCodeLookup(operation))?.StatusCodeEnum ?? defaultEnum;
        }

        public string GetSuccessResponseCodeOperation(IElement operation, string defaultValue, string? resultExpression)
        {
            var lookup = (EndPointResponseCodeLookup?)GetResponseCodeLookup(operation);
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

    internal record EndPointResponseCodeLookup(string Code, string StatusCodeEnum, Func<string?, string> ReturnOperation) : ResponseCodeLookup(Code);

    public record ResponseCodeLookup(string Code);

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

    public abstract class ResponseMapperBase
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
}
