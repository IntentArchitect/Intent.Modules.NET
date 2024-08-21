using System.ComponentModel;
using Intent.Engine;
using Intent.Modules.Application.MediatR.Templates.QueryHandler;
using Intent.Modules.Common.Registrations;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.TemplateDecoratorRegistration", Version = "1.0")]

namespace Intent.Modules.Application.MediatR.CRUD.Decorators
{
    [Description(QueryHandlerCrudDecorator.DecoratorId)]
    public class QueryHandlerCrudDecoratorRegistration : DecoratorRegistration<QueryHandlerTemplate, QueryHandlerDecorator>
    {
        [IntentManaged(Mode.Fully)]
        public override QueryHandlerDecorator CreateDecoratorInstance(QueryHandlerTemplate template, IApplication application)
        {
            return new QueryHandlerCrudDecorator(template, application);
        }

        public override string DecoratorId => QueryHandlerCrudDecorator.DecoratorId;
    }
}