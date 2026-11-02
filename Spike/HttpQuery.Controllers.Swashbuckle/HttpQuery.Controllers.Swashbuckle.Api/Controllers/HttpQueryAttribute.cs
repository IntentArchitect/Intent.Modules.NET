using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Mvc.Routing;

namespace HttpQuery.Controllers.Swashbuckle.Api.Controllers
{
    /// <summary>
    /// Identifies an action that supports the HTTP QUERY method (RFC 10008). ASP.NET Core 10 ships
    /// <see cref="HttpMethods.Query"/> but no attribute for it, so this mirrors <see cref="Microsoft.AspNetCore.Mvc.HttpPostAttribute"/>.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    public sealed class HttpQueryAttribute : HttpMethodAttribute
    {
        // Qualified: HttpMethodAttribute has its own HttpMethods property.
        private static readonly IEnumerable<string> SupportedMethods = [Microsoft.AspNetCore.Http.HttpMethods.Query];

        public HttpQueryAttribute()
            : base(SupportedMethods)
        {
        }

        public HttpQueryAttribute([StringSyntax("Route")] string template)
            : base(SupportedMethods, template)
        {
            ArgumentNullException.ThrowIfNull(template);
        }
    }
}
