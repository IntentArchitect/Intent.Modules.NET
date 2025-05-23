using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Metadata.DocumentDB.Api;
using Intent.Metadata.Models;
using Intent.Modelers.Domain.Api;
using Intent.Modelers.Services.Api;
using Intent.Modules.Application.Dtos.Settings;
using Intent.Modules.Application.Dtos.Templates.DtoModel;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.Constants;
using Intent.Modules.Entities.Repositories.Api.Templates.EntityRepositoryInterface;
using Intent.RoslynWeaver.Attributes;
using GeneralizationModel = Intent.Modelers.Domain.Api.GeneralizationModel;
using OperationModel = Intent.Modelers.Domain.Api.OperationModel;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.FactoryExtension", Version = "1.0")]

namespace Intent.Modules.DocumentDB.Dtos.AutoMapper.CrossAggregateMappingConfigurator;

[IntentManaged(Mode.Fully, Body = Mode.Merge)]
internal static partial class CrossAggregateMappingConfigurator
{
    public static void Execute(IApplication application)
    {

        var templates = application.FindTemplateInstances<DtoModelTemplate>(TemplateDependency.OnTemplate(TemplateRoles.Application.Contracts.Dto));
        foreach (var template in templates)
        {
            var templateModel = ((CSharpTemplateBase<DTOModel>)template).Model;

            template.CSharpFile.AfterBuild(file =>
            {
                if (!IsDtoMappingAccrossAggregates(templateModel, application))
                    return;

                var entityTemplate = GetEntityTemplate(template, templateModel.Mapping.ElementId);
                var dtoClass = file.TypeDeclarations.First();
                if (application.GetSettings().GetAutoMapperSettings().ProfileLocation().IsProfileInDto())
                {
                    file.AddUsing("System.Linq");


                    var method = dtoClass.FindMethod("Mapping");
                    var mappingStatement = method.Statements.OfType<CSharpMethodChainStatement>().FirstOrDefault();
                    AddInMappingActions(application, template, template, templateModel, dtoClass, dtoClass, entityTemplate, mappingStatement);
                }
                else
                {
                    var profileTemplate = application.FindTemplateInstance<ICSharpFileBuilderTemplate>("Intent.Application.Dtos.AutoMapper.DtoMappingProfile", templateModel);
                    profileTemplate.CSharpFile.AddUsing("System.Linq");

                    var mappingClass = profileTemplate.CSharpFile.Classes.First(c => c.BaseType.Name.EndsWith("Profile"));
                    var ctor = mappingClass.Constructors.First();

                    var mappingStatement = ctor.Statements.OfType<CSharpMethodChainStatement>().FirstOrDefault();

                    AddInMappingActions(application, profileTemplate, template, templateModel, dtoClass, mappingClass, entityTemplate, mappingStatement);
                }

            }, 10);
        }
    }

    private static void AddInMappingActions(IApplication application, ICSharpFileBuilderTemplate mappingTemplate, ICSharpFileBuilderTemplate dtoTemplate, DTOModel templateModel, CSharpClass dtoClass, CSharpClass mappingClass, ICSharpFileBuilderTemplate entityTemplate, CSharpMethodChainStatement mappingStatement)
    {

        var mappedFields = new List<CrossAggregateMappedField>();

        var needToElevatePropertyAccessors = NeedToElevatePropertyAccessors(application);

        foreach (var field in templateModel.Fields.Where(x => x.Mapping != null))
        {
            for (int i = 0; i < field.Mapping.Path.Count; i++)
            {
                var pathPart = field.Mapping.Path[i];

                if (pathPart.Element is IAssociationEnd ae && IsAggregational(ae))
                {
                    var statementToRemove = mappingStatement.Statements.FirstOrDefault(s => s.TryGetMetadata<DTOFieldModel>("field", out var currentField) && currentField.Id == field.Id);
                    if (statementToRemove != null)
                    {
                        mappingStatement.Statements.Remove(statementToRemove);
                    }

                    mappedFields.Add(new CrossAggregateMappedField(field, ae, i));
                    //We done with this attribute
                    break;
                }
            }
        }
        mappingStatement.AddChainStatement($"AfterMap<MappingAction>()");

        if (needToElevatePropertyAccessors)
        {
            foreach (var mappedField in mappedFields)
            {
                var property = dtoClass.Properties.FirstOrDefault(s => s.TryGetMetadata<DTOFieldModel>("model", out var currentField) && currentField.Id == mappedField.Field.Id);
                if (property != null)
                    property.Setter.Internal();
            }
        }

        CreateMappingActionClass(mappingTemplate, dtoTemplate, mappingClass, entityTemplate, mappedFields);
    }

    private static bool NeedToElevatePropertyAccessors(IApplication application)
    {
        var setting = application.Settings.GetDTOSettings().PropertySetterAccessibility();
        return setting.IsPrivate() || setting.IsInit();
    }

    private static void CreateMappingActionClass(ICSharpFileBuilderTemplate mappingTemplate, ICSharpFileBuilderTemplate dtoTemplate, CSharpClass mappingClass, ICSharpFileBuilderTemplate entityTemplate, List<CrossAggregateMappedField> mappedFields)
    {
        var repositoriesNeeded = new HashSet<RepositoryInfo>();
        var aggregateLoadInstructions = new Dictionary<string, LoadInstruction>();

        foreach (var mapping in mappedFields)
        {
            var parentAggregate = new PathAggregate(-1, null);
            for (int i = mapping.PathIndex; i < mapping.Field.Mapping.Path.Count; i++)
            {
                var currentPart = mapping.Field.Mapping.Path[i];
                if (currentPart.Element is IAssociationEnd ae && IsAggregational(ae))
                {
                    var aggregate = new PathAggregate(i, GetPathExpression(mapping.Field.Mapping.Path.Take(i + 1)));
                    if (!aggregateLoadInstructions.ContainsKey(aggregate.Expression))
                    {
                        if (!mappingTemplate.TryGetTypeName(EntityRepositoryInterfaceTemplate.TemplateId, ae.AsAssociationEndModel().Class, out var pathRepoInterfaceType))
                        {
                            throw new Exception($"No repository found for {ae.AsAssociationEndModel().Class.Name}");
                        }

                        var repo = new RepositoryInfo(pathRepoInterfaceType);
                        repositoriesNeeded.Add(repo);

                        string fieldPath = "";
                        if (parentAggregate.Index + 1 != i)
                        {
                            var skip = parentAggregate.Index + 1;
                            var take = i - parentAggregate.Index - 1;
                            fieldPath = $".{GetPathExpression(mapping.Field.Mapping.Path.Skip(skip).Take(take))}";
                        }
                        aggregateLoadInstructions.Add(aggregate.Expression, new LoadInstruction(repo, ae.AsAssociationEndModel(), aggregate.Expression, parentAggregate.Expression ?? "source", fieldPath));
                    }
                    parentAggregate = aggregate;
                }
            }
        }

        mappingClass.AddNestedClass($"MappingAction", child =>
        {
            child.Internal();
            child.ImplementsInterface($"IMappingAction<{mappingTemplate.GetTypeName(entityTemplate)},{dtoTemplate.ClassName}>");

            child.AddConstructor(con =>
            {
                foreach (var repo in repositoriesNeeded)
                {
                    con.AddParameter(repo.InterfaceName, repo.ArgumentName, param => param.IntroduceReadonlyField());
                }
                con.AddParameter("IMapper", "mapper", param => param.IntroduceReadonlyField());
            });

            child.AddMethod("void", "Process", method =>
            {
                method.AddParameter(mappingTemplate.GetTypeName(entityTemplate), "source");
                method.AddParameter(dtoTemplate.ClassName, "destination");
                method.AddParameter("ResolutionContext", "context");

                //Aggregates depend on parent aggregates for loading, order by path length handles this automatically 
                var orderedLoads = aggregateLoadInstructions.Select(x => x.Value).OrderBy(l => l.PathExpression.Length);
                foreach (var load in orderedLoads)
                {
                    var fkEntityModel = load.AssociationEndModel.OtherEnd().Class;
                    var fkAttribute = fkEntityModel.Attributes.FirstOrDefault(a => a.HasForeignKey() && a.GetForeignKey().Association().Id == load.AssociationEndModel.Association.TargetEnd.Id);

                    if (fkAttribute == null)
                    {
                        throw new Exception($"No Foreign Key found on : {fkEntityModel.Name} to load associate Aggregate {load.AssociationEndModel.Class.Name}");
                    }

                    var fkExpression = fkAttribute.TypeReference.IsCollection ? $"{fkAttribute.Name.ToPascalCase()}{(fkAttribute.TypeReference.IsNullable ? "?" : "")}.ToArray()" : fkAttribute.Name.ToPascalCase();
                    var valueAccessorExpression = fkAttribute.TypeReference.IsNullable && mappingTemplate.GetTypeInfo(fkAttribute.TypeReference).IsPrimitive
                        ? ".Value"
                        : string.Empty;
                    
                    if (load.IsExpressionOptional)
                    {
                        method.AddStatement($"var {load.Variable} = {load.FieldPath}.{fkExpression} != null ? {load.Repository.FieldName}.{(fkAttribute.TypeReference.IsCollection ? "FindByIdsAsync" : "FindByIdAsync")}({load.FieldPath.Replace("?", "")}.{fkExpression.Replace("?", "")}{valueAccessorExpression}).Result : null;");
                    }
                    else if (load.IsOptional)
                    {
                        method.AddStatement($"var {load.Variable} = {load.FieldPath}.{fkExpression} != null ? {load.Repository.FieldName}.{(fkAttribute.TypeReference.IsCollection ? "FindByIdsAsync" : "FindByIdAsync")}({load.FieldPath}.{fkExpression.Replace("?", "")}{valueAccessorExpression}).Result : null;");
                    }
                    else
                    {
                        method.AddStatement($"var {load.Variable} = {load.Repository.FieldName}.{(fkAttribute.TypeReference.IsCollection ? "FindByIdsAsync" : "FindByIdAsync")}({load.FieldPath}.{fkExpression}).Result;");
                    }
                        
                    if (!fkAttribute.TypeReference.IsCollection && !load.IsOptional && !load.IsExpressionOptional)
                    {
                        method.AddIfStatement($"{load.Variable} == null", ifs =>
                        {
                            ifs.AddStatement($"throw new {mappingTemplate.GetNotFoundExceptionName()}($\"Unable to load required relationship for Id({{{load.FieldPath}.{fkExpression}}}). ({load.AssociationEndModel.OtherEnd().Class.Name})->({load.AssociationEndModel.Class.Name})\");");
                        });
                    }
                }

                foreach (var mapping in mappedFields)
                {
                    string aggregateExpression = GetAggregatePathExpression(mappingTemplate, mapping.Field, mapping.Field.Mapping.Path, out var fieldPath);
                    var load = aggregateLoadInstructions[aggregateExpression];
                    if (load.IsOptional || load.IsExpressionOptional)
                    {
                        method.AddStatement($"destination.{mapping.Field.Name} = {load.Variable} != null ? {load.Variable}.{fieldPath} : null;");
                    }
                    else
                    {
                        method.AddStatement($"destination.{mapping.Field.Name} = {load.Variable}.{fieldPath};");
                    }
                }
            });
        });
    }


    private static string GetAggregatePathExpression(ICSharpFileBuilderTemplate template, DTOFieldModel field, IList<IElementMappingPathTarget> path, out string fieldPath)
    {
        fieldPath = null;
        for (int i = path.Count - 1; i >= 0; i--)
        {
            if (path[i].Element is IAssociationEnd ae && IsAggregational(ae))
            {
                fieldPath = GetPathExpression(path.Skip(i + 1));
                if (field.TypeReference.Element.IsDTOModel())
                {
                    string mapFunction = GetMappingFunction(template, field);
                    if (fieldPath.Length > 0)
                        fieldPath += ".";
                    fieldPath = $"{fieldPath}{mapFunction}(_mapper)";
                }
                return GetPathExpression(path.Take(i + 1));
            }
        }
        throw new Exception("Aggregate not found");
    }

    private static string GetMappingFunction(ICSharpFileBuilderTemplate template, DTOFieldModel field)
    {
        var result = field.TypeReference.IsCollection
            ? $"MapTo{template.GetTypeName(field.TypeReference, "{0}")}List"
            : $"MapTo{template.GetTypeName(field.TypeReference)}";
        if (field.TypeReference.IsNullable)
        {
            return result.Replace("?", "");
        }
        return result;
    }

    private static bool IsDtoMappingAccrossAggregates(DTOModel templateModel, IApplication application)
    {
        foreach (var field in templateModel.Fields.Where(x => x.Mapping != null))
        {
            foreach (var pathPart in field.Mapping.Path)
            {
                var entityTemplate = application.FindTemplateInstance<ICSharpFileBuilderTemplate>(TemplateRoles.Domain.Entity.Primary, templateModel.Mapping.ElementId);
                if (pathPart.Element is IAssociationEnd ae && IsAggregational(ae))
                {
                    if (entityTemplate?.CSharpFile.Classes.First().Properties.All(x => !x.Name.Equals(ae.Name, StringComparison.InvariantCultureIgnoreCase)) == true)
                    {
                        return true;
                    }
                }
            }
        }
        return false;
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
            .Select(PathName));
    }

    private static bool IsAggregational(IAssociationEnd ae)
    {
        return ae.Association.SourceEnd.TypeReference.IsCollection || ae.Association.SourceEnd.TypeReference.IsNullable;
    }


    private static ICSharpFileBuilderTemplate GetEntityTemplate(ICSharpFileBuilderTemplate template, string modelId)
    {
        if (template.TryGetTemplate(TemplateRoles.Domain.Entity.Primary, modelId, out ICSharpFileBuilderTemplate entityTemplate))
            return entityTemplate;
        if (template.TryGetTemplate(TemplateRoles.Domain.ValueObject, modelId, out entityTemplate))
            return entityTemplate;
        return entityTemplate;
    }

    private static string GetNotFoundExceptionName(this ICSharpTemplate template)
    {
        return template.GetTypeName("Domain.NotFoundException", TemplateDiscoveryOptions.DoNotThrow);
    }

}