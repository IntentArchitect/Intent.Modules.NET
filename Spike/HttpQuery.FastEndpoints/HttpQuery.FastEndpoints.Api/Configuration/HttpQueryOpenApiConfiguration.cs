using System.Text.Json.Nodes;
using System.Text.RegularExpressions;
using HttpQuery.FastEndpoints.Api.Filters;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;

namespace HttpQuery.FastEndpoints.Api.Configuration
{
    /// <summary>
    /// Everything Microsoft.AspNetCore.OpenApi needs to describe HTTP QUERY endpoints.
    /// </summary>
    public static partial class HttpQueryOpenApiConfiguration
    {
        public const string OpenApiVersion = "3.2.0";

        public static IServiceCollection ConfigureHttpQueryOpenApi(this IServiceCollection services)
        {
            services.ConfigureAll<OpenApiOptions>(options => options.AddDocumentTransformer<HttpQueryDocumentTransformer>());
            return services;
        }

        /// <summary>
        /// Serves /openapi/{documentName}.json as OpenAPI 3.2. ASP.NET Core 10 writes at most 3.1 and has no serializer hook,
        /// so this writes the same document as 3.1 (3.2 only adds to it) and labels it 3.2.0, the first version with a native
        /// "query" operation. Registered ahead of MapOpenApi, which it answers for. Drop it once ASP.NET Core writes 3.2 itself.
        /// </summary>
        public static IApplicationBuilder UseHttpQueryOpenApi(this WebApplication app)
        {
            app.Use(async (context, next) =>
            {
                var match = OpenApiDocumentPath().Match(context.Request.Path.Value ?? string.Empty);
                var provider = match.Success && HttpMethods.IsGet(context.Request.Method)
                    ? context.RequestServices.GetKeyedService<IOpenApiDocumentProvider>(match.Groups[1].Value)
                    : null;
                if (provider is null)
                {
                    await next(context);
                    return;
                }

                var document = await provider.GetOpenApiDocumentAsync(context.RequestAborted);
                using var buffer = new StringWriter();
                document.SerializeAsV31(new OpenApiJsonWriter(buffer));
                var json = JsonNode.Parse(buffer.ToString())!.AsObject();
                json["openapi"] = OpenApiVersion;

                context.Response.ContentType = "application/json;charset=utf-8";
                await context.Response.WriteAsync(json.ToJsonString(), context.RequestAborted);
            });
            return app;
        }

        [GeneratedRegex(@"^/openapi/([^/]+)\.json$")]
        private static partial Regex OpenApiDocumentPath();
    }
}
