using System.Linq;
using Intent.Exceptions;
using Intent.Metadata.DocumentDB.Api;
using Intent.Modelers.Domain.Api;

namespace Intent.Modules.Aws.DynamoDB.Templates
{
    internal static class AttributeModelExtensionMethods
    {
        public record PrimaryKeyData(AttributeModel PartitionKeyAttribute, AttributeModel? SortKeyAttribute);

        public static PrimaryKeyData? GetPrimaryKeyData(this ClassModel model)
        {
            var partitionKeyAttributeName = model.TryGetTableSettings(out var containerSettings)
                ? containerSettings.PartitionKey
                : null;

            var @class = model;
            while (@class != null)
            {
                var partitionKeyAttributes = @class.Attributes
                    .Where(x => x.HasPrimaryKey() && string.Equals(x.Name, partitionKeyAttributeName, StringComparison.OrdinalIgnoreCase))
                    .ToArray();
                var sortKeyAttributes = @class.Attributes
                    .Where(x => x.HasPrimaryKey() && !string.Equals(x.Name, partitionKeyAttributeName, StringComparison.OrdinalIgnoreCase))
                    .ToArray();

                if (partitionKeyAttributes.Length + sortKeyAttributes.Length == 0)
                {
                    @class = @class.ParentClass;
                    continue;
                }

                if (partitionKeyAttributeName != null)
                {
                    return new PrimaryKeyData(
                        PartitionKeyAttribute: partitionKeyAttributes.Length switch
                        {
                            1 => partitionKeyAttributes[0],
                            _ => throw new ElementException(
                                model.InternalElement,
                                $"Expected 1 primary key attribute with name of \"{partitionKeyAttributeName}\" to use as partition key, " +
                                $"but instead found {partitionKeyAttributes.Length} when searching hierarchy.")
                        },
                        SortKeyAttribute: sortKeyAttributes.Length switch
                        {
                            < 1 => null,
                            1 => sortKeyAttributes[0],
                            > 1 => throw new ElementException(
                                model.InternalElement,
                                $"Expected 0 or 1 non-partition key primary keys attribute to use as sort key " +
                                $"but instead found {sortKeyAttributes.Length} when searching hierarchy.")
                        });
                }

                return new PrimaryKeyData(
                    PartitionKeyAttribute: sortKeyAttributes.Length switch
                    {
                        1 => sortKeyAttributes[0],
                        _ => throw new ElementException(
                            model.InternalElement,
                            $"Expected 1 non-partition key primary key attribute to use as both partition and sort key " +
                            $"but instead found {sortKeyAttributes.Length} when searching hierarchy.")
                    },
                    SortKeyAttribute: null);
            }

            return null;
        }
    }
}
