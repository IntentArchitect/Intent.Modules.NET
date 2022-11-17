using System.Linq;
using Intent.Engine;
using Intent.Modules.AspNetCore.Events;
using Intent.Modules.AspNetCore.Templates.Startup;
using Intent.Modules.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.TemplateDecorator", Version = "1.0")]

namespace Intent.Modules.IdentityServer4.UI.Decorators
{
    [IntentManaged(Mode.Merge)]
    public class RazorPageControllerStartupDecorator : StartupDecorator
    {
        [IntentManaged(Mode.Fully)]
        public const string DecoratorId = "Intent.IdentityServer4.UI.RazorPageControllerStartupDecorator";

        [IntentManaged(Mode.Fully)]
        private readonly StartupTemplate _template;
        [IntentManaged(Mode.Fully)]
        private readonly IApplication _application;

        [IntentManaged(Mode.Merge)]
        public RazorPageControllerStartupDecorator(StartupTemplate template, IApplication application)
        {
            _template = template;
            _application = application;
            Priority = -30;
            
            _template.CSharpFile.AfterBuild(file =>
            {
                var @class = file.Classes.First();
                var configServicesStmts = @class.Methods
                    .First(x => x.Name == "ConfigureServices")
                    .Statements;
                
                configServicesStmts.First().InsertAbove("services.AddControllersWithViews();")
                    .InsertBelow("services.AddRazorPages();");
                
                configServicesStmts
                    .FirstOrDefault(x => x.HasMetadata("configure-services-controllers-generic"))
                    ?.Remove();
            });
        }
    }
}