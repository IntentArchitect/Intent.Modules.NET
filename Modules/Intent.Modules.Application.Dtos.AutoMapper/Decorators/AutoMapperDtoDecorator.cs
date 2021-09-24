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
using Intent.Engine;

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

        [IntentManaged(Mode.Merge, Body = Mode.Fully)]
        public AutoMapperDtoDecorator(DtoModelTemplate template, IApplication application)
        {
            _template = template;
            _application = application;
        }

        private DomainEntityStateTemplate _entityTemplate;
        public DomainEntityStateTemplate EntityTemplate => _entityTemplate ??= _template.GetTemplate<DomainEntityStateTemplate>(DomainEntityStateTemplate.TemplateId, _template.Model.Mapping.ElementId);

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
            return new[] { $"{_template.GetTypeName(MapFromInterfaceTemplate.TemplateId)}<{EntityTemplate.ClassName}>" };
        }

        public override string ExitClass()
        {
            if (!_template.Model.IsMapped)
            {
                return base.ExitClass();
            }

            var memberMappings = new List<string>();
            foreach (var field in _template.Model.Fields.Where(x => x.Mapping != null))
            {
                var shouldCast = _template.GetTypeInfo(field.TypeReference).IsPrimitive &&
                                 field.Mapping.Element?.TypeReference != null &&
                                 _template.GetTypeInfo(field.TypeReference).Name != EntityTemplate.GetTypeInfo(field.Mapping.Element.TypeReference).Name;
                if (field.Name.ToPascalCase() != GetPath(field.Mapping.Path) || shouldCast)
                {
                    memberMappings.Add($@"
                .ForMember(d => d.{field.Name.ToPascalCase()}, opt => opt.MapFrom(src => {(shouldCast ? $"({_template.GetTypeName(field)})" : "")}src.{GetPath(field.Mapping.Path)}))");
                }
            }

            //if (!memberMappings.Any())
            //{
            //    return base.ExitClass();
            //}

            return $@"
        public void Mapping(Profile profile)
        {{
            profile.CreateMap<{_template.GetTypeName(DomainEntityStateTemplate.TemplateId, _template.Model.Mapping.ElementId)}, {_template.ClassName}>(){string.Join($"", memberMappings)};
        }}";
        }

        private string GetPath(IEnumerable<IElementMappingPathTarget> path)
        {
            return string.Join(".", path
                .Where(x => x.Specialization != GeneralizationModel.SpecializationType)
                .Select(x => x.Specialization == OperationModel.SpecializationType ? $"{x.Name.ToPascalCase()}()" : x.Name.ToPascalCase()));
        }
        private readonly IApplication _application;
    }
}