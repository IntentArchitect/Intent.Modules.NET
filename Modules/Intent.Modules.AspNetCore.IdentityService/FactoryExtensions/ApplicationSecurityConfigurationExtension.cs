using Intent.Engine;
using Intent.Exceptions;
using Intent.Modules.AspNetCore.IdentityService.Settings;
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
using Intent.Modules.Modelers.Services.Settings;
using Intent.Plugins.FactoryExtensions;
using Intent.RoslynWeaver.Attributes;
using System;
using System.Linq;
using System.Threading;

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

            var dbContextTemplate = GetDbContextTemplate(application, applicationSecurityConfigurationTemplate.ExecutionContext);

            ConfigureIdentityStore(application, dbContextTemplate);

            applicationSecurityConfigurationTemplate.CSharpFile.AfterBuild(file =>
            {
                file.AddUsing("Microsoft.AspNetCore.Identity");

                var @class = file.Classes.First();

                var method = @class.FindMethod("ConfigureApplicationSecurity");

                method.FindStatement(s => s.Text == "services.AddSingleton<ICurrentUserService, CurrentUserService>();").InsertBelow(ConfigureIdentityServiceManager(file, dbContextTemplate));

                var lastConfigStatement = method.FindStatement(s => s.HasMetadata("add-authentication")) as CSharpStatement;
                var optionsLambdaBlock = (((method.FindStatement(s => s.HasMetadata("add-authentication")) as CSharpMethodChainStatement).Statements.First() as CSharpInvocationStatement).Statements.Last() as CSharpLambdaBlock);
                optionsLambdaBlock.Statements.Clear();

                optionsLambdaBlock.Statements.Add(new CSharpStatement("options.Audience = configuration[\"JwtToken:Audience\"];"));

                var tokenValidationParameters = new CSharpObjectInitializerBlock("options.TokenValidationParameters = new TokenValidationParameters");
                tokenValidationParameters.AddInitStatement("ValidateIssuer", "true");
                tokenValidationParameters.AddInitStatement("ValidIssuer", "configuration[\"JwtToken:Issuer\"]");
                tokenValidationParameters.AddInitStatement("ValidateAudience", "true");
                tokenValidationParameters.AddInitStatement("ValidAudience", "configuration[\"JwtToken:Audience\"]");
                tokenValidationParameters.AddInitStatement("ValidateIssuerSigningKey", "true");
                tokenValidationParameters.AddInitStatement("IssuerSigningKey", "new SymmetricSecurityKey(Convert.FromBase64String(configuration[\"JwtToken:SigningKey\"]))");
                tokenValidationParameters.AddInitStatement("RoleClaimType", "\"role\"").WithSemicolon();

                optionsLambdaBlock.Statements.Add(tokenValidationParameters);
                optionsLambdaBlock.Statements.Add(new CSharpStatement("options.SaveToken = true;"));
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

            var identityStatement = new CSharpStatement("services.AddIdentity<ApplicationIdentityUser, IdentityRole>()");
            if (dbContextTemplate.ExecutionContext.Settings.GetIdentityServiceSettings().RequiresConfirmedAccount())
            {
                identityStatement = new CSharpStatement("services.AddIdentity<ApplicationIdentityUser, IdentityRole>(options => options.SignIn.RequireConfirmedAccount = true)");
            }

            identityStatement = identityStatement
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

        private ICSharpFileBuilderTemplate ConfigureIdentityStore(IApplication application, ICSharpFileBuilderTemplate dbContext)
        {
            var infrastructureDependencyInjection = application.FindTemplateInstance<ICSharpFileBuilderTemplate>("Intent.Infrastructure.DependencyInjection.DependencyInjection");

            if (infrastructureDependencyInjection is not null)
            {
                infrastructureDependencyInjection.AddNugetDependency(NugetPackages.MicrosoftAspNetCoreIdentityEntityFrameworkCore(infrastructureDependencyInjection.OutputTarget));
                infrastructureDependencyInjection.AddNugetDependency(NugetPackages.MicrosoftAspNetCoreIdentity(infrastructureDependencyInjection.OutputTarget));
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