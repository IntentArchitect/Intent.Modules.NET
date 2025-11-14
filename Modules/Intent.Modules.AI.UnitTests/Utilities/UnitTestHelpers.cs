using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modules.Common.Types.Api;

namespace Intent.Modules.AI.UnitTests.Utilities;

public static class UnitTestHelpers
{
    // Specialization Type IDs
    private const string ClassSpecializationTypeId = "04e12b51-ed12-42a3-9667-a6aa81bb6d10";
    private const string OperationSpecializationTypeId = "e030c97a-e066-40a7-8188-808c275df3cb";
    private const string ServiceSpecializationTypeId = "b16578a5-27b1-4047-a8df-f0b783d706bd";
    private const string DTOSpecializationTypeId = "a5e74323-9b24-48c8-9802-f8684e0aaa70";

    public static string GetMockFramework(IApplicationConfigurationProvider applicationConfigurationProvider)
    {
        const string defaultMock = "Moq";

        var unitTestGroup = applicationConfigurationProvider.GetSettings().GetGroup("d62269ea-8e64-44a0-8392-e1a69da7c960");

        if (unitTestGroup is null)
        {
            return defaultMock;
        }

        return unitTestGroup.GetSetting("115c28bc-a4c8-4b30-bd00-2e320fee77dc")?.Value ?? defaultMock;
    }

    public static List<IElement> GetRelatedElements(IElement element)
    {
        // Get IElement objects from TypeReferences
        var relatedElements = element.AssociatedElements
            .Where(x => x.TypeReference?.Element != null)
            .Select(x => x.TypeReference.Element as IElement)
            .Where(x => x != null)
            .ToList();
        
        if (relatedElements.Count == 0)
        {
            return [];
        }
        
        var relatedClasses = relatedElements
            // Include DTOs that are referenced by child elements (e.g. DTO fields referencing other DTOs)
            .Concat(relatedElements
                .Where(x => x.TypeReference?.Element != null && x.TypeReference.Element.SpecializationTypeId == DTOSpecializationTypeId)
                .Select(x => x.TypeReference.Element as IElement)
                .Where(x => x != null))
            // Include parent service for operations (from Services designer)
            .Concat(relatedElements
                .Where(x => x.SpecializationTypeId == OperationSpecializationTypeId && x.ParentElement != null)
                .Select(x => x.ParentElement))
            // Include parent service for operations (from Common.Types)
            .Concat(relatedElements
                .Where(x => OperationModelExtensions.IsOperationModel(x))
                .Select(x => OperationModelExtensions.AsOperationModel(x).InternalElement.ParentElement))
            // Include associated classes for domain classes
            .Concat(relatedElements
                .Where(x => x.SpecializationTypeId == ClassSpecializationTypeId)
                .SelectMany(x => x.AssociatedElements
                    .Where(a => a.TypeReference?.Element != null)
                    .Select(a => a.TypeReference.Element as IElement)
                    .Where(a => a != null)))
            .ToList();
        return relatedClasses;
    }
}
