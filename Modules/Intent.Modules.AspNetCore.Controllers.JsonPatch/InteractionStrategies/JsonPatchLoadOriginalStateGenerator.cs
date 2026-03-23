using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modules.Common.CSharp.Builder;

namespace Intent.Modules.AspNetCore.Controllers.JsonPatch.InteractionStrategies;

/// <summary>
/// Generates reverse-mapped statements for JSON PATCH LoadOriginalState methods.
/// Produces entity-to-command assignments and collection projections in original mapping order.
/// </summary>
internal class JsonPatchLoadOriginalStateGenerator
{
    private const string OperationSpecialization = "Operation";
    private const string ConstructorSpecialization = "Constructor";
    private const string ReverseCommentPrefix = "[JsonPatch Reverse Map]";
    private const string UnsupportedExpressionReason = "unsupported mapping expression";
    private const string UnsupportedOperationReason = "operation mappings are not supported";
    private const string UnsupportedConstructorReason = "constructor mappings are not supported";
    private const string UnsupportedCollectionShapeReason = "unsupported collection mapping shape";

    private readonly ICSharpClassMethodDeclaration _handleMethod;
    private readonly IElementToElementMapping _updateMapping;
    private readonly Dictionary<string, CollectionGroup> _collectionGroups;

    public JsonPatchLoadOriginalStateGenerator(
        ICSharpClassMethodDeclaration handleMethod,
        IElementToElementMapping updateMapping)
    {
        _handleMethod = handleMethod;
        _updateMapping = updateMapping;
        _collectionGroups = new Dictionary<string, CollectionGroup>();
    }

    public IReadOnlyList<CSharpStatement> Generate()
    {
        var reverseMapping = ReverseMappingAdapter.Create(_updateMapping);
        var statements = new List<CSharpStatement>();
            
        // Process in original mapping order to preserve the sequence of assignments
        var mappedEnds = reverseMapping.MappedEnds.ToList();

        BuildCollectionGroups(mappedEnds);

        var processedCollections = new HashSet<string>();

        foreach (var mappedEnd in mappedEnds)
        {
            var collectionKey = GetCollectionGroupKey(mappedEnd);
            if (collectionKey != null && !processedCollections.Contains(collectionKey))
            {
                var collectionStatement = TryCreateCollectionProjectionStatement(_collectionGroups[collectionKey]);
                if (collectionStatement != null)
                {
                    statements.Add(collectionStatement.WithSemicolon());
                }
                else
                {
                    var group = _collectionGroups[collectionKey];
                    statements.Add(new CSharpStatement($"// {ReverseCommentPrefix} Skipped '{group.TargetCollectionPathText}': {UnsupportedCollectionShapeReason}."));
                }

                processedCollections.Add(collectionKey);
                continue;
            }

            if (collectionKey != null)
            {
                continue;
            }

            if (TryGetUnsupportedReason(mappedEnd, out var reason))
            {
                statements.Add(new CSharpStatement($"// {ReverseCommentPrefix} Skipped '{GetPathText(mappedEnd.TargetPath, "command")}': {reason}."));
                continue;
            }

            var targetPath = GetPathText(mappedEnd.TargetPath, "command");
            var sourcePath = GetPathText(mappedEnd.SourcePath, "entity");
            statements.Add(new CSharpAssignmentStatement(targetPath, sourcePath));
        }

        return statements;
    }

    private void BuildCollectionGroups(IEnumerable<IElementToElementMappedEnd> mappedEnds)
    {
        foreach (var mappedEnd in mappedEnds)
        {
            if (!TryGetCollectionPathInfo(mappedEnd, out var targetCollectionIndex, out var sourceCollectionIndex))
            {
                continue;
            }

            var targetCollectionPath = mappedEnd.TargetPath.Take(targetCollectionIndex + 1).ToList();
            var sourceCollectionPath = mappedEnd.SourcePath.Take(sourceCollectionIndex + 1).ToList();
            var key = $"{string.Join("/", targetCollectionPath.Select(x => x.Element.Id))}|{string.Join("/", sourceCollectionPath.Select(x => x.Element.Id))}";

            if (!_collectionGroups.TryGetValue(key, out _))
            {
                var group = new CollectionGroup(targetCollectionPath, sourceCollectionPath, targetCollectionIndex, sourceCollectionIndex);
                _collectionGroups.Add(key, group);
            }

            _collectionGroups[key].MappedEnds.Add(mappedEnd);
        }
    }

    private static string? GetCollectionGroupKey(IElementToElementMappedEnd mappedEnd)
    {
        if (!TryGetCollectionPathInfo(mappedEnd, out var targetCollectionIndex, out var sourceCollectionIndex))
        {
            return null;
        }

        var targetCollectionPath = mappedEnd.TargetPath.Take(targetCollectionIndex + 1).ToList();
        var sourceCollectionPath = mappedEnd.SourcePath.Take(sourceCollectionIndex + 1).ToList();
        return $"{string.Join("/", targetCollectionPath.Select(x => x.Element.Id))}|{string.Join("/", sourceCollectionPath.Select(x => x.Element.Id))}";
    }

    private CSharpStatement? TryCreateCollectionProjectionStatement(CollectionGroup group)
    {
        var targetCollectionElement = group.TargetCollectionPath.Last().Element;
        var sourceCollectionElement = group.SourceCollectionPath.Last().Element;

        if (targetCollectionElement.TypeReference?.Element is not IElement targetCollectionType ||
            sourceCollectionElement.TypeReference?.Element is not IElement)
        {
            return null;
        }

        var projectionMappings = new List<string>();
        foreach (var mappedEnd in group.MappedEnds)
        {
            if (TryGetUnsupportedReason(mappedEnd, out _))
            {
                continue;
            }

            if (mappedEnd.TargetPath.Count <= group.TargetCollectionIndex + 1 ||
                mappedEnd.SourcePath.Count <= group.SourceCollectionIndex + 1)
            {
                continue;
            }

            var targetTail = mappedEnd.TargetPath.Skip(group.TargetCollectionIndex + 1).ToList();
            var sourceTail = mappedEnd.SourcePath.Skip(group.SourceCollectionIndex + 1).ToList();

            // Only generate direct item-property projections for now.
            if (targetTail.Count != 1 || sourceTail.Count != 1)
            {
                continue;
            }

            projectionMappings.Add($"{targetTail[0].Name} = item.{sourceTail[0].Name}");
        }

        if (projectionMappings.Count == 0)
        {
            return null;
        }

        var targetCollectionPath = GetPathText(group.TargetCollectionPath, "command");
        var sourceCollectionPath = GetPathText(group.SourceCollectionPath, "entity");
        var targetItemTypeName = _handleMethod.File.Template.GetTypeName(targetCollectionType);
        var initializer = string.Join($",{Environment.NewLine}                ", projectionMappings);

        var statement =
            $"{targetCollectionPath} = {sourceCollectionPath}.Select(item => new {targetItemTypeName}{Environment.NewLine}" +
            "            {" + Environment.NewLine +
            $"                {initializer}{Environment.NewLine}" +
            "            }).ToList()";

        return new CSharpStatement(statement);
    }

    private static bool TryGetUnsupportedReason(IElementToElementMappedEnd mappedEnd, out string reason)
    {
        reason = string.Empty;

        if (!IsSimpleReverseExpression(mappedEnd))
        {
            reason = UnsupportedExpressionReason;
            return true;
        }

        if (ContainsSpecialization(mappedEnd, OperationSpecialization))
        {
            reason = UnsupportedOperationReason;
            return true;
        }

        if (ContainsSpecialization(mappedEnd, ConstructorSpecialization))
        {
            reason = UnsupportedConstructorReason;
            return true;
        }

        return false;
    }

    private static bool IsSimpleReverseExpression(IElementToElementMappedEnd mappedEnd)
    {
        if (string.IsNullOrWhiteSpace(mappedEnd.MappingExpression))
        {
            return true;
        }

        var expression = mappedEnd.MappingExpression.Trim();
        var source = mappedEnd.Sources?.SingleOrDefault();
        if (source == null)
        {
            return false;
        }

        return string.Equals(expression, $"{{{source.ExpressionIdentifier}}}", StringComparison.Ordinal);
    }

    private static bool ContainsSpecialization(IElementToElementMappedEnd mappedEnd, string specialization)
    {
        return mappedEnd.SourcePath.Any(x => string.Equals(x.Element.SpecializationType, specialization, StringComparison.OrdinalIgnoreCase)) ||
               mappedEnd.TargetPath.Any(x => string.Equals(x.Element.SpecializationType, specialization, StringComparison.OrdinalIgnoreCase));
    }

    private static bool TryGetCollectionPathInfo(
        IElementToElementMappedEnd mappedEnd,
        out int targetCollectionIndex,
        out int sourceCollectionIndex)
    {
        targetCollectionIndex = FindCollectionIndex(mappedEnd.TargetPath);
        sourceCollectionIndex = FindCollectionIndex(mappedEnd.SourcePath);
        return targetCollectionIndex >= 0 && sourceCollectionIndex >= 0;
    }

    private static int FindCollectionIndex(IList<IElementMappingPathTarget> path)
    {
        for (var index = 1; index < path.Count; index++)
        {
            if (path[index].Element.TypeReference?.IsCollection == true)
            {
                return index;
            }
        }

        return -1;
    }

    private static string GetPathText(IList<IElementMappingPathTarget> path, string rootReplacement)
    {
        if (path.Count == 0)
        {
            return rootReplacement;
        }

        var members = path.Skip(1).Select(x => x.Name);
        var suffix = string.Join(".", members);
        return string.IsNullOrWhiteSpace(suffix)
            ? rootReplacement
            : $"{rootReplacement}.{suffix}";
    }

    private sealed class CollectionGroup
    {
        public CollectionGroup(
            IList<IElementMappingPathTarget> targetCollectionPath,
            IList<IElementMappingPathTarget> sourceCollectionPath,
            int targetCollectionIndex,
            int sourceCollectionIndex)
        {
            TargetCollectionPath = targetCollectionPath;
            SourceCollectionPath = sourceCollectionPath;
            TargetCollectionIndex = targetCollectionIndex;
            SourceCollectionIndex = sourceCollectionIndex;
            MappedEnds = new List<IElementToElementMappedEnd>();
        }

        public IList<IElementMappingPathTarget> TargetCollectionPath { get; }
        public IList<IElementMappingPathTarget> SourceCollectionPath { get; }
        public int TargetCollectionIndex { get; }
        public int SourceCollectionIndex { get; }
        public List<IElementToElementMappedEnd> MappedEnds { get; }
        public string TargetCollectionPathText => string.Join(".", TargetCollectionPath.Select(x => x.Name));
    }

    private static class ReverseMappingAdapter
    {
        public static IElementToElementMapping Create(IElementToElementMapping mapping)
        {
            return new ReverseElementToElementMapping(mapping);
        }

        private sealed record ReverseElementToElementMapping(IElementToElementMapping Inner) : IElementToElementMapping
        {
            public string Type => Inner.Type;
            public string TypeId => Inner.TypeId;
            public ICanBeReferencedType HostElement => Inner.HostElement;
            public ICanBeReferencedType SourceElement => Inner.TargetElement;
            public ICanBeReferencedType TargetElement => Inner.SourceElement;
            public IList<IElementToElementMappedEnd> MappedEnds => Inner.MappedEnds.Select(x => (IElementToElementMappedEnd)new ReverseMappedEnd(x)).ToList();
        }

        private sealed record ReverseMappedEnd(IElementToElementMappedEnd Inner) : IElementToElementMappedEnd
        {
            public string MappingType => Inner.MappingType;
            public string MappingTypeId => Inner.MappingTypeId;
            public string MappingExpression => Inner.MappingExpression;
            public IList<IElementMappingPathTarget> TargetPath => Inner.SourcePath.Select(x => (IElementMappingPathTarget)new ReversePathTarget(x)).ToList();
            public ICanBeReferencedType TargetElement => Inner.SourceElement;
            public IEnumerable<IElementToElementMappedEndSource> Sources => Inner.Sources;
            public IList<IElementMappingPathTarget> SourcePath => Inner.TargetPath.Select(x => (IElementMappingPathTarget)new ReversePathTarget(x)).ToList();
            public ICanBeReferencedType SourceElement => Inner.TargetElement;

            public IElementToElementMappedEndSource GetSource(string identifier)
            {
                return Inner.GetSource(identifier);
            }
        }

        private sealed record ReversePathTarget(IElementMappingPathTarget Inner) : IElementMappingPathTarget
        {
            public string Id => Inner.Id;
            public string Type => Inner.Type;
            public string Specialization => Inner.Specialization;
            public string Name => Inner.Name;
            public string SpecializationId => Inner.SpecializationId;
            public ICanBeReferencedType Element => Inner.Element;
        }
    }
}