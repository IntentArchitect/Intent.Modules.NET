using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Exceptions;
using Intent.Metadata.Models;
using Intent.Modelers.Domain.Api;
using Intent.Modelers.Services.DomainInteractions.Api;
using Intent.Modules.Application.DomainInteractions.Extensions;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Constants;
using Intent.Modules.Common.CSharp.Interactions;

namespace Intent.Modules.AspNetCore.Controllers.JsonPatch.InteractionStrategies;

internal record PayloadInfo(string VarName, string Type);

internal static class PatchBuilderHelper
{
    public static void EnsurePatchHelperMethods(
        ICSharpClassMethodDeclaration handleMethod,
        UpdateEntityActionTargetEndModel updateAction,
        ClassModel foundEntity,
        Func<ICSharpClassMethodDeclaration, PayloadInfo> getPayloadInfo)
    {
        ArgumentNullException.ThrowIfNull(getPayloadInfo);

        var payloadInfo = getPayloadInfo(handleMethod);
        if (payloadInfo is null)
        {
            throw new Exception($@"{handleMethod.Class.Name}.{handleMethod.Name}:  ""{nameof(payloadInfo)}"" cannot return `null`.");
        }

        var entityTypeName = handleMethod.File.Template.GetTypeName(TemplateRoles.Domain.Entity.Primary, foundEntity)!;
        var parameterType = payloadInfo.Type;
        var updateMapping = updateAction.Mappings.GetUpdateEntityMapping();
        if (updateMapping == null)
        {
            throw new ElementException(updateAction.InternalAssociationEnd, "No Update Entity mapping was found for PATCH update interaction.");
        }

        var @class = handleMethod.Class;

        @class.AddMethod(parameterType, "LoadOriginalState", method =>
        {
            method.Static();
            method.Private();
            method.AddParameter(entityTypeName, "entity");
            method.AddParameter(parameterType, payloadInfo.VarName);
                    
            method.AddStatement("ArgumentNullException.ThrowIfNull(entity);");
            method.AddStatement($"ArgumentNullException.ThrowIfNull({payloadInfo.VarName});");

            foreach (var statement in GenerateJsonPatchLoadOriginalStateStatements(handleMethod, updateMapping, payloadInfo.VarName))
            {
                if (statement is CSharpAssignmentStatement)
                {
                    statement.WithSemicolon();
                }
                method.AddStatement(statement);
            }

            method.AddStatement($"return {payloadInfo.VarName};");
        });

        @class.AddMethod(entityTypeName, "ApplyChangesTo", method =>
        {
            method.Static();
            method.Private();
            method.AddParameter(parameterType, payloadInfo.VarName);
            method.AddParameter(entityTypeName, "entity");

            method.AddStatement($"ArgumentNullException.ThrowIfNull({payloadInfo.VarName});");
            method.AddStatement("ArgumentNullException.ThrowIfNull(entity);");

            var mappingManager = handleMethod.GetMappingManager();

            // Ensure mapping statements target the helper method parameters.
            mappingManager.SetFromReplacement((IElement)updateAction.OtherEnd().Element, payloadInfo.VarName);
            if (TryGetDtoParameterAfterOperation(updateMapping, out var dtoParameter))
            {
                // Avoid duplicated roots (e.g. dto.dto.Name) when operation and parameter both appear in source path.
                mappingManager.SetFromReplacement(dtoParameter, string.Empty);
            }
            mappingManager.SetToReplacement(foundEntity, "entity");
            mappingManager.SetToReplacement(updateAction, "entity");

            var updateStatements = mappingManager.GenerateUpdateStatements(updateMapping);

            foreach (var statement in updateStatements)
            {
                if (statement is CSharpAssignmentStatement)
                {
                    statement.WithSemicolon();
                }
                method.AddStatement(statement);
            }

            method.AddStatement("return entity;");
        });
    }

    private static bool TryGetDtoParameterAfterOperation(
        IElementToElementMapping updateMapping,
        out IElement parameterElement)
    {
        parameterElement = null!;

        foreach (var mappedEnd in updateMapping.MappedEnds)
        {
            var sourcePath = mappedEnd.SourcePath;
            for (var index = 0; index < sourcePath.Count - 1; index++)
            {
                var current = sourcePath[index].Element;
                var next = sourcePath[index + 1].Element;

                if (!string.Equals(current.SpecializationType, "Operation", StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                if (!string.Equals(next.SpecializationType, "Parameter", StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                parameterElement = (IElement)next;
                return true;
            }
        }

        return false;
    }

    private static IReadOnlyList<CSharpStatement> GenerateJsonPatchLoadOriginalStateStatements(
        ICSharpClassMethodDeclaration handleMethod,
        IElementToElementMapping updateMapping,
        string payloadRootVariable)
    {
        const string operationSpecialization = "Operation";
        const string constructorSpecialization = "Constructor";
        const string reverseCommentPrefix = "[JsonPatch Reverse Map]";
        const string unsupportedExpressionReason = "unsupported mapping expression";
        const string unsupportedOperationReason = "operation mappings are not supported";
        const string unsupportedConstructorReason = "constructor mappings are not supported";
        const string unsupportedCollectionShapeReason = "unsupported collection mapping shape";

        var collectionGroups = new Dictionary<string, CollectionGroup>();

        var reverseMapping = ReverseMappingAdapter.Create(updateMapping);
        var statements = new List<CSharpStatement>();
        var mappingManager = handleMethod.GetMappingManager();

        if (reverseMapping.SourceElement is IElement reverseSourceRoot)
        {
            mappingManager.SetFromReplacement(reverseSourceRoot, "entity");
        }

        if (reverseMapping.TargetElement is IElement reverseTargetRoot)
        {
            mappingManager.SetToReplacement(reverseTargetRoot, payloadRootVariable);
        }

        if (TryGetParameterAfterOperationInTargetPath(reverseMapping, out var reverseParameter))
        {
            mappingManager.SetToReplacement(reverseParameter, string.Empty);
        }
            
        // Process in original mapping order to preserve the sequence of assignments
        var mappedEnds = reverseMapping.MappedEnds.ToList();

        BuildCollectionGroups(mappedEnds, collectionGroups);

        var processedCollections = new HashSet<string>();

        foreach (var mappedEnd in mappedEnds)
        {
            var collectionKey = GetCollectionGroupKey(mappedEnd);
            if (collectionKey != null && !processedCollections.Contains(collectionKey))
            {
                var collectionStatement = TryCreateCollectionProjectionStatement(collectionGroups[collectionKey], handleMethod, payloadRootVariable);
                if (collectionStatement != null)
                {
                    statements.Add(collectionStatement.WithSemicolon());
                }
                else
                {
                    var group = collectionGroups[collectionKey];
                    statements.Add(new CSharpStatement($"// {reverseCommentPrefix} Skipped '{group.TargetCollectionPathText}': {unsupportedCollectionShapeReason}."));
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
                statements.Add(new CSharpStatement($"// {reverseCommentPrefix} Skipped '{GetPathText(mappedEnd.TargetPath, payloadRootVariable)}': {reason}."));
                continue;
            }

            var targetPath = mappingManager.GenerateTargetStatementForMapping(reverseMapping, mappedEnd).ToString();
            var sourcePath = mappingManager.GenerateSourceStatementForMapping(reverseMapping, mappedEnd).ToString();
            statements.Add(new CSharpAssignmentStatement(targetPath, sourcePath));
        }

        return statements;

        static void BuildCollectionGroups(IEnumerable<IElementToElementMappedEnd> mappedEnds, Dictionary<string, CollectionGroup> collectionGroups)
        {
            foreach (var mappedEnd in mappedEnds)
            {
                if (!TryGetCollectionPathInfo(mappedEnd, out var targetCollectionIndex, out var sourceCollectionIndex))
                {
                    continue;
                }

                var targetCollectionPath = mappedEnd.TargetPath.Take(targetCollectionIndex + 1).ToList();
                var sourceCollectionPath = mappedEnd.SourcePath.Take(sourceCollectionIndex + 1).ToList();
                var key =
                    $"{string.Join("/", targetCollectionPath.Select(x => x.Element.Id))}|{string.Join("/", sourceCollectionPath.Select(x => x.Element.Id))}";

                if (!collectionGroups.TryGetValue(key, out _))
                {
                    var group = new CollectionGroup(targetCollectionPath, sourceCollectionPath, targetCollectionIndex, sourceCollectionIndex);
                    collectionGroups.Add(key, group);
                }

                collectionGroups[key].MappedEnds.Add(mappedEnd);
            }
        }

        static string? GetCollectionGroupKey(IElementToElementMappedEnd mappedEnd)
        {
            if (!TryGetCollectionPathInfo(mappedEnd, out var targetCollectionIndex, out var sourceCollectionIndex))
            {
                return null;
            }

            var targetCollectionPath = mappedEnd.TargetPath.Take(targetCollectionIndex + 1).ToList();
            var sourceCollectionPath = mappedEnd.SourcePath.Take(sourceCollectionIndex + 1).ToList();
            return $"{string.Join("/", targetCollectionPath.Select(x => x.Element.Id))}|{string.Join("/", sourceCollectionPath.Select(x => x.Element.Id))}";
        }

        static CSharpStatement? TryCreateCollectionProjectionStatement(CollectionGroup group, ICSharpClassMethodDeclaration handleMethod, string payloadRootVariable)
        {
            var targetCollectionElement = group.TargetCollectionPath.Last().Element;
            var sourceCollectionElement = group.SourceCollectionPath.Last().Element;

            if (targetCollectionElement.TypeReference?.Element is not IElement targetCollectionType ||
                sourceCollectionElement.TypeReference?.Element is not IElement)
            {
                return null;
            }

            var projectionMappings = new List<(IList<IElementMappingPathTarget> TargetTail, IList<IElementMappingPathTarget> SourceTail)>();
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

                projectionMappings.Add((targetTail, sourceTail));
            }

            if (projectionMappings.Count == 0)
            {
                return null;
            }

            if (!TryBuildObjectInitializer(targetCollectionType, projectionMappings, "item", handleMethod, out var objInit))
            {
                return null;
            }

            var targetCollectionPath = GetPathText(group.TargetCollectionPath, payloadRootVariable);
            var sourceCollectionPath = GetPathText(group.SourceCollectionPath, "entity");

            var assignStatement = new CSharpAssignmentStatement(
                lhs: targetCollectionPath,
                rhs: new CSharpStatement(sourceCollectionPath)
                    .AddInvocation("Select", select => select.AddArgument(new CSharpLambdaBlock("item").WithExpressionBody(objInit)))
                    .WithoutSemicolon());

            return assignStatement.AddInvocation("ToList");
        }

        static bool TryBuildObjectInitializer(
            IElement targetType,
            IReadOnlyList<(IList<IElementMappingPathTarget> TargetTail, IList<IElementMappingPathTarget> SourceTail)> mappings,
            string sourceVariable,
            ICSharpClassMethodDeclaration handleMethod,
            out CSharpObjectInitializerBlock objectInitializer)
        {
            var targetItemTypeName = handleMethod.File.Template.GetTypeName(targetType);
            objectInitializer = new CSharpObjectInitializerBlock($"new {targetItemTypeName}");
            var hasMappings = false;

            foreach (var (targetTail, sourceTail) in mappings)
            {
                if (targetTail.Count != 1 || sourceTail.Count != 1)
                {
                    continue;
                }

                if (targetTail[0].Element.TypeReference?.IsCollection == true ||
                    sourceTail[0].Element.TypeReference?.IsCollection == true)
                {
                    continue;
                }

                objectInitializer.AddInitStatement(targetTail[0].Name, $"{sourceVariable}.{sourceTail[0].Name}");
                hasMappings = true;
            }

            var nestedCollectionGroups = mappings
                .Where(x => x.TargetTail.Count > 0 && x.SourceTail.Count > 0)
                .Where(x => x.TargetTail[0].Element.TypeReference?.IsCollection == true &&
                            x.SourceTail[0].Element.TypeReference?.IsCollection == true)
                .GroupBy(x => $"{x.TargetTail[0].Element.Id}|{x.SourceTail[0].Element.Id}")
                .ToList();

            foreach (var nestedCollectionGroup in nestedCollectionGroups)
            {
                var collectionNode = nestedCollectionGroup.First();
                if (collectionNode.TargetTail[0].Element.TypeReference?.Element is not IElement nestedTargetType)
                {
                    continue;
                }

                var nestedMappings = nestedCollectionGroup
                    .Where(x => x.TargetTail.Count > 1 && x.SourceTail.Count > 1)
                    .Select(x =>
                    (
                        TargetTail: (IList<IElementMappingPathTarget>)x.TargetTail.Skip(1).ToList(),
                        SourceTail: (IList<IElementMappingPathTarget>)x.SourceTail.Skip(1).ToList()
                    ))
                    .ToList();

                if (nestedMappings.Count == 0)
                {
                    continue;
                }

                if (!TryBuildObjectInitializer(nestedTargetType, nestedMappings, "nestedItem", handleMethod, out var nestedInitializer))
                {
                    continue;
                }

                var nestedProjection = new CSharpStatement($"{sourceVariable}.{collectionNode.SourceTail[0].Name}")
                    .AddInvocation("Select", select =>
                        select.AddArgument(new CSharpLambdaBlock("nestedItem").WithExpressionBody(nestedInitializer)))
                    .WithoutSemicolon()
                    .AddInvocation("ToList")
                    .WithoutSemicolon();

                objectInitializer.AddInitStatement(collectionNode.TargetTail[0].Name, nestedProjection);
                hasMappings = true;
            }

            return hasMappings;
        }

        static bool TryGetUnsupportedReason(IElementToElementMappedEnd mappedEnd, out string reason)
        {
            reason = string.Empty;

            if (!IsSimpleReverseExpression(mappedEnd))
            {
                reason = unsupportedExpressionReason;
                return true;
            }

            if (ContainsUnsupportedSpecialization(mappedEnd, operationSpecialization))
            {
                reason = unsupportedOperationReason;
                return true;
            }

            if (ContainsUnsupportedSpecialization(mappedEnd, constructorSpecialization))
            {
                reason = unsupportedConstructorReason;
                return true;
            }

            return false;
        }

        static bool IsSimpleReverseExpression(IElementToElementMappedEnd mappedEnd)
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

        static bool ContainsUnsupportedSpecialization(IElementToElementMappedEnd mappedEnd, string specialization)
        {
            return ContainsNonRootSpecialization(mappedEnd.SourcePath, specialization) ||
                   ContainsNonRootSpecialization(mappedEnd.TargetPath, specialization);
        }

        static bool ContainsNonRootSpecialization(IList<IElementMappingPathTarget> path, string specialization)
        {
            for (var index = 1; index < path.Count; index++)
            {
                if (string.Equals(path[index].Element.SpecializationType, specialization, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }

            return false;
        }

        static bool TryGetParameterAfterOperationInTargetPath(IElementToElementMapping mapping, out IElement parameterElement)
        {
            parameterElement = null!;

            foreach (var mappedEnd in mapping.MappedEnds)
            {
                var targetPath = mappedEnd.TargetPath;
                for (var index = 0; index < targetPath.Count - 1; index++)
                {
                    var current = targetPath[index].Element;
                    var next = targetPath[index + 1].Element;

                    if (!string.Equals(current.SpecializationType, "Operation", StringComparison.OrdinalIgnoreCase))
                    {
                        continue;
                    }

                    if (!string.Equals(next.SpecializationType, "Parameter", StringComparison.OrdinalIgnoreCase))
                    {
                        continue;
                    }

                    parameterElement = (IElement)next;
                    return true;
                }
            }

            return false;
        }

        static bool TryGetCollectionPathInfo(
            IElementToElementMappedEnd mappedEnd,
            out int targetCollectionIndex,
            out int sourceCollectionIndex)
        {
            targetCollectionIndex = FindCollectionIndex(mappedEnd.TargetPath);
            sourceCollectionIndex = FindCollectionIndex(mappedEnd.SourcePath);
            return targetCollectionIndex >= 0 && sourceCollectionIndex >= 0;
        }

        static int FindCollectionIndex(IList<IElementMappingPathTarget> path)
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

        static string GetPathText(IList<IElementMappingPathTarget> path, string rootReplacement)
        {
            if (path.Count == 0)
            {
                return rootReplacement;
            }

            var members = path.Skip(1).Select(x => x.Name).ToList();
            if (members.Count > 0 && string.Equals(members[0], rootReplacement, StringComparison.OrdinalIgnoreCase))
            {
                members.RemoveAt(0);
            }

            var suffix = string.Join(".", members);
            return string.IsNullOrWhiteSpace(suffix)
                ? rootReplacement
                : $"{rootReplacement}.{suffix}";
        }
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