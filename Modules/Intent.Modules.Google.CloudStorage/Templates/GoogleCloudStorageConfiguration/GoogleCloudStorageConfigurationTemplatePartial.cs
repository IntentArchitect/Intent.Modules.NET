using System;
using System.Collections.Generic;
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
            }
            this.ExecutionContext.EventDispatcher.Publish(ServiceConfigurationRequest
                .ToRegister("AddGoogleCloudStorage", ServiceConfigurationRequest.ParameterType.Configuration)
                .ForConcern("Infrastructure")
                .HasDependency(this));
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