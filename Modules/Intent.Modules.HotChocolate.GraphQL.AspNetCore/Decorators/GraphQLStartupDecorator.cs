using Intent.Engine;
using Intent.Modules.AspNetCore.Templates.Startup;
using Intent.Modules.Common.CSharp.Builder;
using Intent.RoslynWeaver.Attributes;
using System.Linq;

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
            _template.CSharpFile.AfterBuild(file =>
            {
                var @class = file.Classes.First();
                @class.Methods.First(x => x.Name == "ConfigureServices")
                    .AddStatement(new CSharpInvocationStatement("services.ConfigureGraphQL"), s => s.AddMetadata("configure-services-graphql", true));
                @class.Methods.First(x => x.Name == "Configure")
                    .Statements.OfType<EndpointsStatement>().First()
                    .AddEndpointConfiguration(new CSharpStatement("endpoints.MapGraphQL();").AddMetadata("configure-endpoints-graphql", true));
            });
        }

        public override int Priority => 10;
    }
}