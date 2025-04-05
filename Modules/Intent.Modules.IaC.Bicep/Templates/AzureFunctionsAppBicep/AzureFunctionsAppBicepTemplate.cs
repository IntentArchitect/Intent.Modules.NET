using System;
using System.Collections.Generic;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.FileTemplateStringInterpolation", Version = "1.0")]

namespace Intent.Modules.IaC.Bicep.Templates.AzureFunctionsAppBicep
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class AzureFunctionsAppBicepTemplate
    {
        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public override string TransformText()
        {
            return $$"""
                     {{GetScriptParameters()}}
                     
                     resource appInsights 'Microsoft.Insights/components@2020-02-02' = {
                       name: 'app-insights-${functionAppName}'
                       kind: 'web'
                       location: resourceGroup().location
                       properties: {
                         Application_Type: 'web'
                         DisableIpMasking: false
                         Flow_Type: 'Bluefield'
                         Request_Source: 'rest'
                         publicNetworkAccessForIngestion: 'Enabled'
                         publicNetworkAccessForQuery: 'Enabled'
                       }
                     }
                     
                     resource functionAppHostingPlan 'Microsoft.Web/serverfarms@2021-02-01' = {
                       name: 'asp-${functionAppName}'
                       location: resourceGroup().location
                       sku: {
                         name: 'Y1'
                         tier: 'Dynamic'
                       }
                     }
                     
                     resource storageAccount 'Microsoft.Storage/storageAccounts@2021-02-01' = {
                       name: 'storage${uniqueString(resourceGroup().id)}'
                       location: resourceGroup().location
                       kind: 'StorageV2'
                       sku: {
                         name: 'Standard_LRS'
                       }
                     }
                     
                     var storageAccountKey = storageAccount.listKeys().keys[0].value
                     var storageConnectionString = 'DefaultEndpointsProtocol=https;AccountName=${storageAccount.name};AccountKey=${storageAccountKey};EndpointSuffix=${environment().suffixes.storage}'
                     
                     resource functionApp 'Microsoft.Web/sites@2021-02-01' = {
                       name: functionAppName
                       location: resourceGroup().location
                       kind: 'functionapp'
                       properties: {
                         serverFarmId: functionAppHostingPlan.id
                         siteConfig: {
                           appSettings: {{GetAzureFunctionsAppSettings("      ")}}
                         }
                       }
                     }
                     """;
        }
    }
}