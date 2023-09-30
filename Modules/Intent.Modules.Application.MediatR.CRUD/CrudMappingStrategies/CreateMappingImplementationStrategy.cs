using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modelers.Domain.Api;
using Intent.Modelers.Services.CQRS.Api;
using Intent.Modelers.Services.DomainInteractions.Api;
using Intent.Modules.Application.MediatR.CRUD.CrudStrategies;
using Intent.Modules.Application.MediatR.CRUD.Decorators;
using Intent.Modules.Application.MediatR.CRUD.Mapping.Resolvers;
using Intent.Modules.Application.MediatR.Templates;
using Intent.Modules.Application.MediatR.Templates.CommandHandler;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Mapping;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.Constants;
using Intent.Modules.Entities.Repositories.Api.Templates;
using Intent.Modules.Entities.Settings;
using Intent.Modules.Modelers.Domain.Settings;
using Intent.Templates;
using static Intent.Modules.Constants.TemplateFulfillingRoles.Domain;

namespace Intent.Modules.Application.MediatR.CRUD.CrudMappingStrategies
{
    public class CreateMappingImplementationStrategy : ICrudImplementationStrategy
    {
        private readonly CommandHandlerTemplate _template;
        private readonly CSharpClassMappingManager _csharpMapping;
        private CSharpConstructor _ctor;
        private CSharpClassMethod _handleMethod;


        public CreateMappingImplementationStrategy(CommandHandlerTemplate template)
        {
            _template = template;
            var model = (_template as ITemplateWithModel)?.Model as CommandModel;
            var commandTemplate = _template.ExecutionContext.FindTemplateInstance<ICSharpFileBuilderTemplate>(TemplateFulfillingRoles.Application.Command, model);
            _csharpMapping = new CSharpClassMappingManager(commandTemplate);
            _csharpMapping.AddMappingResolver(new EntityCreationMappingTypeResolver(_template));
            _csharpMapping.AddMappingResolver(new EntityUpdateMappingTypeResolver(_template));
            _csharpMapping.AddMappingResolver(new StandardDomainMappingTypeResolver(_template));
            _csharpMapping.AddMappingResolver(new ValueObjectMappingTypeResolver(_template));

            _csharpMapping.SetFromReplacement(model.InternalElement, "request");
            _template.CSharpFile.Classes.First().AddMetadata("mapping-manager", _csharpMapping);

        }

        public bool IsMatch()
        {
            return _template.Model.CreateEntityActions().Any() || _template.Model.UpdateEntityActions().Any() || _template.Model.DeleteEntityActions().Any();
        }

        public void ApplyStrategy()
        {
            var @class = _template.CSharpFile.Classes.First();
            _ctor = @class.Constructors.First();
            _handleMethod = @class.FindMethod("Handle");

            _template.AddTypeSource(TemplateFulfillingRoles.Domain.Entity.Primary);
            _template.AddTypeSource(TemplateFulfillingRoles.Domain.ValueObject);
            _template.AddUsing("System.Linq");

            _handleMethod.Statements.Clear();
            _handleMethod.Attributes.OfType<CSharpIntentManagedAttribute>().SingleOrDefault()?.WithBodyFully();

            var trackedEntities = new HashSet<EntityDetails>();
            foreach (var createAction in _template.Model.CreateEntityActions())
            {
                var entity = createAction.Element.AsClassModel() ?? createAction.Element.AsClassConstructorModel()?.ParentClass;

                InjectRepositoryForEntity(entity, out var repositoryFieldName);

                trackedEntities.Add(new EntityDetails(entity, createAction.Name, repositoryFieldName, true));

                var mapping = createAction.Mappings.SingleOrDefault();
                var entityVariableName = createAction.Name;
                _handleMethod.AddStatement(new CSharpAssignmentStatement($"var {entityVariableName}", _csharpMapping.GenerateCreationStatement(mapping)).WithSemicolon());

                _csharpMapping.SetFromReplacement(createAction.InternalAssociationEnd, entityVariableName);
                _csharpMapping.SetFromReplacement(entity, entityVariableName);
                _csharpMapping.SetToReplacement(createAction.InternalAssociationEnd, entityVariableName);

                foreach (var actions in createAction.ProcessingActions)
                {
                    _handleMethod.AddStatement(string.Empty);
                    _handleMethod.AddStatements(_csharpMapping.GenerateUpdateStatements(actions.InternalElement.Mappings.Single()));
                    _handleMethod.AddStatement(string.Empty);
                }
            }

            foreach (var updateAction in _template.Model.UpdateEntityActions())
            {
                var foundEntity = updateAction.Element.AsClassModel() ?? updateAction.Element.AsOperationModel()?.ParentClass;
                var queryMapping = updateAction.Mappings.GetQueryEntityMapping();
                var updateMapping = updateAction.Mappings.GetUpdateEntityMapping();

                if (foundEntity != null && queryMapping != null)
                {
                    var entityDetails = QueryEntity(foundEntity, updateAction.InternalAssociationEnd);
                    trackedEntities.Add(entityDetails);

                    _handleMethod.AddStatement(string.Empty);

                    if (entityDetails.IsCollection)
                    {
                        _csharpMapping.SetToReplacement(foundEntity, entityDetails.VariableName.Singularize());
                        _handleMethod.AddStatement(new CSharpForEachStatement(entityDetails.VariableName.Singularize(), entityDetails.VariableName)
                            .AddStatements(_csharpMapping.GenerateUpdateStatements(updateMapping)));
                        if (RepositoryRequiresExplicitUpdate(foundEntity))
                        {
                            _handleMethod.AddStatement(new CSharpInvocationStatement(entityDetails.RepositoryFieldName, "Update").AddArgument(entityDetails.VariableName.Singularize()));
                        }
                    }
                    else
                    {
                        _handleMethod.AddStatements(_csharpMapping.GenerateUpdateStatements(updateMapping));
                        if (RepositoryRequiresExplicitUpdate(foundEntity))
                        {
                            _handleMethod.AddStatement(new CSharpInvocationStatement(entityDetails.RepositoryFieldName, "Update").AddArgument(entityDetails.VariableName));
                        }
                    }

                    foreach (var actions in updateAction.ProcessingActions)
                    {
                        _handleMethod.AddStatement(string.Empty);
                        _handleMethod.AddStatements(_csharpMapping.GenerateUpdateStatements(actions.InternalElement.Mappings.Single()));
                        _handleMethod.AddStatement(string.Empty);
                    }
                }
            }

            foreach (var deleteAction in _template.Model.DeleteEntityActions())
            {
                var foundEntity = deleteAction.Element.AsClassModel();
                if (foundEntity != null)
                {
                    var entityDetails = QueryEntity(foundEntity, deleteAction.InternalAssociationEnd);
                    trackedEntities.Add(entityDetails);

                    if (entityDetails.IsCollection)
                    {
                        _handleMethod.AddStatement(new CSharpForEachStatement(entityDetails.VariableName.Singularize(), entityDetails.VariableName)
                            .AddStatement(new CSharpInvocationStatement(entityDetails.RepositoryFieldName, "Remove").AddArgument(entityDetails.VariableName.Singularize())));
                    }
                    else
                    {
                        _handleMethod.AddStatement(new CSharpInvocationStatement(entityDetails.RepositoryFieldName, "Remove").AddArgument(entityDetails.VariableName));
                    }
                }
            }

            foreach (var entity in trackedEntities.Where(x => x.IsNew))
            {
                _handleMethod.AddStatement($"{entity.RepositoryFieldName}.Add({entity.VariableName});");
            }

            if (_template.Model.TypeReference.Element != null)
            {
                var entityVariableName = trackedEntities.First().VariableName;
                var repositoryFieldName = trackedEntities.First().RepositoryFieldName;
                var foundEntity = trackedEntities.First().Model;
                if (_template.TryGetTypeName("Application.Contract.Dto", _template.Model.TypeReference.Element, out var returnDto))
                {
                    _ctor.AddParameter(_template.UseType("AutoMapper.IMapper"), "mapper", param => param.IntroduceReadonlyField());
                    _handleMethod.AddStatement($"return {entityVariableName}.MapTo{returnDto}(_mapper);");
                }
                else
                {
                    _handleMethod.AddStatement($"await {repositoryFieldName}.UnitOfWork.SaveChangesAsync(cancellationToken);");
                    _handleMethod.AddStatement($"return {entityVariableName}.{foundEntity.Attributes.Concat(foundEntity.ParentClass?.Attributes ?? new List<AttributeModel>()).FirstOrDefault(x => x.IsPrimaryKey())?.Name.ToPascalCase() ?? "Id"};");
                }
            }
        }

        private EntityDetails QueryEntity(ClassModel foundEntity, IAssociationEnd associationEnd)
        {
            var queryMapping = associationEnd.Mappings.GetQueryEntityMapping();

            var entityVariableName = associationEnd.Name;

            _csharpMapping.SetFromReplacement(foundEntity, entityVariableName);
            _csharpMapping.SetFromReplacement(associationEnd, entityVariableName);
            _csharpMapping.SetToReplacement(foundEntity, entityVariableName);
            _csharpMapping.SetToReplacement(associationEnd, entityVariableName);

            InjectRepositoryForEntity(foundEntity, out var repositoryFieldName);

            if (queryMapping.MappedEnds.Any() && queryMapping.MappedEnds.All(x => x.TargetElement.AsAttributeModel()?.IsPrimaryKey() == true))
            {
                var idFields = queryMapping.MappedEnds
                    .OrderBy(x => ((IElement)x.TargetElement).Order)
                    .Select(x => _csharpMapping.GenerateSourceStatementForMapping(queryMapping, x))
                    .ToList();

                if (associationEnd.TypeReference.IsCollection && idFields.Count == 1 && queryMapping.MappedEnds.Single().SourceElement?.TypeReference.IsCollection == true)
                {
                    _handleMethod.AddStatement($"var {entityVariableName} = await {repositoryFieldName}.FindByIdsAsync({idFields.AsSingleOrTuple()}.ToArray(), cancellationToken);");
                }
                else
                {
                    _handleMethod.AddStatement($"var {entityVariableName} = await {repositoryFieldName}.FindByIdAsync({idFields.AsSingleOrTuple()}, cancellationToken);");
                }
            }
            else if (associationEnd.TypeReference.IsCollection)
            {
                var queryFields = queryMapping.MappedEnds
                    .Select(x => $"x.{x.TargetElement.Name} == {_csharpMapping.GenerateSourceStatementForMapping(queryMapping, x)}")
                    .ToList();
                var expression = queryFields.Any() ? $"x => {string.Join(" && ", queryFields)}, " : "";
                _handleMethod.AddStatement($"var {entityVariableName} = await {repositoryFieldName}.FindAllAsync({expression}cancellationToken);");
            }
            else
            {
                var queryFields = queryMapping.MappedEnds
                    .Select(x => $"x.{x.TargetElement.Name} == {_csharpMapping.GenerateSourceStatementForMapping(queryMapping, x)}")
                    .ToList();
                var expression = queryFields.Any() ? $"x => {string.Join(" && ", queryFields)}, " : "";
                _handleMethod.AddStatement($"var {entityVariableName} = await {repositoryFieldName}.FindAsync({expression}cancellationToken);");
            }

            if (!associationEnd.TypeReference.IsNullable && !associationEnd.TypeReference.IsCollection)
            {
                var queryFields = queryMapping.MappedEnds
                    .Select(x => new CSharpStatement($"{{{_csharpMapping.GenerateSourceStatementForMapping(queryMapping, x)}}}"))
                    .ToList();
                _handleMethod.AddStatement(_template.CreateThrowNotFoundIfNullStatement(
                    variable: entityVariableName,
                    message: $"Could not find {foundEntity.Name.ToPascalCase()} '{queryFields.AsSingleOrTuple()}'"));

            }

            return new EntityDetails(foundEntity, entityVariableName, repositoryFieldName, false, associationEnd.TypeReference.IsCollection);
        }

        private void InjectRepositoryForEntity(ClassModel foundEntity, out string repositoryFieldName)
        {
            var repositoryInterface = _template.GetEntityRepositoryInterfaceName(foundEntity);
            var repositoryName = repositoryInterface[1..].ToCamelCase();
            var temp = default(string);

            var ctor = _template.CSharpFile.Classes.First().Constructors.First();
            ctor.AddParameter(repositoryInterface, repositoryName.ToParameterName(),
                param => param.IntroduceReadonlyField(field => temp = field.Name));

            repositoryFieldName = temp;
        }

        private bool RepositoryRequiresExplicitUpdate(IMetadataModel forEntity)
        {
            return _template.TryGetTemplate<ICSharpFileBuilderTemplate>(
                       TemplateFulfillingRoles.Repository.Interface.Entity,
                       forEntity,
                       out var repositoryInterfaceTemplate) &&
                   repositoryInterfaceTemplate.CSharpFile.Interfaces[0].TryGetMetadata<bool>("requires-explicit-update", out var requiresUpdate) &&
                   requiresUpdate;
        }
    }

    public record EntityDetails(ClassModel Model, string VariableName, string RepositoryFieldName, bool IsNew, bool IsCollection = false);

    public static class MappingExtensions
    {
        public static IElementToElementMapping GetQueryEntityMapping(this IEnumerable<IElementToElementMapping> mappings)
        {
            return mappings.SingleOrDefault(x => x.MappingType == "Query Entity Mapping");
        }

        public static IElementToElementMapping GetUpdateEntityMapping(this IEnumerable<IElementToElementMapping> mappings)
        {
            return mappings.SingleOrDefault(x => x.MappingType == "Update Entity Mapping");
        }
    }
}