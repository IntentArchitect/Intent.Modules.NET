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

        [IntentManaged(Mode.Fully)]
        private readonly CommandHandlerTemplate _template;
        [IntentManaged(Mode.Fully)]
        private readonly IApplication _application;
        private readonly ICrudImplementationStrategy _implementationStrategy;

        [IntentManaged(Mode.Merge)]
        public CommandHandlerCrudDecorator(CommandHandlerTemplate template, IApplication application)
        {
            _template = template;
            _application = application;

            //if (File.Exists(_template.GetMetadata().GetFilePath()))
            //{
            //    return;
            //}

            // DJVV: Maybe we should just get rid of the decorator and use a factory extension
            _implementationStrategy = new ICrudImplementationStrategy[]
            {
                new CreateImplementationStrategy(_template, _application, _application.MetadataManager),
                new UpdateImplementationStrategy(_template, _application, _application.MetadataManager),
                new DeleteImplementationStrategy(_template, _application, _application.MetadataManager),
            }.SingleOrDefault(x => x.IsMatch());

            _implementationStrategy?.OnStrategySelection();
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