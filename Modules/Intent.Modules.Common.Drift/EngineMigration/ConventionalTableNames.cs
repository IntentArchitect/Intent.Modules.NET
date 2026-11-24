using System;
using System.Collections.Generic;
using System.Linq;
using Intent.IArchitect.Agent.Persistence.Model;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Intent.Modules.Common.Drift.EngineMigration
{
    /// <summary>
    /// The modelled Table Names the code states only by EF Core's convention (user decision, 2026-11-24: the migration clears
    /// them). The drift engine reads a Table Name only from a <c>ToTable</c> (or <c>ToView</c>) or a <c>[Table]</c>, because
    /// a name that follows the <c>DbSet</c> property or the class isn't modelled (user decision, 2026-11-14). v1's recipes
    /// could read the <c>DbSet</c>'s name, so a v1 model may state one, and every such name would be contradicted on the first
    /// run. A name is cleared only when the code has no table statement for the entity and the name is the one EF Core uses
    /// anyway: a <c>DbSet&lt;Entity&gt;</c> property's, or the class's. Any other name stays, and the engine reports it.
    /// </summary>
    internal static class ConventionalTableNames
    {
        private static readonly HashSet<string> TableCalls = new(StringComparer.Ordinal) { "ToTable", "ToView" };

        /// <param name="modelled">Each Class with a Table Name: its name, as the code names the entity, and the Table Name.</param>
        /// <param name="notes">Gets a line when a table statement's entity can't be told, which leaves every name as it is.</param>
        /// <returns>The classes whose Table Name is conventional.</returns>
        public static IReadOnlyList<string> Find(CodeFiles code, IReadOnlyList<(string Entity, string Table)> modelled, ICollection<string> notes)
        {
            if (modelled.Count == 0)
            {
                return Array.Empty<string>();
            }

            var stated = new HashSet<string>(StringComparer.Ordinal);
            var dbSets = new Dictionary<string, HashSet<string>>(StringComparer.Ordinal);
            foreach (var syntax in code.AllFiles().Where(CodeFiles.IsCSharp).Select(x => code.Get(x)?.Syntax).Where(x => x != null))
            {
                foreach (var type in syntax.DescendantNodes().OfType<ClassDeclarationSyntax>()
                             .Where(x => x.AttributeLists.SelectMany(a => a.Attributes).Any(a => LastName(a.Name) is "Table" or "TableAttribute")))
                {
                    stated.Add(type.Identifier.Text);
                }

                foreach (var property in syntax.DescendantNodes().OfType<PropertyDeclarationSyntax>())
                {
                    if (property.Type is GenericNameSyntax { Identifier.Text: "DbSet", TypeArgumentList.Arguments: [var entity] })
                    {
                        var name = LastName(entity);
                        (dbSets.TryGetValue(name, out var names) ? names : dbSets[name] = new HashSet<string>(StringComparer.Ordinal)).Add(property.Identifier.Text);
                    }
                }

                foreach (var call in syntax.DescendantNodes().OfType<InvocationExpressionSyntax>().Where(x => TableCalls.Contains(CalledName(x) ?? "")))
                {
                    if (EntityOf(call) is not { } entity)
                    {
                        notes.Add($"`{call}` names a table for an entity the migration can't tell, so no Table Name was cleared.");
                        return Array.Empty<string>();
                    }

                    stated.Add(entity);
                }
            }

            return modelled
                .Where(x => !stated.Contains(x.Entity) && (x.Table == x.Entity || dbSets.TryGetValue(x.Entity, out var names) && names.Contains(x.Table)))
                .Select(x => x.Entity)
                .ToList();
        }

        /// <summary>
        /// The entity a table statement configures: <c>Entity&lt;T&gt;()</c> in its receiver chain, an enclosing
        /// <c>Entity&lt;T&gt;(e =&gt; ...)</c>, an enclosing method's <c>EntityTypeBuilder&lt;T&gt;</c> parameter, or the
        /// enclosing class's <c>IEntityTypeConfiguration&lt;T&gt;</c>. A table an owned type maps to counts as its owner's,
        /// which only ever keeps a name.
        /// </summary>
        private static string EntityOf(InvocationExpressionSyntax call)
        {
            for (var receiver = (call.Expression as MemberAccessExpressionSyntax)?.Expression; receiver != null;)
            {
                switch (receiver)
                {
                    case InvocationExpressionSyntax invocation when Entity(invocation) is { } chained:
                        return chained;
                    case InvocationExpressionSyntax invocation:
                        receiver = (invocation.Expression as MemberAccessExpressionSyntax)?.Expression;
                        break;
                    case MemberAccessExpressionSyntax access:
                        receiver = access.Expression;
                        break;
                    default:
                        receiver = null;
                        break;
                }
            }

            foreach (var ancestor in call.Ancestors())
            {
                switch (ancestor)
                {
                    case InvocationExpressionSyntax invocation when Entity(invocation) is { } enclosing:
                        return enclosing;
                    case MethodDeclarationSyntax method when method.ParameterList.Parameters
                        .Select(x => x.Type).OfType<GenericNameSyntax>().FirstOrDefault(x => x.Identifier.Text == "EntityTypeBuilder") is { TypeArgumentList.Arguments: [var builder] }:
                        return LastName(builder);
                    case ClassDeclarationSyntax type when type.BaseList?.Types.Select(x => x.Type is QualifiedNameSyntax q ? q.Right : x.Type)
                        .OfType<GenericNameSyntax>().FirstOrDefault(x => x.Identifier.Text == "IEntityTypeConfiguration") is { TypeArgumentList.Arguments: [var configured] }:
                        return LastName(configured);
                }
            }

            return null;
        }

        /// <summary><c>T</c> of an <c>Entity&lt;T&gt;(...)</c> call, however it's called on.</summary>
        private static string Entity(InvocationExpressionSyntax invocation) =>
            invocation.Expression switch
            {
                MemberAccessExpressionSyntax { Name: GenericNameSyntax { Identifier.Text: "Entity", TypeArgumentList.Arguments: [var type] } } => LastName(type),
                GenericNameSyntax { Identifier.Text: "Entity", TypeArgumentList.Arguments: [var type] } => LastName(type),
                _ => null
            };

        private static string CalledName(InvocationExpressionSyntax invocation) => invocation.Expression switch
        {
            MemberAccessExpressionSyntax access => access.Name.Identifier.Text,
            SimpleNameSyntax name => name.Identifier.Text,
            _ => null
        };

        private static string LastName(TypeSyntax type) => type switch
        {
            QualifiedNameSyntax qualified => qualified.Right.Identifier.Text,
            AliasQualifiedNameSyntax alias => alias.Name.Identifier.Text,
            SimpleNameSyntax simple => simple.Identifier.Text,
            _ => type.ToString()
        };

        /// <summary>
        /// Clears each Class's conventional Table Name (<see cref="Find"/>) on the packages this application owns, and logs it.
        /// Its Schema, and the Table stereotype, stay.
        /// </summary>
        public static void Clear(ApplicationPersistable application, CodeFiles code, ICollection<string> log)
        {
            var named = application.GetDesigners()
                .SelectMany(x => GuardrailsModelCleanup.Owned(application, x))
                .SelectMany(x => x.FindAllChildElements().OfType<ElementPersistable>())
                .Where(x => x.SpecializationType == "Class")
                .Select(x => (Element: x, Name: x.Stereotypes?.FirstOrDefault(s => s.DefinitionId == V1Footprint.TableStereotype.DefinitionId)
                    ?.Properties.FirstOrDefault(p => p.DefinitionId == V1Footprint.TableStereotype.Name)))
                .Where(x => !string.IsNullOrWhiteSpace(x.Name?.Value))
                .ToList();
            var conventional = new HashSet<string>(Find(code, named.Select(x => (x.Element.Name, x.Name.Value)).ToList(), log), StringComparer.Ordinal);
            foreach (var (element, name) in named.Where(x => conventional.Contains(x.Element.Name)))
            {
                log.Add($"Cleared {element.Name}'s Table Name `{name.Value}`: the code names that table only as EF Core's convention does, and the drift engine reads a Table Name only from a ToTable or [Table].");
                name.Value = string.Empty;
            }
        }
    }
}
