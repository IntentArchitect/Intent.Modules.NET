using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.RDBMS.Api;
using Intent.Modelers.Domain.Api;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.Templates;
using Intent.Utils;

namespace Intent.Modules.EntityFrameworkCore.Templates.EntityTypeConfiguration;

public class EfCoreAssociationConfigStatement : CSharpStatement
{
    private readonly AssociationEndModel _associationEnd;
    protected IList<CSharpStatement> RelationshipStatements { get; } = new List<CSharpStatement>();
    protected IList<CSharpStatement> AdditionalStatements { get; } = new List<CSharpStatement>();
    public RequiredEntityProperty[] RequiredProperties = Array.Empty<RequiredEntityProperty>();

    public static EfCoreAssociationConfigStatement CreateOwnsOne(AssociationEndModel associationEnd)
    {
        var statement = new EfCoreAssociationConfigStatement(associationEnd);
        statement.RelationshipStatements.Add($"builder.OwnsOne(x => x.{associationEnd.Name.ToPascalCase()}, Configure{associationEnd.Name.ToPascalCase()})");
        if (!associationEnd.TypeReference.IsNullable)
        {
            statement.AdditionalStatements.Add($".Navigation(x => x.{associationEnd.Name.ToPascalCase()}).IsRequired()");
        }

        return statement;
    }

    public EfCoreAssociationConfigStatement CreateWithOwner()
    {
        var statement = new EfCoreAssociationConfigStatement(_associationEnd);
        statement.RelationshipStatements.Add(@$"builder.WithOwner({(_associationEnd.OtherEnd().IsNavigable ? $"x => x.{_associationEnd.OtherEnd().Name.ToPascalCase()}" : "")})");
        return statement;
    }

    public static EfCoreAssociationConfigStatement CreateOwnsMany(AssociationEndModel associationEnd)
    {
        var statement = new EfCoreAssociationConfigStatement(associationEnd);
        statement.RelationshipStatements.Add($"builder.OwnsMany(x => x.{associationEnd.Name.ToPascalCase()}, Configure{associationEnd.Name.ToPascalCase()})");
        return statement;
    }

    public static EfCoreAssociationConfigStatement CreateHasOne(AssociationEndModel associationEnd)
    {
        var statement = new EfCoreAssociationConfigStatement(associationEnd);
        statement.RelationshipStatements.Add($"builder.HasOne(x => x.{associationEnd.Name.ToPascalCase()})");

        if (associationEnd.OtherEnd().IsCollection)
        {
            statement.RelationshipStatements.Add($".WithMany({(associationEnd.OtherEnd().IsNavigable ? "x => x." + associationEnd.OtherEnd().Name.ToPascalCase() : "")})");
            statement.AdditionalStatements.Add($".OnDelete(DeleteBehavior.Restrict)");
        }
        else
        {
            statement.RelationshipStatements.Add($".WithOne({(associationEnd.OtherEnd().IsNavigable ? $"x => x.{associationEnd.OtherEnd().Name.ToPascalCase()}" : "")})");
            if (!associationEnd.OtherEnd().IsNullable)
            {
                statement.AdditionalStatements.Add($".IsRequired()");
                statement.AdditionalStatements.Add($".OnDelete(DeleteBehavior.Cascade)");
            }
            else
            {
                statement.AdditionalStatements.Add($".OnDelete(DeleteBehavior.Restrict)");
            }
        }

        return statement;
    }

    public static EfCoreAssociationConfigStatement CreateHasMany(AssociationEndModel associationEnd)
    {
        var statement = new EfCoreAssociationConfigStatement(associationEnd);
        statement.RelationshipStatements.Add($"builder.HasMany(x => x.{associationEnd.Name.ToPascalCase()})");
        if (associationEnd.OtherEnd().IsCollection)
        {
            statement.RelationshipStatements.Add($".WithMany({(associationEnd.OtherEnd().IsNavigable ? $"x => x.{associationEnd.OtherEnd().Name.ToPascalCase()}" : $"\"{associationEnd.OtherEnd().Name.ToPascalCase()}\"")})");
            statement.RelationshipStatements.Add($".UsingEntity(x => x.ToTable(\"{associationEnd.OtherEnd().Class.Name}{associationEnd.Class.Name.ToPluralName()}\"))");
            statement.RequiredProperties = new[]
            {
                new RequiredEntityProperty(
                    Class: associationEnd.Element,
                    Name: associationEnd.OtherEnd().Name.ToPascalCase(),
                    Type: associationEnd.OtherEnd().Class.InternalElement,
                    IsNullable: false,
                    IsCollection: true,
                    ConfigureProperty: property =>
                    {
                        property.Protected().Virtual();
                    })
            };
        }
        else
        {
            statement.RelationshipStatements.Add($".WithOne({(associationEnd.OtherEnd().IsNavigable ? $"x => x.{associationEnd.OtherEnd().Name.ToPascalCase()}" : $"")})");
            if (!associationEnd.OtherEnd().IsNullable)
            {
                statement.AdditionalStatements.Add($".IsRequired()");
                statement.AdditionalStatements.Add($".OnDelete(DeleteBehavior.Cascade)");
            }
        }


        return statement;
    }

    private EfCoreAssociationConfigStatement(AssociationEndModel associationEnd) : base(null)
    {
        _associationEnd = associationEnd;

        if (associationEnd.Element.Id.Equals(associationEnd.OtherEnd().Element.Id)
            && associationEnd.Name.Equals(associationEnd.Element.Name))
        {
            Logging.Log.Warning($"Self referencing relationship detected using the same name for the Association as the Class: {associationEnd.Class.Name}. This might cause problems.");
        }

        AddMetadata("model", associationEnd);
    }

    public EfCoreAssociationConfigStatement WithForeignKey(bool enabled = true)
    {
        if (!enabled)
        {
            return this;
        }
        switch (_associationEnd.Association.GetRelationshipType())
        {
            case RelationshipType.OneToMany:
                return AddForeignKey(GetForeignColumns(_associationEnd));
            case RelationshipType.OneToOne:
                if (_associationEnd.OtherEnd().IsNullable)
                {
                    return AddForeignKey(GetForeignColumns(_associationEnd.OtherEnd()));
                }
                return AddForeignKey(GetForeignColumns(_associationEnd));
            case RelationshipType.ManyToOne:
                return AddForeignKey(GetForeignColumns(_associationEnd.OtherEnd()));
            case RelationshipType.ManyToMany:
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    public EfCoreAssociationConfigStatement AddStatement(CSharpStatement statement)
    {
        AdditionalStatements.Add(statement);
        return this;
    }

    public EfCoreAssociationConfigStatement AddStatements(IEnumerable<CSharpStatement> statements)
    {
        foreach (var statement in statements)
        {
            AdditionalStatements.Add(statement);
        }
        return this;
    }

    private EfCoreAssociationConfigStatement AddForeignKey(params RequiredEntityProperty[] columns)
    {
        RequiredProperties = columns;

        string genericType = null;
        if (!RelationshipStatements.First().GetText(string.Empty).StartsWith("builder.WithOwner") &&
            _associationEnd.Association.GetRelationshipType() == RelationshipType.OneToOne)
        {
            if (_associationEnd.IsNullable)
            {
                if (_associationEnd.OtherEnd().IsNullable)
                {
                    genericType = _associationEnd.OtherEnd().Class.Name;
                }
                else
                {
                    genericType = _associationEnd.Class.Name;
                }
            }
            else
            {
                genericType = _associationEnd.OtherEnd().Class.Name;
            }
        }

        if (columns?.Length == 1)
        {
            RelationshipStatements.Add($".HasForeignKey{(genericType != null ? $"<{genericType}>" : string.Empty)}(x => x.{columns.Single().Name})");
            return this;
        }

        RelationshipStatements.Add($".HasForeignKey{(genericType != null ? $"<{genericType}>" : string.Empty)}(x => new {{ {string.Join(", ", columns.Select(x => "x." + x.Name))}}})");
        return this;
    }

    private RequiredEntityProperty[] GetForeignColumns(AssociationEndModel associationEnd)
    {

        if (associationEnd.OtherEnd().Class.GetExplicitPrimaryKey().Any())
        {
            if (!associationEnd.Association.IsOneToOne() || associationEnd.OtherEnd().IsNullable)
            {
                return associationEnd.OtherEnd().Class.GetExplicitPrimaryKey()
                    .Select(selector: x => new RequiredEntityProperty(
                        Class: associationEnd.Element,
                        Name: $"{associationEnd.OtherEnd().Name.ToPascalCase()}{x.Name.ToPascalCase()}",
                        Type: x.Type.Element,
                        IsNullable: associationEnd.IsNullable))
                    .ToArray();
            }
            // compositional one-to-ones:
            return associationEnd.OtherEnd().Class.GetExplicitPrimaryKey()
                .Select(selector: x => new RequiredEntityProperty(
                    Class: associationEnd.Element,
                    Name: $"{x.Name.ToPascalCase()}",
                    Type: x.Type.Element,
                    IsNullable: associationEnd.OtherEnd().IsNullable))
                .ToArray();
        }
        else // implicit Id
        {
            if (!associationEnd.Association.IsOneToOne() || associationEnd.OtherEnd().IsNullable)
            {
                return new[] { new RequiredEntityProperty(
                    Class: associationEnd.Element,
                    Name: $"{associationEnd.OtherEnd().Name.ToPascalCase()}Id",
                    Type: null,
                    IsNullable: associationEnd.OtherEnd().IsNullable) };
            }
            // compositional one-to-ones:
            return new[] { new RequiredEntityProperty(
                Class: associationEnd.Element,
                Name: "Id",
                Type: null,
                IsNullable: false) };
        }
    }

    public override string GetText(string indentation)
    {
        var x = $@"{indentation}{string.Join(@$"
{indentation}    ", RelationshipStatements.Select(x => x.GetText(string.Empty)))}{(AdditionalStatements.Any() ? $@"
{indentation}    {string.Join(@$"
{indentation}    ", AdditionalStatements.Select(x => x.GetText(string.Empty)))}" : string.Empty)};";
        return x;
    }

}