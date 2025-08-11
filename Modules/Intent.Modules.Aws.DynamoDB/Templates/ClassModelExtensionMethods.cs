using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Intent.Aws.DynamoDB.Api;
using Intent.Exceptions;
using Intent.Metadata.DocumentDB.Api;
using Intent.Metadata.Models;
using Intent.Modelers.Domain.Api;
using Intent.Modules.Common;

namespace Intent.Modules.Aws.DynamoDB.Templates;

internal static class ClassModelExtensionMethods
{
    public static bool TryGetPartitionAttribute(
        this ClassModel model,
        [NotNullWhen(true)] out AttributeModel? partitionAttribute)
    {
        var partitionKeyAttributeName = model.TryGetTableSettings(out var tableSettings) &&
                                        !string.IsNullOrWhiteSpace(tableSettings.PartitionKey)
            ? tableSettings.PartitionKey
            : null;

        var @class = model;
        while (@class != null)
        {
            partitionAttribute = @class.Attributes.FirstOrDefault(x => tableSettings != null && x.Name.Equals(tableSettings.PartitionKey, StringComparison.OrdinalIgnoreCase)) ??
                                 @class.Attributes.SingleOrDefault(x => x.HasPrimaryKey());
            if (partitionAttribute != null)
            {
                return true;
            }

            @class = @class.ParentClass;
        }

        partitionAttribute = null;
        return false;
    }

    public static AttributeModel? GetVersionAttribute(this ClassModel model)
    {
        var @class = model;
        while (@class != null)
        {
            var attribute = @class.Attributes.FirstOrDefault(x => x.HasVersion());
            if (attribute != null)
            {
                Validate(attribute);
                return attribute;
            }

            @class = @class.ParentClass;
        }

        @class = model;
        while (@class != null)
        {
            var attribute = @class.Attributes.FirstOrDefault(x => string.Equals(x.Name, "Version", StringComparison.OrdinalIgnoreCase));
            if (attribute != null)
            {
                Validate(attribute);
                return attribute;
            }

            @class = @class.ParentClass;
        }

        return null;

        void Validate(AttributeModel attribute)
        {
            if (!attribute.TypeReference.IsNullable ||
                (!attribute.TypeReference.Element.IsLongType() && !attribute.TypeReference.Element.IsIntType()))
            {
                throw new ElementException(
                    attribute.InternalElement,
                    "Version attributes must be nullable and of type \"long\" or \"int\" when the optimistic concurrency setting is enabled.");
            }
        }
    }

    public static bool TryGetTableSettings(
        this ClassModel classModel,
        [NotNullWhen(true)] out TableSettings? tableSettings)
    {
        IHasStereotypes? hasStereotypes = classModel.InternalElement;

        while (hasStereotypes != null)
        {
            var stereotype = hasStereotypes.GetStereotype(FolderModelStereotypeExtensions.Table.DefinitionId);
            if (stereotype != null)
            {
                var name = stereotype.GetProperty<string>("5227f984-a290-4f6a-9f0a-940dcd8a93c6");
                var partitionKey = stereotype.GetProperty<string>("53973836-dc92-4f6b-80e6-17934f84aad3");
                var throughputModeString = stereotype.GetProperty<string>("ace7ecc8-2874-4afc-8e4d-b4cad3c93488");

                tableSettings = new TableSettings
                {
                    Name = name,
                    PartitionKey = partitionKey
                };

                if (string.IsNullOrWhiteSpace(throughputModeString))
                {
                    return true;
                }

                var throughputMode = throughputModeString switch
                {
                    "On-demand" => TableThroughputMode.OnDemand,
                    "Provisioned" => TableThroughputMode.Provisioned,
                    _ => throw new InvalidOperationException($"Unexpected value for throughput mode: {throughputModeString}")
                };
                var maximumReadThroughput = stereotype.GetProperty<int?>("fe066a7d-71ef-487e-8955-922d35a7493b");
                var maximumWriteThroughput = stereotype.GetProperty<int?>("2f4bdce8-b053-47f8-9ee6-79a8f673c782");
                var readThroughput = stereotype.GetProperty<int?>("6e49570f-ce07-4010-8147-582bd2431b5e");
                var writeThroughput = stereotype.GetProperty<int?>("cddbd29b-2c22-45a2-986a-54604f4c7936");

                tableSettings.ThroughputMode = throughputMode;

                switch (throughputMode)
                {
                    case TableThroughputMode.OnDemand:
                        tableSettings.MaximumReadThroughput = maximumReadThroughput;
                        tableSettings.MaximumWriteThroughput = maximumWriteThroughput;
                        break;
                    case TableThroughputMode.Provisioned:
                        tableSettings.ReadThroughput = readThroughput;
                        tableSettings.WriteThroughput = writeThroughput;
                        break;
                    default:
                        throw new InvalidOperationException($"Unexpected value for throughput mode: {throughputMode}");
                }

                return true;
            }

            if (hasStereotypes is not IElement element)
            {
                break;
            }

            hasStereotypes = element.ParentElement ?? (IHasStereotypes)element.Package;
        }

        tableSettings = null;
        return false;
    }
}