using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.IArchitect.Agent.Persistence;
using Intent.Metadata.Models;
using Intent.Modelers.Services.Api;
using Intent.Modelers.Services.CQRS.Api;
using Intent.Modules.AspNetCore.OutputCaching.Redis.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Plugins;
using Intent.Modules.Constants;
using Intent.Plugins.FactoryExtensions;
using Intent.RoslynWeaver.Attributes;
using static Intent.Modules.AspNetCore.OutputCaching.Redis.Api.IControllerOperationModelExtensions;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.FactoryExtension", Version = "1.0")]

namespace Intent.Modules.AspNetCore.OutputCaching.Redis.FactoryExtensions
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class ControllerExtension : FactoryExtensionBase
    {
        public override string Id => "Intent.AspNetCore.OutputCaching.Redis.ControllerExtension";

        [IntentManaged(Mode.Ignore)]
        public override int Order => 0;

        protected override void OnAfterTemplateRegistrations(IApplication application)
        {
            var serviceOperations = application.MetadataManager.Services(application).GetServiceModels()
                .SelectMany(x => x.Operations.Where(q => q.HasStereotype("Http Settings") && q.GetStereotypeProperty<string>("Http Settings", "Verb") == "GET")).Select(o => o.InternalElement)
                .ToArray();

            var alloperations = serviceOperations
                .Concat(application.MetadataManager.Services(application).GetQueryModels()
                    .Where(x => x.HasStereotype("Http Settings"))
                    .Select(x => x.InternalElement));

            var controllers = new Dictionary<string, List<(IElement Element, CachingAggregate CachingConfig)>>();
            foreach (var operation in alloperations)
            {
                if (operation.TryGetCaching(out var cachingSettings))
                {
                    if (!controllers.TryGetValue(operation.ParentElement.Id, out var operations))
                    {
                        operations = new List<(IElement, CachingAggregate)>();
                        controllers.Add(operation.ParentElement.Id, operations);
                    }
                    operations.Add(new(operation, cachingSettings));
                }
            }
            foreach (var controller in controllers)
            {
                var template = application.FindTemplateInstance<ICSharpFileBuilderTemplate>(TemplateRoles.Distribution.WebApi.Controller, controller.Key);
                var endpoints = controller.Value;
                template.CSharpFile.OnBuild(file =>
                {
                    var @class = file.Classes.FirstOrDefault();
                    foreach (var endpoint in endpoints)
                    {
                        file.AddUsing("Microsoft.AspNetCore.OutputCaching");
                        var method = @class.FindMethod(m => m.HasMetadata("modelId") && m.GetMetadata<string>("modelId") == endpoint.Element.Id);
                        method.AddAttribute("OutputCache", att =>
                        {
                            if (endpoint.CachingConfig.Policy != null)
                            {
                                att.AddArgument($"PolicyName = \"{endpoint.CachingConfig.Policy}\"");
                            }
                            if (endpoint.CachingConfig.Duration != null)
                            {
                                att.AddArgument($"Duration = {endpoint.CachingConfig.Duration}");
                            }
                            if (endpoint.CachingConfig.Tags != null)
                            {
                                var value = string.Join(", ", endpoint.CachingConfig.Tags.Split(",").Select(x => $"\"{x.Trim()}\""));
                                att.AddArgument($"Tags = [{value}]");
                            }
                            if (endpoint.CachingConfig.VaryByRouteValueNames != null)
                            {
                                var value = string.Join(", ", endpoint.CachingConfig.VaryByRouteValueNames.Split(",").Select(x => $"\"{x.Trim()}\""));
                                att.AddArgument($"VaryByRouteValueNames = [{value}]");
                            }
                            if (endpoint.CachingConfig.VaryByQueryKeys != null)
                            {
                                var value = string.Join(", ", endpoint.CachingConfig.VaryByQueryKeys.Split(",").Select(x => $"\"{x.Trim()}\""));
                                att.AddArgument($"VaryByQueryKeys = [{value}]");
                            }
                            if (endpoint.CachingConfig.VaryByHeaderNames != null)
                            {
                                var value = string.Join(", ", endpoint.CachingConfig.VaryByHeaderNames.Split(",").Select(x => $"\"{x.Trim()}\""));
                                att.AddArgument($"VaryByHeaderNames = [{value}]");
                            }
                        });
                    }
                });
            }
        }
    }
}