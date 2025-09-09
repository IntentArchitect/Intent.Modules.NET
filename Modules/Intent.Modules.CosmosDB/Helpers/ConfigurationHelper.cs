using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modelers.Domain.Api;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.CosmosDB.Templates;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intent.Modules.CosmosDB.Helpers;
public static class ConfigurationHelper
{
    public static void BuildConfigurationLambda(IDesigner domainDesigner, ICSharpFileBuilderTemplate template, 
        CSharpInvocationStatement invocation, bool useOptions = false)
    {
        var classes = domainDesigner.GetClassModels()
                    .Where(x => CosmosDbProvider.FilterDbProvider(x) &&
                                x.IsAggregateRoot() &&
                                !x.IsAbstract)
                    .ToArray();

        if (classes.All(x => !x.TryGetContainerSettings(out _)))
        {
            return;
        }

        var @class = template.CSharpFile.Classes.FirstOrDefault();
        var builderMethod = @class?.Methods.FirstOrDefault(m => m.Name == "BuildContainer");

        invocation.AddArgument(new CSharpLambdaBlock("options"), options =>
        {
            options.AddStatements(builderMethod is null ? 
                GetBuilderStatements(template, useOptions, classes) :
                GetBuilderStatements(builderMethod));
        });
    }

    public static List<CSharpStatement> GetConfigurationStatements(IDesigner domainDesigner, ICSharpFileBuilderTemplate template, bool useOptions = false)
    {
        var classes = domainDesigner.GetClassModels()
                    .Where(x => CosmosDbProvider.FilterDbProvider(x) &&
                                x.IsAggregateRoot() &&
                                !x.IsAbstract)
                    .ToArray();

        if (classes.All(x => !x.TryGetContainerSettings(out _)))
        {
            return [];
        }

        return GetBuilderStatements(template, useOptions, classes);
    }

    private static List<CSharpStatement> GetBuilderStatements(ICSharpFileBuilderTemplate template, bool useOptions, ClassModel[] classes)
    {
        const string defaultContainerId = "defaultContainerId";
        var hasDefaultContainerName = classes.Any(x => !x.TryGetContainerSettings(out var containerSettings) ||
                                                       containerSettings.Name == null);

        var statements = new List<CSharpStatement>();
        
        if (hasDefaultContainerName)
        {
            if (!useOptions)
            {
                statements.Add(new CSharpStatement($"var {defaultContainerId} = configuration.GetValue<string>(\"RepositoryOptions:ContainerId\");"));
            }
            else
            {
                statements.Add(new CSharpStatement($"var {defaultContainerId} = cosmosOptions.ContainerId;"));
            }

            statements.Add(new CSharpIfStatement("string.IsNullOrWhiteSpace(defaultContainerId)")
                .AddStatement($"throw new {template.UseType("System.Exception")}(\"\\\"RepositoryOptions:ContainerId\\\" configuration not specified\");"));
        }

        var containerTypeStatement = new CSharpStatement("options.ContainerPerItemType = true;");
        if (hasDefaultContainerName)
        {
            containerTypeStatement.SeparatedFromPrevious();
        }
        statements.Add(containerTypeStatement);

        var containerBuilderStatement = new CSharpMethodChainStatement("options.ContainerBuilder");

        foreach (var @class in classes)
        {
            @class.TryGetContainerSettings(out var containerSettings);

            var documentTypeName = template.GetCosmosDBDocumentName(@class);
            containerBuilderStatement.AddChainStatement(new CSharpInvocationStatement($"Configure<{documentTypeName}>"), l =>
            {
                var configureContainer = (CSharpInvocationStatement)l;
                configureContainer.WithoutSemicolon();

                var cSharpMethodChainStatement = new CSharpMethodChainStatement("c => c");
                configureContainer.AddArgument(cSharpMethodChainStatement.WithoutSemicolon());

                var containerName = containerSettings?.Name != null ? $"\"{containerSettings.Name}\"" : defaultContainerId;
                cSharpMethodChainStatement.AddChainStatement($"WithContainer({containerName})");

                if (containerSettings?.PartitionKey != null)
                {
                    cSharpMethodChainStatement.AddChainStatement($"WithPartitionKey(\"/{containerSettings.PartitionKey}\")");
                }

                switch (containerSettings?.ThroughputType)
                {
                    case ContainerThroughputType.Autoscale:
                        cSharpMethodChainStatement.AddChainStatement($"WithAutoscaleThroughput({containerSettings.AutomaticThroughputMax:D})");
                        break;
                    case ContainerThroughputType.Manual:
                        cSharpMethodChainStatement.AddChainStatement($"WithManualThroughput({containerSettings.ManualThroughput:D})");
                        break;
                    case ContainerThroughputType.Serverless:
                        cSharpMethodChainStatement.AddChainStatement($"WithServerlessThroughput({containerSettings.ManualThroughput:D})");
                        break;
                }
            });
        }
        statements.Add(containerBuilderStatement);

        return statements;
    }

    private static List<CSharpStatement> GetBuilderStatements(CSharpClassMethod method)
    {
        if(method.Parameters.Count == 1)
        {
            return [new CSharpInvocationStatement("BuildContainer").AddArgument("options")];
        }

        if (method.Parameters.Count == 2)
        {
            return [new CSharpInvocationStatement("BuildContainer")
                .AddArgument("options")
                .AddArgument("cosmosOptions")];
        }

        return [new CSharpInvocationStatement("BuildContainer")];
    }
}
