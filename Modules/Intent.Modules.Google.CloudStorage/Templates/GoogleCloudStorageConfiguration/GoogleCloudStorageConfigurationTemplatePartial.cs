using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.DependencyInjection;
using Intent.Modules.Common.CSharp.Multitenancy;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.Google.CloudStorage.Settings;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;
using Microsoft.VisualBasic;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.Google.CloudStorage.Templates.GoogleCloudStorageConfiguration
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class GoogleCloudStorageConfigurationTemplate : CSharpTemplateBase<object>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.Google.CloudStorage.GoogleCloudStorageConfiguration";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public GoogleCloudStorageConfigurationTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddClass($"GoogleCloudStorageConfiguration", @class =>
                {
                    @class.Static();
                    @class.AddMethod(UseType("Microsoft.Extensions.DependencyInjection.IServiceCollection"), "AddGoogleCloudStorage", method =>
                    {
                        method.Static();
                        method.AddParameter(UseType("Microsoft.Extensions.DependencyInjection.IServiceCollection"), "services", param => param.WithThisModifier());
                        method.AddParameter(UseType("Microsoft.Extensions.Configuration.IConfiguration"), "configuration");

                        method.AddStatement($"services.AddTransient<{this.GetCloudStorageInterfaceName()}, {this.GetGoogleCloudStorageImplementationName()}>();");

                        if (ExecutionContext.Settings.GetMultitenancySettings()?.GoogleCloudStorageDataIsolation()?.IsSeparateStorageAccount() == true)
                        {
                            var teneantConnectionsTemplate = this.GetTemplate<ICSharpFileBuilderTemplate>("Intent.Modules.AspNetCore.MultiTenancy.TenantConnectionsInterfaceTemplate");

                            method.AddStatement($"services.AddSingleton<{this.GetGoogleCloudStorageMultiTenantConnectionFactoryName()}>();");
                            method.AddInvocationStatement("services.AddScoped", invoc =>
                            {
                                invoc.AddLambdaBlock("sp", lambda =>
                                {
                                    lambda.AddStatement($"var tenantConnections = sp.GetService <{this.GetTypeName(teneantConnectionsTemplate)}> ();");
                                    lambda.AddIfStatement("tenantConnections is null || tenantConnections.Id is null || tenantConnections.GoogleCloudStorageConnection is null", stmt => stmt
                                        .AddStatement("throw new Finbuckle.MultiTenant.MultiTenantException(\"Failed to resolve tenant MongoDb connection information\");"));
                                    lambda.AddStatement($"var factory = sp.GetRequiredService <{this.GetGoogleCloudStorageMultiTenantConnectionFactoryName()}> ();");
                                    lambda.AddStatement($"return factory.GetStorageClient(tenantConnections.Id, tenantConnections.GoogleCloudStorageConnection);");
                                });
                            });
                        }
                        else
                        {
                            method.AddInvocationStatement("services.AddSingleton", invoc =>
                            {
                                invoc.AddLambdaBlock("sp", lambda =>
                                {
                                    lambda.AddObjectInitStatement("var credentialFileLocation", "sp.GetRequiredService<IConfiguration>().GetValue<string>(\"GCP:CloudStorageAuthFileLocation\");");
                                    lambda.AddObjectInitStatement("var googleCredential", $"{UseType("Google.Apis.Auth.OAuth2.GoogleCredential")}.FromFile(credentialFileLocation);");
                                    lambda.AddReturn($"{UseType("Google.Cloud.Storage.V1.StorageClient")}.Create(googleCredential)");
                                });
                            });
                        }
                        method.AddStatement("return services;", stmt => stmt.SeparatedFromPrevious());
                    });
                });
        }

        public override void AfterTemplateRegistration()
        {
            base.AfterTemplateRegistration();
            if (ExecutionContext.Settings.GetMultitenancySettings()?.GoogleCloudStorageDataIsolation()?.IsSeparateStorageAccount() == true)
            {
                ExecutionContext.EventDispatcher.Publish(new MultitenantConnectionStringRegistrationRequest("GoogleCloudStorageConnection", $"JsonConnection-{{tenant}}"));

                FixUpEntityFrameworkCoreConnectionResolution();
            }
            this.ExecutionContext.EventDispatcher.Publish(ServiceConfigurationRequest
                .ToRegister("AddGoogleCloudStorage", ServiceConfigurationRequest.ParameterType.Configuration)
                .ForConcern("Infrastructure")
                .HasDependency(this));
        }

        // Claiming our own named GoogleCloudStorageConnection above means TenantExtendedInfoTemplate omits the
        // generic ConnectionString property from the tenant class (the two are mutually exclusive by design).
        // Intent.Modules.AspNetCore.MultiTenancy's own DependencyInjection integration
        // (AspNetCoreIntegrationExtension.GetSeparateDatabaseDataIsolationConfiguration) doesn't know about our
        // setting and always assumes ConnectionString exists whenever this app also has an EF Core DbContext
        // with separate-database isolation - so patch that generated statement here (this module already
        // depends on AspNetCore.MultiTenancy, so this is the correct direction for this fix-up to live in,
        // rather than teaching AspNetCore.MultiTenancy about this module's specific setting). Runs at Final
        // priority (1000) so the statement already exists to find and replace by the time this executes.
        private void FixUpEntityFrameworkCoreConnectionResolution()
        {
            // "Data Isolation" (owned by AspNetCore.MultiTenancy) - read via the generic GetSetting(id) lookup
            // since this module's own MultitenancySettings shim only exposes the properties it directly uses.
            const string dataIsolationSettingId = "be7c671e-bbef-4d75-b42d-a6547de3ae82";
            if (ExecutionContext.Settings.GetMultitenancySettings()?.GetSetting(dataIsolationSettingId)?.Value != "separate-database")
            {
                return;
            }

            if (GetTemplate<object>("Infrastructure.Data.DbContext", new TemplateDiscoveryOptions { ThrowIfNotFound = false, TrackDependency = false }) == null)
            {
                return;
            }

            var dependencyInjectionTemplate = GetTemplate<ICSharpFileBuilderTemplate>("Infrastructure.DependencyInjection", new TemplateDiscoveryOptions { ThrowIfNotFound = false, TrackDependency = false });
            dependencyInjectionTemplate?.CSharpFile.AfterBuild(file =>
            {
                var method = file.Classes.First().FindMethod("AddInfrastructure");

                method?.FindStatement(x => x.GetText(string.Empty).Contains("tenantInfo?.ConnectionString"))
                    ?.FindAndReplace("tenantInfo?.ConnectionString", "tenantInfo?.Identifier");

                // The comment is factually wrong once the expression above becomes tenantInfo?.Identifier -
                // it no longer resolves a connection string, so correct it to match.
                method?.FindStatement(x => x.GetText(string.Empty).Contains("its connection string is used"))
                    ?.FindAndReplace(
                        "// Design-time safe: at runtime the tenant is always resolved and its connection string is used; at design time (EF tooling) no tenant is resolved, so fall back to DefaultConnection so FindContextTypes()/migrations do not throw.",
                        "// Design-time safe: at runtime the tenant is always resolved and its identifier keys a separate\n                // in-memory database per tenant; at design time (EF tooling) no tenant is resolved, so fall back\n                // to DefaultConnection so FindContextTypes()/migrations do not throw.");
            }, 1000);
        }

        [IntentManaged(Mode.Fully)]
        public CSharpFile CSharpFile { get; }

        [IntentManaged(Mode.Fully)]
        protected override CSharpFileConfig DefineFileConfig()
        {
            return CSharpFile.GetConfig();
        }

        [IntentManaged(Mode.Fully)]
        public override string TransformText()
        {
            return CSharpFile.ToString();
        }
    }
}