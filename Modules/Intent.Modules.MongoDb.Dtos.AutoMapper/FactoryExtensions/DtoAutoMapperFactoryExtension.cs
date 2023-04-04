using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modelers.Domain.Api;
using Intent.Modelers.Services.Api;
using Intent.Modules.Application.Dtos.Templates.DtoModel;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Plugins;
using Intent.Modules.Common.Templates;
using Intent.Modules.Constants;
using Intent.Modules.Entities.Repositories.Api.Templates.EntityRepositoryInterface;
using Intent.MongoDb.Api;
using Intent.Plugins.FactoryExtensions;
using Intent.RoslynWeaver.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using GeneralizationModel = Intent.Modelers.Domain.Api.GeneralizationModel;
using OperationModel = Intent.Modelers.Domain.Api.OperationModel;
using Intent.Metadata.RDBMS.Api;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.FactoryExtension", Version = "1.0")]

namespace Intent.Modules.MongoDb.Dtos.AutoMapper.FactoryExtensions
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class DtoAutoMapperFactoryExtension : FactoryExtensionBase
    {
        public override string Id => "Intent.MongoDb.Dtos.AutoMapper.DtoAutoMapperFactoryExtension";

        [IntentManaged(Mode.Ignore)]
        public override int Order => 0;

        /// <summary>
        /// This is an example override which would extend the
        /// <see cref="ExecutionLifeCycleSteps.Start"/> phase of the Software Factory execution.
        /// See <see cref="FactoryExtensionBase"/> for all available overrides.
        /// </summary>
        /// <remarks>
        /// It is safe to update or delete this method.
        /// </remarks>
        protected override void OnAfterTemplateRegistrations(IApplication application)
        {
            var templates = application.FindTemplateInstances<ICSharpFileBuilderTemplate>(TemplateDependency.OnTemplate(TemplateFulfillingRoles.Application.Contracts.Dto));
            foreach (DtoModelTemplate template in templates)
            {
                var templateModel = ((CSharpTemplateBase<DTOModel>)template).Model;

                var dtoNeedsChange = CheckIfDtoNeedsChanges(templateModel);

                if (!dtoNeedsChange)
                    continue;

                template.CSharpFile.AfterBuild(file =>
                {
                    file.AddUsing("System.Linq");
                    var @class = file.Classes.First();
                    var entityTemplate = GetEntityTemplate(template, templateModel.Mapping.ElementId);

                    var method = @class.FindMethod("Mapping");
                    var mappingStatement = method.Statements.OfType<CSharpMethodChainStatement>().FirstOrDefault();

                    var actionMaps = new Dictionary<string, ActionMapData>();

                    foreach (var field in templateModel.Fields.Where(x => x.Mapping != null))
                    {
                        string actionMapPath = "";
                        int pathSkip = 0;
                        foreach (var pathPart in field.Mapping.Path)
                        {
                            pathSkip++;
                            if (pathPart.Specialization != GeneralizationModel.SpecializationType)
                            {
                                actionMapPath += PathName(pathPart);
                            }
                            if (pathPart.Element is IAssociationEnd ae && IsAggregational(ae))
                            {
                                var statementToRemove = mappingStatement.Statements.FirstOrDefault(s => s.TryGetMetadata<DTOFieldModel>("field", out var currentField) ? currentField.Id == field.Id : false);
                                if (statementToRemove != null)
                                {
                                    mappingStatement.Statements.Remove(statementToRemove);
                                }
                                if (!actionMaps.TryGetValue(actionMapPath, out var mapData))
                                {
                                    actionMaps.Add(actionMapPath, new ActionMapData(ae, new List<DTOFieldModel> { field }, pathSkip));
                                    mappingStatement.AddChainStatement($"BeforeMap<{actionMapPath}Action>()");
                                }
                                else
                                {
                                    mapData.Fields.Add(field);
                                }
                            }
                        }
                    }
                    foreach (var actionMap in actionMaps)
                    {
                        CreateMappingActionClass(template, @class, entityTemplate, actionMap);
                    }
                }, 10);
            }
        }

        private static void CreateMappingActionClass(DtoModelTemplate template, CSharpClass @class, ICSharpFileBuilderTemplate entityTemplate, KeyValuePair<string, ActionMapData> actionMap)
        {
            var mapData = actionMap.Value;
            @class.AddNestedClass($"{actionMap.Key}Action", child =>
            {
                child.Internal();
                child.ImplementsInterface($"IMappingAction<{template.GetTypeName(entityTemplate)},{template.ClassName}>");

                if (!template.TryGetTypeName(EntityRepositoryInterfaceTemplate.TemplateId, mapData.AssociationEnd.AsAssociationEndModel().Class, out var repoInterfaceType))
                {
                    throw new Exception($"No repository found for {mapData.AssociationEnd.AsAssociationEndModel().Class.Name}");
                }
                child.AddProperty(repoInterfaceType, "Repository", p =>
                {
                    p.WithoutSetter();
                });
                child.AddProperty("IMapper", "Mapper", p =>
                {
                    p.WithoutSetter();
                });

                child.AddConstructor(con =>
                {
                    con.AddParameter(repoInterfaceType, "repository");
                    con.AddParameter("IMapper", "mapper");
                    con.AddStatement("Repository = repository;");
                    con.AddStatement("Mapper = mapper;");
                });

                child.AddMethod("void", "Process", method =>
                {
                    method.AddParameter(template.GetTypeName(entityTemplate), "source");
                    method.AddParameter(template.ClassName, "destination");
                    method.AddParameter("ResolutionContext", "context");

                    var fkEntityTemplate = mapData.AssociationEnd.OtherEnd().AsAssociationEndModel().Class;
                    var fkAttribure = fkEntityTemplate.Attributes.FirstOrDefault(a => a.HasForeignKey() && a.GetForeignKey().Association().Id == mapData.AssociationEnd.Association.TargetEnd.Id);
                    
                    if (fkAttribure == null)
                    {
                        throw new Exception($"No Foreign Key found on : {fkEntityTemplate.Name} to load associate Aggregate {mapData.AssociationEnd.AsAssociationEndModel().Class.Name}");
                    }

                    string fkExpression = fkAttribure.TypeReference.IsCollection ? $"{fkAttribure.Name}.ToArray()" : fkAttribure.Name;
                    
                    method.AddStatement($"var data = Repository.{(fkAttribure.TypeReference.IsCollection ? "FindByIdsAsync" : "FindByIdAsync")}(source.{fkExpression}).Result;");
                    foreach (var field in mapData.Fields)
                    {
                        if (field.TypeReference.Element.IsDTOModel())
                        {
                            string mapFunction = GetMappingFunction(template, field);
                            method.AddStatement($"destination.{field.Name} = data.{GetPathExpression(field.Mapping.Path.Skip(mapData.PathSkip))}{mapFunction}(Mapper);");
                        }
                        else
                        {
                            method.AddStatement($"destination.{field.Name} = data.{GetPathExpression(field.Mapping.Path.Skip(mapData.PathSkip))};");
                        }
                    }
                });
            });
        }

        private static string GetMappingFunction(DtoModelTemplate template, DTOFieldModel field)
        {
            return field.TypeReference.IsCollection ? 
                $"MapTo{template.GetTypeName(field.TypeReference, "{0}" )}List" 
                : $"MapTo{template.GetTypeName(field.TypeReference)}";
        }

        private static bool CheckIfDtoNeedsChanges(DTOModel templateModel)
        {
            foreach (var field in templateModel.Fields.Where(x => x.Mapping != null))
            {
                foreach (var pathPart in field.Mapping.Path)
                {
                    if (pathPart.Element is IAssociationEnd ae && IsAggregational(ae))
                    {
                        if (ae.Package.IsMongoDomainPackageModel())
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        private class ActionMapData
        {
            public ActionMapData(IAssociationEnd associationEnd, List<DTOFieldModel> fields, int pathSkip)
            {
                AssociationEnd = associationEnd;
                Fields = fields;
                PathSkip = pathSkip;
            }

            public int PathSkip { get; }
            public IAssociationEnd AssociationEnd { get; }
            public List<DTOFieldModel> Fields { get; }
        }

        private static string PathName(IElementMappingPathTarget pathPart)
        {
            // Can't just .ToPascalCase(), since it turns string like "Count(x => x.IsAssigned())" into "Count(x => X.IsAssigned())"
            var name = !string.IsNullOrWhiteSpace(pathPart.Name)
                ? char.ToUpperInvariant(pathPart.Name[0]) + pathPart.Name[1..]
                : pathPart.Name;

            return pathPart.Specialization == OperationModel.SpecializationType
                ? $"{name}()"
                : pathPart.Element.TypeReference.IsNullable 
                    ? $"{name}?" 
                    : name;
        }

        private static string GetPathExpression(IEnumerable<IElementMappingPathTarget> path)
        {
            return string.Join(".", path
                .Where(x => x.Specialization != GeneralizationModel.SpecializationType)
                .Select(x =>
                {
                    return PathName(x);
                }));
        }

        private static bool IsAggregational(IAssociationEnd ae)
        {            
            return ae.Association.SourceEnd.TypeReference.IsCollection || ae.Association.SourceEnd.TypeReference.IsNullable;
        }


        private static ICSharpFileBuilderTemplate GetEntityTemplate(ICSharpFileBuilderTemplate template, string modelId)
        {
            if (template.TryGetTemplate(TemplateFulfillingRoles.Domain.Entity.Primary, modelId, out ICSharpFileBuilderTemplate entityTemplate))
                return entityTemplate;
            if (template.TryGetTemplate(TemplateFulfillingRoles.Domain.ValueObject, modelId, out entityTemplate))
                return entityTemplate;
            return entityTemplate;
        }
    }
}