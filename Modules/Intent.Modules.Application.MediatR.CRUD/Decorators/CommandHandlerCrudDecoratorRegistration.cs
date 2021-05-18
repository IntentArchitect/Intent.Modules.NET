using System.ComponentModel;
using Intent.Engine;
using Intent.Modules.Application.MediatR.Templates.CommandHandler;
using Intent.Modules.Common.Registrations;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.TemplateDecoratorRegistration", Version = "1.0")]

namespace Intent.Modules.Application.MediatR.CRUD.Decorators
{
    [Description(CommandHandlerCrudDecorator.DecoratorId)]
    public class CommandHandlerCrudDecoratorRegistration : DecoratorRegistration<CommandHandlerTemplate, CommandHandlerDecorator>
    {
        private readonly IMetadataManager _metadataManager;

        public CommandHandlerCrudDecoratorRegistration(IMetadataManager metadataManager)
        {
            _metadataManager = metadataManager;
        }
        public override CommandHandlerDecorator CreateDecoratorInstance(CommandHandlerTemplate template, IApplication application)
        {
            return new CommandHandlerCrudDecorator(template, application, _metadataManager);
        }

        public override string DecoratorId => CommandHandlerCrudDecorator.DecoratorId;
    }
}