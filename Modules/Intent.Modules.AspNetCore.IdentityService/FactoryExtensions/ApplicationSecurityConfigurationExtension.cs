using System;
using System.Linq;
using System.Threading;
using Intent.Engine;
using Intent.Exceptions;
using Intent.Modules.AspNetCore.IdentityService.Settings;
using Intent.Modules.AspNetCore.IdentityService.Templates.ApplicationIdentityUser;
using Intent.Modules.AspNetCore.IdentityService.Templates.EmailSenderOptions;
using Intent.Modules.AspNetCore.IdentityService.Templates.IdentityServiceManager;
using Intent.Modules.AspNetCore.IdentityService.Templates.IdentityServiceManagerInterface;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Plugins;
using Intent.Modules.Common.Templates;
using Intent.Modules.Constants;
using Intent.Modules.Modelers.Services.Settings;
using Intent.Plugins.FactoryExtensions;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.FactoryExtension", Version = "1.0")]

namespace Intent.Modules.AspNetCore.IdentityService.FactoryExtensions
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class ApplicationSecurityConfigurationExtension : FactoryExtensionBase
    {
        public override string Id => "Intent.AspNetCore.IdentityService.ApplicationSecurityConfigurationExtension";

        [IntentManaged(Mode.Ignore)]
        public override int Order => 0;

        protected override void OnAfterTemplateRegistrations(IApplication application)
        {
            var applicationSecurityConfigurationTemplate = application.FindTemplateInstance<ICSharpFileBuilderTemplate>("Intent.Application.Identity.ApplicationSecurityConfiguration");

            if (applicationSecurityConfigurationTemplate is null)
            {
                return;
            }

            var dbContextTemplate = GetDbContextTemplate(application, applicationSecurityConfigurationTemplate.ExecutionContext);

            ConfigureIdentityStore(application, dbContextTemplate);
        }

        private ICSharpFileBuilderTemplate ConfigureIdentityStore(IApplication application, ICSharpFileBuilderTemplate dbContext)
        {
            var infrastructureDependencyInjection = application.FindTemplateInstance<ICSharpFileBuilderTemplate>("Intent.Infrastructure.DependencyInjection.DependencyInjection");

            if (infrastructureDependencyInjection is not null)
            {
                infrastructureDependencyInjection.AddNugetDependency(NugetPackages.MicrosoftAspNetCoreIdentityEntityFrameworkCore(infrastructureDependencyInjection.OutputTarget));
                infrastructureDependencyInjection.AddNugetDependency(NugetPackages.MicrosoftAspNetCoreIdentity(infrastructureDependencyInjection.OutputTarget));
                infrastructureDependencyInjection.AddNugetDependency(NugetPackages.SystemIdentityModelTokensJwt(infrastructureDependencyInjection.OutputTarget));
            }

            if (dbContext == null)
            {
                return null;
            }

            dbContext.CSharpFile.AfterBuild(file =>
            {
                file.AddUsing("Microsoft.AspNetCore.Identity");
                file.AddUsing("Microsoft.AspNetCore.Identity.EntityFrameworkCore");

                var priClass = file.Classes.First();
                file.Template.GetTypeName(ApplicationIdentityUserTemplate.TemplateId);
                priClass.WithBaseType("IdentityDbContext<ApplicationIdentityUser>");
            }, 1000);

            return dbContext;
        }

        private static ICSharpFileBuilderTemplate GetDbContextTemplate(IApplication application, ISoftwareFactoryExecutionContext executionContext)
        {
            var result = application.FindTemplateInstance<ICSharpFileBuilderTemplate>(TemplateDependency.OnTemplate(TemplateRoles.Infrastructure.Data.DbContext));
            if (result != null) return result;
            result = application.FindTemplateInstance<ICSharpFileBuilderTemplate>(TemplateDependency.OnTemplate("Intent.EntityFrameworkCore.DbContext"));

            if (result != null)
            {
                return result;
            }

            throw new Exception("Unable to find DB Context template. The 'Intent.AspNetCore.IdentityService' modules require the 'Intent.EntityFrameworkCore' module to be installed, along with a properly configured Domain package.");
        }
    }
}