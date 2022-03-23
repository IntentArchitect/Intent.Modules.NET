using System.ComponentModel;
using Intent.Engine;
using Intent.Modules.AspNetCore.Templates.Startup;
using Intent.Modules.Common.Registrations;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.TemplateDecoratorRegistration", Version = "1.0")]

namespace Intent.Modules.HotChocolate.GraphQL.AspNetCore.Decorators
{
    [Description(GraphQLStartupDecorator.DecoratorId)]
    public class GraphQLStartupDecoratorRegistration : DecoratorRegistration<StartupTemplate, StartupDecorator>
    {
        public override StartupDecorator CreateDecoratorInstance(StartupTemplate template, IApplication application)
        {
            return new GraphQLStartupDecorator(template, application);
        }

        public override string DecoratorId => GraphQLStartupDecorator.DecoratorId;
    }
}