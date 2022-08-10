using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modelers.Domain.Api;
using Intent.Modelers.Services.Api;
using Intent.Modules.Application.AutoMapper.Templates.MapFromInterface;
using Intent.Modules.Application.Dtos.Templates.DtoModel;
using Intent.Modules.Common;
using Intent.Modules.Common.Templates;
using Intent.Modules.Entities.Templates.DomainEntityState;
using Intent.RoslynWeaver.Attributes;
using OperationModel = Intent.Modelers.Domain.Api.OperationModel;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.TemplateDecorator", Version = "1.0")]

namespace Intent.Modules.Application.Dtos.AutoMapper.Decorators
{
    [IntentManaged(Mode.Merge)]
    public class AutoMapperDtoDecorator : DtoModelDecorator, IDeclareUsings
    {
        [IntentManaged(Mode.Fully)] public const string DecoratorId = "Intent.Application.Dtos.AutoMapper.AutoMapperDtoDecorator";

        [IntentManaged(Mode.Fully)] private readonly DtoModelTemplate _template;
        [IntentManaged(Mode.Fully)] private readonly IApplication _application;

        [IntentManaged(Mode.Merge, Body = Mode.Fully)]
        public AutoMapperDtoDecorator(DtoModelTemplate template, IApplication application)
        {
            _template = template;
            _application = application;
        }

        private DomainEntityStateTemplate _entityTemplate;

        public DomainEntityStateTemplate EntityTemplate => _entityTemplate ??=
            _template.GetTemplate<DomainEntityStateTemplate>(DomainEntityStateTemplate.TemplateId, _template.Model.Mapping.ElementId);

        public IEnumerable<string> DeclareUsings()
        {
            if (!_template.Model.HasMapFromDomainMapping())
            {
                yield break;
            }

            yield return "AutoMapper";
        }

        public override IEnumerable<string> BaseInterfaces()
        {
            if (!_template.Model.HasMapFromDomainMapping())
            {
                return base.BaseInterfaces();
            }

            return new[] { $"{_template.GetTypeName(MapFromInterfaceTemplate.TemplateId)}<{EntityTemplate.ClassName}>" };
        }

        public override string ExitClass()
        {
            if (!_template.Model.HasMapFromDomainMapping())
            {
                return base.ExitClass();
            }

            var memberMappings = new List<string>();
            foreach (var field in _template.Model.Fields.Where(x => x.Mapping != null))
            {
                var shouldCast = _template.GetTypeInfo(field.TypeReference).IsPrimitive
                                 && field.Mapping.Element?.TypeReference != null
                                 && _template.GetTypeInfo(field.TypeReference).Name != EntityTemplate.GetTypeInfo(field.Mapping.Element.TypeReference).Name;
                var mappingExpression = GetMappingExpression(field);
                if (field.Name.ToPascalCase() != mappingExpression || shouldCast)
                {
                    memberMappings.Add($@"
                .ForMember(d => d.{field.Name.ToPascalCase()}, opt => opt.MapFrom(src => {(shouldCast ? $"({_template.GetTypeName(field)})" : string.Empty)}src.{mappingExpression}))");
                }
                else if (field.TypeReference.IsCollection && field.TypeReference.Element.Name != field.Mapping.Element?.TypeReference?.Element.Name)
                {
                    memberMappings.Add($@"
                .ForMember(d => d.{field.Name.ToPascalCase()}, opt => opt.MapFrom(src => src.{mappingExpression}))");
                }
            }

            return $@"
        public void Mapping(Profile profile)
        {{
            profile.CreateMap<{_template.GetTypeName(DomainEntityStateTemplate.TemplateId, _template.Model.Mapping.ElementId)}, {_template.ClassName}>(){string.Join(string.Empty, memberMappings)};
        }}";
        }

        private string GetMappingExpression(DTOFieldModel field)
        {
            var path = field.Mapping.Path;
            var fieldType = field.TypeReference.Element.Name;

            if (path.Count != 1
                || !path.First().Element.IsAssociationEndModel()
                || !_template.IsSurrogateKeyType(fieldType))
            {
                return GetPath(path);
            }

            var association = path.First().Element.AsAssociationEndModel().Association;
            var result = (association.SourceEnd.Multiplicity, association.TargetEnd.Multiplicity) switch
            {
                (Multiplicity.ZeroToOne, Multiplicity.ZeroToOne) => GetPK(path),
                (Multiplicity.ZeroToOne, Multiplicity.One) => GetPK(path),
                (Multiplicity.ZeroToOne, Multiplicity.Many) => GetMultiplePK(path),
                (Multiplicity.One, Multiplicity.ZeroToOne) => GetPK(path),
                (Multiplicity.One, Multiplicity.One) => GetPK(path),
                (Multiplicity.One, Multiplicity.Many) => GetMultiplePK(path),
                (Multiplicity.Many, Multiplicity.ZeroToOne) => GetLocalFK(path),
                (Multiplicity.Many, Multiplicity.One) => GetLocalFK(path),
                (Multiplicity.Many, Multiplicity.Many) => GetMultiplePK(path),
                _ => throw new InvalidOperationException($"Problem resolving association {association.SourceEnd.Multiplicity} -> {association.TargetEnd.Multiplicity}")
            };

            return result;
        }

        private string GetPK(IList<IElementMappingPathTarget> path)
        {
            var association = path.First().Element.AsAssociationEndModel();
            var explicitPKs = association.Class.GetExplicitPrimaryKey();

            if (explicitPKs.Count == 0)
            {
                return $"{GetPath(path)}.Id";
            }
            if (explicitPKs.Count == 1)
            {
                return $"{GetPath(path)}.{explicitPKs.First().Name.ToPascalCase()}";
            }

            // We're not catering for composite keys
            return GetPath(path);
        }

        private string GetMultiplePK(IList<IElementMappingPathTarget> path)
        {
            var association = path.First().Element.AsAssociationEndModel();
            var explicitPKs = association.Class.GetExplicitPrimaryKey();

            if (explicitPKs.Count == 0)
            {
                return $"{GetPath(path)}.Select(x => x.Id).ToArray()";
            }
            if (explicitPKs.Count == 1)
            {
                _template.AddUsing("System.Linq");
                return $"{GetPath(path)}.Select(x => x.{explicitPKs.First().Name.ToPascalCase()}).ToArray()";
            }

            // We're not catering for composite keys
            return GetPath(path);
        }

        private string GetLocalFK(IList<IElementMappingPathTarget> path)
        {
            var association = path.First().Element.AsAssociationEndModel();
            var explicitPKs = association.Class.GetExplicitPrimaryKey();

            if (explicitPKs.Count == 0)
            {
                return $"{association.Name.ToPascalCase()}Id";
            }
            if (explicitPKs.Count == 1)
            {
                var pk = explicitPKs.First();
                return $"{association.Name.ToPascalCase()}{pk.Name.ToPascalCase()}";
            }

            // We're not catering for composite keys
            return GetPath(path);
        }

        private string GetPath(IEnumerable<IElementMappingPathTarget> path)
        {
            return string.Join(".", path
                .Where(x => x.Specialization != GeneralizationModel.SpecializationType)
                .Select(x => x.Specialization == OperationModel.SpecializationType ? $"{x.Name.ToPascalCase()}()" : x.Name.ToPascalCase()));
        }
    }
}