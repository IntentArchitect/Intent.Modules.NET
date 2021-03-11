using Intent.Modules.Common;
using Intent.Modules.Common.VisualStudio;
using Intent.Modules.EntityFrameworkCore.Events;
using Intent.Modules.EntityFrameworkCore.Templates.DbContext;
using System;
using System.Collections.Generic;
using System.Text;

namespace Intent.Modules.IdentityServer4.Identity.EntityFrameworkCore.Decorators
{
    public class IdentityDbContextDecorator :
        DbContextDecoratorBase, IDecoratorExecutionHooks, IDeclareUsings, IHasNugetDependencies
    {
        public const string DecoratorId = "IdentityServer4.Identity.EntityFramework.IdentityDbContextDecorator";

        private readonly Engine.IApplication _application;

        public IdentityDbContextDecorator(Engine.IApplication application)
        {
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
                NugetPackages.MicrosoftAspNetCoreIdentityEntityFrameworkCore
            };
        }

        public override string GetBaseClass()
        {
            return "IdentityDbContext<IdentityUser>";
        }
    }
}
