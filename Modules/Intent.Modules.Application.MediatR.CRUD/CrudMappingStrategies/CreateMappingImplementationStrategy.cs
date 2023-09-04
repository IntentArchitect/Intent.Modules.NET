using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modelers.Domain.Api;
using Intent.Modelers.Services.Api;
using Intent.Modelers.Services.CQRS.Api;
using Intent.Modelers.Services.DomainInteractions.Api;
using Intent.Modules.Application.MediatR.CRUD.CrudStrategies;
using Intent.Modules.Application.MediatR.CRUD.Decorators;
using Intent.Modules.Application.MediatR.CRUD.Mapping.Resolvers;
using Intent.Modules.Application.MediatR.Templates;
using Intent.Modules.Application.MediatR.Templates.CommandHandler;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Mapping;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.Constants;
using Intent.Modules.Entities.Repositories.Api.Templates;
using Intent.Modules.Entities.Settings;
using Intent.Modules.Modelers.Domain.Settings;
using Intent.Templates;

namespace Intent.Modules.Application.MediatR.CRUD.CrudMappingStrategies
{
    public class CreateMappingImplementationStrategy : ICrudImplementationStrategy
    {
        private readonly CommandHandlerTemplate _template;
        private CSharpClassMappingManager _csharpMapping;


        public CreateMappingImplementationStrategy(CommandHandlerTemplate template)
        {
            _template = template;
            var model = (_template as ITemplateWithModel)?.Model as CommandModel;
            _csharpMapping = new CSharpClassMappingManager(_template);
            _csharpMapping.AddMappingResolver(new SelfUpdateMappingTypeResolver(_template, model));
            _csharpMapping.AddMappingResolver(new ValueObjectMappingTypeResolver(_template));
            _csharpMapping.AddMappingResolver(new EntityCreationMappingTypeResolver(_template));
            _csharpMapping.AddMappingResolver(new EntityUpdateMappingTypeResolver(_template));

            _csharpMapping.SetFromReplacement(model.InternalElement, "request");
            _template.CSharpFile.Classes.First().AddMetadata("mapping-manager", _csharpMapping);
        }

        public bool IsMatch()
        {
            return _template.Model.CreateEntityActions().Any() || _template.Model.UpdateEntityActions().Any();
        }

        public void ApplyStrategy()
        {
            _template.AddTypeSource(TemplateFulfillingRoles.Domain.Entity.Primary);
            _template.AddTypeSource(TemplateFulfillingRoles.Domain.ValueObject);
            _template.AddUsing("System.Linq");

            var @class = _template.CSharpFile.Classes.First();
            var ctor = @class.Constructors.First();
            var handleMethod = @class.FindMethod("Handle");
            handleMethod.Statements.Clear();
            handleMethod.Attributes.OfType<CSharpIntentManagedAttribute>().SingleOrDefault()?.WithBodyFully();

            var entitiesToAdd = new HashSet<EntityDetails>();
            foreach (var createAction in _template.Model.CreateEntityActions())
            {
                var repositoryInterface = _template.GetEntityRepositoryInterfaceName(createAction.Element.AsClassModel());
                var repositoryFieldName = default(string);
                ctor.AddParameter(repositoryInterface, repositoryInterface.Substring(1).ToParameterName(),
                    param => param.IntroduceReadonlyField(field => repositoryFieldName = field.Name));
                var entity = createAction.Element.AsClassModel();

                entitiesToAdd.Add(new EntityDetails(entity, entity.GetNewVariableName(), repositoryFieldName));

                handleMethod.AddStatements(GetImplementation(createAction));
                handleMethod.AddStatement(string.Empty);
            }

            foreach (var updateAction in _template.Model.UpdateEntityActions())
            {
                var foundEntity = updateAction.Element.AsClassModel();
                if (foundEntity != null)
                {
                    var repositoryInterface = _template.GetEntityRepositoryInterfaceName(foundEntity);
                    var repositoryName = repositoryInterface.Substring(1).ToCamelCase();
                    var repositoryFieldName = default(string);

                    ctor.AddParameter(repositoryInterface, repositoryName.ToParameterName(),
                        param => param.IntroduceReadonlyField(field => repositoryFieldName = field.Name));

                    var entityVariableName = foundEntity.GetExistingVariableName();
                    _csharpMapping.SetFromReplacement(foundEntity, entityVariableName);
                    _csharpMapping.SetFromReplacement(updateAction, entityVariableName);

                    var idFields = _template.Model.Properties.GetEntityIdFields(foundEntity);
                    handleMethod.AddStatement($"var {entityVariableName} = await {repositoryFieldName}.FindByIdAsync({idFields.GetEntityIdFromRequest()}, cancellationToken);");
                    handleMethod.AddStatement(_template.CreateThrowNotFoundIfNullStatement(
                        variable: entityVariableName,
                        message: $"Could not find {foundEntity.Name.ToPascalCase()} '{idFields.GetEntityIdFromRequestDescription()}'"));
                }
                handleMethod.AddStatement(string.Empty);
                handleMethod.AddStatements(GetImplementation(updateAction));
                handleMethod.AddStatement(string.Empty);
            }

            foreach (var processingAction in new CommandExtensionModel(_template.Model.InternalElement).ProcessingActions)
            {
                handleMethod.AddStatements(_csharpMapping.GenerateUpdateStatements(processingAction.InternalElement.Mappings.Single()));
                handleMethod.AddStatement(string.Empty);
            }

            foreach (var entity in entitiesToAdd)
            {
                handleMethod.AddStatement($"{entity.RepositoryFieldName}.Add({entity.VariableName});");
            }

            if (_template.Model.TypeReference.Element != null)
            {
                var entityVariableName = entitiesToAdd.First().VariableName;
                var repositoryFieldName = entitiesToAdd.First().RepositoryFieldName;
                var foundEntity = entitiesToAdd.First().Model;
                if (_template.TryGetTypeName("Application.Contract.Dto", _template.Model.TypeReference.Element, out var returnDto))
                {
                    ctor.AddParameter(_template.UseType("AutoMapper.IMapper"), "mapper", param => param.IntroduceReadonlyField());
                    handleMethod.AddStatement($"return {entityVariableName}.MapTo{returnDto}(_mapper);");
                }
                else
                {
                    handleMethod.AddStatement($"await {repositoryFieldName}.UnitOfWork.SaveChangesAsync(cancellationToken);");
                    handleMethod.AddStatement($"return {entityVariableName}.{foundEntity.Attributes.Concat(foundEntity.ParentClass?.Attributes ?? new List<AttributeModel>()).FirstOrDefault(x => x.IsPrimaryKey())?.Name.ToPascalCase() ?? "Id"};");
                }
            }
        }

        public IEnumerable<CSharpStatement> GetImplementation(CreateEntityActionTargetEndModel createAction)
        {
            var codeLines = new CSharpStatementAggregator();

            var model = (_template as ITemplateWithModel)?.Model as CommandModel;


            _csharpMapping.SetFromReplacement(model, "request");
            if (model.CreateEntityActions().Any())
            {

                var foundEntity = createAction.Element.AsClassModel();
                var entityVariableName = foundEntity.GetNewVariableName();
                var mapping = createAction.Mappings.SingleOrDefault();
                if (mapping == null)
                {
                    return Array.Empty<CSharpStatement>();
                }

                if (mapping.ToElement.IsClassModel())
                {
                    codeLines.Add(new CSharpAssignmentStatement($"var {entityVariableName}", _csharpMapping.GenerateCreationStatement(mapping)).WithSemicolon());
                    _csharpMapping.SetFromReplacement(createAction.InternalAssociationEnd, entityVariableName);
                    _csharpMapping.SetFromReplacement(foundEntity, entityVariableName);
                    _csharpMapping.SetToReplacement(createAction.InternalAssociationEnd, entityVariableName);
                }
            }

            return codeLines.ToList();
        }

        public IEnumerable<CSharpStatement> GetImplementation(UpdateEntityActionTargetEndModel updateAction)
        {
            var codeLines = new CSharpStatementAggregator();

            var foundEntity = updateAction.Element.AsClassModel();
            if (foundEntity != null)
            {
                _csharpMapping.SetToReplacement(foundEntity.InternalElement, foundEntity.GetExistingVariableName());
            }

            var mapping = updateAction.Mappings.SingleOrDefault();
            if (mapping == null)
            {
                return Array.Empty<CSharpStatement>();
            }

            codeLines.AddRange(_csharpMapping.GenerateUpdateStatements(mapping));


            return codeLines.ToList();
        }
    }

    public record EntityDetails(ClassModel Model, string VariableName, string RepositoryFieldName);
}