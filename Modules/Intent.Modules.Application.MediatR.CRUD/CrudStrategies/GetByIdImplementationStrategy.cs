using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Modelers.Domain.Api;
using Intent.Modelers.Services.Api;
using Intent.Modelers.Services.CQRS.Api;
using Intent.Modules.Application.MediatR.CRUD.Decorators;
using Intent.Modules.Application.MediatR.Templates;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.Constants;

namespace Intent.Modules.Application.MediatR.CRUD.CrudStrategies
{
    public class GetByIdImplementationStrategy : ICrudImplementationStrategy
    {
        private readonly CSharpTemplateBase<QueryModel> _template;
        private readonly IApplication _application;

        private readonly Lazy<StrategyData> _matchingElementDetails;

        public GetByIdImplementationStrategy(CSharpTemplateBase<QueryModel> template, IApplication application)
        {
            _template = template;
            _application = application;
            _matchingElementDetails = new Lazy<StrategyData>(GetMatchingElementDetails);
        }

        public bool IsMatch()
        {
            return _matchingElementDetails.Value.IsMatch;
        }

        public void BindToTemplate(ICSharpFileBuilderTemplate template)
        {
            template.CSharpFile.AfterBuild(_ => ApplyStrategy());
        }

        public void ApplyStrategy()
        {
            var @class = ((ICSharpFileBuilderTemplate)_template).CSharpFile.Classes.First(x => x.HasMetadata("handler"));
            var ctor = @class.Constructors.First();
            var repository = _matchingElementDetails.Value.Repository;
            ctor.AddParameter(repository.Type, repository.Name.ToParameterName(), param => param.IntroduceReadonlyField())
                .AddParameter(_template.UseType("AutoMapper.IMapper"), "mapper", param => param.IntroduceReadonlyField());

            var handleMethod = @class.FindMethod("Handle");
            handleMethod.Statements.Clear();
            handleMethod.Attributes.OfType<CSharpIntentManagedAttribute>().SingleOrDefault()?.WithBodyFully();
            handleMethod.AddStatements(GetImplementation());
        }

        public IEnumerable<CSharpStatement> GetImplementation()
        {
            var foundEntity = _matchingElementDetails.Value.FoundEntity;
            var repository = _matchingElementDetails.Value.Repository;
            var idFields = _matchingElementDetails.Value.IdFields;
            var dtoToReturn = _matchingElementDetails.Value.DtoToReturn;

            var codeLines = new CSharpStatementAggregator(); // swap back to list.
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
                codeLines.Add($@"var element = aggregateRoot.{association.Name.ToCSharpIdentifier(CapitalizationBehaviour.AsIs)}.FirstOrDefault({idFields.GetPropertyToRequestMatchClause()});");
                codeLines.Add(_template.CreateThrowNotFoundIfNullStatement(
                    variable: "element",
                    message: $"Could not find {foundEntity.Name.ToPascalCase()} '{idFields.GetEntityIdFromRequestDescription()}'"));

                codeLines.Add(string.Empty);
                codeLines.Add($@"return element.MapTo{_template.GetDtoName(dtoToReturn)}(_mapper);");

                return codeLines.ToList();
            }


            codeLines.Add($@"var {foundEntity.Name.ToCamelCase()} = await {repository.FieldName}.FindByIdAsync({idFields.GetEntityIdFromRequest(_template.Model.InternalElement)}, cancellationToken);");

            if (_template.Model.TypeReference.IsNullable)
            {
                codeLines.Add(_template.CreateReturnNullIfNullStatement(variable: foundEntity.Name.ToCamelCase()));
            }
            else
            {
                codeLines.Add(_template.CreateThrowNotFoundIfNullStatement(
                    variable: foundEntity.Name.ToCamelCase(),
                    message: $"Could not find {foundEntity.Name.ToPascalCase()} '{idFields.GetEntityIdFromRequestDescription()}'"));
            }

            codeLines.Add(string.Empty);
            codeLines.Add($@"return {foundEntity.Name.ToCamelCase()}.MapTo{_template.GetDtoName(dtoToReturn)}(_mapper);");

            return codeLines.ToList();
        }

        private StrategyData GetMatchingElementDetails()
        {
            var queryNameLowercase = _template.Model.Name.ToLower();
            if ((!queryNameLowercase.Contains("get") &&
                 !queryNameLowercase.Contains("find") &&
                 !queryNameLowercase.Contains("lookup"))
                || _template.Model.Mapping?.Element.IsClassModel() != true)
            {
                return NoMatch;
            }

            var foundEntity = _template.Model.Mapping.Element.AsClassModel();

            var dtoToReturn = _application.MetadataManager.Services(_application).GetDTOModels().SingleOrDefault(x =>
                x.Id == _template.Model.TypeReference.Element.Id && x.IsMapped &&
                x.Mapping.ElementId == foundEntity.Id);

            if (dtoToReturn == null)
            {
                return NoMatch;
            }

            var idFields = _template.Model.Properties.GetEntityIdFields(foundEntity, _template.ExecutionContext);
            if (!idFields.Any())
            {
                return NoMatch;
            }

            var nestedCompOwner = foundEntity.GetNestedCompositionalOwner();
            if (!_template.TryGetTypeName(TemplateRoles.Repository.Interface.Entity, nestedCompOwner != null ? nestedCompOwner : foundEntity, out var repositoryInterface))
            {
                return NoMatch;
            }

            var repository = new RequiredService(type: repositoryInterface,
                name: repositoryInterface.Substring(1).ToCamelCase());

            return new StrategyData(true, foundEntity, dtoToReturn, idFields, repository);
        }

        private static readonly StrategyData NoMatch = new StrategyData(false, null, null, null, null);

        private class StrategyData
        {
            public StrategyData(bool isMatch, ClassModel foundEntity, DTOModel dtoToReturn, List<DTOFieldModel> idFields,
                RequiredService repository)
            {
                IsMatch = isMatch;
                FoundEntity = foundEntity;
                DtoToReturn = dtoToReturn;
                IdFields = idFields;
                Repository = repository;
            }

            public bool IsMatch { get; }
            public ClassModel FoundEntity { get; }
            public DTOModel DtoToReturn { get; }
            public List<DTOFieldModel> IdFields { get; }
            public RequiredService Repository { get; }
        }
    }
}