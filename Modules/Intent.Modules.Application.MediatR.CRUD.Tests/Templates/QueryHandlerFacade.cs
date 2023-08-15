using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Modelers.Domain.Api;
using Intent.Modelers.Services.Api;
using Intent.Modelers.Services.CQRS.Api;
using Intent.Modules.Application.MediatR.CRUD.CrudStrategies;
using Intent.Modules.Application.MediatR.Templates;
using Intent.Modules.Application.MediatR.Templates.QueryHandler;
using Intent.Modules.Application.MediatR.Templates.QueryModels;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.Constants;

namespace Intent.Modules.Application.MediatR.CRUD.Tests.Templates;

internal class QueryHandlerFacade
{
    private readonly ICSharpFileBuilderTemplate _activeTemplate;
    private readonly QueryModel _model;

    public QueryHandlerFacade(ICSharpFileBuilderTemplate activeTemplate, QueryModel model)
    {
        _activeTemplate = activeTemplate;
        _model = model;

        DomainClassModel = model.Mapping?.Element?.AsClassModel()
            ?? model.TypeReference.Element.AsDTOModel().Mapping?.Element?.AsClassModel()
            ?? throw new Exception($"No Domain Class mapping found for Query Model Id = {model.Id} / Name = {model.Name}");
        
        QueryIdFields = model.Properties.GetEntityIdFields(DomainClassModel);
        DomainIdAttributes = DomainClassModel.GetEntityIdAttributes(activeTemplate.ExecutionContext).ToList();
        SimpleDomainClassName = DomainClassModel.Name.ToPascalCase();
        SimpleQueryName = model.Name.ToPascalCase();
        PluralDomainClassName = SimpleDomainClassName.Pluralize();
    }

    private QueryHandlerTemplate _queryHandlerTemplate;

    public QueryHandlerTemplate QueryHandlerTemplate
    {
        get
        {
            if (_queryHandlerTemplate is not null)
            {
                return _queryHandlerTemplate;
            }
            
            if(!_activeTemplate.TryGetTemplate<QueryHandlerTemplate>(QueryHandlerTemplate.TemplateId, _model, out var foundQueryHandlerTemplate))
            {
                throw new Exception($"Could not find {nameof(QueryHandlerTemplate)} for QueryModel Id = {_model.Id} / Name = {_model.Name}");
            }

            _queryHandlerTemplate = foundQueryHandlerTemplate;

            return _queryHandlerTemplate;
        }
    }
    public ClassModel DomainClassModel { get; }
    public string SimpleQueryName { get; }
    public string SimpleDomainClassName { get; }
    public string PluralDomainClassName { get; }
    public IReadOnlyList<DTOFieldModel> QueryIdFields { get; }
    public IReadOnlyList<ImplementationStrategyTemplatesExtensions.EntityIdAttribute> DomainIdAttributes { get; }

    public string QueryHandlerTypeName => _activeTemplate.GetQueryHandlerName(_model);
    public string DomainClassAggregateRepositoryTypeName => _activeTemplate.GetTypeName(TemplateFulfillingRoles.Repository.Interface.Entity, DomainClassModel);
    public string DomainClassAggregateOwnerRepositoryTypeName => _activeTemplate.GetTypeName(TemplateFulfillingRoles.Repository.Interface.Entity, DomainClassCompositionalOwner);
    public string DomainAggregateRepositoryVarName => GetHandlerConstructorParameters().First(p => p.Type == DomainClassAggregateRepositoryTypeName).Name;
    public string DomainAggregateOwnerRepositoryVarName => GetHandlerConstructorParameters().First(p => p.Type == DomainClassAggregateOwnerRepositoryTypeName).Name;
    public string DomainEventBaseName => _activeTemplate.TryGetTypeName("Intent.DomainEvents.DomainEventBase", out var domainEventBaseName) ? domainEventBaseName : null;
    public string QueryTypeName => _activeTemplate.GetTypeName(QueryModelsTemplate.TemplateId, _model);
    public string DomainClassTypeName => _activeTemplate.GetTypeName(TemplateFulfillingRoles.Domain.Entity.Primary, DomainClassModel);
    public ClassModel DomainClassCompositionalOwner => DomainClassModel.GetNestedCompositionalOwner();
    public string DomainClassCompositionalOwnerTypeName => _activeTemplate.GetTypeName(TemplateFulfillingRoles.Domain.Entity.Primary, DomainClassCompositionalOwner);
    public IReadOnlyList<DTOFieldModel> QueryOwnerIdFields => _model.Properties.GetNestedCompositionalOwnerIdFields(DomainClassCompositionalOwner).ToList();
    public IReadOnlyList<ImplementationStrategyTemplatesExtensions.EntityNestedCompositionalIdAttribute> DomainClassOwnerIdAttributes => DomainClassModel.GetNestedCompositionalOwnerIdAttributes(DomainClassCompositionalOwner, _activeTemplate.ExecutionContext).ToList();
    public string AggregateOwnerAssociationCompositeName => DomainClassCompositionalOwner.GetNestedCompositeAssociation(DomainClassModel).Name.ToCSharpIdentifier(CapitalizationBehaviour.AsIs);
    
    public bool HasDomainEventBaseName()
    {
        return DomainEventBaseName is not null;
    }
    
    public void AddHandlerConstructorMockUsings()
    {
        foreach (var declareUsing in QueryHandlerTemplate.DeclareUsings())
        {
            _activeTemplate.AddUsing(declareUsing);
        }
    }
    
    public IReadOnlyCollection<CSharpStatement> GetAutoMapperProfilesAndAddBackendField(CSharpClass @class)
    {
        @class.AddField("IMapper", "_mapper", prop => prop.PrivateReadOnly());

        var statements = new List<CSharpStatement>();
        
        statements.Add(new CSharpInvocationStatement("var mapperConfiguration = new MapperConfiguration")
            .AddArgument(new CSharpLambdaBlock("config")
                .AddStatement($"config.AddMaps(typeof({QueryHandlerTypeName}));"))
            .WithArgumentsOnNewLines());
        statements.Add("_mapper = mapperConfiguration.CreateMapper();");
        
        return statements;
    }

    public IReadOnlyCollection<CSharpStatement> Get_InitialAutoFixture_Aggregate_TestDataStatements(bool includeVar)
    {
        return Get_InitialAutoFixture_TestDataStatements(DomainClassModel, includeVar);
    }

    public IReadOnlyCollection<CSharpStatement> Get_InitialAutoFixture_AggregateOwner_TestDataStatements(bool includeVar)
    {
        return Get_InitialAutoFixture_TestDataStatements(DomainClassCompositionalOwner, includeVar);
    }
    
    private IReadOnlyCollection<CSharpStatement> Get_InitialAutoFixture_TestDataStatements(ClassModel targetDomainModel, bool includeVar)
    {
        var statements = new List<CSharpStatement>();

        statements.Add($"{(includeVar ? "var " : string.Empty)}fixture = new Fixture();");
        statements.AddRange(GetDomainEventBaseAutoFixtureRegistrationStatements(targetDomainModel));

        return statements;
    }

    public IReadOnlyCollection<CSharpStatement> Get_SingleAggregateOwnerDomainEntity_TestDataStatements(bool emptyNestedCompositeItems, bool includeVar, bool includeTestQuery)
    {
        return Get_SingleDomainEntity_TestDataStatements(true, includeVar, includeTestQuery, emptyNestedCompositeItems);
    }

    public IReadOnlyCollection<CSharpStatement> Get_SingleAggregateDomainEntity_TestDataStatements(bool includeVar, bool includeTestQuery)
    {
        return Get_SingleDomainEntity_TestDataStatements(false, includeVar, includeTestQuery);
    }
    
    private IReadOnlyCollection<CSharpStatement> Get_SingleDomainEntity_TestDataStatements(bool hasAggregateOwner, bool includeVar, bool includeTestQuery, bool? emptyNestedCompositeItems = null)
    {
        var statements = new List<CSharpStatement>();
        var entityVarName = hasAggregateOwner ? "existingOwnerEntity" : "existingEntity";
        var targetDomainClassType = hasAggregateOwner ? DomainClassCompositionalOwner.Name.ToPascalCase() : DomainClassTypeName;

        statements.Add(string.Empty);
        statements.Add($"{(includeVar ? "var " : string.Empty)}{entityVarName} = fixture.Create<{targetDomainClassType}>();");
        
        if (QueryIdFields.Count == 1 && !hasAggregateOwner)
        {
            statements.Add(
                $"fixture.Customize<{QueryTypeName}>(comp => comp.With(x => x.{QueryIdFields[0].Name.ToCSharpIdentifier()}, {entityVarName}.{DomainIdAttributes[0].IdName.ToCSharpIdentifier()}));");
        }
        else
        {
            var fluent = new CSharpMethodChainStatement("comp").WithoutSemicolon();
            statements.Add(new CSharpInvocationStatement($"fixture.Customize<{QueryTypeName}>")
                .AddArgument(new CSharpLambdaBlock("comp").WithExpressionBody(fluent)));

            if (hasAggregateOwner)
            {
                for (var index = 0; index < DomainIdAttributes.Count; index++)
                {
                    var idAttribute = DomainIdAttributes[index];
                    var idField = QueryOwnerIdFields[index];
                    fluent.AddChainStatement($"With(x => x.{idField.Name.ToCSharpIdentifier()}, {entityVarName}.{idAttribute.IdName.ToCSharpIdentifier()})");
                }

                if (emptyNestedCompositeItems.HasValue && emptyNestedCompositeItems.Value)
                {
                    statements.Add($"fixture.Customize<{DomainClassCompositionalOwnerTypeName}>(comp => comp.With(p => p.{AggregateOwnerAssociationCompositeName}, new List<{DomainClassTypeName}>()));");
                }
            }
            
            for (var i = 0; i < QueryIdFields.Count; i++)
            {
                var queryIdField = QueryIdFields[i];
                var domainIdAttribute = DomainIdAttributes[i];
                fluent.AddChainStatement($"With(x => x.{queryIdField.Name.ToCSharpIdentifier()}, {entityVarName}.{domainIdAttribute.IdName.ToCSharpIdentifier()})");
            }
        }

        if (includeTestQuery)
        {
            statements.Add($"{(includeVar ? "var " : string.Empty)}testQuery = fixture.Create<{QueryTypeName}>();");
            statements.Add($"yield return new object[] {{ testQuery, {entityVarName} }};");
        }
        else
        {
            statements.Add($"yield return new object[] {{ {entityVarName} }};");
        }

        return statements;
    }
    
    public IReadOnlyCollection<CSharpStatement> Get_ProduceEntityOwnerAndCompositeAndQuery_TestDataStatements()
    {
        var statements = new List<CSharpStatement>();
        var targetDomainClassType = DomainClassCompositionalOwner.Name.ToPascalCase();

        statements.Add($"var existingOwnerEntity = fixture.Create<{targetDomainClassType}>();");
        statements.Add($"var expectedEntity = existingOwnerEntity.{AggregateOwnerAssociationCompositeName}.First();");

        var fluent = new CSharpMethodChainStatement("comp").WithoutSemicolon();
        statements.Add(new CSharpInvocationStatement($"fixture.Customize<{QueryTypeName}>")
            .AddArgument(new CSharpLambdaBlock("comp").WithExpressionBody(fluent)));

        for (var index = 0; index < DomainIdAttributes.Count; index++)
        {
            var idAttribute = DomainIdAttributes[index];
            var idField = QueryOwnerIdFields[index];
            fluent.AddChainStatement($"With(x => x.{idField.Name.ToCSharpIdentifier()}, existingOwnerEntity.{idAttribute.IdName.ToCSharpIdentifier()})");
        }
            
        for (var i = 0; i < QueryIdFields.Count; i++)
        {
            var commandIdField = QueryIdFields[i];
            var domainIdAttribute = DomainIdAttributes[i];
            fluent.AddChainStatement($"With(x => x.{commandIdField.Name.ToCSharpIdentifier()}, expectedEntity.{domainIdAttribute.IdName.ToCSharpIdentifier()})");
        }

        statements.Add($"var testQuery = fixture.Create<{QueryTypeName}>();");
        statements.Add($"yield return new object[] {{ testQuery, existingOwnerEntity, expectedEntity }};");

        return statements;
    }

    public IReadOnlyCollection<CSharpStatement> GetNewAggregateOwnerWithoutCompositesStatements()
    {
        var statements = new List<CSharpStatement>();

        statements.Add("var fixture = new Fixture();");
        statements.AddRange(GetDomainEventBaseAutoFixtureRegistrationStatements(DomainClassCompositionalOwner));
        statements.Add($"fixture.Customize<{DomainClassCompositionalOwnerTypeName}>(comp => comp.With(p => p.{AggregateOwnerAssociationCompositeName}, new List<{DomainClassTypeName}>()));");
        statements.Add($"var existingOwnerEntity = fixture.Create<{DomainClassCompositionalOwnerTypeName}>();");
        statements.Add($"var testQuery = fixture.Create<{QueryTypeName}>();");
        
        return statements;
    }
    
    public IReadOnlyCollection<CSharpStatement> Get_ManyAggregateDomainEntities_TestDataStatements(int? numberOfItems = null)
    {
        var statements = new List<CSharpStatement>();

        statements.Add($"yield return new object[] {{ fixture.CreateMany<{DomainClassTypeName}>({(numberOfItems.HasValue ? numberOfItems.Value : string.Empty)}).ToList() }};");

        return statements;
    }
    
    public IReadOnlyCollection<CSharpStatement> GetDomainEventBaseAutoFixtureRegistrationStatements(ClassModel targetDomainModel)
    {
        if (!HasDomainEventBaseName())
        {
            return ArraySegment<CSharpStatement>.Empty;
        }

        var statements = new List<CSharpStatement>();
        
        statements.Add($@"fixture.Register<{DomainEventBaseName}>(() => null!);");
        
        statements.Add(
            $@"fixture.Customize<{_activeTemplate.GetTypeName(TemplateFulfillingRoles.Domain.Entity.Primary, targetDomainModel)}>(comp => comp.Without(x => x.DomainEvents));");

        return statements;
    }
    
    public IReadOnlyCollection<CSharpStatement> GetQueryHandlerConstructorParameterMockStatements()
    {
        return GetHandlerConstructorParameters()
            .Where(p => !p.Type.Contains("IMapper"))
            .Select(param => new CSharpStatement($"var {param.Name.ToLocalVariableName()} = Substitute.For<{param.Type}>();")
                .AddMetadata("type", param.Type))
            .ToArray();
    }
    
    public IReadOnlyCollection<CSharpStatement> GetNewQueryAutoFixtureInlineStatements(string queryVarName)
    {
        var statements = new List<CSharpStatement>();
        statements.Add($"var fixture = new Fixture();");
        statements.Add($"var {queryVarName} = fixture.Create<{QueryTypeName}>();");
        return statements;
    }

    public IReadOnlyCollection<CSharpStatement> GetDomainAggregateRepositoryFindAllMockingStatements(string entitiesVarName)
    {
        return GetDomainRepositoryFindAllMockingStatements(entitiesVarName, DomainAggregateRepositoryVarName);
    }
    
    public IReadOnlyCollection<CSharpStatement> GetDomainAggregateOwnerRepositoryFindAllMockingStatements(string entitiesVarName)
    {
        return GetDomainRepositoryFindAllMockingStatements(entitiesVarName, DomainAggregateOwnerRepositoryVarName);
    }
    
    private IReadOnlyCollection<CSharpStatement> GetDomainRepositoryFindAllMockingStatements(string entitiesVarName, string repositoryVarName)
    {
        var statements = new List<CSharpStatement>();
        statements.Add($"{repositoryVarName}.FindAllAsync(CancellationToken.None).Returns(Task.FromResult({entitiesVarName}));");
        return statements;
    }
    
    public IReadOnlyCollection<CSharpStatement> GetQueryHandlerConstructorSutStatement()
    {
        var inv = new CSharpInvocationStatement($"var sut = new {QueryHandlerTemplate.GetQueryHandlerName(_model)}");
        foreach (var param in GetHandlerConstructorParameters())
        {
            if (param.Type.Contains("IMapper"))
            {
                inv.AddArgument("_mapper");
            }
            else
            {
                inv.AddArgument(param.Name.ToLocalVariableName());
            }
        }

        return new[] { inv };
    }
    
    public IReadOnlyCollection<CSharpStatement> GetSutHandleInvocationStatement(string queryVarName)
    {
        var inv = new CSharpInvocationStatement("var results = await sut.Handle")
            .AddArgument(queryVarName)
            .AddArgument("CancellationToken.None");
        return new[] { inv };
    }
    
    public IReadOnlyCollection<CSharpStatement> GetSutHandleInvocationActLambdaStatement(string queryVarName)
    {
        var inv = new CSharpInvocationStatement("var act = async () => await sut.Handle")
            .AddArgument(queryVarName)
            .AddArgument("CancellationToken.None");
        return new[] { inv };
    }

    public IReadOnlyCollection<CSharpStatement> Get_Aggregate_AssertionComparingHandlerResultsWithExpectedResults(string entitiesVarName)
    {
        return GetAssertionComparingHandlerResultsWithExpectedResults(entitiesVarName, DomainClassModel);
    }
    
    public IReadOnlyCollection<CSharpStatement> Get_AggregateOwner_AssertionComparingHandlerResultsWithExpectedResults(string entitiesVarName)
    {
        return GetAssertionComparingHandlerResultsWithExpectedResults(entitiesVarName, DomainClassCompositionalOwner);
    }
    
    private IReadOnlyCollection<CSharpStatement> GetAssertionComparingHandlerResultsWithExpectedResults(string entitiesVarName, ClassModel classModel)
    {
        var inv = new CSharpInvocationStatement($"{_activeTemplate.GetAssertionClassName(classModel)}.AssertEquivalent")
            .AddArgument("results")
            .AddArgument(entitiesVarName);
        return new[] { inv };
    }
    
    public enum MockRepositoryResponse
    {
        ReturnDomainVariable,
        ReturnDefault
    }

    public IReadOnlyCollection<CSharpStatement> GetDomainAggregateRepositoryFindByIdMockingStatements(string queryVarName, string entityVarName, MockRepositoryResponse response)
    {
        return GetDomainRepositoryFindByIdMockingStatements(queryVarName, entityVarName, response, DomainAggregateRepositoryVarName, QueryIdFields);
    }
    
    public IReadOnlyCollection<CSharpStatement> GetDomainAggregateOwnerRepositoryFindByIdMockingStatements(string queryVarName, string entityVarName, MockRepositoryResponse response)
    {
        return GetDomainRepositoryFindByIdMockingStatements(queryVarName, entityVarName, response, DomainAggregateOwnerRepositoryVarName, QueryOwnerIdFields);
    }
    
    private IReadOnlyCollection<CSharpStatement> GetDomainRepositoryFindByIdMockingStatements(string queryVarName, string entityVarName, MockRepositoryResponse response, string repositoryVarName, IReadOnlyList<DTOFieldModel> queryIdFields)
    {
        var statements = new List<CSharpStatement>();
        var returns = response switch
        {
            MockRepositoryResponse.ReturnDomainVariable => $".Returns(Task.FromResult({entityVarName ?? throw new ArgumentNullException(nameof(entityVarName))}))",
            MockRepositoryResponse.ReturnDefault => $".Returns(Task.FromResult<{DomainClassTypeName}>(default))",
            _ => throw new ArgumentOutOfRangeException(nameof(response), response, null)
        };
        statements.Add($"{repositoryVarName}.FindByIdAsync({GetQueryIdKeysList(queryVarName, queryIdFields)}, CancellationToken.None)!{returns};");
        return statements;
    }
    
    public IReadOnlyCollection<CSharpStatement> GetThrowsExceptionAssertionStatement(string exceptionTypeName)
    {
        var statements = new List<CSharpStatement>();
        statements.Add($"await act.Should().ThrowAsync<{exceptionTypeName}>();");
        return statements;
    }
    
    private IReadOnlyCollection<(string Type, string Name)> GetHandlerConstructorParameters()
    {
        var ctor = QueryHandlerTemplate.CSharpFile.Classes.First().Constructors.First();
        return ctor.Parameters
            .Select(param => (param.Type, param.Name.ToLocalVariableName()))
            .ToArray();
    }
    
    private string GetQueryIdKeysList(string queryVarName, IReadOnlyList<DTOFieldModel> queryIdFields)
    {
        var left = queryIdFields.Count > 1 ? "(" : string.Empty;
        var right = queryIdFields.Count > 1 ? ")" : string.Empty;
        
        return $"{left}{string.Join(", ", queryIdFields.Select(idField => $"{queryVarName}.{idField.Name.ToCSharpIdentifier()}"))}{right}";
    }
}