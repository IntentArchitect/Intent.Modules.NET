
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modelers.Domain.Api;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Mapping;
using Intent.Modules.Common.CSharp.Templates;


namespace Intent.Modules.Application.DomainInteractions.Mapping.Resolvers;

internal class DataContractInheritedChildrenMapping : MapChildrenMapping
{
	public DataContractInheritedChildrenMapping(MappingModel model, ICSharpFileBuilderTemplate template) : base(model, template)
	{
	}

	public override bool TryGetTargetReplacement(IMetadataModel type, out string replacement)
	{
		var generalizationAssociation = Model.AsDataContractGeneralizationTargetEndModel();
		if (type.Id == generalizationAssociation.Id)
		{
			if (Parent is DataContractInheritedChildrenMapping)
			{
				return base.TryGetTargetReplacement(Parent.Model, out replacement);
			}
			var specializationType = generalizationAssociation.OtherEnd().Element;
			return base.TryGetTargetReplacement(specializationType, out replacement);
		}

		return base.TryGetTargetReplacement(type, out replacement);
	}
}

