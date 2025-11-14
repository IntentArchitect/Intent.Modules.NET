using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modelers.Domain.Api;
using Intent.Modelers.Services.Api;

namespace Intent.Modules.AI.UnitTests.Utilities;

public static class UnitTestHelpers
{
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

    public static List<ICanBeReferencedType> GetRelatedElements(IElement element)
    {
        var relatedElements = element.AssociatedElements.Where(x => x.TypeReference.Element != null)
            .Select(x => x.TypeReference.Element)
            .ToList();
        if (relatedElements.Count == 0)
        {
            return [];
        }
        var relatedClasses = relatedElements
            .Concat(relatedElements.Where(x => x.TypeReference?.Element?.IsDTOModel() == true).Select(x => x.TypeReference.Element))
            .Concat(relatedElements.Where(Intent.Modelers.Services.Api.OperationModelExtensions.IsOperationModel).Select(x => Intent.Modelers.Services.Api.OperationModelExtensions.AsOperationModel(x).ParentService.InternalElement))
            .Concat(relatedElements.Where(Intent.Modules.Common.Types.Api.OperationModelExtensions.IsOperationModel).Select(x => Intent.Modules.Common.Types.Api.OperationModelExtensions.AsOperationModel(x).InternalElement.ParentElement))
            .Concat(relatedElements.Where(x => x.IsClassModel()).Select(x => x.AsClassModel()).SelectMany(x => x.AssociatedClasses.Select(y => y.TypeReference.Element)))
            .ToList();
        return relatedClasses;
    }
}
