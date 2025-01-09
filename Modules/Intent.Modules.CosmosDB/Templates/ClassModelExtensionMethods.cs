using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modelers.Domain.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.Templates;

namespace Intent.Modules.CosmosDB.Templates;

internal static class ClassModelExtensionMethods
{
    public static bool TryGetPartitionAttribute(
        this ClassModel model,
        [NotNullWhen(true)] out AttributeModel? partitionAttribute)
    {
        if (!model.TryGetContainerSettings(out var containerSettings) ||
            containerSettings.PartitionKey == null)
        {
            partitionAttribute = null;
            return false;
        }

        var @class = model;
        while (@class != null)
        {
            partitionAttribute = @class.Attributes.FirstOrDefault(x => x.Name.Equals(containerSettings.PartitionKey, StringComparison.OrdinalIgnoreCase));
            if (partitionAttribute != null)
            {
                return true;
            }
            @class = @class.ParentClass;
        }

        partitionAttribute = null;
        return false;
    }

    public static bool TryGetContainerSettings(
        this ClassModel classModel,
        [NotNullWhen(true)] out ContainerSettings? containerSettings)
    {
        const string nameProperty = "7ebc3aff-936c-465b-ac34-3e52362090ef";
        const string partitionKeyProperty = "bda56f7f-8ac6-4b36-bbc1-91bba18e71d9";
        const string throughputTypeProperty = "2c4ac3b3-b72b-459f-a22b-b1d6fb7fd3ea";
        const string automaticThroughputMaxProperty = "55056d4b-08db-4df3-b073-ae2ccc0ac7ac";
        const string manualThroughputProperty = "e06caad5-06a1-478b-9827-7e110f94039a";

        containerSettings = null;
        IHasStereotypes? hasStereotypes = classModel.InternalElement;

        while (hasStereotypes != null)
        {
            if (hasStereotypes.HasStereotype("Container"))
            {
                // If statement is to maintain compatibility where historically these two properties intentionally
                // did not get "mixed" over the hierarchy:
                if (containerSettings?.Name == null &&
                    containerSettings?.PartitionKey == null)
                {
                    var name = hasStereotypes.GetStereotypeProperty<string>("Container", nameProperty)?.Trim();
                    if (!string.IsNullOrWhiteSpace(name) && string.IsNullOrWhiteSpace(containerSettings?.Name))
                    {
                        containerSettings ??= new ContainerSettings();
                        containerSettings.Name = name;
                    }

                    var partitionKey = hasStereotypes.GetStereotypeProperty<string>("Container", partitionKeyProperty)?.Trim().TrimStart('/');
                    if (!string.IsNullOrWhiteSpace(partitionKey) && string.IsNullOrWhiteSpace(containerSettings?.PartitionKey))
                    {
                        containerSettings ??= new ContainerSettings();
                        containerSettings.PartitionKey = partitionKey.ToCamelCase();
                    }
                }

                var throughputTypeString = hasStereotypes.GetStereotypeProperty<string>("Container", throughputTypeProperty)?.Trim();
                if (!string.IsNullOrWhiteSpace(throughputTypeString))
                {
                    containerSettings ??= new ContainerSettings();
                    switch (throughputTypeString)
                    {
                        case "Autoscale":
                            containerSettings.ThroughputType = ContainerThroughputType.Autoscale;
                            containerSettings.AutomaticThroughputMax = hasStereotypes.GetStereotypeProperty<int?>("Container", automaticThroughputMaxProperty);
                            break;
                        case "Manual":
                            containerSettings.ThroughputType = ContainerThroughputType.Manual;
                            containerSettings.ManualThroughput = hasStereotypes.GetStereotypeProperty<int?>("Container", manualThroughputProperty);
                            break;
                        case "Serverless":
                            containerSettings.ThroughputType = ContainerThroughputType.Serverless;
                            break;
                        default:
                            if (!string.IsNullOrWhiteSpace(throughputTypeString)) throw new InvalidOperationException($"Unknown throughput type: {throughputTypeString}");
                            break;
                    }
                }
            }

            if (hasStereotypes is not IElement element)
            {
                break;
            }

            hasStereotypes = element.ParentElement ?? (IHasStereotypes)element.Package;
        }

        return containerSettings != null;
    }
}