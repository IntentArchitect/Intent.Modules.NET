using Intent.Engine;
using Intent.Modules.AspNetCore.Templates.Startup;
using Intent.Modules.HotChocolate.GraphQL.AspNetCore.Templates.ConfigureGraphQL;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.TemplateDecorator", Version = "1.0")]

namespace Intent.Modules.HotChocolate.GraphQL.AspNetCore.Decorators
{
    [IntentManaged(Mode.Merge)]
    public class GraphQLStartupDecorator : StartupDecorator
    {
        [IntentManaged(Mode.Fully)]
        public const string DecoratorId = "Intent.HotChocolate.GraphQL.AspNetCore.GraphQLStartupDecorator";

        [IntentManaged(Mode.Fully)]
        private readonly StartupTemplate _template;
        [IntentManaged(Mode.Fully)]
        private readonly IApplication _application;

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public GraphQLStartupDecorator(StartupTemplate template, IApplication application)
        {
            _template = template;
            _application = application;
            _template.AddTemplateDependency(ConfigureGraphQLTemplate.TemplateId);
        }

        public override int Priority => 10;

        public override string ConfigureServices()
        {
            return @"
            services.ConfigureGraphQL(Configuration);";
        }

        public override string EndPointMappings()
        {
            return @"endpoints.MapGraphQL();";
        }
    }
}