using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
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
            DataFile = new DataFile($"ocelot")
                .WithJsonWriter()
                .WithRootObject(this, x =>
                {
                    x.WithObject("GlobalConfiguration", config =>
                    {
                        config.WithValue("BaseUrl", "http://localhost:5000");
                        config.WithObject("Hosts", hosts =>
                        {
                            foreach (var packageName in Model
                                         .Select(s => s.DownstreamEndpoints().FirstOrDefault())
                                         .Where(p => p is not null)
                                         .OfType<DownstreamEndModel>()
                                         .Select(s => s.Element as IElement)
                                         .OfType<IElement>()
                                         .Select(s => s.Package.Name)
                                         .Distinct())
                            {
                                hosts.WithValue(packageName, "");
                            }
                        });
                    });
                    x.WithArray("Routes", routes =>
                    {
                        foreach (var routeModel in Model)
                        {
                            var entry = new DataFileObjectValue();
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

                            var downstreamRoute = downstreamOperation.GetStereotype("Http Settings")?.GetProperty("Route")?.Value;
                            var downstreamServiceRoute = downstreamOperation.ParentElement?.GetStereotype("Http Service Settings")?.GetProperty("Route")?.Value;
                            var separator = string.IsNullOrWhiteSpace(downstreamServiceRoute) || downstreamServiceRoute.EndsWith("/")
                                ? string.Empty
                                : !string.IsNullOrWhiteSpace(downstreamRoute)
                                    ? "/"
                                    : string.Empty;

                            entry.WithValue("DownstreamPathTemplate", $"/{downstreamServiceRoute}{separator}{downstreamRoute}");
                            entry.WithValue("DownstreamHttpMethod", downstreamOperation.GetStereotype("Http Settings")?.GetProperty("Verb")?.Value);
                            entry.WithArray("DownstreamHostAndPorts", arr => arr.Add(new DataFileObjectValue().WithValue("Host", $"{{{downstreamOperation.Package.Name}}}")));

                            routes.Add(entry);
                        }
                    });
                });
        }

        [IntentManaged(Mode.Fully)]
        public IDataFile DataFile { get; }

        [IntentManaged(Mode.Fully)]
        public override ITemplateFileConfig GetTemplateFileConfig() => DataFile.GetConfig();

        [IntentManaged(Mode.Fully)]
        public override string TransformText() => DataFile.ToString();
    }
}