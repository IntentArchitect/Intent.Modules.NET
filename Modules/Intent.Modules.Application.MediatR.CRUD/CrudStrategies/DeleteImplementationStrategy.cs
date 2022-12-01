using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Modelers.Domain.Api;
using Intent.Modelers.Services.Api;
using Intent.Modules.Application.MediatR.CRUD.Decorators;
using Intent.Modules.Application.MediatR.Templates;
using Intent.Modules.Application.MediatR.Templates.CommandHandler;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.Constants;

namespace Intent.Modules.Application.MediatR.CRUD.CrudStrategies
{
    class DeleteImplementationStrategy : ICrudImplementationStrategy
    {
        private readonly CommandHandlerTemplate _template;
        private readonly IApplication _application;
        private readonly IMetadataManager _metadataManager;

        private readonly Lazy<StrategyData> _matchingElementDetails;

        public DeleteImplementationStrategy(CommandHandlerTemplate template, IApplication application,
            IMetadataManager metadataManager)
        {
            _template = template;
            _application = application;
            _metadataManager = metadataManager;

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
            ctor.AddParameter(repository.Type, repository.Name.ToParameterName(),
                param => param.IntroduceReadonlyField());

            var handleMethod = @class.FindMethod("Handle");
            handleMethod.Statements.Clear();
            handleMethod.Attributes.OfType<CSharpIntentManagedAttribute>().SingleOrDefault()?.WithBodyFully();
            handleMethod.AddStatements(GetImplementation());
        }

        public IEnumerable<CSharpStatement> GetImplementation()
        {
            var foundEntity = _matchingElementDetails.Value.FoundEntity;
            var idField = _matchingElementDetails.Value.IdField;
            var repository = _matchingElementDetails.Value.Repository;

            var codeLines = new CSharpStatementAggregator();

            var aggrRootOwner = foundEntity.GetAggregateRootOwner();
            if (aggrRootOwner != null)
            {
                var aggregateRootField = _template.Model.Properties.GetAggregateRootIdField(aggrRootOwner);
                if (aggregateRootField == null)
                {
                    throw new Exception($"Nested Compositional Entity {foundEntity.Name} doesn't have an Id that refers to its owning Entity {aggrRootOwner.Name}.");
                }
                
                _template.AddUsing("System.Linq");

                codeLines.Add($"var aggregateRoot = await {repository.FieldName}.FindByIdAsync(request.{aggregateRootField.Name.ToCSharpIdentifier(CapitalizationBehaviour.AsIs)}, cancellationToken);");
                codeLines.Add($"if (aggregateRoot == null)");
                codeLines.Add(new CSharpStatementBlock()
                    .AddStatement($@"throw new InvalidOperationException($""{{nameof({_template.GetTypeName(TemplateFulfillingRoles.Domain.Entity.Primary, aggrRootOwner)})}} of Id '{{request.{aggregateRootField.Name.ToCSharpIdentifier(CapitalizationBehaviour.AsIs)}}}' could not be found"");"));

                var association = aggrRootOwner.GetNestedCompositeAssociation(foundEntity);
                
                codeLines.Add("");
                codeLines.Add($"var element = aggregateRoot.{association.Name.ToCSharpIdentifier(CapitalizationBehaviour.AsIs)}.FirstOrDefault(p => p.Id == request.Id);");
                codeLines.Add($@"if (element == null)");
                codeLines.Add(new CSharpStatementBlock()
                    .AddStatement($@"throw new InvalidOperationException($""{{nameof({_template.GetTypeName(TemplateFulfillingRoles.Domain.Entity.Primary, foundEntity)})}} of Id '{{request.Id}}' could not be found associated with {{nameof({_template.GetTypeName(TemplateFulfillingRoles.Domain.Entity.Primary, aggrRootOwner)})}} of Id '{{request.{aggregateRootField.Name.ToCSharpIdentifier(CapitalizationBehaviour.AsIs)}}}'"");"));
                
                codeLines.Add($@"aggregateRoot.{association.Name.ToCSharpIdentifier(CapitalizationBehaviour.AsIs)}.Remove(element);");
                codeLines.Add("return Unit.Value;");
                
                return codeLines.ToList();
            }

            codeLines.Add($@"var existing{foundEntity.Name} = await {repository.FieldName}.FindByIdAsync(request.{idField.Name.ToPascalCase()}, cancellationToken);
                {repository.FieldName}.Remove(existing{foundEntity.Name});
                return Unit.Value;");

            return codeLines.ToList();
        }


        private StrategyData GetMatchingElementDetails()
        {
            // if (_template.Model.Properties.Count() != 1)
            // {
            //     return NoMatch;
            // }
            //
            // var matchingEntities = _metadataManager.Domain(_application)
            //     .GetClassModels().Where(x => new[]
            //     {
            //         $"delete{x.Name.ToLower()}",
            //         $"remove{x.Name.ToLower()}",
            //     }.Contains(_template.Model.Name.ToLower().RemoveSuffix("command")))
            //     .ToList();
            //
            // if (matchingEntities.Count() != 1)
            // {
            //     return NoMatch;
            // }
            //
            //var foundEntity = matchingEntities.Single();

            var commandNameLowercase = _template.Model.Name.ToLower();
            if ((!commandNameLowercase.Contains("delete") &&
                 !commandNameLowercase.Contains("remove"))
                || _template.Model.Mapping?.Element.IsClassModel() != true)
            {
                return NoMatch;
            }
            
            var foundEntity = _template.Model.Mapping.Element.AsClassModel();

            var idField = _template.Model.Properties.FirstOrDefault(p =>
                string.Equals(p.Name, "id", StringComparison.InvariantCultureIgnoreCase) ||
                string.Equals(p.Name, $"{foundEntity.Name}Id", StringComparison.InvariantCultureIgnoreCase));
            if (idField == null)
            {
                return NoMatch;
            }

            var aggrRootOwner = foundEntity.GetAggregateRootOwner();
            var repositoryInterface = _template.GetEntityRepositoryInterfaceName(aggrRootOwner != null ? aggrRootOwner : foundEntity);
            if (repositoryInterface == null)
            {
                return NoMatch;
            }

            var repository = new RequiredService(type: repositoryInterface,
                name: repositoryInterface.Substring(1).ToCamelCase());

            return new StrategyData(true, foundEntity, idField, repository);
        }

        private static readonly StrategyData NoMatch = new StrategyData(false, null, null, null);

        private class StrategyData
        {
            public StrategyData(bool isMatch, ClassModel foundEntity, DTOFieldModel idField, RequiredService repository)
            {
                IsMatch = isMatch;
                FoundEntity = foundEntity;
                IdField = idField;
                Repository = repository;
            }

            public bool IsMatch { get; }
            public ClassModel FoundEntity { get; }
            public DTOFieldModel IdField { get; }
            public RequiredService Repository { get; }
        }
    }
}