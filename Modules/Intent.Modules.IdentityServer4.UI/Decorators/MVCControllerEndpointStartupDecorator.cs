using System.Linq;
using Intent.Engine;
using Intent.Modules.AspNetCore.Templates.Startup;
using Intent.Modules.Common.CSharp.Builder;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.TemplateDecorator", Version = "1.0")]

namespace Intent.Modules.IdentityServer4.UI.Decorators
{
    [IntentManaged(Mode.Merge)]
    public class MVCControllerEndpointStartupDecorator : StartupDecorator
    {
        [IntentManaged(Mode.Fully)]
        public const string DecoratorId = "Intent.IdentityServer4.UI.MVCControllerEndpointStartupDecorator";

        [IntentManaged(Mode.Fully)]
        private readonly StartupTemplate _template;
        [IntentManaged(Mode.Fully)]
        private readonly IApplication _application;

        [IntentManaged(Mode.Merge)]
        public MVCControllerEndpointStartupDecorator(StartupTemplate template, IApplication application)
        {
            _template = template;
            _application = application;
            Priority = -30;

            _template.CSharpFile.AfterBuild(file =>
            {
                var @class = file.Classes.First();
                var endpointStmts = @class.Methods
                    .First(x => x.Name == "Configure")
                    .Statements
                    .OfType<EndpointsStatement>()
                    .ToList();

                endpointStmts.First().AddEndpointConfiguration(
                    new CSharpInvocationStatement("endpoints.MapControllerRoute")
                        .AddArgument(@"name: ""default""")
                        .AddArgument(@"pattern: ""{controller=Home}/{action=Index}/{id?}""")
                        .WithArgumentsOnNewLines());

                endpointStmts
                    .SelectMany(s => s.Statements)
                    .FirstOrDefault(p => p.HasMetadata("configure-endpoints-controllers-generic"))
                    ?.Remove();
            });
        }
    }
}