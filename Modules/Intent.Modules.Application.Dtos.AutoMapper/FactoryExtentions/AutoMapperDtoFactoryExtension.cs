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
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Plugins;
using Intent.Modules.Common.Templates;
using Intent.Modules.Constants;
using Intent.Plugins.FactoryExtensions;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;
using GeneralizationModel = Intent.Modelers.Domain.Api.GeneralizationModel;
using OperationModel = Intent.Modelers.Domain.Api.OperationModel;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.FactoryExtension", Version = "1.0")]

namespace Intent.Modules.Application.Dtos.AutoMapper.FactoryExtentions
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class AutoMapperDtoFactoryExtension : FactoryExtensionBase
    {
        public override string Id => "Intent.Application.Dtos.AutoMapper.AutoMapperDtoFactoryExtension";

        [IntentManaged(Mode.Ignore)]
        public override int Order => 0;

        protected override void OnAfterTemplateRegistrations(IApplication application)
        {
            var templates = application.FindTemplateInstances<ICSharpFileBuilderTemplate>(TemplateDependency.OnTemplate(TemplateFulfillingRoles.Application.Contracts.Dto));
            foreach (DtoModelTemplate template in templates)
            {
                template.CSharpFile.AfterBuild(file =>
                {
                    var @class = file.Classes.First();
                    var templateModel = ((CSharpTemplateBase<DTOModel>)template).Model;

                    if (!templateModel.HasMapFromDomainMapping())
                    {
                        return;
                    }

                    var entityTemplate = GetEntityTemplate(template, templateModel);

                    file.AddUsing("AutoMapper");
                    @class.ImplementsInterface($"{template.GetTypeName(MapFromInterfaceTemplate.TemplateId)}<{entityTemplate.ClassName}>");

                    @class.AddMethod("void", "Mapping", method =>
                    {
                        method.AddParameter("Profile", "profile");
                        method.AddMethodChainStatement($"profile.CreateMap <{template.GetTypeName(entityTemplate)}, {template.ClassName}> ()", statement =>
                        {
                            foreach (var field in templateModel.Fields.Where(x => x.Mapping != null))
                            {
                                var shouldCast = template.GetTypeInfo(field.TypeReference).IsPrimitive
                                                 && field.Mapping.Element?.TypeReference != null
                                                 && template.GetFullyQualifiedTypeName(field.TypeReference) != entityTemplate.GetFullyQualifiedTypeName(field.Mapping.Element.TypeReference);
                                var mappingExpression = GetMappingExpression(template, field);
                                if ("src." + field.Name.ToPascalCase() != mappingExpression || shouldCast)
                                {
                                    var mapping = $@"
                .ForMember(d => d.{field.Name.ToPascalCase()}, opt => opt.MapFrom(src => {(shouldCast ? $"({template.GetTypeName(field)})" : string.Empty)}{mappingExpression}))";
                                    AddFieldMapping(statement, field, mapping);
                                }
                                else if (field.TypeReference.IsCollection && field.TypeReference.Element.Name != field.Mapping.Element?.TypeReference?.Element.Name)
                                {
                                    var mapping = $@"
                .ForMember(d => d.{field.Name.ToPascalCase()}, opt => opt.MapFrom(src => {mappingExpression}))";
                                    AddFieldMapping(statement, field, mapping);
                                }
                            }
                        });
                    });
                });
            }
        }

        private static void AddFieldMapping(CSharpMethodChainStatement statement, DTOFieldModel field, string mapping)
        {
            statement.AddChainStatement(mapping, chain =>
            {
                chain.AddMetadata("field", field);
            });
        }

        private string GetMappingExpression(ICSharpFileBuilderTemplate template, DTOFieldModel field)
        {
            var path = field.Mapping.Path;

            if (path.Count != 1
                || !path.First().Element.IsAssociationEndModel()
                || !template.GetTypeInfo(field.TypeReference).IsPrimitive)
            {
                return $"src.{GetPath(path)}";
            }

            var association = path.First().Element.AsAssociationEndModel().Association;
            var result = (association.SourceEnd.Multiplicity, association.TargetEnd.Multiplicity) switch
            {
                (Multiplicity.ZeroToOne, Multiplicity.ZeroToOne) => GetPK(path),
                (Multiplicity.ZeroToOne, Multiplicity.One) => GetPK(path),
                (Multiplicity.ZeroToOne, Multiplicity.Many) => GetMultiplePK(template, path),
                (Multiplicity.One, Multiplicity.ZeroToOne) => GetPK(path),
                (Multiplicity.One, Multiplicity.One) => GetPK(path),
                (Multiplicity.One, Multiplicity.Many) => GetMultiplePK(template, path),
                (Multiplicity.Many, Multiplicity.ZeroToOne) => GetLocalFK(path),
                (Multiplicity.Many, Multiplicity.One) => GetLocalFK(path),
                (Multiplicity.Many, Multiplicity.Many) => GetMultiplePK(template, path),
                _ => throw new InvalidOperationException($"Problem resolving association {association.SourceEnd.Multiplicity} -> {association.TargetEnd.Multiplicity}")
            };

            return result;
        }

        private string GetPK(IList<IElementMappingPathTarget> path)
        {
            return $"src.{GetPath(path)}.Id";
        }

        private string GetMultiplePK(ICSharpFileBuilderTemplate template, IList<IElementMappingPathTarget> path)
        {
            template.AddUsing("System.Linq");
            return $"src.{GetPath(path)}.Select(x => x.Id).ToArray()";
        }

        private string GetLocalFK(IList<IElementMappingPathTarget> path)
        {
            var association = path.First().Element.AsAssociationEndModel();
            return $"src.{association.Name.ToPascalCase()}Id";
        }

        private static string GetPath(IEnumerable<IElementMappingPathTarget> path)
        {
            return string.Join(".", path
                .Where(x => x.Specialization != GeneralizationModel.SpecializationType)
                .Select(x =>
                {
                    // Can't just .ToPascalCase(), since it turns string like "Count(x => x.IsAssigned())" into "Count(x => X.IsAssigned())"
                    var name = !string.IsNullOrWhiteSpace(x.Name)
                        ? char.ToUpperInvariant(x.Name[0]) + x.Name[1..]
                        : x.Name;

                    return x.Specialization == OperationModel.SpecializationType
                        ? $"{name}()"
                        : name;
                }));
        }


        private ICSharpFileBuilderTemplate GetEntityTemplate(ICSharpFileBuilderTemplate template, DTOModel templateModel)
        {
            if (template.TryGetTemplate(TemplateFulfillingRoles.Domain.Entity.Primary, templateModel.Mapping.ElementId, out ICSharpFileBuilderTemplate entityTemplate))
                return entityTemplate;
            if (template.TryGetTemplate(TemplateFulfillingRoles.Domain.ValueObject, templateModel.Mapping.ElementId, out entityTemplate))
                return entityTemplate;
            return entityTemplate;
        }
    }
}