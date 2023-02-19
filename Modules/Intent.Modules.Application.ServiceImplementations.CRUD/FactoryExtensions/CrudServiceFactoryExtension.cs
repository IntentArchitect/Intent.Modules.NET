using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Modules.Application.ServiceImplementations.Conventions.CRUD.MethodImplementationStrategies;
using Intent.Modules.Application.ServiceImplementations.Templates.ServiceImplementation;
using Intent.Modules.Common;
using Intent.Modules.Common.Plugins;
using Intent.Modules.Common.Templates;
using Intent.Plugins.FactoryExtensions;
using Intent.RoslynWeaver.Attributes;
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
            var templates = application.FindTemplateInstances<ServiceImplementationTemplate>(TemplateDependency.OnTemplate(ServiceImplementationTemplate.TemplateId));
            foreach (var template in templates)
            {
                var strategies = new List<IImplementationStrategy>
                {
                    new GetAllImplementationStrategy(template, application),
                    new GetAllPaginationImplementationStrategy(template, application),
                    new GetByIdImplementationStrategy(template, application),
                    new CreateImplementationStrategy(template, application),
                    new UpdateImplementationStrategy(template, application),
                    new DeleteWithReturnDtoImplementationStrategy(template, application),
                    new LegacyDeleteImplementationStrategy(template, application)
                };

                foreach (var operation in template.Model.Operations)
                {
                    var matchedStrategies = strategies.Where(strategy => strategy.IsMatch(operation)).ToArray();
                    if (matchedStrategies.Length == 1)
                    {
                        template.CSharpFile.AfterBuild(file => matchedStrategies[0].ApplyStrategy(operation));
                    }
                    else if (matchedStrategies.Length > 1)
                    {
                        Logging.Log.Warning($@"Multiple CRUD implementation strategies were found that can implement this service operation [{template.Model.Name}, {operation.Name}]");
                        Logging.Log.Debug($@"Strategies: {string.Join(", ", matchedStrategies.Select(s => s.GetType().Name))}");
                    }
                }
            }
        }
    }
}