using System.Text;
using System.Text.Json.Nodes;
using System.Text.RegularExpressions;
using HttpQuery.Controllers.OpenApi.Api.Filters;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;

namespace HttpQuery.Controllers.OpenApi.Api.Configuration
{
    /// <summary>
    /// Everything Microsoft.AspNetCore.OpenApi needs to describe HTTP QUERY endpoints, plus Swagger UI on the same document.
    /// </summary>
    public static partial class HttpQueryOpenApiConfiguration
    {
        public const string OpenApiVersion = "3.2.0";

        // Swagger UI shows QUERY operations from 5.34.0; Swashbuckle.AspNetCore.SwaggerUI 10.3.1 still bundles 5.33.3.
        public const string SwaggerUiVersion = "5.34.1";

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

            // Swagger UI on the Microsoft.AspNetCore.OpenApi document, with QUERY in its submit methods.
            app.UseSwaggerUI(options =>
            {
                options.RoutePrefix = "swagger";
                options.SwaggerEndpoint("/openapi/v1.json", "v1");
                options.IndexStream = () => new MemoryStream(Encoding.UTF8.GetBytes(SwaggerUiIndex));
            });
            return app;
        }

        [GeneratedRegex(@"^/openapi/([^/]+)\.json$")]
        private static partial Regex OpenApiDocumentPath();

        private static readonly string SwaggerUiIndex = $$"""
            <!DOCTYPE html>
            <html lang="en">
            <head>
                <meta charset="UTF-8">
                <title>%(DocumentTitle)</title>
                <link rel="stylesheet" type="text/css" href="https://unpkg.com/swagger-ui-dist@{{SwaggerUiVersion}}/swagger-ui.css">
                %(HeadContent)
            </head>
            <body>
                <div id="swagger-ui"></div>
                <script src="https://unpkg.com/swagger-ui-dist@{{SwaggerUiVersion}}/swagger-ui-bundle.js"></script>
                <script src="https://unpkg.com/swagger-ui-dist@{{SwaggerUiVersion}}/swagger-ui-standalone-preset.js"></script>
                <script>
                    window.onload = function () {
                        var configObject = JSON.parse('%(ConfigObject)');
                        // Swashbuckle's SubmitMethod enum has no QUERY, so "Try it out" would be hidden for QUERY operations.
                        configObject.supportedSubmitMethods = (configObject.supportedSubmitMethods || []).concat(["query"]);
                        configObject.dom_id = "#swagger-ui";
                        configObject.presets = [SwaggerUIBundle.presets.apis, SwaggerUIStandalonePreset];
                        configObject.layout = "StandaloneLayout";
                        window.ui = SwaggerUIBundle(configObject);
                    };
                </script>
            </body>
            </html>
            """;
    }
}
