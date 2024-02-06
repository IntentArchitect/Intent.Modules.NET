using System;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modelers.Domain.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.Templates;

namespace Intent.Modules.Redis.Om.Repositories.Templates;

internal static class ClassModelExtensionMethods
{
    public static bool TryGetPartitionKeySettings(this ClassModel classModel, out string partitionKey)
    {
        IHasStereotypes hasStereotypes = classModel.InternalElement;

        while (hasStereotypes != null)
        {
            if (hasStereotypes.HasStereotype("Container"))
            {
                partitionKey = hasStereotypes.GetStereotypeProperty<string>("Container", "Partition Key")?.Trim();

                if (string.IsNullOrWhiteSpace(partitionKey))
                {
                    partitionKey = default;
                }

                return true;
            }

            if (hasStereotypes is not IElement element)
            {
                break;
            }

            hasStereotypes = element.ParentElement ?? (IHasStereotypes)element.Package;
        }

        partitionKey = default;
        return false;
    }

    public static bool TryGetPartitionAttribute(this ClassModel model, out AttributeModel partitionAttribute)
    {
        partitionAttribute = null;
        model.TryGetPartitionKeySettings(out var partitionKey);
        if (string.IsNullOrEmpty(partitionKey)) return false;

        var @class = model;
        while (@class != null)
        {
            partitionAttribute = @class.Attributes.FirstOrDefault(x => x.Name.Equals(partitionKey, StringComparison.OrdinalIgnoreCase));
            if (partitionAttribute != null)
            {
                return true;
            }
            @class = @class.ParentClass;
        }
        return false;
    }

    public static bool TryGetContainerSettings(this ClassModel classModel, out string name, out string partitionKey)
    {
        IHasStereotypes hasStereotypes = classModel.InternalElement;

        while (hasStereotypes != null)
        {
            if (hasStereotypes.HasStereotype("Container"))
            {
                name = hasStereotypes.GetStereotypeProperty<string>("Container", "Name")?.Trim();
                partitionKey = hasStereotypes.GetStereotypeProperty<string>("Container", "Partition Key")?.Trim();

                if (string.IsNullOrWhiteSpace(partitionKey) ||
                    "id".Equals(partitionKey.Trim('/'), StringComparison.OrdinalIgnoreCase))
                {
                    partitionKey = default;
                }
                else
                {
                    partitionKey = partitionKey.TrimStart('/').ToCamelCase();
                }

                return true;
            }

            if (hasStereotypes is not IElement element)
            {
                break;
            }

            hasStereotypes = element.ParentElement ?? (IHasStereotypes)element.Package;
        }

        name = default;
        partitionKey = default;
        return false;
    }
}