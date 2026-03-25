using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Exceptions;
using Intent.Metadata.Models;
using Intent.Modelers.Domain.Api;
using Intent.Modelers.Services.DomainInteractions.Api;
using Intent.Modules.Application.DomainInteractions.Extensions;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Mapping;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Constants;
using Intent.Modules.Common.CSharp.Interactions;
using Intent.Modules.AspNetCore.Controllers.JsonPatch.Mapping.Resolvers;
using Intent.Utils;

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

        // Validate that the update mapping does not contain unsupported specializations (e.g., operations)
        ValidateUpdateMappingForUnsupportedSpecializations(updateMapping, updateAction.InternalAssociationEnd);

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

    private static void ValidateUpdateMappingForUnsupportedSpecializations(
        IElementToElementMapping updateMapping,
        IAssociationEnd updateActionAssociationEnd)
    {
        const string operationSpecialization = "Operation";
        const string constructorSpecialization = "Constructor";

        foreach (var mappedEnd in updateMapping.MappedEnds)
        {
            // Check source and target paths for unsupported specializations
            if (PathContainsNonRootSpecialization(mappedEnd.SourcePath, operationSpecialization) ||
                PathContainsNonRootSpecialization(mappedEnd.TargetPath, operationSpecialization))
            {
                throw new ElementException(
                    updateActionAssociationEnd,
                    "JSON Patch Update association does not support mapping to or from operations. " +
                    "Please remove the mapping that includes an operation from the Update Entity Action.");
            }

            if (PathContainsNonRootSpecialization(mappedEnd.SourcePath, constructorSpecialization) ||
                PathContainsNonRootSpecialization(mappedEnd.TargetPath, constructorSpecialization))
            {
                throw new ElementException(
                    updateActionAssociationEnd,
                    "JSON Patch Update association does not support mapping to or from constructors. " +
                    "Please remove the mapping that includes a constructor from the Update Entity Action.");
            }
        }
    }

    private static bool PathContainsNonRootSpecialization(IList<IElementMappingPathTarget> path, string specialization)
    {
        // Skip the root element (index 0) and check the rest of the path
        for (var index = 1; index < path.Count; index++)
        {
            if (string.Equals(path[index].Element.SpecializationType, specialization, StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
        }

        return false;
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

    private static List<CSharpStatement> GenerateJsonPatchLoadOriginalStateStatements(
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
        var initializedTargetPaths = new HashSet<string>(StringComparer.Ordinal);
        var mappingManager = handleMethod.GetMappingManager();
        EnsureJsonPatchReverseResolverRegistered(handleMethod, mappingManager);

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
                var collectionStatement = TryCreateCollectionProjectionStatement(
                    collectionGroups[collectionKey],
                    payloadRootVariable,
                    mappingManager,
                    reverseMapping);
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

            foreach (var guardStatement in CreateTargetInstantiationGuardStatements(
                         mappedEnd.TargetPath,
                         payloadRootVariable,
                         handleMethod,
                         initializedTargetPaths))
            {
                statements.Add(guardStatement.WithSemicolon());
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

        static CSharpStatement? TryCreateCollectionProjectionStatement(
            CollectionGroup group,
            string payloadRootVariable,
            CSharpClassMappingManager mappingManager,
            IElementToElementMapping reverseMapping)
        {
            var targetCollectionPathText = GetPathText(group.TargetCollectionPath, payloadRootVariable);
            var sourceCollectionPathText = GetPathText(group.SourceCollectionPath, "entity");
            var collectionRootMappedEnd = group.MappedEnds.FirstOrDefault(x =>
                x.TargetPath.Count == group.TargetCollectionIndex + 1 &&
                x.SourcePath.Count == group.SourceCollectionIndex + 1);

            if (collectionRootMappedEnd != null)
            {
                try
                {
                    var targetPath = mappingManager.GenerateTargetStatementForMapping(reverseMapping, collectionRootMappedEnd).ToString();
                    var sourceStatement = mappingManager.GenerateSourceStatementForMapping(reverseMapping, collectionRootMappedEnd).WithoutSemicolon();
                    var sourceText = sourceStatement.ToString();

                    // Reverse collection mapping must materialize DTO items via projection.
                    // Accept manager output only when it includes Select(...), e.g. entity.Items.Select(...).ToList().
                    // If it resolves to direct assignment (e.g. entity.Items), the generated reverse code would
                    // incorrectly assign an entity collection to a DTO collection, so we reject it.
                    if (sourceText.Contains("Select(", StringComparison.Ordinal) ||
                        sourceText.Contains(".Select(", StringComparison.Ordinal))
                    {
                        Logging.Log.Debug($"{reverseCommentPrefix} Mapping-manager projection accepted for '{targetPath}' from '{sourceCollectionPathText}'.");
                        return new CSharpAssignmentStatement(targetPath, sourceStatement);
                    }

                    Logging.Log.Warning($"{reverseCommentPrefix} Mapping-manager projection rejected for '{targetPath}'. Source was '{sourceText}'.");
                }
                catch (Exception ex)
                {
                    Logging.Log.Warning($"{reverseCommentPrefix} Mapping-manager projection threw for '{targetCollectionPathText}' from '{sourceCollectionPathText}'. Reason: {ex.Message}");
                }
            }
            else
            {
                Logging.Log.Warning($"{reverseCommentPrefix} No collection root mapped end was found for '{targetCollectionPathText}' from '{sourceCollectionPathText}'.");
            }
            return null;
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

    private static IEnumerable<CSharpStatement> CreateTargetInstantiationGuardStatements(
        IList<IElementMappingPathTarget> targetPath,
        string rootReplacement,
        ICSharpClassMethodDeclaration handleMethod,
        ISet<string> initializedTargetPaths)
    {
        if (targetPath.Count < 3)
        {
            yield break;
        }

        if (handleMethod.File.Template is not ICSharpFileBuilderTemplate template)
        {
            yield break;
        }

        for (var index = 1; index < targetPath.Count - 1; index++)
        {
            var pathTarget = targetPath[index];
            if (string.Equals(pathTarget.Element.SpecializationType, "Operation", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(pathTarget.Element.SpecializationType, "Parameter", StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            if (pathTarget.Element.TypeReference?.IsCollection == true)
            {
                yield break;
            }

            var targetPathText = GetPathTextCore(targetPath.Take(index + 1).ToList(), rootReplacement);
            if (string.Equals(targetPathText, rootReplacement, StringComparison.Ordinal))
            {
                continue;
            }

            if (!initializedTargetPaths.Add(targetPathText))
            {
                continue;
            }

            if (!TryResolveInstantiationTypeName(pathTarget, template, out var typeName))
            {
                continue;
            }

            yield return new CSharpStatement($"{targetPathText} ??= new {typeName}()");
        }
    }

    private static bool TryResolveInstantiationTypeName(
        IElementMappingPathTarget pathTarget,
        ICSharpFileBuilderTemplate template,
        out string typeName)
    {
        typeName = null!;

        if (pathTarget.Element is not IElement element)
        {
            return false;
        }

        var targetType = element.TypeReference?.Element as IElement;
        if (targetType == null || element.TypeReference?.IsCollection == true)
        {
            return false;
        }

        return template.TryGetTypeName(TemplateRoles.Application.Contracts.Dto, targetType, out typeName) ||
               template.TryGetTypeName("Application.Contract.Command", targetType, out typeName) ||
               template.TryGetTypeName(TemplateRoles.Domain.DataContract, targetType, out typeName) ||
               TryUseFallbackTypeName(targetType, out typeName);
    }

    private static bool TryUseFallbackTypeName(IElement targetType, out string typeName)
    {
        typeName = targetType.Name;
        return !string.IsNullOrWhiteSpace(typeName);
    }

    private static string GetPathTextCore(IList<IElementMappingPathTarget> path, string rootReplacement)
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

    private static void EnsureJsonPatchReverseResolverRegistered(
        ICSharpClassMethodDeclaration handleMethod,
        CSharpClassMappingManager mappingManager)
    {
        const string metadataKey = "jsonpatch-reverse-update-resolver-registered";
        if (handleMethod.TryGetMetadata<bool>(metadataKey, out var registered) && registered)
        {
            return;
        }

        if (handleMethod.File.Template is not ICSharpFileBuilderTemplate fileBuilderTemplate)
        {
            return;
        }
        mappingManager.AddMappingResolver(new JsonPatchReverseUpdateMappingTypeResolver(fileBuilderTemplate), priority: -2);
        handleMethod.AddMetadata(metadataKey, true);
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
            MappedEnds = [];
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
            public IList<IElementToElementMappedEnd> MappedEnds => Inner.MappedEnds.Select(IElementToElementMappedEnd (x) => new ReverseMappedEnd(x)).ToList();
        }

        private sealed record ReverseMappedEnd(IElementToElementMappedEnd Inner) : IElementToElementMappedEnd
        {
            public string MappingType => Inner.MappingType;
            public string MappingTypeId => Inner.MappingTypeId;
            public string MappingExpression => Inner.MappingExpression;
            public IList<IElementMappingPathTarget> TargetPath => Inner.SourcePath.Select(IElementMappingPathTarget (x) => new ReversePathTarget(x)).ToList();
            public ICanBeReferencedType TargetElement => Inner.SourceElement;
            public IEnumerable<IElementToElementMappedEndSource> Sources => Inner.Sources.Select(IElementToElementMappedEndSource (x) => new ReverseMappedEndSource(x, Inner.TargetPath));
            public IList<IElementMappingPathTarget> SourcePath => Inner.TargetPath.Select(IElementMappingPathTarget (x) => new ReversePathTarget(x)).ToList();
            public ICanBeReferencedType SourceElement => Inner.TargetElement;

            public IElementToElementMappedEndSource GetSource(string identifier)
            {
                var source = Sources.FirstOrDefault(x => string.Equals(x.ExpressionIdentifier, identifier, StringComparison.Ordinal));
                return source ?? Sources.First();
            }
        }

        private sealed record ReverseMappedEndSource(
            IElementToElementMappedEndSource Inner,
            IList<IElementMappingPathTarget> ReverseSourcePath) : IElementToElementMappedEndSource
        {
            public string ExpressionIdentifier => Inner.ExpressionIdentifier;
            public string MappingType => Inner.MappingType;
            public string MappingTypeId => Inner.MappingTypeId;
            public ICanBeReferencedType Element => Inner.Element;
            public IList<IElementMappingPathTarget> Path => ReverseSourcePath.Select(IElementMappingPathTarget (x) => new ReversePathTarget(x)).ToList();
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