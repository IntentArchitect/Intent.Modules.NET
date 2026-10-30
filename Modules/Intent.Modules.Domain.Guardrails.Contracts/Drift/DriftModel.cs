using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modules.Domain.Guardrails.Contracts.Templates;

namespace Intent.Modules.Domain.Guardrails.Contracts.Drift
{
    /// <summary>
    /// The Domain designer's database facts, read from the model's own stereotypes (Intent.Metadata.RDBMS and
    /// Intent.Metadata.DocumentDB, by name, so neither is a dependency), and the domain events each constructor or
    /// operation raises, read from Intent.Modelers.Domain.Events' Domain Event Origin associations by type id. Only this
    /// application's packages count.
    /// </summary>
    internal static class DriftModel
    {
        private static readonly HashSet<string> DatabaseStereotypes = new(StringComparer.Ordinal)
        {
            "Table", "View", "Primary Key", "Foreign Key", "Column", "Text Constraints", "Decimal Constraints",
            "Default Constraint", "Computed Value", "Index", "Field Setting"
        };

        public static IReadOnlyList<ModelFact> Read(IDesigner designer, string applicationId)
        {
            var packages = designer.Packages.Where(x => x.ApplicationId == applicationId).ToList();
            var classes = packages.SelectMany(x => Descendants(x.ChildElements)).Where(x => x.SpecializationType == "Class").ToList();
            var persisted = classes.Where(IsPersisted).ToList();
            var facts = new List<ModelFact>();

            foreach (var @class in persisted)
            {
                var placeholders = new Dictionary<string, string> { ["Entity"] = @class.Name };
                var table = Stereotype(@class, "Table") ?? Stereotype(@class, "View");
                if (table != null)
                {
                    facts.Add(Fact(FactKinds.TableName, @class.Name, Property(table, "Name"), placeholders, @class.Id));
                    facts.Add(Fact(FactKinds.TableSchema, @class.Name, Property(table, "Schema") is { Length: > 0 } schema ? schema : InheritedSchema(@class), placeholders, @class.Id));
                    facts.Add(Fact(FactKinds.TableKind, @class.Name, table.Name, placeholders, @class.Id));
                }

                var attributes = @class.ChildElements.Where(x => x.SpecializationType == "Attribute").ToList();
                var keys = attributes.Where(x => Stereotype(x, "Primary Key") != null).Select(x => x.Name);
                facts.Add(Fact(FactKinds.Key, @class.Name, string.Join(",", keys), placeholders, @class.Id));

                foreach (var attribute in attributes)
                {
                    var element = $"{@class.Name}.{attribute.Name}";
                    var attributePlaceholders = new Dictionary<string, string> { ["Entity"] = @class.Name, ["Attribute"] = attribute.Name };
                    var typeName = attribute.TypeReference?.Element?.Name;
                    var isKey = Stereotype(attribute, "Primary Key") != null;
                    var column = Stereotype(attribute, "Column");
                    var columnName = Property(column, "Name") is { Length: > 0 } name ? name : Property(Stereotype(attribute, "Field Setting"), "Name");
                    facts.Add(Fact(FactKinds.ColumnName, element, columnName, attributePlaceholders, @class.Id, attribute.Id));
                    facts.Add(Fact(FactKinds.ColumnType, element, Property(column, "Type"), attributePlaceholders, @class.Id, attribute.Id));
                    facts.Add(Fact(FactKinds.ColumnDefault, element, Property(Stereotype(attribute, "Default Constraint"), "Value"), attributePlaceholders, @class.Id, attribute.Id));
                    if (typeName == "string")
                    {
                        var text = Stereotype(attribute, "Text Constraints");
                        facts.Add(Fact(FactKinds.ColumnTextType, element, Property(text, "SQL Data Type") is { Length: > 0 } sqlType ? sqlType : "DEFAULT", attributePlaceholders, @class.Id, attribute.Id));
                        facts.Add(Fact(FactKinds.ColumnMaxLength, element, Property(text, "MaxLength"), attributePlaceholders, @class.Id, attribute.Id));
                    }

                    if (typeName is "string" or "binary" && !isKey)
                    {
                        facts.Add(Fact(FactKinds.ColumnRequired, element, attribute.TypeReference.IsNullable ? "false" : "true", attributePlaceholders, @class.Id, attribute.Id));
                    }

                    if (typeName == "decimal")
                    {
                        var @decimal = Stereotype(attribute, "Decimal Constraints");
                        facts.Add(Fact(FactKinds.ColumnPrecision, element, Property(@decimal, "Precision"), attributePlaceholders, @class.Id, attribute.Id));
                        facts.Add(Fact(FactKinds.ColumnScale, element, Property(@decimal, "Scale"), attributePlaceholders, @class.Id, attribute.Id));
                    }
                }

                foreach (var (name, columns, unique) in Indexes(@class, attributes))
                {
                    var indexPlaceholders = new Dictionary<string, string> { ["Entity"] = @class.Name, ["Index"] = name };
                    facts.Add(Fact(FactKinds.IndexColumns, $"{@class.Name}.{name}", string.Join(",", columns), indexPlaceholders, @class.Id));
                    facts.Add(Fact(FactKinds.IndexUnique, $"{@class.Name}.{name}", unique ? "true" : "false", indexPlaceholders, @class.Id));
                }
            }

            var classIds = new HashSet<string>(classes.Select(x => x.Id), StringComparer.Ordinal);
            var persistedIds = new HashSet<string>(persisted.Select(x => x.Id), StringComparer.Ordinal);
            var foreignKeys = persisted
                .SelectMany(x => x.ChildElements.Where(a => a.SpecializationType == "Attribute"))
                .Select(x => (Attribute: x, Association: ElementId(Stereotype(x, "Foreign Key"), "Association")))
                .Where(x => x.Association != null)
                .ToList();
            var associations = designer.Associations
                .Where(x => x.SourceEnd?.TypeReference?.Element != null && x.TargetEnd?.TypeReference?.Element != null)
                .Where(x => classIds.Contains(x.SourceEnd.TypeReference.Element.Id) && classIds.Contains(x.TargetEnd.TypeReference.Element.Id))
                .Where(x => IsDatabaseRelationship(x, persistedIds))
                .OrderBy(x => x.Id, StringComparer.Ordinal);
            foreach (var association in associations)
            {
                // The target end is typed as the target class and named as its navigation on the source class.
                var source = association.SourceEnd.TypeReference.Element.Name;
                var target = association.TargetEnd.TypeReference.Element.Name;
                var placeholders = new Dictionary<string, string>
                {
                    ["Source"] = source,
                    ["Target"] = target,
                    ["SourceEnd"] = association.SourceEnd.Name ?? string.Empty,
                    ["TargetEnd"] = association.TargetEnd.Name ?? string.Empty
                };
                var element = !string.IsNullOrEmpty(association.TargetEnd.Name)
                    ? $"{source}.{association.TargetEnd.Name}"
                    : !string.IsNullOrEmpty(association.SourceEnd.Name) ? $"{target}.{association.SourceEnd.Name}" : $"{source} → {target}";
                facts.Add(Fact(FactKinds.RelationshipMultiplicity, element, $"{Multiplicity(association.SourceEnd)}->{Multiplicity(association.TargetEnd)}", placeholders, association.SourceEnd.TypeReference.Element.Id));

                var ids = new[] { association.Id, association.SourceEnd.Id, association.TargetEnd.Id };
                var keys = foreignKeys.Where(x => ids.Contains(x.Association, StringComparer.Ordinal)).Select(x => x.Attribute.Name);
                facts.Add(Fact(FactKinds.RelationshipForeignKey, element, string.Join(",", keys), placeholders, association.SourceEnd.TypeReference.Element.Id));
            }

            foreach (var @class in classes)
            {
                AddRaisedEvents(@class, facts);
            }

            return facts;
        }

        /// <summary>
        /// One <c>intent.raises</c> fact per Domain Event Origin from the class's constructors and operations. A constructor's
        /// member is the class's name, as in C#; the parameter names choose between overloads.
        /// </summary>
        private static void AddRaisedEvents(IElement @class, List<ModelFact> facts)
        {
            foreach (var member in @class.ChildElements.Where(x => x.SpecializationType is "Class Constructor" or "Operation"))
            {
                var name = member.SpecializationType == "Class Constructor" ? @class.Name : member.Name;
                var parameterNames = member.ChildElements.Where(x => x.SpecializationType == "Parameter").Select(x => x.Name).ToList();
                var origins = (member.AssociatedElements ?? Enumerable.Empty<IAssociationEnd>())
                    .Where(x => string.Equals(x.Association?.SpecializationTypeId, DomainEventsMetadata.OriginAssociationTypeId, StringComparison.OrdinalIgnoreCase) && x.IsTargetEnd())
                    .OrderBy(x => x.Association?.Id ?? x.Id, StringComparer.Ordinal);
                foreach (var origin in origins)
                {
                    var domainEvent = origin.TypeReference?.Element?.Name;
                    if (string.IsNullOrEmpty(domainEvent))
                    {
                        continue;
                    }

                    var placeholders = new Dictionary<string, string> { ["Entity"] = @class.Name, ["Member"] = name, ["DomainEvent"] = domainEvent };
                    facts.Add(new ModelFact
                    {
                        Kind = FactKinds.IntentRaises,
                        Element = $"{@class.Name}.{name}",
                        ElementIds = new[] { @class.Id, member.Id },
                        Value = domainEvent,
                        Placeholders = placeholders,
                        ParameterNames = parameterNames
                    });
                }
            }
        }

        /// <summary>
        /// A persisted class has <c>Table</c> or <c>View</c>, or a database stereotype on itself or one of its
        /// attributes.
        /// </summary>
        private static bool IsPersisted(IElement @class)
        {
            return HasDatabaseStereotype(@class) ||
                @class.ChildElements.Where(x => x.SpecializationType == "Attribute").Any(HasDatabaseStereotype);
        }

        /// <summary>
        /// A relationship is in the database when both classes are persisted, or when a persisted class owns the other
        /// (a composition to a class with no table of its own). A class nothing maps, such as a scaffolded entity no
        /// DbContext uses, has no relationship in the database, even to a mapped one.
        /// </summary>
        private static bool IsDatabaseRelationship(IAssociation association, HashSet<string> persistedIds)
        {
            var owned = !association.SourceEnd.TypeReference.IsNullable && !association.SourceEnd.TypeReference.IsCollection;
            return persistedIds.Contains(association.SourceEnd.TypeReference.Element.Id) &&
                (persistedIds.Contains(association.TargetEnd.TypeReference.Element.Id) || owned);
        }

        private static bool HasDatabaseStereotype(IHasStereotypes element) => element.Stereotypes.Any(x => DatabaseStereotypes.Contains(x.Name));

        /// <summary>
        /// The class's named indexes, in both of Intent.Metadata.RDBMS's forms: <c>Index</c> on attributes, one index per
        /// <c>UniqueKey</c> with its attributes ordered by <c>Order</c> and the first one's <c>IsUnique</c>; and <c>Index</c>
        /// elements with <c>Use Default Name</c> off, with their <c>Key</c> columns. An attribute can have several
        /// <c>Index</c> stereotypes, one per index it is in, and every one counts; RDBMS's own <c>GetIndexes</c> reads only
        /// the first. An index with no name isn't stated: nothing could tell it apart in the code.
        /// </summary>
        private static IEnumerable<(string Name, IReadOnlyList<string> Columns, bool Unique)> Indexes(IElement @class, IReadOnlyList<IElement> attributes)
        {
            var byKey = attributes
                .SelectMany(attribute => attribute.Stereotypes.Where(x => x.Name == "Index").Select(index => (Attribute: attribute, Index: index)))
                .Where(x => Property(x.Index, "UniqueKey").Length > 0)
                .GroupBy(x => Property(x.Index, "UniqueKey"), StringComparer.Ordinal);
            foreach (var index in byKey)
            {
                var columns = index
                    .OrderBy(x => int.TryParse(Property(x.Index, "Order"), out var order) ? order : 0)
                    .Select(x => x.Attribute.Name)
                    .ToList();
                yield return (index.Key, columns, IsTrue(Property(index.First().Index, "IsUnique")));
            }

            foreach (var index in @class.ChildElements.Where(x => x.SpecializationType == "Index" && !string.IsNullOrWhiteSpace(x.Name)))
            {
                var settings = Stereotype(index, "Settings");
                if (IsTrue(Property(settings, "Use Default Name")))
                {
                    continue;
                }

                var columns = index.ChildElements
                    .Where(x => x.SpecializationType == "Index Column" && Property(Stereotype(x, "Settings"), "Type") is "" or "Key")
                    .Select(x => x.MappedElement?.Element?.Name is { Length: > 0 } mapped ? mapped : x.Name)
                    .ToList();
                yield return (index.Name.Trim(), columns, IsTrue(Property(settings, "Unique")));
            }
        }

        private static bool IsTrue(string value) => string.Equals(value, "true", StringComparison.OrdinalIgnoreCase);

        private static string InheritedSchema(IElement element)
        {
            for (var parent = element.ParentElement; parent != null; parent = parent.ParentElement)
            {
                if (Property(Stereotype(parent, "Schema"), "Name") is { Length: > 0 } schema)
                {
                    return schema;
                }
            }

            return Property(Stereotype(element.Package, "Schema"), "Name");
        }

        private static string Multiplicity(IAssociationEnd end)
        {
            return end.TypeReference.IsCollection ? "*" : end.TypeReference.IsNullable ? "0..1" : "1";
        }

        private static ModelFact Fact(FactKind kind, string element, string value, IReadOnlyDictionary<string, string> placeholders, params string[] elementIds)
        {
            return new ModelFact { Kind = kind, Element = element, ElementIds = elementIds.Where(x => x != null).ToList(), Value = value?.Trim() ?? string.Empty, Placeholders = placeholders };
        }

        /// <summary>The application's elements in this designer, by id, for the errors that open them.</summary>
        public static IReadOnlyDictionary<string, IElement> ElementsById(IDesigner designer, string applicationId)
        {
            var result = new Dictionary<string, IElement>(StringComparer.Ordinal);
            foreach (var element in designer.Packages.Where(x => x.ApplicationId == applicationId).SelectMany(x => Descendants(x.ChildElements)))
            {
                result.TryAdd(element.Id, element);
            }

            return result;
        }

        private static IEnumerable<IElement> Descendants(IEnumerable<IElement> elements)
        {
            foreach (var element in elements ?? Enumerable.Empty<IElement>())
            {
                yield return element;
                foreach (var descendant in Descendants(element.ChildElements))
                {
                    yield return descendant;
                }
            }
        }

        private static IStereotype Stereotype(IHasStereotypes element, string name)
        {
            return element?.Stereotypes.FirstOrDefault(x => x.Name == name);
        }

        private static string Property(IStereotype stereotype, string name)
        {
            return stereotype != null && stereotype.TryGetProperty(name, out var property) ? property.Value?.Trim() ?? string.Empty : string.Empty;
        }

        /// <summary>The id an element-typed stereotype property points at.</summary>
        private static string ElementId(IStereotype stereotype, string name)
        {
            if (stereotype == null || !stereotype.TryGetProperty(name, out var property))
            {
                return null;
            }

            return property is IStereotypeProperty<IElement> typed && typed.Value != null ? typed.Value.Id : property.Value?.Trim();
        }
    }
}
