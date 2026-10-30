using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.Constants;

namespace Intent.Modules.CosmosDB.Templates
{
    // This is for disambiguating the extension methods
    using Intent.Modelers.Domain.Api;

    internal static class ValueObjectAssociationHelpers
    {
        /// <summary>
        /// The navigable association ends from a Value Object to other Value Objects which the domain Value Object
        /// class really exposes.
        /// </summary>
        /// <remarks>
        /// The domain class is the source of truth for what a Value Object contains, so only association ends the
        /// domain template has turned into members are included. A document member without a matching domain member
        /// could not be mapped back, which would otherwise be the case when paired with a version of
        /// <c>Intent.ValueObjects</c> which ignores associations.
        /// </remarks>
        public static IReadOnlyList<AssociationEndModel> GetValueObjectAssociationEnds(
            this CSharpTemplateBase<IElement> template,
            IElement valueObject)
        {
            if (!template.TryGetTemplate<ICSharpFileBuilderTemplate>(TemplateRoles.Domain.ValueObject, valueObject, out var domainTemplate))
            {
                return [];
            }

            var domainFile = domainTemplate.CSharpFile;

            // Record Value Objects declare their members through a primary constructor, which carries no per-member
            // metadata to check against.
            var domainClass = domainFile.Classes.FirstOrDefault();
            var domainMemberModelIds = domainClass?.Properties
                .Select(GetModelId)
                .Where(x => x != null)
                .ToHashSet();

            return valueObject.AssociatedElements
                .Where(x => x.IsAssociationEndModel())
                .Select(x => x.AsAssociationEndModel())
                .Where(x => x.IsTargetEnd() &&
                            x.IsNavigable &&
                            x.TypeReference.Element?.SpecializationType == "Value Object" &&
                            (domainMemberModelIds == null || domainMemberModelIds.Contains(x.Id)))
                .ToList();
        }

        private static string GetModelId(CSharpMetadataBase member)
        {
            return member.TryGetMetadata<IMetadataModel>("model", out var model) && model is AssociationEndModel
                ? model.Id
                : null;
        }
    }
}
