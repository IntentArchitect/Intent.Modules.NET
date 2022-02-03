using System;
using System.Collections.Generic;
using System.Text;
using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.VisualStudio;
using Intent.Modules.Common.VisualStudio;
using Intent.Modules.EntityFrameworkCore.Events;
using Intent.Modules.EntityFrameworkCore.Templates.DbContext;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.TemplateDecorator", Version = "1.0")]
[assembly: DefaultIntentManaged(Mode.Merge)]

namespace Intent.Modules.IdentityServer4.Identity.EFCore.Decorators
{
    [IntentManaged(Mode.Merge)]
    public class IdentityDbContextDecorator :
        DbContextDecoratorBase, IDecoratorExecutionHooks, IDeclareUsings, IHasNugetDependencies
    {
        [IntentManaged(Mode.Fully)]
        public const string DecoratorId = "Intent.IdentityServer4.Identity.EFCore.IdentityDbContextDecorator";


        [IntentManaged(Mode.Fully)]
        private readonly DbContextTemplate _template;
        private readonly Engine.IApplication _application;

        [IntentManaged(Mode.Merge)]
        public IdentityDbContextDecorator(DbContextTemplate template, Engine.IApplication application)
        {
            _template = template;
            _application = application;
        }

        public void BeforeTemplateExecution()
        {
            _application.EventDispatcher.Publish(new OverrideDbContextOptionsEvent
            {
                UseDbContextAsOptionsParameter = true
            });
        }

        public IEnumerable<string> DeclareUsings()
        {
            return new string[]
            {
                "Microsoft.AspNetCore.Identity.EntityFrameworkCore",
                "Microsoft.AspNetCore.Identity"
            };
        }

        public IEnumerable<INugetPackageInfo> GetNugetDependencies()
        {
            return new INugetPackageInfo[]
            {
                NugetPackages.IdentityServer4EntityFramework,
                NugetPackages.MicrosoftAspNetCoreIdentityEntityFrameworkCore(_template.OutputTarget.GetProject())
            };
        }

        public override string GetBaseClass()
        {
            return "IdentityDbContext<IdentityUser>";
        }
    }
}
