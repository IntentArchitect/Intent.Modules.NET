using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Modelers.Services.Api;
using Intent.Modules.Application.ServiceImplementations.Conventions.CRUD.CrudMappingStrategies;
using Intent.Modules.Application.ServiceImplementations.Conventions.CRUD.MethodImplementationStrategies;
using Intent.Modules.Application.ServiceImplementations.Templates.ServiceImplementation;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Plugins;
using Intent.Modules.Common.Templates;
using Intent.Plugins.FactoryExtensions;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;
using Intent.Utils;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.FactoryExtension", Version = "1.0")]

namespace Intent.Modules.Application.ServiceImplementations.Conventions.CRUD.FactoryExtensions
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class CrudServiceFactoryExtension : FactoryExtensionBase
    {
        public override string Id => "Intent.Application.ServiceImplementations.Conventions.CRUD.CrudServiceFactoryExtension";

        [IntentManaged(Mode.Ignore)]
        public override int Order => 0;

        protected override void OnAfterTemplateRegistrations(IApplication application)
        {
            var templates = application.FindTemplateInstances<ICSharpFileBuilderTemplate>(TemplateDependency.OnTemplate("Application.Implementation.Custom")).ToList();
            if (!templates.Any())
            {
                templates = application.FindTemplateInstances<ICSharpFileBuilderTemplate>(TemplateDependency.OnTemplate(ServiceImplementationTemplate.TemplateId)).ToList();
            }

            foreach (var template in templates)
            {
                template.AddKnownType("System.Linq.Dynamic.Core.PagedResult");

                var strategies = new List<IImplementationStrategy>
                {
                    new OperationMappingImplementationStrategy(template),
                    new GetAllImplementationStrategy(template, application),
                    new GetAllPaginationImplementationStrategy(template, application),
                    new GetByIdImplementationStrategy(template, application),
                    new CreateImplementationStrategy(template, application),
                    new UpdateImplementationStrategy(template, application),
                    new DeleteWithReturnDtoImplementationStrategy(template, application),
                    new LegacyDeleteImplementationStrategy(template, application)
                };

                template.CSharpFile.OnBuild(file =>
                {
                    var serviceModel = file.Classes.First().GetMetadata<ServiceModel>("model");

                    foreach (var operation in serviceModel.Operations)
                    {
                        var matchedStrategies = strategies.Where(strategy => strategy.IsMatch(operation)).ToArray();
                        if (matchedStrategies.Length == 1)
                        {
                            matchedStrategies[0].BindToTemplate(template, operation);
                        }
                        else if (matchedStrategies.Length > 1)
                        {
                            Logging.Log.Warning($@"Multiple CRUD implementation strategies were found that can implement this service operation [{serviceModel.Name}, {operation.Name}]");
                            Logging.Log.Debug($@"Strategies: {string.Join(", ", matchedStrategies.Select(s => s.GetType().Name))}");
                        }
                    }
                });
            }
        }
    }
}