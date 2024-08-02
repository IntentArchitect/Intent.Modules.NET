using System;
using System.Collections.Generic;
using System.Linq;
using Intent.AzureFunctions.Api;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.TypeResolution;
using Intent.Modules.Common.VisualStudio;
using Intent.Modules.AzureFunctions.Api;

namespace Intent.Modules.AzureFunctions.Templates.AzureFunctionClass.TriggerStrategies
{
    public class CosmosDBTriggerHandler : IFunctionTriggerHandler
    {
        private readonly AzureFunctionClassTemplate _template;
        private readonly IAzureFunctionModel _azureFunctionModel;

        public CosmosDBTriggerHandler(AzureFunctionClassTemplate template, IAzureFunctionModel azureFunctionModel)
        {
            _template = template;
            _azureFunctionModel = azureFunctionModel;
        }

        public void ApplyMethodParameters(CSharpClassMethod method)
        {
            if (_azureFunctionModel.Parameters.Count == 0)
            {
                throw new Exception($"Please specify the parameter for the Cosmos DB triggered Azure Function [{_azureFunctionModel.Name}]");
            }

            if (_azureFunctionModel.Parameters.Count > 1)
            {
                throw new Exception($"Please specify only one parameter for the Queue triggered Azure Function [{_azureFunctionModel.Name}]");
            }

            if (!_azureFunctionModel.TryGetCosmosDBTrigger(out var cosmosConfig))
            {
                throw new Exception($"Missing Cosmos DB Trigger stereotype for the Cosmos DB triggered Azure Function [{_azureFunctionModel.Name}]");
            }

            string messageType = string.Format("IReadOnlyCollection<{0}>", _template.GetTypeName(_azureFunctionModel.Parameters.Single().TypeReference.Element.AsTypeReference()));

            method.AddParameter(
                type: messageType,
                name: "rawCollection",
                configure: param =>
                {
                    param.AddAttribute("CosmosDBTrigger", attr =>
                    {
                        attr.AddArgument($@"databaseName: ""{cosmosConfig.DatabaseName()}""");
                        attr.AddArgument($@"containerName: ""{cosmosConfig.ContainerName()}""");
                        if (!string.IsNullOrEmpty(cosmosConfig.Connection()))
                        {
                            attr.AddArgument($@"Connection = ""{cosmosConfig.Connection()}""");
                        }
                        attr.AddArgument($@"CreateLeaseContainerIfNotExists = {cosmosConfig.CreateLeaseContainerIfNotExists().ToString().ToLower()}");

                        if (!string.IsNullOrEmpty(cosmosConfig.LeaseConnection()))
                        {
                            attr.AddArgument($@"LeaseConnection  = ""{cosmosConfig.LeaseConnection()}""");
                        }
                        if (cosmosConfig.LeasesContainerThroughput() != null)
                        {
                            attr.AddArgument($@"LeasesContainerThroughput  = {cosmosConfig.LeasesContainerThroughput().Value}");
                        }
                        if (!string.IsNullOrEmpty(cosmosConfig.LeaseContainerName()))
                        {
                            attr.AddArgument($@"LeaseContainerName  = ""{cosmosConfig.LeaseContainerName()}""");
                        }
                        if (!string.IsNullOrEmpty(cosmosConfig.LeaseContainerPrefix()))
                        {
                            attr.AddArgument($@"LeaseContainerPrefix  = ""{cosmosConfig.LeaseContainerPrefix()}""");
                        }
                        if (!string.IsNullOrEmpty(cosmosConfig.LeaseDatabaseName()))
                        {
                            attr.AddArgument($@"LeaseDatabaseName  = ""{cosmosConfig.LeaseDatabaseName()}""");
                        }
                    });
                });
            method.AddParameter(_template.UseType("System.Threading.CancellationToken"), "cancellationToken");
        }

        public void ApplyMethodStatements(CSharpClassMethod method)
        {
            _template.AddUsing("System.Linq");
            string parameterName = _azureFunctionModel.Parameters.Single().Name.ToParameterName();
            method.AddStatement("if (rawCollection == null || rawCollection.Count == 0) return;");

            if (_azureFunctionModel.Parameters.Single().TypeReference.IsCollection)
            {
                method.AddStatement($"var {parameterName} = rawCollection.ToList();");
            }
            else
            {
                method.AddForEachStatement(parameterName, "rawCollection", stmt => stmt.AddMetadata("service-invoke", "true"));
            }
        }

        public IEnumerable<INugetPackageInfo> GetNugetDependencies()
        {
            yield return NugetPackages.MicrosoftAzureWebJobsExtensionsCosmosDB(_template.OutputTarget);
        }
    }
}