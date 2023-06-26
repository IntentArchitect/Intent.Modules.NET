using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Modelers.Domain.Api;
using Intent.Modelers.Services.Api;
using Intent.Modules.Application.MediatR.CRUD.Decorators;
using Intent.Modules.Application.MediatR.Templates;
using Intent.Modules.Application.MediatR.Templates.CommandHandler;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.Constants;
using Intent.Modules.Entities.Repositories.Api.Templates;
using Intent.Modules.Entities.Settings;
using Intent.Modules.Modelers.Domain.Settings;

namespace Intent.Modules.Application.MediatR.CRUD.CrudStrategies
{
    public class DeleteImplementationStrategy : ICrudImplementationStrategy
    {
        private readonly CommandHandlerTemplate _template;

        private readonly Lazy<StrategyData> _matchingElementDetails;

        public DeleteImplementationStrategy(CommandHandlerTemplate template)
        {
            _template = template;

            _matchingElementDetails = new Lazy<StrategyData>(GetMatchingElementDetails);
        }

        public bool IsMatch()
        {
            return _matchingElementDetails.Value.IsMatch;
        }

        internal StrategyData GetStrategyData() => _matchingElementDetails.Value;

        public void ApplyStrategy()
        {
            var @class = _template.CSharpFile.Classes.First();
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
            var idField = _matchingElementDetails.Value.IdField;
            var repository = _matchingElementDetails.Value.Repository;

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
                codeLines.Add(new CSharpIfStatement($"aggregateRoot is null")
                    .AddStatement($@"throw new {_template.GetNotFoundExceptionName()}($""{{nameof({_template.GetTypeName(TemplateFulfillingRoles.Domain.Entity.Primary, nestedCompOwner)})}} of Id '{{request.{nestedCompOwnerIdField.Name.ToCSharpIdentifier(CapitalizationBehaviour.AsIs)}}}' could not be found"");"));

                var association = nestedCompOwner.GetNestedCompositeAssociation(foundEntity);

                codeLines.Add("");
                codeLines.Add($"var existing{foundEntity.Name} = aggregateRoot.{association.Name.ToCSharpIdentifier(CapitalizationBehaviour.AsIs)}.FirstOrDefault(p => p.{_matchingElementDetails.Value.FoundEntity.GetEntityIdAttribute(_template.ExecutionContext).IdName} == request.{idField.Name.ToPascalCase()});");
                codeLines.Add(new CSharpIfStatement($"existing{foundEntity.Name} is null")
                    .AddStatement($@"throw new {_template.GetNotFoundExceptionName()}($""{{nameof({_template.GetTypeName(TemplateFulfillingRoles.Domain.Entity.Primary, foundEntity)})}} of Id '{{request.{idField.Name.ToPascalCase()}}}' could not be found associated with {{nameof({_template.GetTypeName(TemplateFulfillingRoles.Domain.Entity.Primary, nestedCompOwner)})}} of Id '{{request.{nestedCompOwnerIdField.Name.ToCSharpIdentifier(CapitalizationBehaviour.AsIs)}}}'"");"));

                codeLines.Add($@"aggregateRoot.{association.Name.ToCSharpIdentifier(CapitalizationBehaviour.AsIs)}.Remove(existing{foundEntity.Name});");

                if (RepositoryRequiresExplicitUpdate())
                {
                    codeLines.Add(new CSharpStatement($"{repository.FieldName}.Update(aggregateRoot);").SeparatedFromPrevious());
                }

                codeLines.Add("return Unit.Value;");

                return codeLines.ToList();
            }

            codeLines.Add($@"var existing{foundEntity.Name} = await {repository.FieldName}.FindByIdAsync(request.{idField.Name.ToPascalCase()}, cancellationToken);");
            codeLines.Add(new CSharpIfStatement($"existing{foundEntity.Name} is null")
                .AddStatement($@"throw new {_template.GetNotFoundExceptionName()}($""Could not find {foundEntity.Name.ToPascalCase()} {{request.{idField.Name.ToPascalCase()}}}"");"));
            codeLines.Add($"{repository.FieldName}.Remove(existing{foundEntity.Name});");
            var dtoToReturn = _matchingElementDetails.Value.DtoToReturn;
            codeLines.Add(dtoToReturn != null
                ? $@"return existing{foundEntity.Name}.MapTo{_template.GetDtoName(dtoToReturn)}(_mapper);"
                : $"return Unit.Value;");

            return codeLines.ToList();
        }

        private bool RepositoryRequiresExplicitUpdate()
        {
            return _template.TryGetTemplate<ICSharpFileBuilderTemplate>(
                       TemplateFulfillingRoles.Repository.Interface.Entity,
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

            var idField = _template.Model.Properties.GetEntityIdField(foundEntity);
            if (idField == null)
            {
                return NoMatch;
            }

            var nestedCompOwner = foundEntity.GetNestedCompositionalOwner();
            var repositoryInterfaceModel = nestedCompOwner != null ? nestedCompOwner : foundEntity;

            var repositoryInterface = _template.GetEntityRepositoryInterfaceName(repositoryInterfaceModel);
            if (repositoryInterface == null)
            {
                return NoMatch;
            }

            var repository = new RequiredService(type: repositoryInterface,
                name: repositoryInterface.Substring(1).ToCamelCase());

            var dtoToReturn = _template.Model.TypeReference.Element?.AsDTOModel();

            return new StrategyData(true, foundEntity, idField, repository, dtoToReturn, repositoryInterfaceModel);
        }

        private static readonly StrategyData NoMatch = new StrategyData(false, null, null, null, null, null);

        internal class StrategyData
        {
            public StrategyData(bool isMatch, ClassModel foundEntity, DTOFieldModel idField, RequiredService repository, DTOModel dtoToReturn, ClassModel repositoryInterfaceModel)
            {
                IsMatch = isMatch;
                FoundEntity = foundEntity;
                IdField = idField;
                Repository = repository;
                DtoToReturn = dtoToReturn;
                RepositoryInterfaceModel = repositoryInterfaceModel;
            }

            public bool IsMatch { get; }
            public ClassModel FoundEntity { get; }
            public DTOFieldModel IdField { get; }
            public RequiredService Repository { get; }
            public DTOModel DtoToReturn { get; }
            public ClassModel RepositoryInterfaceModel { get; }
        }
    }
}