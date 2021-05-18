using System.Collections.Generic;
using System.IO;
using System.Linq;
using Intent.Engine;
using Intent.Modelers.Domain.Api;
using Intent.Modules.Application.MediatR.CRUD.CrudStrategies;
using Intent.Modules.Application.MediatR.Templates;
using Intent.Modules.Application.MediatR.Templates.CommandHandler;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.TemplateDecorator", Version = "1.0")]

namespace Intent.Modules.Application.MediatR.CRUD.Decorators
{
    [IntentManaged(Mode.Merge)]
    public class CommandHandlerCrudDecorator : CommandHandlerDecorator
    {
        [IntentManaged(Mode.Fully)]
        public const string DecoratorId = "Intent.Application.MediatR.CRUD.CommandHandlerCrudDecorator";

        private readonly CommandHandlerTemplate _template;
        private readonly IApplication _application;
        private readonly IMetadataManager _metadataManager;
        private readonly ICrudImplementationStrategy _implementationStrategy;

        [IntentManaged(Mode.Merge)]
        public CommandHandlerCrudDecorator(CommandHandlerTemplate template, IApplication application, IMetadataManager metadataManager)
        {
            _template = template;
            _application = application;
            _metadataManager = metadataManager;

            if (File.Exists(_template.GetMetadata().GetFilePath()))
            {
                return;
            }

            _implementationStrategy = new ICrudImplementationStrategy[]
            {
                new CreateImplementationStrategy(_template, _application, _metadataManager),
                new UpdateImplementationStrategy(_template, _application, _metadataManager),
                new DeleteImplementationStrategy(_template, _application, _metadataManager),
            }.SingleOrDefault(x => x.IsMatch());
        }

        public override IEnumerable<RequiredService> GetRequiredServices()
        {
            return _implementationStrategy?.GetRequiredServices() ?? base.GetRequiredServices();
        }

        public override string GetImplementation()
        {
            return _implementationStrategy?.GetImplementation() ?? base.GetImplementation();
        }
    }
}