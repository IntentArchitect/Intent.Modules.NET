using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Intent.Metadata.Models;
using Intent.Modelers.Domain.Api;
using Intent.Modules.Application.AutoMapper.Templates.MapFromInterface;
using Intent.Modules.Application.Dtos.Templates.DtoModel;
using Intent.Modules.Common;
using Intent.Modules.Common.Templates;
using Intent.Modules.Entities.Templates.DomainEntityState;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.TemplateDecorator", Version = "1.0")]

namespace Intent.Modules.Application.Dtos.AutoMapper.Decorators
{
    [IntentManaged(Mode.Merge)]
    public class AutoMapperDtoDecorator : DtoModelDecorator, IDeclareUsings
    {
        [IntentManaged(Mode.Fully)]
        public const string DecoratorId = "Intent.Application.Dtos.AutoMapper.AutoMapperDtoDecorator";

        private readonly DtoModelTemplate _template;

        public AutoMapperDtoDecorator(DtoModelTemplate template)
        {
            _template = template;
        }

        public IEnumerable<string> DeclareUsings()
        {
            if (!_template.Model.IsMapped)
            {
                yield break;
            }

            yield return "AutoMapper";
        }

        public override IEnumerable<string> BaseInterfaces()
        {
            if (!_template.Model.IsMapped)
            {
                return base.BaseInterfaces();
            }
            return new[] { $"{_template.GetTypeName(MapFromInterfaceTemplate.TemplateId)}<{_template.GetTypeName(DomainEntityStateTemplate.Identifier, _template.Model.Mapping.ElementId)}>" };
        }

        public override string ExitClass()
        {
            if (!_template.Model.IsMapped)
            {
                return base.ExitClass();
            }

            var memberMappings = new List<string>();
            foreach (var field in _template.Model.Fields)
            {
                if (field.Mapping != null && field.Name.ToPascalCase() != GetPath(field.Mapping.Path))
                {
                    memberMappings.Add($@"
                .ForMember(d => d.{field.Name.ToPascalCase()}, opt => opt.MapFrom(src => src.{GetPath(field.Mapping.Path)}))");
                }
            }

            if (!memberMappings.Any())
            {
                return base.ExitClass();
            }

            return $@"
        public void Mapping(Profile profile)
        {{
            profile.CreateMap<{_template.GetTypeName(DomainEntityStateTemplate.Identifier, _template.Model.Mapping.ElementId)}, {_template.ClassName}>(){string.Join($"", memberMappings)};
        }}";
        }

        private string GetPath(IEnumerable<IElementMappingPathTarget> path)
        {
            return string.Join(".", path
                .Where(x => x.Specialization != GeneralizationModel.SpecializationType)
                .Select(x => x.Specialization == OperationModel.SpecializationType ? $"{x.Name.ToPascalCase()}()" : x.Name.ToPascalCase()));
        }
    }
}