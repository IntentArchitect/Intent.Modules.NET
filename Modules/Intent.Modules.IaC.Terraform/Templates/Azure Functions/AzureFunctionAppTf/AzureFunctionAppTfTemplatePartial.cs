using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.IaC.Terraform.Api;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.ProjectItemTemplate.Partial", Version = "1.0")]

namespace Intent.Modules.IaC.Terraform.Templates.AzureFunctions.AzureFunctionAppTf
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    partial class AzureFunctionAppTfTemplate : IntentTemplateBase<ApplicationInfo>
    {
        private readonly string _sanitizedApplicationName;

        [IntentManaged(Mode.Fully)]
        public const string TemplateId = "Intent.IaC.Terraform.Azure Functions.AzureFunctionAppTf";

        [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
        public AzureFunctionAppTfTemplate(IOutputTarget outputTarget, ApplicationInfo model) : base(TemplateId, outputTarget, model)
        {
            _sanitizedApplicationName = Model.Name.Replace('.', '-').ToKebabCase();
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public override ITemplateFileConfig GetTemplateFileConfig()
        {
            return new TemplateFileConfig(
                fileName: _sanitizedApplicationName,
                fileExtension: "tf"
            );
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public override string TransformText()
        {
            var builder = new TerraformFileBuilder();

            const int randomSeedLength = 3;
            builder.AddResource("random_string", _sanitizedApplicationName, resource =>
            {
                resource.AddSetting("length", randomSeedLength)
                    .AddSetting("special", false)
                    .AddSetting("upper", false);
            });

            var appNameRef = $"{_sanitizedApplicationName}_app_name";
            var storageNameRef = $"{_sanitizedApplicationName}_storage_name";
            var localAppName = $"local.{appNameRef}";
            var localStorageName = $"local.{storageNameRef}";

            builder.AddLocals(locals =>
            {
                locals.AddSetting(appNameRef, _sanitizedApplicationName);
                locals.AddSetting(storageNameRef, GetStorageResourceName(_sanitizedApplicationName, randomSeedLength));
            });

            var functionPlanRef = $"{_sanitizedApplicationName}_function_plan";

            builder.AddComment("Hosting Plan (App Service Plan)");
            builder.AddResource("azurerm_service_plan", functionPlanRef, resource => resource
                .AddSetting("name", $"asp-${{{localAppName}}}")
                .AddRawSetting("location", Terraform.azurerm_resource_group.main_rg.location)
                .AddRawSetting("resource_group_name", Terraform.azurerm_resource_group.main_rg.name)
                .AddSetting("os_type", "Windows")
                .AddSetting("sku_name", "Y1"));

            builder.AddComment("Storage Account");
            builder.AddResource("azurerm_storage_account", storageNameRef, resource =>
            {
                resource.AddRawSetting("name", localStorageName);
                resource.AddRawSetting("location", Terraform.azurerm_resource_group.main_rg.location);
                resource.AddRawSetting("resource_group_name", Terraform.azurerm_resource_group.main_rg.name);
                resource.AddSetting("account_tier", "Standard");
                resource.AddSetting("account_replication_type", "LRS");
                resource.AddSetting("account_kind", "StorageV2");
            });

            var storageNameExpression = $"azurerm_storage_account.{storageNameRef}.name";
            var storagePrimaryAccessKeyExpression = $"azurerm_storage_account.{storageNameRef}.primary_access_key";

            builder.AddComment("Function App");
            builder.AddResource("azurerm_windows_function_app", $"{_sanitizedApplicationName}_function_app", resource =>
            {
                resource.AddRawSetting("name", localAppName);
                resource.AddRawSetting("location", Terraform.azurerm_resource_group.main_rg.location);
                resource.AddRawSetting("resource_group_name", Terraform.azurerm_resource_group.main_rg.name);
                resource.AddRawSetting("service_plan_id", $"azurerm_service_plan.{functionPlanRef}.id");
                resource.AddRawSetting("storage_account_name", storageNameExpression);
                resource.AddRawSetting("storage_account_access_key", storagePrimaryAccessKeyExpression);
                resource.AddBlock("site_config", siteConfig =>
                {
                    siteConfig.AddBlock("application_stack", appStack =>
                    {
                        const string visualStudioDesignerId = "0701433c-36c0-4569-b1f4-9204986b587d";
                        var designer = ExecutionContext.MetadataManager.GetDesigner(Model.Id, visualStudioDesignerId);
                        
                        const string csharpProjectType = "8e9e6693-2888-4f48-a0d6-0f163baab740";
                        const string projectStereotype = "a490a23f-5397-40a1-a3cb-6da7e0b467c0";
                        const string azureFuncVersionProperty = "9da51e12-529a-4592-9119-491d7bc9d5d5";
                        var project = designer.GetElementsOfType(csharpProjectType)
                            .FirstOrDefault(p => (p.GetStereotype(projectStereotype)?.TryGetProperty(azureFuncVersionProperty, out var version) ?? false) && version?.Value != null);

                        if (project is not null)
                        {
                            const string targetFrameworkProperty = "d53ab03c-90cf-4b6a-b733-73b6983ab603";
                            var versionValue = project.GetStereotype(projectStereotype).GetProperty<IElement>(targetFrameworkProperty);
                            appStack.AddSetting("dotnet_version", $"v{versionValue.Value.Replace("net", string.Empty)}");
                        }
                    });
                    siteConfig.AddBlock("cors", cors => cors.AddSetting("allowed_origins", new[] { "https://portal.azure.com" }));
                });
                resource.AddObject("app_settings", appSettings =>
                {
                    appSettings.AddRawSetting(@"""APPINSIGHTS_INSTRUMENTATIONKEY""", Terraform.azurerm_application_insights.app_insights.instrumentation_key);
                    appSettings.AddSetting(@"""AzureWebJobsStorage""", $"DefaultEndpointsProtocol=https;AccountName=${{{storageNameExpression}}};AccountKey=${{{storagePrimaryAccessKeyExpression}}};EndpointSuffix=core.windows.net");

                    // foreach (var request in _appSettingsRequests.OrderBy(x => x.Key))
                    // {
                    //     if (configVarMappings.TryGetValue(request.Key, out var varName))
                    //     {
                    //         appSettings.AddRawSetting($@"""{request.Key}""", varName);
                    //     }
                    //     else
                    //     {
                    //         var value = request.Value?.ToString();
                    //         appSettings.AddRawSetting($@"""{request.Key}""", value is null ? "null" : value == "" ? @"""""" : @$"""{value}""");
                    //     }
                    // }
                });
            });

            return builder.Build();
        }

        private string GetStorageResourceName(string seed, int randomSeedLength)
        {
            const string storageSuffix = "storage";
            const int maxLength = 24;
            var context = seed.Replace("-", string.Empty).Replace("_", string.Empty).Replace(".", string.Empty);
            var calc = context.Length + randomSeedLength + storageSuffix.Length - maxLength;
            if (calc > 0)
            {
                context = context.Substring(0, maxLength - randomSeedLength - storageSuffix.Length - 1);
            }

            return $"{context}${{random_string.{_sanitizedApplicationName}.result}}{storageSuffix}";
        }
    }
}