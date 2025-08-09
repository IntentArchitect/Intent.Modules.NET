using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Intent.Engine;
using Intent.Modelers.Domain.Api;
using Intent.Modules.Blazor.Authentication.Settings;
using Intent.Modules.Blazor.Authentication.Templates.Templates.Client.ServerAuthorizationMessageHandler;
using Intent.Modules.Blazor.Authentication.Templates.Templates.Server.ApplicationDbContext;
using Intent.Modules.Blazor.Authentication.Templates.Templates.Server.ApplicationUser;
using Intent.Modules.Blazor.Authentication.Templates.Templates.Server.AspNetCoreIdentityAuthServiceConcrete;
using Intent.Modules.Blazor.Authentication.Templates.Templates.Server.AuthServiceInterface;
using Intent.Modules.Blazor.Authentication.Templates.Templates.Server.IdentityNoOpEmailSender;
using Intent.Modules.Blazor.Authentication.Templates.Templates.Server.IdentityRevalidatingAuthenticationStateProvider;
using Intent.Modules.Blazor.Authentication.Templates.Templates.Server.JwtAuthServiceConcrete;
using Intent.Modules.Blazor.Authentication.Templates.Templates.Server.OidcAuthenticationOptions;
using Intent.Modules.Blazor.Authentication.Templates.Templates.Server.OidcAuthServiceConcrete;
using Intent.Modules.Blazor.Authentication.Templates.Templates.Server.PersistingRevalidatingAuthenticationStateProvider;
using Intent.Modules.Blazor.Authentication.Templates.Templates.Server.PersistingServerAuthenticationStateProvider;
using Intent.Modules.Blazor.Settings;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.AppStartup;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Plugins;
using Intent.Plugins.FactoryExtensions;
using Intent.RoslynWeaver.Attributes;
using Intent.Utils;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.FactoryExtension", Version = "1.0")]

namespace Intent.Modules.Blazor.Authentication.FactoryExtensions
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class ServerAddAuthentication : FactoryExtensionBase
    {
        public override string Id => "Intent.Blazor.Authentication.ServerAddAuthentication";

        [IntentManaged(Mode.Ignore)]
        public override int Order => 0;

        protected override void OnAfterTemplateRegistrations(IApplication application)
        {
            var startup = application.FindTemplateInstance<IAppStartupTemplate>(IAppStartupTemplate.RoleName);

            if (startup == null)
            {
                Logging.Log.Warning("Unable to install Authentication. Startup class could not be found.");
                return;
            }

            startup.CSharpFile.AfterBuild(file =>
            {
                startup.StartupFile.ConfigureServices((statements, context) =>
                {
                    startup.AddUsing("Microsoft.AspNetCore.Components.Authorization");
                    startup.AddUsing("Microsoft.AspNetCore.Identity");
                    startup.AddUsing("Microsoft.Extensions.Configuration");
                    startup.AddUsing("System");

                    statements.AddStatement($"{context.Services}.AddCascadingAuthenticationState();");
                    statements.AddStatement($"{context.Services}.AddHttpContextAccessor();");

                    if (startup.ExecutionContext.GetSettings().GetBlazor().Authentication().IsJwt())
                    {
                        statements.AddStatement($"{context.Services}.AddHttpClient(\"jwtClient\", client => client.BaseAddress = {context.Configuration}.GetValue<Uri?>(\"TokenEndpoint:Uri\"));");
                    }
                    else if (startup.ExecutionContext.GetSettings().GetBlazor().Authentication().IsOidc())
                    {
                        statements.AddStatement($"{context.Services}.AddHttpClient(\"oidcClient\", client => client.BaseAddress = {context.Configuration}.GetValue<Uri?>(\"TokenEndpoint:Uri\"));");
                        statements.AddStatement($"{context.Services}.Configure<{startup.GetTypeName(OidcAuthenticationOptionsTemplate.TemplateId)}>({context.Configuration}.GetSection(\"Authentication:OIDC\"));");
                    }
                    statements.AddStatement($"{context.Services}.AddScoped<IdentityRedirectManager>();");

                    if (startup.ExecutionContext.GetSettings().GetBlazor().Authentication().IsAspnetcoreIdentity())
                    {
                        file.AddUsing("Microsoft.EntityFrameworkCore");
                        AddPersistanceProvider(startup, statements, context);
                        statements.AddStatement($"{context.Services}.AddScoped<{startup.GetTypeName(AuthServiceInterfaceTemplate.TemplateId)}, {startup.GetTypeName(AspNetCoreIdentityAuthServiceConcreteTemplate.TemplateId)}>();");
                        statements.AddStatement($"{context.Services}.AddAuthorization();");
                        statements.AddStatement($"{context.Services}.AddScoped<IdentityUserAccessor>();");
                        statements.AddStatement($"{context.Services}.AddScoped<IdentityRedirectManager>();");
                        statements.AddStatements(@$"{context.Services}.AddAuthentication(options =>
                        {{
                            options.DefaultScheme = IdentityConstants.ApplicationScheme;
                            options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
                        }}).AddIdentityCookies();".ConvertToStatements());

                        if (!startup.ExecutionContext.InstalledModules.Any(im => im.ModuleId == "Intent.AspNetCore.Identity"))
                        {
                            statements.AddStatements(@$"var connectionString = {context.Configuration}.GetConnectionString(""DefaultConnection"") ?? throw new InvalidOperationException(""Connection string 'DefaultConnection' not found."");
                        {context.Services}.AddDbContext<{startup.GetTypeName(ApplicationDbContextTemplate.TemplateId)}>(options =>
                            options.UseSqlServer(connectionString));".ConvertToStatements());

                            statements.AddStatements(@$"{context.Services}.AddIdentityCore<{startup.GetTypeName(ApplicationUserTemplate.TemplateId)}>(options => options.SignIn.RequireConfirmedAccount = true)
                        .AddEntityFrameworkStores<{startup.GetTypeName(ApplicationDbContextTemplate.TemplateId)}>()
                        .AddSignInManager()
                        .AddDefaultTokenProviders();".ConvertToStatements());

                            statements.AddStatement($"{context.Services}.AddSingleton<IEmailSender<{startup.GetTypeName(ApplicationUserTemplate.TemplateId)}>, {startup.GetTypeName(IdentityNoOpEmailSenderTemplate.TemplateId)}>();");
                        }
                        else
                        {
                            var identityUserName = IdentityHelperExtensions.GetIdentityUserClass(startup);
                            var aspNetCoreIdentityConfiguration = startup.GetTemplate<ICSharpFileBuilderTemplate>("Intent.AspNetCore.Identity.AspNetCoreIdentityConfiguration");

                            aspNetCoreIdentityConfiguration.CSharpFile.AfterBuild(file =>
                            {
                                var @class = file.Classes.First();

                                var configureMethod = @class.Methods.First(x => x.Name == "ConfigureIdentity");

                                var addIdentity = configureMethod.FindAndReplaceStatement(
                                    m => m.Text.Contains($"services.AddIdentityWithoutCookieAuth<{identityUserName}, {IdentityHelperExtensions.GetIdentityRoleClass(startup)}>()"),
                                    new CSharpMethodChainStatement($"services.AddIdentityCore<{identityUserName}>()")
                                        .AddChainStatement("AddSignInManager()")
                                        .AddChainStatement($"AddRoles<{IdentityHelperExtensions.GetIdentityRoleClass(startup)}>()")
                                        .AddChainStatement($"AddEntityFrameworkStores<{startup.GetTypeName("Intent.EntityFrameworkCore.DbContext")}>()")
                                        .AddChainStatement("AddDefaultTokenProviders()"));
                            });

                            statements.AddStatement($"{context.Services}.AddSingleton<IEmailSender<{identityUserName}>, {startup.GetTypeName(IdentityNoOpEmailSenderTemplate.TemplateId)}>();");
                        }
                    }
                    else
                    {
                        file.AddUsing("Microsoft.AspNetCore.Authentication.Cookies");
                        AddPersistanceProvider(startup, statements, context);
                        if (startup.ExecutionContext.GetSettings().GetBlazor().RenderMode().IsInteractiveServer())
                        {
                            statements.AddStatement($"{context.Services}.AddScoped<{startup.GetTypeName(ServerAuthorizationMessageHandlerTemplate.TemplateId)}>();");
                        }
                        if (startup.ExecutionContext.GetSettings().GetBlazor().Authentication().IsJwt())
                        {
                            statements.AddStatement($"{context.Services}.AddScoped<{startup.GetTypeName(AuthServiceInterfaceTemplate.TemplateId)}, {startup.GetTypeName(JwtAuthServiceConcreteTemplate.TemplateId)}>();");
                        }
                        else
                        {
                            statements.AddStatement($"{context.Services}.AddScoped<{startup.GetTypeName(AuthServiceInterfaceTemplate.TemplateId)}, {startup.GetTypeName(OidcAuthServiceConcreteTemplate.TemplateId)}>();");
                        }
                        statements.AddStatement($"{context.Services}.AddAuthorization();");
                        statements.AddStatements(@$"{context.Services}.AddAuthentication(options =>
                        {{
                            options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                        }}).AddCookie();".ConvertToStatements());
                    }
                });

                startup.StartupFile.ConfigureApp((statements, context) =>
                {
                    //Not 100% sure whats happing here I think we need to remove app.UseRouting() on Blazor Server. because it doesn't us EndPoints.
                    if (statements.Statements.FirstOrDefault(s => s.Text == "app.UseEndpoints") == null)
                    {
                        var routing = statements.Statements.FirstOrDefault(s => s.Text == "app.UseRouting();");
                        if (routing is not null)
                        {
                            statements.Statements.Remove(routing);
                        }
                    }

                    if (statements.Statements.FirstOrDefault(s => s.Text == "app.UseAuthentication();") == null)
                    {
                        statements.InsertStatement(statements.Statements.IndexOf(statements.Statements.First(s => s.Text == "app.UseAuthorization();")),
                            new CSharpStatement("app.UseAuthentication();"));

                    }

                    if (statements.Statements.FirstOrDefault(s => s.Text == "app.Run();") != null)
                    {
                        if (startup.ExecutionContext.GetSettings().GetBlazor().Authentication().IsAspnetcoreIdentity())
                        {
                            statements.InsertStatement(statements.Statements.IndexOf(statements.Statements.First(s => s.Text == "app.Run();")),
                            new CSharpStatement("app.MapAdditionalIdentityEndpoints();"));
                        }
                    }
                });
            });

            if (startup.ExecutionContext.GetSettings().GetBlazor().RenderMode().IsInteractiveServer() &&
                !startup.ExecutionContext.GetSettings().GetBlazor().Authentication().IsAspnetcoreIdentity())
            {

                var httpClients = application.FindTemplateInstance<ICSharpFileBuilderTemplate>("Intent.Blazor.HttpClients.HttpClientConfiguration");

                if (httpClients is null)
                {
                    return;
                }

                httpClients.CSharpFile.OnBuild(file =>
                {
                    var @class = file.Classes.First();

                    var addHttpClients = @class.Methods.FirstOrDefault(m => m.Name == "AddHttpClients");

                    if (addHttpClients is null)
                    {
                        return;
                    }

                    foreach (var statement in addHttpClients.Statements)
                    {
                        if (statement is CSharpMethodChainStatement mcs)
                        {
                            var addHttpMessageHandlerStatement = mcs.Statements.FirstOrDefault(s => s.Text.Contains("AddHttpMessageHandler"));

                            if (addHttpMessageHandlerStatement != null)
                            {
                                mcs.Statements.Remove(addHttpMessageHandlerStatement);
                                mcs.Statements.Add(new CSharpInvocationStatement($"AddHttpMessageHandler<{httpClients.GetTypeName(ServerAuthorizationMessageHandlerTemplate.TemplateId)}>").WithoutSemicolon());
                            }
                        }
                    }
                });
            }
        }

        private void AddPersistanceProvider(IAppStartupTemplate startupTemplate, IHasCSharpStatements statements, IAppStartupFile.IServiceConfigurationContext context)
        {
            switch (startupTemplate.ExecutionContext.GetSettings().GetGroup("489a67db-31b2-4d51-96d7-52637c3795be").GetSetting("3e3d24f8-ad29-44d6-b7e5-e76a5af2a7fa").Value)
            {
                case "interactive-web-assembly":
                    {
                        statements.AddStatement($"{context.Services}.AddScoped<AuthenticationStateProvider, {startupTemplate.GetTypeName(PersistingServerAuthenticationStateProviderTemplate.TemplateId)}>();");
                        break;
                    }
                case "interactive-auto":
                    {
                        statements.AddStatement($"{context.Services}.AddScoped<AuthenticationStateProvider, {startupTemplate.GetTypeName(PersistingRevalidatingAuthenticationStateProviderTemplate.TemplateId)}>();");
                        break;
                    }
                case "interactive-server":
                    {
                        statements.AddStatement($"{context.Services}.AddScoped<AuthenticationStateProvider, {startupTemplate.GetTypeName(IdentityRevalidatingAuthenticationStateProviderTemplate.TemplateId)}>();");
                        break;
                    }
                default:
                    break;
            }
        }


    }

    [IntentIgnore]
    public static class IdentityHelperExtensions
    {
        public static (string Namespace, string Name) GetIdentityUserClassTuple(ICSharpTemplate template)
        {
            var associations = template.ExecutionContext.MetadataManager.Domain(template.ExecutionContext.GetApplicationConfig().Id).GetClassModels().Select(c => c.InternalElement).SelectMany(a => a.AssociatedElements);

            var models = associations.Where(a => a is not null).Where(e => e.Association.SourceEnd is not null).Select(s => s.Association.SourceEnd);

            var identityModels = GetIdentityClassModels(template.ExecutionContext.MetadataManager, template.ExecutionContext.GetApplicationConfig().Id);

            if (identityModels.Count > 0)
            {
                string name = GetName(identityModels, "IdentityUser", template, out var ns, false);
                return (ns ?? "Microsoft.AspNetCore.Identity", $"{name}");
            }
            else
            {
                var identityModel = GetIdentityUserClass(template.ExecutionContext.MetadataManager, template.ExecutionContext.GetApplicationConfig().Id);
                if (identityModel is not null && template.TryGetTemplate<ICSharpFileBuilderTemplate>("Domain.Entity", identityModel, out var domainTemplate))
                {
                    return (domainTemplate.Namespace, domainTemplate.ClassName);
                }
                template.UseType("Microsoft.AspNetCore.Identity.IdentityUser");
                return ("Microsoft.AspNetCore.Identity", "IdentityUser");
            }
        }
        public static string GetIdentityUserClass(ICSharpTemplate template)
        {
            var associations = template.ExecutionContext.MetadataManager.Domain(template.ExecutionContext.GetApplicationConfig().Id).GetClassModels().Select(c => c.InternalElement).SelectMany(a => a.AssociatedElements);

            var models = associations.Where(a => a is not null).Where(e => e.Association.SourceEnd is not null).Select(s => s.Association.SourceEnd);

            var identityModels = GetIdentityClassModels(template.ExecutionContext.MetadataManager, template.ExecutionContext.GetApplicationConfig().Id);

            if (identityModels.Count > 0)
            {
                return $"{GetName(identityModels, "IdentityUser", template, out var _, false)}";
            }
            else
            {
                var identityModel = GetIdentityUserClass(template.ExecutionContext.MetadataManager, template.ExecutionContext.GetApplicationConfig().Id);
                var identityUserClass = identityModel is not null ? template.GetTypeName("Domain.Entity", identityModel) : null;

                return identityUserClass ?? template.UseType("Microsoft.AspNetCore.Identity.IdentityUser");
            }
        }

        public static string GetIdentityRoleClass(ICSharpTemplate template)
        {
            var associations = template.ExecutionContext.MetadataManager.Domain(template.ExecutionContext.GetApplicationConfig().Id).GetClassModels().Select(c => c.InternalElement).SelectMany(a => a.AssociatedElements);

            var models = associations.Where(a => a is not null).Where(e => e.Association.SourceEnd is not null).Select(s => s.Association.SourceEnd);

            var identityModels = GetIdentityClassModels(template.ExecutionContext.MetadataManager, template.ExecutionContext.GetApplicationConfig().Id);

            if (identityModels.Count > 0)
            {
                return $"{GetName(identityModels, "IdentityRole", template, out var _)}";
            }
            else
            {
                var identityModel = GetIdentityUserClass(template.ExecutionContext.MetadataManager, template.ExecutionContext.GetApplicationConfig().Id);
                var identityUserClass = identityModel is not null ? template.GetTypeName("Domain.Entity", identityModel) : null;

                return identityUserClass ?? template.UseType("Microsoft.AspNetCore.Identity.IdentityUser");
            }
        }

        private static string GetName(List<ClassModel> classModels, string entityName, ICSharpTemplate template, out string ns, bool includeGeneric = true)
        {
            ns = null;
            if (classModels.Any(c => c.Name == entityName))
            {
                var @class = classModels.First(c => c.Name == entityName).ChildClasses.First();
                template.GetTypeName("Domain.Entity", @class);

                var entityTemplate = template.GetTemplate<ICSharpFileBuilderTemplate>("Domain.Entity", @class);

                entityTemplate.CSharpFile.AfterBuild(c =>
                {
                    c.AddUsing("Microsoft.AspNetCore.Identity");
                });
                ns = entityTemplate.Namespace;
                return @class.Name;
            }

            if (includeGeneric)
            {
                return $"{entityName}<string>";
            }
            return $"{entityName}";
        }

        public static List<ClassModel> GetIdentityClassModels(IMetadataManager metadataManager, string applicationId)
        {
            var associations = metadataManager.Domain(applicationId).GetClassModels().Select(c => c.InternalElement).SelectMany(a => a.AssociatedElements);

            var models = associations.Where(a => a is not null).Where(e => e.Association.SourceEnd is not null).Select(s => s.Association.SourceEnd);

            return models.Select(p => p.ParentElement.AsClassModel())
                .Where(m => m is not null && (m.Name == "IdentityUserRole" || m.Name == "IdentityRole" ||
                                               m.Name == "IdentityUser" || m.Name == "IdentityRoleClaim" || m.Name == "IdentityUserToken" || m.Name == "IdentityUserClaim" ||
                                               m.Name == "IdentityUserLogin")).ToList();
        }

        internal static ClassModel GetIdentityUserClass(IMetadataManager metadataManager, string applicationId)
        {
            var identityModels = metadataManager.Domain(applicationId).GetClassModels()
                .Where(x => x.HasStereotype("Identity User"))
                .ToArray();
            if (identityModels.Length > 1)
            {
                var sb = new StringBuilder("More than one class has the \"Identity User\" stereotype applied to it:");
                foreach (var model in identityModels)
                {
                    sb.Append($"{Environment.NewLine}- \"{model.Name}\" [{model.Id}]");
                }

                Logging.Log.Failure(sb.ToString());
                return null;
            }

            return identityModels.SingleOrDefault();
        }
    }
}