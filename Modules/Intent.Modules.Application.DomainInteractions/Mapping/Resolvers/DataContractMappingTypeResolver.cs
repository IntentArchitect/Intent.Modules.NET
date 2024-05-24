using Intent.Configuration;
using Intent.Modelers.Domain.Api;
using Intent.Modules.Common.CSharp.Mapping;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Templates;
using System;
using System.Collections.Generic;
using System.Text;

namespace Intent.Modules.Application.DomainInteractions.Mapping.Resolvers;

public class DataContractMappingTypeResolver : IMappingTypeResolver
{
	private readonly ICSharpFileBuilderTemplate _sourceTemplate;

	public DataContractMappingTypeResolver(ICSharpFileBuilderTemplate sourceTemplate)
	{
		_sourceTemplate = sourceTemplate;
	}

	public ICSharpMapping ResolveMappings(MappingModel mappingModel)
	{
		
		var model = mappingModel.Model;

		
		if (model.IsDataContractGeneralizationTargetEndModel())
		{
			var mapping = new DataContractInheritedChildrenMapping(mappingModel, _sourceTemplate);
			return mapping;
		}

		if (model.SpecializationType is "Stored Procedure Parameter" or "Stored Procedure")
			return null;

		if (model.TypeReference?.Element?.SpecializationType == "Data Contract" )
		{
			return new ConstructorMapping(mappingModel, _sourceTemplate);
		}

		return null;
	}
}