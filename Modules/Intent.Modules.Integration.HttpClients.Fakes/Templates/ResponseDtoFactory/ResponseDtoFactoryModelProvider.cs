using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modelers.Services.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.Templates;
using Intent.Modules.Common.Types.Api;
using Intent.Modules.Contracts.Clients.Shared.Templates.PagedResult;
using Intent.Modules.Integration.HttpClients.Shared.Templates;

namespace Intent.Modules.Integration.HttpClients.Fakes.Templates.ResponseDtoFactory;

internal static class ResponseDtoFactoryModelProvider
{
    private const string ServiceProxiesDesignerId = "2799aa83-e256-46fe-9589-b96f7d6b09f7";

    public static IReadOnlyCollection<ResponseDtoFactoryModel> GetModels(IMetadataManager metadataManager, IApplication application)
    {
        var serviceProxies = metadataManager.GetServiceProxyModels(
            application.Id,
            application,
            applicationId => metadataManager.GetDesigner(applicationId, ServiceProxiesDesignerId),
            metadataManager.Services);

        return GetModels(serviceProxies);
    }

    public static IReadOnlyCollection<ResponseDtoFactoryModel> GetModels(IEnumerable<IServiceProxyModel> serviceProxies)
    {
        return serviceProxies
            .SelectMany(GetModels)
            .DistinctBy(model => model.Id)
            .ToArray();
    }

    private static IReadOnlyCollection<ResponseDtoFactoryModel> GetModels(IServiceProxyModel serviceProxy)
    {
        var responseDtoUsages = GetResponseDtoUsages(serviceProxy);
        var factoryNamesByDtoId = responseDtoUsages.ToDictionary(usage => usage.Dto.Id, usage => usage.FactoryName);

        return responseDtoUsages
            .Select(usage => new ResponseDtoFactoryModel(
                serviceProxy,
                usage.Dto,
                usage.RequiresCreateList,
                usage.CyclicTargetDtoIds,
                usage.FactoryName,
                factoryNamesByDtoId))
            .ToArray();
    }

    private static IReadOnlyCollection<ResponseDtoUsage> GetResponseDtoUsages(IServiceProxyModel serviceProxy)
    {
        var responseDtos = new Dictionary<string, ResponseDtoUsage>();
        var dtoReferences = new Dictionary<string, HashSet<string>>();
        var visitedTypeReferences = new HashSet<string>();
        var stack = new Stack<ITypeReference>(
            serviceProxy.Endpoints
                .Where(endpoint => endpoint.ReturnType != null)
                .Select(endpoint => endpoint.ReturnType!));

        while (stack.Count > 0)
        {
            var typeReference = stack.Pop();
            if (!visitedTypeReferences.Add(GetTypeReferenceKey(typeReference)))
            {
                continue;
            }

            DiscoverListFactoryUsage(typeReference, responseDtos, currentDtoId: null);

            if (typeReference.Element is IElement typeReferenceElement &&
                typeReferenceElement.IsDTOModel() &&
                IsGenericDto(typeReferenceElement))
            {
                continue;
            }

            foreach (var genericTypeParameter in typeReference.GenericTypeParameters)
            {
                stack.Push(genericTypeParameter);
            }

            if (typeReference.Element is not IElement element ||
                !element.IsDTOModel() ||
                IsGenericDto(element))
            {
                continue;
            }

            var dto = GetOrAddUsage(responseDtos, element);
            var references = GetOrAddReferenceSet(dtoReferences, element.Id);

            foreach (var field in element.ChildElements.Where(child => child.IsDTOFieldModel()))
            {
                if (field.TypeReference == null)
                {
                    continue;
                }

                DiscoverListFactoryUsage(field.TypeReference, responseDtos, dto.Dto.Id);

                if (field.TypeReference.Element is IElement fieldElement && fieldElement.IsDTOModel())
                {
                    references.Add(fieldElement.Id);
                }

                stack.Push(field.TypeReference);
            }
        }

        AssignCyclicTargets(responseDtos, dtoReferences);
        AssignCreateListRequirements(serviceProxy, responseDtos);
        AssignFactoryNames(responseDtos);

        return responseDtos.Values.ToArray();
    }

    private static void DiscoverListFactoryUsage(
        ITypeReference typeReference,
        Dictionary<string, ResponseDtoUsage> responseDtos,
        string? currentDtoId)
    {
        if (typeReference.IsCollection &&
            typeReference.Element is IElement collectionItemElement &&
            collectionItemElement.IsDTOModel() &&
            !IsGenericDto(collectionItemElement) &&
            collectionItemElement.Id != currentDtoId)
        {
            GetOrAddUsage(responseDtos, collectionItemElement);
        }

        if (typeReference.Element?.Id == PagedResultTemplateBase.TypeDefinitionElementId &&
            typeReference.GenericTypeParameters.FirstOrDefault()?.Element is IElement pagedItemElement &&
            pagedItemElement.IsDTOModel() &&
            !IsGenericDto(pagedItemElement))
        {
            GetOrAddUsage(responseDtos, pagedItemElement);
        }
    }

    /// <summary>
    /// For each DTO, records which of the DTOs it references can transitively reach back to it (a direct
    /// self-reference counts). The factory must emit an empty/default placeholder for those edges so the
    /// generated <c>Create()</c> calls terminate instead of recursing forever.
    /// </summary>
    private static void AssignCyclicTargets(
        Dictionary<string, ResponseDtoUsage> responseDtos,
        Dictionary<string, HashSet<string>> dtoReferences)
    {
        foreach (var (dtoId, usage) in responseDtos)
        {
            if (!dtoReferences.TryGetValue(dtoId, out var referencedDtoIds))
            {
                continue;
            }

            foreach (var referencedDtoId in referencedDtoIds)
            {
                if (CanReach(dtoReferences, fromDtoId: referencedDtoId, targetDtoId: dtoId))
                {
                    usage.CyclicTargetDtoIds.Add(referencedDtoId);
                }
            }
        }
    }

    private static void AssignCreateListRequirements(
        IServiceProxyModel serviceProxy,
        Dictionary<string, ResponseDtoUsage> responseDtos)
    {
        foreach (var usage in responseDtos.Values)
        {
            usage.RequiresCreateList = false;
        }

        foreach (var endpoint in serviceProxy.Endpoints.Where(endpoint => endpoint.ReturnType != null))
        {
            MarkEndpointCreateListRequirement(endpoint.ReturnType!, responseDtos);
        }

        foreach (var usage in responseDtos.Values)
        {
            foreach (var field in usage.Dto.Fields)
            {
                if (field.TypeReference == null)
                {
                    continue;
                }

                MarkFactoryFieldCreateListRequirement(field.TypeReference, responseDtos, usage);
            }
        }
    }

    private static void MarkEndpointCreateListRequirement(
        ITypeReference typeReference,
        Dictionary<string, ResponseDtoUsage> responseDtos)
    {
        MarkDictionaryValueCreateListRequirement(
            typeReference,
            responseDtos,
            currentDtoId: null,
            cyclicTargetDtoIds: null);

        MarkCollectionCreateListRequirement(
            typeReference,
            responseDtos,
            currentDtoId: null,
            cyclicTargetDtoIds: null);

        if (typeReference.Element?.Id == PagedResultTemplateBase.TypeDefinitionElementId &&
            typeReference.GenericTypeParameters.FirstOrDefault()?.Element is IElement pagedItemElement)
        {
            MarkDtoCreateListRequirement(pagedItemElement, responseDtos);
        }
    }

    private static void MarkFactoryFieldCreateListRequirement(
        ITypeReference typeReference,
        Dictionary<string, ResponseDtoUsage> responseDtos,
        ResponseDtoUsage containingUsage)
    {
        MarkDictionaryValueCreateListRequirement(
            typeReference,
            responseDtos,
            containingUsage.Dto.Id,
            containingUsage.CyclicTargetDtoIds);

        if (!typeReference.IsCollection)
        {
            return;
        }

        MarkCollectionCreateListRequirement(
            typeReference,
            responseDtos,
            containingUsage.Dto.Id,
            containingUsage.CyclicTargetDtoIds);
    }

    private static void MarkCollectionCreateListRequirement(
        ITypeReference typeReference,
        Dictionary<string, ResponseDtoUsage> responseDtos,
        string? currentDtoId,
        IReadOnlyCollection<string>? cyclicTargetDtoIds)
    {
        if (typeReference.Element is not IElement collectionItemElement ||
            collectionItemElement.Id == currentDtoId ||
            cyclicTargetDtoIds?.Contains(collectionItemElement.Id) == true)
        {
            return;
        }

        MarkDtoCreateListRequirement(collectionItemElement, responseDtos);
    }

    private static void MarkDictionaryValueCreateListRequirement(
        ITypeReference typeReference,
        Dictionary<string, ResponseDtoUsage> responseDtos,
        string? currentDtoId,
        IReadOnlyCollection<string>? cyclicTargetDtoIds)
    {
        if (!IsDictionaryType(typeReference))
        {
            return;
        }

        var valueTypeReference = typeReference.GenericTypeParameters.Skip(1).FirstOrDefault();
        if (valueTypeReference == null)
        {
            return;
        }

        MarkCollectionCreateListRequirement(
            valueTypeReference,
            responseDtos,
            currentDtoId,
            cyclicTargetDtoIds);

        MarkDictionaryValueCreateListRequirement(
            valueTypeReference,
            responseDtos,
            currentDtoId,
            cyclicTargetDtoIds);
    }

    private static void MarkDtoCreateListRequirement(
        IElement dtoElement,
        Dictionary<string, ResponseDtoUsage> responseDtos)
    {
        if (!dtoElement.IsDTOModel() ||
            IsGenericDto(dtoElement) ||
            !responseDtos.TryGetValue(dtoElement.Id, out var usage))
        {
            return;
        }

        usage.RequiresCreateList = true;
    }

    private static void AssignFactoryNames(Dictionary<string, ResponseDtoUsage> responseDtos)
    {
        foreach (var group in responseDtos.Values.GroupBy(usage => ResponseDtoFactoryModel.GetDefaultFactoryName(usage.Dto)))
        {
            var usages = group.ToArray();
            if (usages.Length == 1)
            {
                usages[0].FactoryName = group.Key;
                continue;
            }

            foreach (var usage in usages)
            {
                usage.FactoryName = GetQualifiedFactoryName(usage.Dto, group.Key);
            }

            foreach (var duplicateNameGroup in usages.GroupBy(usage => usage.FactoryName).Where(grouping => grouping.Count() > 1))
            {
                foreach (var usage in duplicateNameGroup)
                {
                    usage.FactoryName = $"{usage.FactoryName.RemoveSuffix("Factory")}{GetStableNameSuffix(usage.Dto.Id)}Factory";
                }
            }
        }
    }

    private static string GetQualifiedFactoryName(DTOModel dto, string defaultFactoryName)
    {
        var qualifier = string.Concat(GetFactoryQualifierSegments(dto)
            .Select(segment => segment.Replace(".", "_").ToPascalCase())
            .Where(segment => !string.IsNullOrWhiteSpace(segment)));

        return string.IsNullOrWhiteSpace(qualifier)
            ? $"{defaultFactoryName.RemoveSuffix("Factory")}{GetStableNameSuffix(dto.Id)}Factory"
            : $"{qualifier}{defaultFactoryName}";
    }

    private static IEnumerable<string> GetFactoryQualifierSegments(DTOModel dto)
    {
        if (!string.IsNullOrWhiteSpace(dto.InternalElement.Package?.Name))
        {
            yield return dto.InternalElement.Package.Name;
        }

        foreach (var parent in dto.InternalElement.GetParentPath())
        {
            yield return parent.Name;
        }
    }

    private static string GetStableNameSuffix(string id)
    {
        var suffix = new string(id
            .Where(char.IsLetterOrDigit)
            .Take(8)
            .ToArray());

        return string.IsNullOrWhiteSpace(suffix)
            ? "Model"
            : $"Model{suffix}";
    }

    private static bool IsDictionaryType(ITypeReference typeReference)
    {
        return typeReference.GenericTypeParameters.Count() >= 2 &&
               typeReference.Element?.Name.EndsWith("Dictionary", StringComparison.Ordinal) == true;
    }

    private static bool CanReach(
        Dictionary<string, HashSet<string>> dtoReferences,
        string fromDtoId,
        string targetDtoId)
    {
        var visited = new HashSet<string>();
        var stack = new Stack<string>();
        stack.Push(fromDtoId);

        while (stack.Count > 0)
        {
            var current = stack.Pop();
            if (current == targetDtoId)
            {
                return true;
            }

            if (!visited.Add(current))
            {
                continue;
            }

            if (dtoReferences.TryGetValue(current, out var next))
            {
                foreach (var nextDtoId in next)
                {
                    stack.Push(nextDtoId);
                }
            }
        }

        return false;
    }

    private static ResponseDtoUsage GetOrAddUsage(
        Dictionary<string, ResponseDtoUsage> responseDtos,
        IElement dtoElement)
    {
        if (!responseDtos.TryGetValue(dtoElement.Id, out var usage))
        {
            usage = new ResponseDtoUsage(new DTOModel(dtoElement));
            responseDtos.Add(dtoElement.Id, usage);
        }

        return usage;
    }

    private static HashSet<string> GetOrAddReferenceSet(
        Dictionary<string, HashSet<string>> dtoReferences,
        string dtoId)
    {
        if (!dtoReferences.TryGetValue(dtoId, out var references))
        {
            references = new HashSet<string>();
            dtoReferences.Add(dtoId, references);
        }

        return references;
    }

    // Generic DTOs (open type parameters) can't be expressed as a parameterless `{T}Factory.Create()`
    // and have no concrete placeholder for their type-parameter members, so they are excluded from
    // factory generation. References to them fall back to a structural default placeholder instead.
    private static bool IsGenericDto(IElement element)
    {
        return new DTOModel(element).GenericTypes.Any();
    }

    private static string GetTypeReferenceKey(ITypeReference typeReference)
    {
        var genericTypeParameterKeys = string.Join(
            ",",
            typeReference.GenericTypeParameters.Select(GetTypeReferenceKey));

        return $"{typeReference.Element?.Id ?? "<null>"}:{typeReference.IsCollection}:{typeReference.IsNullable}<{genericTypeParameterKeys}>";
    }

    private sealed class ResponseDtoUsage
    {
        public ResponseDtoUsage(DTOModel dto)
        {
            Dto = dto;
        }

        public DTOModel Dto { get; }
        public bool RequiresCreateList { get; set; }
        public string FactoryName { get; set; } = string.Empty;
        public HashSet<string> CyclicTargetDtoIds { get; } = new();
    }
}
