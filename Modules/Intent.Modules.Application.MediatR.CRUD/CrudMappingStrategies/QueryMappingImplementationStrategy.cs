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
using Intent.Modules.Application.MediatR.Templates.QueryHandler;
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

namespace Intent.Modules.Application.MediatR.CRUD.CrudMappingStrategies
{
    public class QueryMappingImplementationStrategy : ICrudImplementationStrategy
    {
        private readonly QueryHandlerTemplate _template;
        private readonly CSharpClassMappingManager _csharpMapping;
        private CSharpConstructor _ctor;
        private CSharpClassMethod _handleMethod;


        public QueryMappingImplementationStrategy(QueryHandlerTemplate template)
        {
            _template = template;
            var model = (_template as ITemplateWithModel)?.Model as QueryModel;
            var queryTemplate = _template.ExecutionContext.FindTemplateInstance<ICSharpFileBuilderTemplate>(TemplateFulfillingRoles.Application.Query, model);
            // commandTemplate needs to be passed in so that the DefaultMapping can correctly resolve the mappings (e.g. pascal-casing for properties, even if the mapping is to a camel-case element)
            _csharpMapping = new CSharpClassMappingManager(queryTemplate); // TODO: Improve this template resolution system - it's not clear which template should be passed in initially.
            _csharpMapping.AddMappingResolver(new EntityCreationMappingTypeResolver(_template));
            _csharpMapping.AddMappingResolver(new EntityUpdateMappingTypeResolver(_template));
            _csharpMapping.AddMappingResolver(new StandardDomainMappingTypeResolver(_template));
            _csharpMapping.AddMappingResolver(new ValueObjectMappingTypeResolver(_template));

            _csharpMapping.SetFromReplacement(model.InternalElement, "request");
            _template.CSharpFile.AddMetadata("mapping-manager", _csharpMapping);

        }

        public bool IsMatch()
        {
            return _template.Model.QueryEntityActions().Any(x => x.Mappings.GetQueryEntityMapping() != null);
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

            foreach (var queryAction in _template.Model.QueryEntityActions())
            {
                var foundEntity = queryAction.Element.AsClassModel();
                if (foundEntity != null && queryAction.Mappings.GetQueryEntityMapping() != null)
                {
                    var entityDetails = QueryEntity(foundEntity, queryAction.InternalAssociationEnd);
                    trackedEntities.Add(entityDetails);
                }
            }

            if (_template.Model.TypeReference.Element != null && trackedEntities.Any())
            {
                var entityVariableName = trackedEntities.First().VariableName;
                if (_template.TryGetTypeName("Application.Contract.Dto", _template.Model.TypeReference.Element, out var returnDto) && _template.Model.TypeReference.Element.AsDTOModel().IsMapped)
                {
                    _ctor.AddParameter(_template.UseType("AutoMapper.IMapper"), "mapper", param => param.IntroduceReadonlyField());
                    _handleMethod.AddStatement($"return {entityVariableName}.MapTo{returnDto}{(_template.Model.TypeReference.IsCollection ? "List" : "")}(_mapper);");
                }
                else
                {
                    _handleMethod.AddStatement("throw new NotImplementedException(\"Implement return type mapping...\");");
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
    }

    public class DomainInteractionsManager
    {
        private readonly ICSharpFileBuilderTemplate _template;
        private readonly CSharpClassMappingManager _csharpMapping;

        public DomainInteractionsManager(ICSharpFileBuilderTemplate template)
        {
            _template = template;
            var model = (_template as ITemplateWithModel)?.Model as IElementWrapper;
            var queryTemplate = _template.ExecutionContext.FindTemplateInstance<ICSharpFileBuilderTemplate>(TemplateFulfillingRoles.Application.Query, model);
            // commandTemplate needs to be passed in so that the DefaultMapping can correctly resolve the mappings (e.g. pascal-casing for properties, even if the mapping is to a camel-case element)
            _csharpMapping = new CSharpClassMappingManager(queryTemplate); // TODO: Improve this template resolution system - it's not clear which template should be passed in initially.
            _csharpMapping.AddMappingResolver(new EntityCreationMappingTypeResolver(_template));
            _csharpMapping.AddMappingResolver(new EntityUpdateMappingTypeResolver(_template));
            _csharpMapping.AddMappingResolver(new StandardDomainMappingTypeResolver(_template));
            _csharpMapping.AddMappingResolver(new ValueObjectMappingTypeResolver(_template));

            _csharpMapping.SetFromReplacement(model.InternalElement, "request");
            _template.CSharpFile.AddMetadata("mapping-manager", _csharpMapping);
        }

        private List<CSharpStatement> QueryEntity(ClassModel foundEntity, IAssociationEnd associationEnd, out EntityDetails entityDetails)
        {
            var queryMapping = associationEnd.Mappings.GetQueryEntityMapping();

            var entityVariableName = associationEnd.Name;

            _csharpMapping.SetFromReplacement(foundEntity, entityVariableName);
            _csharpMapping.SetFromReplacement(associationEnd, entityVariableName);
            _csharpMapping.SetToReplacement(foundEntity, entityVariableName);
            _csharpMapping.SetToReplacement(associationEnd, entityVariableName);

            InjectRepositoryForEntity(foundEntity, out var repositoryFieldName);

            var statements = new List<CSharpStatement>();
            if (queryMapping.MappedEnds.Any() && queryMapping.MappedEnds.All(x => x.TargetElement.AsAttributeModel()?.IsPrimaryKey() == true))
            {
                var idFields = queryMapping.MappedEnds
                    .OrderBy(x => ((IElement)x.TargetElement).Order)
                    .Select(x => _csharpMapping.GenerateSourceStatementForMapping(queryMapping, x))
                    .ToList();

                if (associationEnd.TypeReference.IsCollection && idFields.Count == 1 && queryMapping.MappedEnds.Single().SourceElement?.TypeReference.IsCollection == true)
                {
                    statements.Add($"var {entityVariableName} = await {repositoryFieldName}.FindByIdsAsync({idFields.AsSingleOrTuple()}.ToArray(), cancellationToken);");
                }
                else
                {
                    statements.Add($"var {entityVariableName} = await {repositoryFieldName}.FindByIdAsync({idFields.AsSingleOrTuple()}, cancellationToken);");
                }
            }
            else if (associationEnd.TypeReference.IsCollection)
            {
                var queryFields = queryMapping.MappedEnds
                    .Select(x => $"x.{x.TargetElement.Name} == {_csharpMapping.GenerateSourceStatementForMapping(queryMapping, x)}")
                    .ToList();
                var expression = queryFields.Any() ? $"x => {string.Join(" && ", queryFields)}, " : "";
                statements.Add($"var {entityVariableName} = await {repositoryFieldName}.FindAllAsync({expression}cancellationToken);");
            }
            else
            {
                var queryFields = queryMapping.MappedEnds
                    .Select(x => $"x.{x.TargetElement.Name} == {_csharpMapping.GenerateSourceStatementForMapping(queryMapping, x)}")
                    .ToList();
                var expression = queryFields.Any() ? $"x => {string.Join(" && ", queryFields)}, " : "";
                statements.Add($"var {entityVariableName} = await {repositoryFieldName}.FindAsync({expression}cancellationToken);");
            }

            if (!associationEnd.TypeReference.IsNullable && !associationEnd.TypeReference.IsCollection)
            {
                var queryFields = queryMapping.MappedEnds
                    .Select(x => new CSharpStatement($"{{{_csharpMapping.GenerateSourceStatementForMapping(queryMapping, x)}}}"))
                    .ToList();
                statements.Add(_template.CreateThrowNotFoundIfNullStatement(
                    variable: entityVariableName,
                    message: $"Could not find {foundEntity.Name.ToPascalCase()} '{queryFields.AsSingleOrTuple()}'"));

            }
            entityDetails = new EntityDetails(foundEntity, entityVariableName, repositoryFieldName, false, associationEnd.TypeReference.IsCollection);

            return statements;
        }

        private void InjectRepositoryForEntity(ClassModel foundEntity, out string repositoryFieldName)
        {
            var repositoryInterface = (_template as IntentTemplateBase).GetEntityRepositoryInterfaceName(foundEntity);
            var repositoryName = repositoryInterface[1..].ToCamelCase();
            var temp = default(string);

            var ctor = _template.CSharpFile.Classes.First().Constructors.First();
            ctor.AddParameter(repositoryInterface, repositoryName.ToParameterName(),
                param => param.IntroduceReadonlyField(field => temp = field.Name));

            repositoryFieldName = temp;
        }
    }
}