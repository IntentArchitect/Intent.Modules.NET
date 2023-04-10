using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Metadata.RDBMS.Api;
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
using OperationModelExtensions = Intent.Modelers.Domain.Api.OperationModelExtensions;
using ParameterModelExtensions = Intent.Modelers.Domain.Api.ParameterModelExtensions;

namespace Intent.Modules.Application.MediatR.CRUD.CrudStrategies
{
    public class CreateImplementationStrategy : ICrudImplementationStrategy
    {
        private readonly CommandHandlerTemplate _template;

        private readonly Lazy<StrategyData> _matchingElementDetails;

        public CreateImplementationStrategy(CommandHandlerTemplate template)
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

            var entityName = _template.GetDomainEntityName(foundEntity) ?? foundEntity.Name;

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

            var assignmentStatements = GetDTOPropertyAssignments(entityVarName: "", dtoVarName: "request", domainModel: foundEntity,
                dtoFields: _template.Model.Properties.Where(FilterForAnaemicMapping).ToList(),
                skipIdField: true);
            codeLines.Add(GetConstructorStatement($"new{foundEntity.Name}", entityName, "request", assignmentStatements.Any()));
            if (assignmentStatements.Any())
            {
                codeLines.Add(new CSharpStatementBlock()
                    .AddStatements(assignmentStatements)
                    .WithSemicolon());
            }

            GenerateOperationInvocationCode("request", $"new{foundEntity.Name}");

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
                codeLines.Add($"return new{foundEntity.Name}.{(foundEntity.Attributes).Concat(foundEntity.ParentClass?.Attributes ?? new List<AttributeModel>()).FirstOrDefault(x => x.HasPrimaryKey())?.Name.ToPascalCase() ?? "Id"};");
            }
            else
            {
                codeLines.Add($"return Unit.Value;");
            }

            return codeLines.ToList();

            CSharpStatement GetConstructorStatement(string entityVarName, string entityName, string dtoVarName, bool hasInitStatements)
            {
                var ctorParamLookup = _template.Model.Properties
                    .Where(p => (p.Mapping?.Path[0].Element).IsClassConstructorModel() &&
                                ParameterModelExtensions.IsParameterModel(p.Mapping?.Element))
                    .ToLookup(field => (field.Mapping.Path[0].Element).AsClassConstructorModel());

                if (!ctorParamLookup.Any())
                {
                    return $"var {entityVarName} = new {entityName}{(hasInitStatements ? "" : "();")}";
                }

                if (ctorParamLookup.Count > 1)
                {
                    throw new Exception($"Multiple constructors are used to map against a single Class [{entityName}]");
                }

                var kvp = ctorParamLookup.First();
                var paramList = kvp.Key.Parameters
                    .Select(s => kvp.First(p => p.Mapping.Element.Id == s.Id))
                    .Select(s => $"{dtoVarName}.{s.Name.ToPascalCase()}");
                var statement = new CSharpInvocationStatement($@"var {entityVarName} = new {entityName}");
                if (hasInitStatements)
                {
                    statement.WithoutSemicolon();
                }
                foreach (var param in paramList)
                {
                    statement.AddArgument(param);
                }

                return statement;
            }

            bool FilterForAnaemicMapping(DTOFieldModel field)
            {
                return field.Mapping?.Element == null ||
                       field.Mapping.Element.IsAttributeModel() ||
                       field.Mapping.Element.IsAssociationEndModel();
            }

            void GenerateOperationInvocationCode(string dtoVarName, string entityVarName)
            {
                var operationParamLookup = _template.Model.Properties
                    .Where(p => OperationModelExtensions.IsOperationModel(p.Mapping?.Path[0].Element) &&
                                ParameterModelExtensions.IsParameterModel(p.Mapping?.Element))
                    .ToLookup(field => OperationModelExtensions.AsOperationModel(field.Mapping.Path[0].Element));

                foreach (var kvp in operationParamLookup)
                {
                    var paramList = kvp.Key.Parameters
                        .Select(s => kvp.First(p => p.Mapping.Element.Id == s.Id))
                        .Select(s => $"{dtoVarName}.{s.Name.ToPascalCase()}");
                    var statement = new CSharpInvocationStatement($@"{entityVarName}.{kvp.Key.Name.ToPascalCase()}");
                    foreach (var param in paramList)
                    {
                        statement.AddArgument(param);
                    }
                    codeLines.Add(statement);
                }
            }
        }

        private IList<CSharpStatement> GetDTOPropertyAssignments(string entityVarName, string dtoVarName, ClassModel domainModel, IList<DTOFieldModel> dtoFields, bool skipIdField)
        {
            var codeLines = new CSharpStatementAggregator();
            foreach (var field in dtoFields)
            {
                if (skipIdField && field.Name.Equals("id", StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                if (field.Mapping?.Element == null
                    && domainModel.Attributes.All(p => p.Name != field.Name))
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
                        var attribute = field.Mapping?.Element?.AsAttributeModel()
                                        ?? domainModel.Attributes.First(p => p.Name == field.Name);
                        var toListExpression = field.TypeReference.IsCollection
                            ? field.TypeReference.IsNullable ? "?.ToList()" : ".ToList()"
                            : string.Empty;
                        codeLines.Add($"{entityVarExpr}{attribute.Name.ToPascalCase()} = {dtoVarName}.{field.Name.ToPascalCase()}{toListExpression},");

                        break;
                    case AssociationTargetEndModel.SpecializationTypeId:
                        {
                            var association = field.Mapping.Element.AsAssociationTargetEndModel();
                            var targetEntity = association.Element.AsClassModel();
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
                                        $"{entityVarExpr}{attributeName} = {dtoVarName}.{field.Name.ToPascalCase()} != null ? {GetCreateMethodName(targetEntity.InternalElement)}({dtoVarName}.{field.Name.ToPascalCase()}) : null,");
                                }
                                else
                                {
                                    codeLines.Add($"{entityVarExpr}{attributeName} = {GetCreateMethodName(targetEntity.InternalElement)}({dtoVarName}.{field.Name.ToPascalCase()}),");
                                }
                            }
                            else
                            {
                                codeLines.Add($"{entityVarExpr}{attributeName} = {dtoVarName}.{field.Name.ToPascalCase()}{(field.TypeReference.IsNullable ? "?" : "")}.Select({GetCreateMethodName(targetEntity.InternalElement)}).ToList(){(field.TypeReference.IsNullable ? $" ?? new List<{targetEntity.Name.ToPascalCase()}>()" : "")},");
                            }

                            var @class = _template.CSharpFile.Classes.First();
                            @class.AddMethod(_template.GetTypeName(targetEntity.InternalElement),
                                GetCreateMethodName(targetEntity.InternalElement),
                                method => method.Private()
                                    .Static()
                                    .AddAttribute(CSharpIntentManagedAttribute.Fully())
                                    .AddParameter(_template.GetTypeName((IElement)field.TypeReference.Element), "dto")
                                    .AddStatement($"return new {targetEntity.Name.ToPascalCase()}")
                                    .AddStatement(new CSharpStatementBlock()
                                        .AddStatements(GetDTOPropertyAssignments(
                                            entityVarName: "",
                                            dtoVarName: $"dto",
                                            domainModel: targetEntity,
                                            dtoFields: field.TypeReference.Element.AsDTOModel().Fields,
                                            skipIdField: true))
                                        .WithSemicolon()));
                        }
                        break;
                }
            }

            return codeLines.ToList();
        }

        private StrategyData GetMatchingElementDetails()
        {
            if (_template.ExecutionContext.Settings.GetDomainSettings().EnsurePrivatePropertySetters())
            {
                return NoMatch;
            }

            var commandNameLowercase = _template.Model.Name.ToLower();
            if ((!commandNameLowercase.StartsWith("create") &&
                 !commandNameLowercase.StartsWith("add") &&
                 !commandNameLowercase.StartsWith("new"))
                || _template.Model.Mapping?.Element.IsClassModel() != true)
            {
                return NoMatch;
            }

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

        private static string GetCreateMethodName(ICanBeReferencedType classModel)
        {
            return $"Create{classModel.Name.ToPascalCase()}";
        }

        private static readonly StrategyData NoMatch = new StrategyData(false, null, null);

        internal class StrategyData
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