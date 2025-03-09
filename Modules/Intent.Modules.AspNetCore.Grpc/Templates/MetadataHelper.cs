using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modelers.Services.Api;
using Intent.Modules.Common;
using static Intent.Modules.AspNetCore.Grpc.Templates.MetadataIds;

namespace Intent.Modules.AspNetCore.Grpc.Templates
{
    internal static class MetadataHelper
    {
        public static IEnumerable<IElement> GetMessageContracts(this IMetadataManager _metadataManager, IApplication application)
        {
            var commandsAndQueries = _metadataManager.Services(application).Elements
                .Where(x => x.SpecializationTypeId is CommandElementTypeId or QueryElementTypeId &&
                            x.HasStereotype(ExposeWithGrpcStereotypeId))
                .ToArray();

            var operations = _metadataManager.Services(application).Elements
                .Where(x => x.IsOperationModel() || x.HasStereotype(ExposeWithGrpcStereotypeId))
                .ToArray();

            return Enumerable.Empty<IElement>()
                .Union(commandsAndQueries)
                .Union(GetReferencedDtosRecursive(commandsAndQueries.Union(operations).ToArray()));
        }

        private static IEnumerable<IElement> GetReferencedDtosRecursive(IReadOnlyCollection<IElement> elements)
        {
            HashSet<IElement> alreadyChecked = [];

            return elements.SelectMany(x => Local(x, alreadyChecked));

            static IEnumerable<IElement> Local(
                IElement element,
                HashSet<IElement> alreadyChecked)
            {
                if (!alreadyChecked.Add(element))
                {
                    yield break;
                }

                if (element.IsDTOModel())
                {
                    yield return element;
                }

                var referencedElements = Enumerable.Empty<ITypeReference>()
                    .Append(element.TypeReference)
                    .Union(element.TypeReference?.GenericTypeParameters ?? [])
                    .Select(typeReference => typeReference?.Element)
                    .OfType<IElement>()
                    .Union(element.ChildElements);

                foreach (var referencedElement in referencedElements.SelectMany(x => Local(x, alreadyChecked)))
                {
                    yield return referencedElement;
                }
            }
        }
    }
}
