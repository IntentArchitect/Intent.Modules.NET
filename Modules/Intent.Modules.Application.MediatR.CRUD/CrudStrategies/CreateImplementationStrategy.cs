using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Metadata.RDBMS.Api;
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
using Intent.Modules.Entities.Repositories.Api.Templates.EntityRepositoryInterface;
using Intent.Modules.Metadata.RDBMS.Settings;
using JetBrains.Annotations;

namespace Intent.Modules.Application.MediatR.CRUD.CrudStrategies
{
    class CreateImplementationStrategy : ICrudImplementationStrategy
    {
        private readonly CommandHandlerTemplate _template;
        private readonly IApplication _application;
        private readonly IMetadataManager _metadataManager;

        private readonly Lazy<StrategyData> _matchingElementDetails;

        public CreateImplementationStrategy(CommandHandlerTemplate template, IApplication application,
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
            _template.AddTypeSource(TemplateFulfillingRoles.Domain.Entity.Primary);
            _template.AddTypeSource(TemplateFulfillingRoles.Domain.ValueObject);
            _template.AddUsing("System.Linq");

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
            var repository = _matchingElementDetails.Value.Repository;

            var entityName = _template.GetDomainEntityName(foundEntity);

            var codeLines = new CSharpStatementAggregator();

            var nestedCompOwner = foundEntity.GetNestedCompositionalOwner();
            if (nestedCompOwner != null)
            {
                var nestedCompOwnerIdField = _template.Model.Properties.GetNestedCompositionalOwnerIdField(owner: nestedCompOwner);
                if (nestedCompOwnerIdField == null)
                {
                    throw new Exception($"Nested Compositional Entity {foundEntity.Name} doesn't have an Id that refers to its owning Entity {nestedCompOwner.Name}.");
                }

                codeLines.Add($"var aggregateRoot = await {repository.FieldName}.FindByIdAsync(request.{nestedCompOwnerIdField.Name.ToCSharpIdentifier(CapitalizationBehaviour.AsIs)}, cancellationToken);");
                codeLines.Add($"if (aggregateRoot == null)");
                codeLines.Add(new CSharpStatementBlock()
                    .AddStatement(
                        $@"throw new InvalidOperationException($""{{nameof({_template.GetTypeName(TemplateFulfillingRoles.Domain.Entity.Primary, nestedCompOwner)})}} of Id '{{request.{nestedCompOwnerIdField.Name.ToCSharpIdentifier(CapitalizationBehaviour.AsIs)}}}' could not be found"");"));
            }

            codeLines.Add($"var new{foundEntity.Name} = new {entityName ?? foundEntity.Name}");
            codeLines.Add(new CSharpStatementBlock()
                .AddStatements(GetDTOPropertyAssignments("", "request", foundEntity.InternalElement, _template.Model.Properties))
                .WithSemicolon());

            if (nestedCompOwner != null)
            {
                var association = nestedCompOwner.GetNestedCompositeAssociation(foundEntity);
                codeLines.Add($"aggregateRoot.{association.Name.ToCSharpIdentifier(CapitalizationBehaviour.AsIs)}.Add(new{foundEntity.Name});", x => x.SeparatedFromPrevious());
            }
            else
            {
                codeLines.Add($"{repository.FieldName}.Add(new{foundEntity.Name});", x => x.SeparatedFromPrevious());
            }

            if (_template.Model.TypeReference.Element != null)
            {
                codeLines.Add($"await {repository.FieldName}.UnitOfWork.SaveChangesAsync(cancellationToken);");
                codeLines.Add($"return new{foundEntity.Name}.{foundEntity.Attributes.FirstOrDefault(x => x.HasPrimaryKey())?.Name.ToPascalCase() ?? "Id"};");
            }
            else
            {
                codeLines.Add($"return Unit.Value;");
            }

            return codeLines.ToList();
        }

        private IEnumerable<CSharpStatement> GetDTOPropertyAssignments(string entityVarName, string dtoVarName, IElement domainModel, IList<DTOFieldModel> dtoFields)
        {
            var codeLines = new List<CSharpStatement>();
            foreach (var field in dtoFields)
            {
                if (field.Mapping?.Element == null
                    && domainModel.ChildElements.All(p => p.Name != field.Name))
                {
                    codeLines.Add($"#warning No matching field found for {field.Name}");
                    continue;
                }

                var entityVarExpr = !string.IsNullOrWhiteSpace(entityVarName) ? $"{entityVarName}." : string.Empty;
                switch (field.Mapping?.Element?.SpecializationTypeId)
                {
                    default:
                        var mappedPropertyName = field.Mapping?.Element?.Name ?? "<null>";
                        codeLines.Add($"#warning No matching type for Domain: {mappedPropertyName} and DTO: {field.Name}");
                        break;
                    case null:
                    case AttributeModel.SpecializationTypeId:
                        var attribute = field.Mapping?.Element
                                        ?? domainModel.ChildElements.First(p => p.Name == field.Name);
                        if (!attribute.Name.Equals("Id", StringComparison.OrdinalIgnoreCase))
                        {
                            codeLines.Add($"{entityVarExpr}{attribute.Name.ToPascalCase()} = {dtoVarName}.{field.Name.ToPascalCase()},");
                            break;
                        }

                        break;
                    case AssociationTargetEndModel.SpecializationTypeId:
                    {
                        var association = field.Mapping.Element.AsAssociationTargetEndModel();
                        var targetType = association.Element as IElement;
                        var attributeName = association.Name.ToPascalCase();

                        if (association.Association.AssociationType == AssociationType.Aggregation)
                        {
                            codeLines.Add($@"#warning Field not a composite association: {field.Name.ToPascalCase()}");
                            break;
                        }

                        if (association.Multiplicity is Multiplicity.One or Multiplicity.ZeroToOne)
                        {
                            if (field.TypeReference.IsNullable)
                            {
                                codeLines.Add(
                                    $"{entityVarExpr}{attributeName} = {dtoVarName}.{field.Name.ToPascalCase()} != null ? {GetCreateMethodName(targetType, attributeName)}({dtoVarName}.{field.Name.ToPascalCase()}) : null,");
                            }
                            else
                            {
                                codeLines.Add($"{entityVarExpr}{attributeName} = {GetCreateMethodName(targetType, attributeName)}({dtoVarName}.{field.Name.ToPascalCase()}),");
                            }
                        }
                        else
                        {
                            codeLines.Add(
                                $"{entityVarExpr}{attributeName} = {dtoVarName}.{field.Name.ToPascalCase()}{(field.TypeReference.IsNullable ? "?" : "")}.Select({GetCreateMethodName(targetType, attributeName)}).ToList() ?? new List<{targetType.Name.ToPascalCase()}>(),");
                        }

                        var @class = _template.CSharpFile.Classes.First();
                        @class.AddMethod(_template.GetTypeName(targetType),
                            GetCreateMethodName(targetType, attributeName),
                            method => method.Private()
                                .AddAttribute(CSharpIntentManagedAttribute.Fully())
                                .AddParameter(_template.GetTypeName((IElement)field.TypeReference.Element), "dto")
                                .AddStatement($"return new {targetType.Name.ToPascalCase()}")
                                .AddStatement(new CSharpStatementBlock()
                                    .AddStatements(GetDTOPropertyAssignments("", $"dto", targetType,
                                        ((IElement)field.TypeReference.Element).ChildElements.Where(x => x.IsDTOFieldModel()).Select(x => x.AsDTOFieldModel()).ToList()))
                                    .WithSemicolon()));
                    }
                        break;
                }
            }

            return codeLines;
        }

        private StrategyData GetMatchingElementDetails()
        {
            var commandNameLowercase = _template.Model.Name.ToLower();
            if ((commandNameLowercase.Contains("create") ||
                 commandNameLowercase.Contains("add") ||
                 commandNameLowercase.Contains("new"))
                && _template.Model.Mapping?.Element.IsClassModel() == true)
            {
                var foundEntity = _template.Model.Mapping.Element.AsClassModel();
                var nestedCompOwner = foundEntity.GetNestedCompositionalOwner();
                var repositoryInterface = _template.GetEntityRepositoryInterfaceName(nestedCompOwner != null ? nestedCompOwner : foundEntity);
                if (repositoryInterface == null)
                {
                    return NoMatch;
                }

                var repository = new RequiredService(type: repositoryInterface,
                    name: repositoryInterface.Substring(1).ToCamelCase());

                return new StrategyData(true, foundEntity, repository);
            }

            return NoMatch;
        }

        private string GetCreateMethodName(ICanBeReferencedType classModel, [CanBeNull] string attributeName)
        {
            return $"Create{classModel.Name.ToPascalCase()}";
        }

        private static readonly StrategyData NoMatch = new StrategyData(false, null, null);

        private class StrategyData
        {
            public StrategyData(bool isMatch, ClassModel foundEntity, RequiredService repository)
            {
                IsMatch = isMatch;
                FoundEntity = foundEntity;
                Repository = repository;
            }

            public bool IsMatch { get; }
            public ClassModel FoundEntity { get; }
            public RequiredService Repository { get; }
        }
    }
}