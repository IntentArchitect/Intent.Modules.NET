using System.Collections.Generic;
using System.IO;
using System.Linq;
using Intent.Engine;
using Intent.Modules.Application.MediatR.CRUD.CrudStrategies;
using Intent.Modules.Application.MediatR.Templates;
using Intent.Modules.Application.MediatR.Templates.QueryHandler;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.TemplateDecorator", Version = "1.0")]

namespace Intent.Modules.Application.MediatR.CRUD.Decorators
{
    [IntentManaged(Mode.Merge)]
    public class QueryHandlerCrudDecorator : QueryHandlerDecorator
    {
        [IntentManaged(Mode.Fully)]
        public const string DecoratorId = "Intent.Application.MediatR.CRUD.QueryHandlerCrudDecorator";

        [IntentManaged(Mode.Fully)]
        private readonly QueryHandlerTemplate _template;
        [IntentManaged(Mode.Fully)]
        private readonly IApplication _application;
        private readonly ICrudImplementationStrategy _implementationStrategy;

        [IntentManaged(Mode.Merge)]
        public QueryHandlerCrudDecorator(QueryHandlerTemplate template, IApplication application)
        {
            _template = template;
            _application = application;

            //if (File.Exists(_template.GetMetadata().GetFilePath()))
            //{
            //    return;
            //}

            _implementationStrategy = new ICrudImplementationStrategy[]
            {
                new GetAllImplementationStrategy(_template, _application, _application.MetadataManager),
                new GetByIdImplementationStrategy(_template, _application, _application.MetadataManager),
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