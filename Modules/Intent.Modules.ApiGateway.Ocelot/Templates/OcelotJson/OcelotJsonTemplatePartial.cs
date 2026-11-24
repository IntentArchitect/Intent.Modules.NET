using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using Intent.ApiGateway.Ocelot.Api;
using Intent.Engine;
using Intent.Exceptions;
using Intent.Metadata.ApiGateway.Api;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.Modules.Common.FileBuilders.DataFileBuilder;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.ProjectItemTemplate.Partial", Version = "1.0")]

namespace Intent.Modules.ApiGateway.Ocelot.Templates.OcelotJson
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class OcelotJsonTemplate : IntentTemplateBase<IList<ApiGatewayRouteModel>>, IDataFileBuilderTemplate
    {
        [IntentManaged(Mode.Fully)]
        public const string TemplateId = "Intent.ApiGateway.Ocelot.OcelotJson";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public OcelotJsonTemplate(IOutputTarget outputTarget, IList<ApiGatewayRouteModel> model) : base(TemplateId, outputTarget, model)
        {
            // The writer merges what this template generates into the ocelot.json on disk (OcelotJsonMerger), so hand edits
            // survive regeneration without the Intent.Code.Weaving.Json module.
            DataFile = new DataFile($"ocelot")
                .WithWriter(() => new OcelotJsonWriter(() => _existingFile), "json")
                .WithRootObject(this, x =>
                {
                    _existingFile = ReadExistingFile();

                    var anySecured = Model.Any(route => !IsUnsecured(route));
                    var rateLimitedKeys = new List<string>();

                    x.WithArray("Routes", routes =>
                    {
                        var keys = new HashSet<string>(StringComparer.Ordinal);
                        foreach (var routeModel in Model)
                        {
                            if (!keys.Add(routeModel.Name))
                            {
                                throw new ElementException(routeModel.InternalElement,
                                    $"Another API Gateway Route is also named '{routeModel.Name}', usually because two routes target the same endpoint. A route's name becomes its Ocelot route Key, which must be unique. Remove the duplicate route.");
                            }

                            var entry = new DataFileObjectValue();
                            entry.WithValue("Key", routeModel.Name);
                            entry.WithValue("UpstreamPathTemplate", $"/{routeModel.GetUpstreamRouteInfo().Route}");
                            if (routeModel.GetUpstreamRouteInfo().Verb.HasValue)
                            {
                                entry.WithArray("UpstreamHttpMethod",
                                    arr => arr.Add(new DataFileScalarValue(routeModel.GetUpstreamRouteInfo().Verb.ToString())));
                            }

                            // Until we support aggregates we can assume there will be only one downstream service route
                            var downstreamOperation = routeModel.DownstreamEndpoints().FirstOrDefault()?.Element as IElement;
                            if (downstreamOperation is null)
                            {
                                continue;
                            }

                            // The designer flags this too, but a model saved with the error, or an endpoint whose Http Settings
                            // were removed later, must still fail here with the route named rather than as a null value in the writer.
                            var downstreamVerb = downstreamOperation.GetStereotype("Http Settings")?.GetProperty("Verb")?.Value;
                            if (string.IsNullOrWhiteSpace(downstreamVerb))
                            {
                                throw new ElementException(routeModel.InternalElement,
                                    $"API Gateway Route '{routeModel.Name}' targets '{downstreamOperation.Name}', which has no Http Settings, so there is no HTTP verb or route to forward to. Expose '{downstreamOperation.Name}' as an HTTP endpoint, or point the route at an endpoint that is.");
                            }

                            var downstreamRoute = downstreamOperation.GetStereotype("Http Settings")?.GetProperty("Route")?.Value;
                            var downstreamServiceRoute = downstreamOperation.ParentElement?.GetStereotype("Http Service Settings")?.GetProperty("Route")?.Value;
                            var separator = string.IsNullOrWhiteSpace(downstreamServiceRoute) || downstreamServiceRoute.EndsWith("/")
                                ? string.Empty
                                : !string.IsNullOrWhiteSpace(downstreamRoute)
                                ? "/"
                                : string.Empty;

                            entry.WithValue("DownstreamPathTemplate", $"/{downstreamServiceRoute}{separator}{downstreamRoute}");
                            entry.WithValue("DownstreamHttpMethod", downstreamVerb);
                            entry.WithArray("DownstreamHostAndPorts", arr => arr.Add(new DataFileObjectValue().WithValue("Host", $"{{{downstreamOperation.Package.Name}}}")));

                            if (HasTrailingPlaceholderSibling(routeModel))
                            {
                                entry.WithValue("Priority", 2);
                            }

                            if (anySecured && IsUnsecured(routeModel))
                            {
                                entry.WithObject("AuthenticationOptions", auth => auth.WithValue("AllowAnonymous", true));
                            }

                            // The route itself gets no RateLimitOptions: Ocelot rejects a route rule without Limit and Period, and
                            // a rule written here would be model-owned. The shared rule in GlobalConfiguration lists it by Key instead.
                            if (routeModel.HasRateLimit())
                            {
                                rateLimitedKeys.Add(routeModel.Name);
                            }

                            routes.Add(entry);
                        }
                    });

                    // Secured by default: the gateway is the only way in, so every route needs a bearer token unless the model
                    // marks it Unsecured. With no secured route at all no authentication is written, so a gateway without a
                    // Bearer scheme can still start.
                    // GlobalConfiguration itself is always written, even empty, so keys added to it by hand (such as BaseUrl)
                    // always have a home; only its contents vary.
                    x.WithObject("GlobalConfiguration", global =>
                    {
                        if (anySecured)
                        {
                            global.WithObject("AuthenticationOptions", auth => auth
                                .WithArray("AuthenticationProviderKeys", providers => providers.WithValue(AuthenticationScheme)));
                        }

                        if (rateLimitedKeys.Count > 0)
                        {
                            AddSharedRateLimitRule(global, rateLimitedKeys);
                        }
                    });
                });
        }

        // Ocelot looks the scheme up by name in the gateway's registered authentication schemes. Fixed until someone needs another.
        [IntentManaged(Mode.Ignore)]
        private const string AuthenticationScheme = "Bearer";

        [IntentManaged(Mode.Ignore)]
        private const string SecuredStereotypeId = "a9eade71-1d56-4be7-a80c-81046c0c978b";

        [IntentManaged(Mode.Ignore)]
        private const string UnsecuredStereotypeId = "8b65c29e-1448-43ac-a92a-0e0f86efd6c6";


        /// <summary>
        /// A route is unsecured when the closest Secured or Unsecured stereotype, looking at the route, its folders and then its
        /// package, is Unsecured. Where one element carries both, Secured wins, so a contradiction fails closed.
        /// </summary>
        [IntentManaged(Mode.Ignore)]
        private static bool IsUnsecured(ApiGatewayRouteModel route)
        {
            for (var element = route.InternalElement; element != null; element = element.ParentElement)
            {
                if (element.HasStereotype(SecuredStereotypeId))
                {
                    return false;
                }

                if (element.HasStereotype(UnsecuredStereotypeId))
                {
                    return true;
                }
            }

            var package = route.InternalElement.Package;
            return !package.HasStereotype(SecuredStereotypeId) && package.HasStereotype(UnsecuredStereotypeId);
        }

        /// <summary>
        /// Ocelot lets a trailing placeholder match nothing, so <c>/orders/{id}</c> also serves <c>GET /orders</c> when it is listed
        /// first, taking its authentication and rate limit with it. A route that another route of the same verb extends by one
        /// trailing placeholder gets a higher priority so it keeps its own requests.
        /// </summary>
        [IntentManaged(Mode.Ignore)]
        private bool HasTrailingPlaceholderSibling(ApiGatewayRouteModel route)
        {
            var path = route.GetUpstreamRouteInfo().Route?.Trim('/') ?? string.Empty;
            var verb = route.GetUpstreamRouteInfo().Verb;
            return Model.Any(other =>
            {
                if (other == route)
                {
                    return false;
                }

                var otherVerb = other.GetUpstreamRouteInfo().Verb;
                if (verb.HasValue && otherVerb.HasValue && verb != otherVerb)
                {
                    return false;
                }

                var otherPath = other.GetUpstreamRouteInfo().Route?.Trim('/') ?? string.Empty;
                var prefix = path.Length == 0 ? string.Empty : path + "/";
                if (!otherPath.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
                {
                    return false;
                }

                var lastSegment = otherPath.Substring(prefix.Length);
                return lastSegment.StartsWith("{") && lastSegment.EndsWith("}") && !lastSegment.Contains('/');
            });
        }

        /// <summary>
        /// Ocelot's one shared rate limit rule, applied to the marked routes by <c>RouteKeys</c>. <c>EnableRateLimiting</c> and
        /// <c>RouteKeys</c> follow the model; <c>Limit</c>, <c>Period</c>, <c>ClientIdHeader</c> and <c>ClientWhitelist</c> are
        /// starting values that <see cref="OcelotJsonMerger"/> writes only when the file does not have them yet.
        /// </summary>
        /// <remarks>
        /// Written only while a route is marked, and never with an empty <c>RouteKeys</c>: Ocelot applies a rule without keys
        /// to every route, and a disabled one switches off rate limits written by hand on individual routes.
        /// </remarks>
        [IntentManaged(Mode.Ignore)]
        private static void AddSharedRateLimitRule(IDataFileObjectValue global, IReadOnlyCollection<string> routeKeys)
        {
            global.WithObject("RateLimitOptions", options =>
            {
                options.WithValue("EnableRateLimiting", true);
                options.WithArray("RouteKeys", keys =>
                {
                    foreach (var key in routeKeys)
                    {
                        keys.WithValue(key);
                    }
                });
                options.WithValue("Limit", 100);
                options.WithValue("Period", "1m");
                options.WithValue("ClientIdHeader", "Oc-Client");
                options.WithArray("ClientWhitelist");
            });
        }

        [IntentManaged(Mode.Ignore)]
        private JsoncDocument? _existingFile;

        /// <summary>
        /// The <c>ocelot.json</c> on disk, which the generated content is merged into. A file that is not valid JSON stops the
        /// run rather than being overwritten, since that would lose everything written in it by hand.
        /// </summary>
        [IntentManaged(Mode.Ignore)]
        private JsoncDocument? ReadExistingFile()
        {
            if (!TryGetExistingFileContent(out var content))
            {
                return null;
            }

            try
            {
                return JsoncParser.Parse(content);
            }
            catch (JsonException ex)
            {
                // The reader's own message ends with a 0-based "LineNumber: n | BytePositionInLine: n"; report the line as an editor shows it.
                var reason = ex.Message.Split(" LineNumber:")[0].TrimEnd();
                var where = ex.LineNumber.HasValue ? $" at line {ex.LineNumber + 1}" : string.Empty;
                var message = $"ocelot.json is not valid JSON{where} ({reason}), so the Software Factory cannot merge the routes into it without losing your changes. Fix the file, or delete it to have it generated again.";
                var package = Model.FirstOrDefault()?.InternalElement.Package;
                throw package is not null
                    ? new ElementException(package, message, ex)
                    : new InvalidOperationException(message, ex);
            }
        }

        [IntentManaged(Mode.Fully)]
        public IDataFile DataFile { get; }

        [IntentManaged(Mode.Fully)]
        public override ITemplateFileConfig GetTemplateFileConfig() => DataFile.GetConfig();

        [IntentManaged(Mode.Fully)]
        public override string TransformText() => DataFile.ToString();
    }
}
