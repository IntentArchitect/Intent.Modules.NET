using System.Diagnostics.CodeAnalysis;
using Intent.RoslynWeaver.Attributes;
using Microsoft.AspNetCore.Mvc.Routing;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.AspNetCore.Controllers.HttpQueryAttribute", Version = "1.0")]

namespace IntegrationTesting.SQLLite.Tests.Api.Controllers
{
    /// <summary>
    /// Identifies an action that supports the HTTP QUERY method (RFC 10008). ASP.NET Core 10 ships
    /// <see cref="Microsoft.AspNetCore.Http.HttpMethods.Query"/> but no attribute for it, so this mirrors <see cref="Microsoft.AspNetCore.Mvc.HttpPostAttribute"/>.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    public sealed class HttpQueryAttribute : HttpMethodAttribute
    {
        private static readonly IEnumerable<string> SupportedMethods = [Microsoft.AspNetCore.Http.HttpMethods.Query];

        public HttpQueryAttribute() : base(SupportedMethods)
        {
        }

        public HttpQueryAttribute([StringSyntax("Route")] string template) : base(SupportedMethods, template)
        {
            ArgumentNullException.ThrowIfNull(template);
        }
    }
}