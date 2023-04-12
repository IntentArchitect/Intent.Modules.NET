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
    public class DomainCtorImplementationStrategy : ICrudImplementationStrategy
    {
        private readonly CommandHandlerTemplate _template;

        private readonly Lazy<StrategyData> _matchingElementDetails;

        public DomainCtorImplementationStrategy(CommandHandlerTemplate template)
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

            codeLines.Add(GetConstructorStatement($"entity", entityName, "request", false));

            codeLines.Add($"{repository.FieldName}.Add(entity);", x => x.SeparatedFromPrevious());

            if (_template.Model.TypeReference.Element != null)
            {
                codeLines.Add($"await {repository.FieldName}.UnitOfWork.SaveChangesAsync(cancellationToken);");
                codeLines.Add($"return entity.{(foundEntity.Attributes).Concat(foundEntity.ParentClass?.Attributes ?? new List<AttributeModel>()).FirstOrDefault(x => x.HasPrimaryKey())?.Name.ToPascalCase() ?? "Id"};");
            }
            else
            {
                codeLines.Add($"return Unit.Value;");
            }

            return codeLines.ToList();

            CSharpStatement GetConstructorStatement(string entityVarName, string entityName, string dtoVarName, bool hasInitStatements)
            {
                var ctor = ClassConstructorModelExtensions.AsClassConstructorModel(_template.Model.Mapping.Element);
                var ctorParams = ctor?.Parameters; 

                if (ctorParams?.Any() != true)
                {
                    return $"var {entityVarName} = new {entityName}{(hasInitStatements ? "" : "();")}";
                }

                var paramList = ctorParams
                    .Select(s => _template.Model.Properties.First(p => p.Mapping.Element.Id == s.Id))
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
        }

        private StrategyData GetMatchingElementDetails()
        {
            if (_template.Model.Mapping?.Element == null || (
                    !OperationModelExtensions.IsOperationModel(_template.Model.Mapping.Element) && 
                    !ClassConstructorModelExtensions.IsClassConstructorModel(_template.Model.Mapping.Element)))
            {
                return NoMatch;
            }

            ClassModel foundEntity = null;

            if (_template.Model.Mapping.Element.IsClassConstructorModel())
            {
                foundEntity = ClassConstructorModelExtensions.AsClassConstructorModel(_template.Model.Mapping.Element).ParentClass;
            }

            if (foundEntity == null)
            {
                return NoMatch;
            }

            var repositoryInterface = _template.GetEntityRepositoryInterfaceName(foundEntity);
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