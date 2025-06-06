using System.Linq;
using Intent.Engine;
using Intent.Modules.AspNetCore.IdentityService.Templates.ApplicationIdentityUser;
using Intent.Modules.AspNetCore.IdentityService.Templates.EmailSender;
using Intent.Modules.AspNetCore.IdentityService.Templates.EmailSenderInterface;
using Intent.Modules.AspNetCore.IdentityService.Templates.EmailSenderOptions;
using Intent.Modules.AspNetCore.IdentityService.Templates.IdentityServiceManager;
using Intent.Modules.AspNetCore.IdentityService.Templates.IdentityServiceManagerInterface;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Plugins;
using Intent.Modules.Common.Templates;
using Intent.Modules.Constants;
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
            // Your custom logic here.
            var applicationSecurityConfigurationTemplate = application.FindTemplateInstance<ICSharpFileBuilderTemplate>("Intent.Application.Identity.ApplicationSecurityConfiguration");

            if (applicationSecurityConfigurationTemplate is null)
            {
                return;
            }

            var dbContextTemplate = ConfigureIdentityStore(application);

            applicationSecurityConfigurationTemplate.CSharpFile.AfterBuild(file =>
            {
                file.AddUsing("Microsoft.AspNetCore.Identity");

                var @class = file.Classes.First();

                var method = @class.FindMethod("ConfigureApplicationSecurity");

                method.FindStatement(s => s.Text == "services.AddSingleton<ICurrentUserService, CurrentUserService>();").InsertBelow(ConfigureIdentityServiceManager(file, dbContextTemplate));

                var lastConfigStatement = method.FindStatement(s => s.HasMetadata("add-authentication")) as CSharpStatement;
                lastConfigStatement.FindAndReplace("JwtBearerDefaults.AuthenticationScheme", "options =>" +
                    "{" +
                        "options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;" +
                        "options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;" +
                    "}");
            }, 100);
        }

        private CSharpStatement[] ConfigureIdentityServiceManager(ICSharpFile file, ICSharpFileBuilderTemplate dbContextTemplate)
        {
            file.Template.GetTypeName(IdentityServiceManagerInterfaceTemplate.TemplateId);
            file.Template.GetTypeName(IdentityServiceManagerTemplate.TemplateId);
            file.Template.GetTypeName(ApplicationIdentityUserTemplate.TemplateId);

            var identityStatement = new CSharpStatement("services.AddIdentity<ApplicationIdentityUser, IdentityRole>()")
                .AddInvocation("AddDefaultTokenProviders", c => c.OnNewLine())
                .AddInvocation("AddTokenProvider<DataProtectorTokenProvider<ApplicationIdentityUser>>", c => c.OnNewLine())
                .AddArgument("IdentityConstants.BearerScheme");

            if (dbContextTemplate is not null)
            {
                file.Template.UseType(dbContextTemplate.FullTypeName());
                identityStatement = identityStatement.AddInvocation($"AddEntityFrameworkStores<{dbContextTemplate.ClassName}>", c => c.OnNewLine());
            }

            file.Template.GetTypeName(EmailSenderOptionsTemplate.TemplateId);
            var statements = new CSharpStatement[]
            {
                new CSharpStatement("services.AddScoped<IIdentityServiceManager, IdentityServiceManager>();"),
                identityStatement,
                new CSharpStatement("services.AddHttpContextAccessor();"),
                new CSharpStatement($"services.AddScoped<{file.Template.GetFullyQualifiedTypeName(EmailSenderInterfaceTemplate.TemplateId)}<ApplicationIdentityUser>, {file.Template.GetTypeName(EmailSenderTemplate.TemplateId)}>();"),
                new CSharpStatement($"services.Configure<EmailSenderOptions>(configuration.GetSection(\"EmailSender\"));")
            };

            return statements;
        }

        private ICSharpFileBuilderTemplate ConfigureIdentityStore(IApplication application)
        {
            var infrastructureDependencyInjection = application.FindTemplateInstance<ICSharpFileBuilderTemplate>("Intent.Infrastructure.DependencyInjection.DependencyInjection");

            if (infrastructureDependencyInjection is not null)
            {
                infrastructureDependencyInjection.AddNugetDependency(NugetPackages.MicrosoftAspNetCoreIdentityEntityFrameworkCore(infrastructureDependencyInjection.OutputTarget));
                infrastructureDependencyInjection.AddNugetDependency(NugetPackages.MicrosoftAspNetCoreIdentity(infrastructureDependencyInjection.OutputTarget));
            }

            var dbContext = GetDbContextTemplate(application);
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

        private static ICSharpFileBuilderTemplate GetDbContextTemplate(IApplication application)
        {
            var result = application.FindTemplateInstance<ICSharpFileBuilderTemplate>(TemplateDependency.OnTemplate(TemplateRoles.Infrastructure.Data.DbContext));
            if (result != null) return result;
            result = application.FindTemplateInstance<ICSharpFileBuilderTemplate>(TemplateDependency.OnTemplate("Intent.EntityFrameworkCore.DbContext"));
            return result;
        }
    }
}