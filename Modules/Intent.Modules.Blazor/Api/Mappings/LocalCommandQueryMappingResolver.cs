using Intent.Exceptions;
using Intent.IArchitect.Agent.Persistence.Model.Module;
using Intent.Metadata.Models;
using Intent.Modules.Blazor.Settings;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Mapping;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Constants;
using Intent.Templates;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Collections.Specialized.BitVector32;

namespace Intent.Modules.Blazor.Api.Mappings;


public class LocalCommandQueryMappingResolver : IMappingTypeResolver
{
    private readonly ICSharpTemplate _template;

    private const string QueryModelSpecializationTypeId = "e71b0662-e29d-4db2-868b-8a12464b25d0";
    private const string CommandModelSpecializationTypeId = "ccf14eb6-3a55-4d81-b5b9-d27311c70cb9";

    public LocalCommandQueryMappingResolver(ICSharpTemplate template)
    {
        _template = template;
    }

    public ICSharpMapping? ResolveMappings(MappingModel mappingModel)
    {
        if (!_template.ExecutionContext.GetSettings().GetBlazor().RenderMode().IsInteractiveServer() ||
            mappingModel.Mapping == null ||
            mappingModel.Model.SpecializationTypeId is not (CommandModelSpecializationTypeId or QueryModelSpecializationTypeId))
        {
            return null;
        }

        if ((_template.TryGetTemplate<ICSharpFileBuilderTemplate>(TemplateRoles.Application.Command, mappingModel.Model.Id, out var templateInstance) ||
             _template.TryGetTemplate<ICSharpFileBuilderTemplate>(TemplateRoles.Application.Query, mappingModel.Model.Id, out templateInstance)) 
             )
        {
            var ctors = templateInstance.CSharpFile.Classes.FirstOrDefault(x => x.RepresentedModel?.Id == mappingModel.Model.Id)?.Constructors;
            if (ctors?.Any(x => x.Parameters.Count > 0) != true)
            {
                return new ParameterlessConstructor(mappingModel, _template, templateInstance);
            }
            else
            {
                var ctorToUse = ctors
                    .Where(c => c.AccessModifier == "public ")
                    .OrderByDescending(c => c.Parameters.Count)
                    .FirstOrDefault();
                if (ctorToUse !=null)
                {
                    _template.AddUsing(templateInstance.Namespace);
                    return new ConstructorMapping(mappingModel, _template, new ConstructorMappingOptions() { CtorToUse = ctorToUse });
                }
                return new ConstructorMapping(mappingModel, _template);
            }
        }
        return null;
    }

    private class ParameterlessConstructor(MappingModel model, ICSharpTemplate template, ICSharpFileBuilderTemplate commandOrQuery) : CSharpMappingBase(model, template)
    {        
        public override CSharpStatement GetSourceStatement(bool? targetIsNullable = null)
        {
            return $"new {Template.GetTypeName(commandOrQuery)}()";
        }
    }
}