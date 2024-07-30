using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Modelers.Domain.Api;
using Intent.Modelers.Services.Api;
using Intent.Modelers.Services.CQRS.Api;
using Intent.Modules.Application.MediatR.CRUD.Decorators;
using Intent.Modules.Application.MediatR.Templates;
using Intent.Modules.Application.MediatR.Templates.CommandHandler;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.Constants;
using Intent.Modules.Entities.Settings;
using Intent.Modules.Modelers.Domain.Settings;

namespace Intent.Modules.Application.MediatR.CRUD.CrudStrategies
{
    public class DeleteImplementationStrategy : ICrudImplementationStrategy
    {
        private readonly CSharpTemplateBase<CommandModel> _template;

        private readonly Lazy<StrategyData> _matchingElementDetails;

        public DeleteImplementationStrategy(CSharpTemplateBase<CommandModel> template)
        {
            _template = template;

            _matchingElementDetails = new Lazy<StrategyData>(GetMatchingElementDetails);
        }
        public void BindToTemplate(ICSharpFileBuilderTemplate template)
        {
            template.CSharpFile.AfterBuild(_ => ApplyStrategy());
        }

        public bool IsMatch()
        {
            return _matchingElementDetails.Value.IsMatch;
        }

        internal StrategyData GetStrategyData() => _matchingElementDetails.Value;

        public void ApplyStrategy()
        {
            var @class = ((ICSharpFileBuilderTemplate)_template).CSharpFile.Classes.First(x => x.HasMetadata("handler"));
            var ctor = @class.Constructors.First();
            var repository = _matchingElementDetails.Value.Repository;
            ctor.AddParameter(repository.Type, repository.Name.ToParameterName(),
                param => param.IntroduceReadonlyField());

            var handleMethod = @class.FindMethod("Handle");
            handleMethod.Statements.Clear();
            handleMethod.Attributes.OfType<CSharpIntentManagedAttribute>().SingleOrDefault()?.WithBodyFully();
            handleMethod.AddStatements(GetImplementation());
            if (_matchingElementDetails.Value.DtoToReturn != null)
            {
                ctor.AddParameter(_template.UseType("AutoMapper.IMapper"), "mapper", param => param.IntroduceReadonlyField());
            }
        }

        public IEnumerable<CSharpStatement> GetImplementation()
        {
            var foundEntity = _matchingElementDetails.Value.FoundEntity;
            var idFields = _matchingElementDetails.Value.IdFields;
            var repository = _matchingElementDetails.Value.Repository;
            var entityVariableName = foundEntity.GetExistingVariableName();

            var codeLines = new CSharpStatementAggregator();

            var nestedCompOwner = foundEntity.GetNestedCompositionalOwner();
            if (nestedCompOwner != null)
            {
                var nestedCompOwnerIdFields = _template.Model.Properties.GetNestedCompositionalOwnerIdFields(nestedCompOwner);
                if (!nestedCompOwnerIdFields.Any())
                {
                    throw new Exception($"Nested Compositional Entity {foundEntity.Name} doesn't have an Id that refers to its owning Entity {nestedCompOwner.Name}.");
                }

                _template.AddUsing("System.Linq");

                codeLines.Add($"var aggregateRoot = await {repository.FieldName}.FindByIdAsync({nestedCompOwnerIdFields.GetEntityIdFromRequest(_template.Model.InternalElement)}, cancellationToken);");
                codeLines.Add(_template.CreateThrowNotFoundIfNullStatement(
                    variable: "aggregateRoot",
                    message: $"{{nameof({_template.GetTypeName(TemplateRoles.Domain.Entity.Primary, nestedCompOwner)})}} of Id '{nestedCompOwnerIdFields.GetEntityIdFromRequestDescription()}' could not be found"));
                codeLines.Add(string.Empty);

                var association = nestedCompOwner.GetNestedCompositeAssociation(foundEntity);

                codeLines.Add($"var {entityVariableName} = aggregateRoot.{association.Name.ToCSharpIdentifier(CapitalizationBehaviour.AsIs)}.FirstOrDefault({idFields.GetPropertyToRequestMatchClause()});");
                codeLines.Add(_template.CreateThrowNotFoundIfNullStatement(
                    variable: $"{entityVariableName}",
                    message: $"{{nameof({_template.GetTypeName(TemplateRoles.Domain.Entity.Primary, foundEntity)})}} of Id '{idFields.GetEntityIdFromRequestDescription()}' could not be found associated with {{nameof({_template.GetTypeName(TemplateRoles.Domain.Entity.Primary, nestedCompOwner)})}} of Id '{nestedCompOwnerIdFields.GetEntityIdFromRequestDescription()}'"));
                codeLines.Add(string.Empty);

                codeLines.Add($@"aggregateRoot.{association.Name.ToCSharpIdentifier(CapitalizationBehaviour.AsIs)}.Remove({entityVariableName});");

                if (RepositoryRequiresExplicitUpdate())
                {
                    codeLines.Add(new CSharpStatement($"{repository.FieldName}.Update(aggregateRoot);").SeparatedFromPrevious());
                }

                return codeLines.ToList();
            }

            codeLines.Add($@"var {entityVariableName} = await {repository.FieldName}.FindByIdAsync({idFields.GetEntityIdFromRequest(_template.Model.InternalElement)}, cancellationToken);");
            codeLines.Add(_template.CreateThrowNotFoundIfNullStatement(
                variable: $"{entityVariableName}",
                message: $"Could not find {foundEntity.Name.ToPascalCase()} '{idFields.GetEntityIdFromRequestDescription()}'"));
            codeLines.Add(string.Empty);

            codeLines.Add($"{repository.FieldName}.Remove({entityVariableName});");
            var dtoToReturn = _matchingElementDetails.Value.DtoToReturn;
            if (dtoToReturn != null)
            {
                codeLines.Add($@"return {entityVariableName}.MapTo{_template.GetDtoName(dtoToReturn)}(_mapper);");
            }

            return codeLines.ToList();
        }

        private bool RepositoryRequiresExplicitUpdate()
        {
            return _template.TryGetTemplate<ICSharpFileBuilderTemplate>(
                       TemplateRoles.Repository.Interface.Entity,
                       _matchingElementDetails.Value.RepositoryInterfaceModel,
                       out var repositoryInterfaceTemplate) &&
                   repositoryInterfaceTemplate.CSharpFile.Interfaces[0].TryGetMetadata<bool>("requires-explicit-update", out var requiresUpdate) &&
                   requiresUpdate;
        }


        private StrategyData GetMatchingElementDetails()
        {
            var commandNameLowercase = _template.Model.Name.ToLower();
            if ((!commandNameLowercase.StartsWith("delete") &&
                 !commandNameLowercase.StartsWith("remove"))
                || _template.Model.Mapping?.Element.IsClassModel() != true)
            {
                return NoMatch;
            }

            var foundEntity = _template.Model.Mapping.Element.AsClassModel();

            if (!foundEntity.IsAggregateRoot() &&
                _template.ExecutionContext.Settings.GetDomainSettings().EnsurePrivatePropertySetters())
            {
                return NoMatch;
            }

            var idFields = _template.Model.Properties.GetEntityIdFields(foundEntity, _template.ExecutionContext);
            if (!idFields.Any())
            {
                return NoMatch;
            }

            var nestedCompOwner = foundEntity.GetNestedCompositionalOwner();
            var repositoryInterfaceModel = nestedCompOwner != null ? nestedCompOwner : foundEntity;

            if (!_template.TryGetTypeName(TemplateRoles.Repository.Interface.Entity, repositoryInterfaceModel, out var repositoryInterface))
            {
                return NoMatch;
            }

            var repository = new RequiredService(type: repositoryInterface,
                name: repositoryInterface.Substring(1).ToCamelCase());

            var dtoToReturn = _template.Model.TypeReference.Element?.AsDTOModel();

            return new StrategyData(true, foundEntity, idFields, repository, dtoToReturn, repositoryInterfaceModel);
        }

        private static readonly StrategyData NoMatch = new StrategyData(false, null, null, null, null, null);

        internal class StrategyData
        {
            public StrategyData(bool isMatch, ClassModel foundEntity, List<DTOFieldModel> idFields, RequiredService repository, DTOModel dtoToReturn, ClassModel repositoryInterfaceModel)
            {
                IsMatch = isMatch;
                FoundEntity = foundEntity;
                IdFields = idFields;
                Repository = repository;
                DtoToReturn = dtoToReturn;
                RepositoryInterfaceModel = repositoryInterfaceModel;
            }

            public bool IsMatch { get; }
            public ClassModel FoundEntity { get; }
            public List<DTOFieldModel> IdFields { get; }
            public RequiredService Repository { get; }
            public DTOModel DtoToReturn { get; }
            public ClassModel RepositoryInterfaceModel { get; }
        }
    }
}