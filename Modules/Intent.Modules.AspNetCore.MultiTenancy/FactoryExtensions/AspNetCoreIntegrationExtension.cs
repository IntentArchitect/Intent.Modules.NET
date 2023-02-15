using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Modules.AspNetCore.MultiTenancy.Settings;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Plugins;
using Intent.Modules.Common.VisualStudio;
using Intent.Modules.Constants;
using Intent.Plugins.FactoryExtensions;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.FactoryExtension", Version = "1.0")]

namespace Intent.Modules.AspNetCore.MultiTenancy.FactoryExtensions
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class AspNetCoreIntegrationExtension : FactoryExtensionBase
    {
        public override string Id => "Intent.Modules.AspNetCore.MultiTenancy.AspNetCoreIntegrationExtension";

        [IntentManaged(Mode.Ignore)]
        public override int Order => 0;

        /// <summary>
        /// This is an example override which would extend the
        /// <see cref="ExecutionLifeCycleSteps.Start"/> phase of the Software Factory execution.
        /// See <see cref="FactoryExtensionBase"/> for all available overrides.
        /// </summary>
        /// <remarks>
        /// It is safe to update or delete this method.
        /// </remarks>
        protected override void OnAfterTemplateRegistrations(IApplication application)
        {
            application.FindTemplateInstance<ICSharpFileBuilderTemplate>("App.Startup")
                ?.CSharpFile.AfterBuild(file =>
                {
                    file.Classes.First().FindMethod("ConfigureServices")
                        ?.AddStatement("services.ConfigureMultiTenancy(Configuration);");

                    var statement = file.Classes.First().FindMethod("Configure")
                        .FindStatement(s => s.GetText(string.Empty).Contains("app.UseRouting()"))
                        ?.InsertBelow("app.UseMultiTenancy();");

                    if (statement == null)
                    {
                        throw new Exception("app.UseRouting() was not configured");
                    }
                });

            var dbContext = application.FindTemplateInstance<ICSharpFileBuilderTemplate>("Infrastructure.Data.DbContext");
            if (dbContext != null && application.Settings.GetMultitenancySettings().DataIsolation().IsSeparateDatabase())
            {
                var dependencyInjection = application.FindTemplateInstance<ICSharpFileBuilderTemplate>(TemplateFulfillingRoles.Infrastructure.DependencyInjection);
                dependencyInjection?.AddNugetDependency(new NugetPackageInfo("Finbuckle.MultiTenant", "6.5.1"));
                dependencyInjection?.AddNugetDependency(new NugetPackageInfo("Finbuckle.MultiTenant.EntityFrameworkCore", "6.5.1"));

                dependencyInjection?.CSharpFile.AfterBuild(file =>
                {
                    var method = file.Classes.First().FindMethod("AddInfrastructure");

                    method?.FindAndReplaceStatement(x => x.HasMetadata("is-connection-string"), "tenantInfo.ConnectionString");
                    //method?.FindStatement(x => x.GetText(string.Empty).StartsWith("services.AddDbContext"))
                    //    ?.ReplaceText("configuration.GetConnectionString(\"DefaultConnection\")", "tenantInfo.ConnectionString") // for SqlServer and PostgreSQL
                    //    .ReplaceText("configuration[\"Cosmos:DatabaseName\"]", "tenantInfo.ConnectionString"); // For Cosmos

                    method?.FindStatement(x => x.GetText(string.Empty).StartsWith("options.Use"))
                        ?.InsertAbove($@"var tenantInfo = sp.GetService<{dependencyInjection.UseType("Finbuckle.MultiTenant.ITenantInfo")}>() ?? throw new {dependencyInjection.UseType("Finbuckle.MultiTenant.MultiTenantException")}(""Failed to resolve tenant info."");");
                });
            }
        }
    }
}