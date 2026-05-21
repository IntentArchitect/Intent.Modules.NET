using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modelers.Domain.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;

namespace Intent.Modules.Application.DomainInteractions.Extensions;

internal enum PrimaryKeyDataSourceFilter
{
    Any,
    UserSupplied,
    NonUserSupplied
}

internal static class AttributeModelExtensions
{
    public static bool IsPrimaryKey(
        this AttributeModel attribute,
        PrimaryKeyDataSourceFilter dataSourceFilter = PrimaryKeyDataSourceFilter.Any)
    {
        if (!attribute.HasStereotype("Primary Key"))
        {
            return false;
        }

        if (dataSourceFilter == PrimaryKeyDataSourceFilter.Any)
        {
            return true;
        }

        var isUserSuppliedPrimaryKey =
            attribute.GetStereotype("Primary Key").TryGetProperty("Data source", out var property) &&
            property.Value == "User supplied";

        return dataSourceFilter switch
        {
            PrimaryKeyDataSourceFilter.UserSupplied => isUserSuppliedPrimaryKey,
            PrimaryKeyDataSourceFilter.NonUserSupplied => !isUserSuppliedPrimaryKey,
            _ => throw new ArgumentOutOfRangeException(nameof(dataSourceFilter), dataSourceFilter, null)
        };
    }
    public static bool IsForeignKey(this AttributeModel attribute)
    {
        return attribute.HasStereotype("Foreign Key");
    }

    public static AssociationTargetEndModel? GetForeignKeyAssociation(this AttributeModel attribute)
    {
        return attribute.GetStereotype("Foreign Key")?.GetProperty<IElement>("Association")?.AsAssociationTargetEndModel();
    }

    public static string AsSingleOrTuple(this IEnumerable<CSharpStatement> idFields)
    {
        var enumeratedIdFields = idFields as CSharpStatement[] ?? idFields.ToArray();

        return enumeratedIdFields.Length switch
        {
            <= 0 => throw new Exception("Expected count of at least 1"),
            1 => $"{enumeratedIdFields[0]}",
            > 1 => $"({string.Join(", ", enumeratedIdFields.Select(idField => $"{idField}"))})"
        };
    }
}
