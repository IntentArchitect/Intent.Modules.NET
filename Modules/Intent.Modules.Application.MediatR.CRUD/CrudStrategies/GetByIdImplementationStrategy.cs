using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Modelers.Domain.Api;
using Intent.Modelers.Services.Api;
using Intent.Modules.Application.MediatR.CRUD.Decorators;
using Intent.Modules.Application.MediatR.Templates;
using Intent.Modules.Application.MediatR.Templates.CommandHandler;
using Intent.Modules.Application.MediatR.Templates.QueryHandler;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.Constants;
using Intent.Modules.Entities.Repositories.Api.Templates.EntityRepositoryInterface;

namespace Intent.Modules.Application.MediatR.CRUD.CrudStrategies
{
    public class GetByIdImplementationStrategy : ICrudImplementationStrategy
    {
        private readonly QueryHandlerTemplate _template;
        private readonly IApplication _application;

        private readonly Lazy<StrategyData> _matchingElementDetails;

        public GetByIdImplementationStrategy(QueryHandlerTemplate template, IApplication application)
        {
            _template = template;
            _application = application;
            _matchingElementDetails = new Lazy<StrategyData>(GetMatchingElementDetails);
        }

        public bool IsMatch()
        {
            return _matchingElementDetails.Value.IsMatch;
        }

        public void ApplyStrategy()
        {
            var @class = _template.CSharpFile.Classes.First();
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
            var idField = _matchingElementDetails.Value.IdField;
            var dtoToReturn = _matchingElementDetails.Value.DtoToReturn;

            var codeLines = new CSharpStatementAggregator();
            var nestedCompOwner = foundEntity.GetNestedCompositionalOwner();
            if (nestedCompOwner != null)
            {
                var nestedCompOwnerIdField = _template.Model.Properties.GetNestedCompositionalOwnerIdField(nestedCompOwner);
                if (nestedCompOwnerIdField == null)
                {
                    throw new Exception($"Nested Compositional Entity {foundEntity.Name} doesn't have an Id that refers to its owning Entity {nestedCompOwner.Name}.");
                }
                
                _template.AddUsing("System.Linq");
                
                codeLines.Add($"var aggregateRoot = await {repository.FieldName}.FindByIdAsync(request.{nestedCompOwnerIdField.Name.ToCSharpIdentifier(CapitalizationBehaviour.AsIs)}, cancellationToken);");
                codeLines.Add($"if (aggregateRoot == null)");
                codeLines.Add(new CSharpStatementBlock()
                    .AddStatement($@"throw new InvalidOperationException($""{{nameof({_template.GetTypeName(TemplateFulfillingRoles.Domain.Entity.Primary, nestedCompOwner)})}} of Id '{{request.{nestedCompOwnerIdField.Name.ToCSharpIdentifier(CapitalizationBehaviour.AsIs)}}}' could not be found"");"));

                var association = nestedCompOwner.GetNestedCompositeAssociation(foundEntity);
                codeLines.Add("");
                codeLines.Add($@"var element = aggregateRoot.{association.Name.ToCSharpIdentifier(CapitalizationBehaviour.AsIs)}.FirstOrDefault(p => p.{_matchingElementDetails.Value.FoundEntity.GetEntityIdAttribute().IdName} == request.{idField.Name.ToPascalCase()});");
                codeLines.Add($@"return element == null ? null : element.MapTo{_template.GetDtoName(dtoToReturn)}(_mapper);");
                

                return codeLines.ToList();
            }
            
            
            codeLines.Add($@"var {foundEntity.Name.ToCamelCase()} = await {repository.FieldName}.FindByIdAsync(request.{idField.Name.ToPascalCase()}, cancellationToken);");
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

            var idField = _template.Model.Properties.GetEntityIdField(foundEntity);
            if (idField == null)
            {
                return NoMatch;
            }

            var nestedCompOwner = foundEntity.GetNestedCompositionalOwner();
            var repositoryInterface = _template.GetEntityRepositoryInterfaceName(nestedCompOwner != null ? nestedCompOwner : foundEntity);
            if (repositoryInterface == null)
            {
                return NoMatch;
            }

            var repository = new RequiredService(type: repositoryInterface,
                name: repositoryInterface.Substring(1).ToCamelCase());

            return new StrategyData(true, foundEntity, dtoToReturn, idField, repository);
        }

        private static readonly StrategyData NoMatch = new StrategyData(false, null, null, null, null);

        private class StrategyData
        {
            public StrategyData(bool isMatch, ClassModel foundEntity, DTOModel dtoToReturn, DTOFieldModel idField,
                RequiredService repository)
            {
                IsMatch = isMatch;
                FoundEntity = foundEntity;
                DtoToReturn = dtoToReturn;
                IdField = idField;
                Repository = repository;
            }

            public bool IsMatch { get; }
            public ClassModel FoundEntity { get; }
            public DTOModel DtoToReturn { get; }
            public DTOFieldModel IdField { get; }
            public RequiredService Repository { get; }
        }
    }
}