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

internal enum QueryTargetDomain
{
    Aggregate,
    NestedEntity
}

internal enum QueryTestDataReturn
{
    AggregateDomain,
    QueryAndAggregateWithNestedEntityDomain
}

internal class QueryHandlerFacade
{
    private readonly ICSharpFileBuilderTemplate _activeTemplate;
    private readonly QueryModel _model;

    public QueryHandlerFacade(ICSharpFileBuilderTemplate activeTemplate, QueryModel model)
    {
        _activeTemplate = activeTemplate;
        _model = model;

        TargetDomainModel = model.GetClassModel() ?? throw new Exception($"No Domain Class mapping found for Query Model Id = {model.Id} / Name = {model.Name}");

        QueryIdFields = model.Properties.GetEntityIdFields(TargetDomainModel, activeTemplate.ExecutionContext);
        DomainIdAttributes = TargetDomainModel.GetEntityPkAttributes(activeTemplate.ExecutionContext).ToList();
        SingularTargetDomainName = TargetDomainModel.Name.ToPascalCase();
        SingularQueryName = model.Name.ToPascalCase();
        PluralTargetDomainName = SingularTargetDomainName.Pluralize();
    }

    private CSharpTemplateBase<QueryModel> _queryHandlerTemplate;

    public CSharpTemplateBase<QueryModel> QueryHandlerTemplate
    {
        get
        {
            if (_queryHandlerTemplate is not null)
            {
                return _queryHandlerTemplate;
            }

            _queryHandlerTemplate = _activeTemplate.GetQueryHandlerTemplate(_model, trackDependency: true)
                                    ?? throw new Exception($"Could not find {nameof(QueryHandlerTemplate)} for QueryModel Id = {_model.Id} / Name = {_model.Name}");

            return _queryHandlerTemplate;
        }
    }

    public ClassModel TargetDomainModel { get; }
    public string SingularQueryName { get; }
    public string SingularTargetDomainName { get; }
    public string PluralTargetDomainName { get; }
    public IReadOnlyList<DTOFieldModel> QueryIdFields { get; }
    public IReadOnlyList<ImplementationStrategyTemplatesExtensions.EntityIdAttribute> DomainIdAttributes { get; }

    public string QueryHandlerTypeName => _activeTemplate.GetQueryHandlerName(_model);
    public string DomainAggregateRepositoryTypeName => _activeTemplate.GetTypeName(TemplateRoles.Repository.Interface.Entity, TargetDomainModel);
    public string DomainAggregateOwnerRepositoryTypeName => _activeTemplate.GetTypeName(TemplateRoles.Repository.Interface.Entity, AggregateOwnerDomainModel);
    public string DomainAggregateRepositoryVarName => GetHandlerConstructorParameters().First(p => p.Type == DomainAggregateRepositoryTypeName).Name;
    public string DomainAggregateOwnerRepositoryVarName => GetHandlerConstructorParameters().First(p => p.Type == DomainAggregateOwnerRepositoryTypeName).Name;
    public string DomainEventBaseName => _activeTemplate.TryGetTypeName("Intent.DomainEvents.DomainEventBase", out var domainEventBaseName) ? domainEventBaseName : null;
    public string QueryTypeName => _activeTemplate.GetTypeName(QueryModelsTemplate.TemplateId, _model);
    public string TargetDomainTypeName => _activeTemplate.GetTypeName(TemplateRoles.Domain.Entity.Primary, TargetDomainModel);
    public ClassModel AggregateOwnerDomainModel => TargetDomainModel.GetNestedCompositionalOwner();
    public string AggregateOwnerDomainTypeName => _activeTemplate.GetTypeName(TemplateRoles.Domain.Entity.Primary, AggregateOwnerDomainModel);
    public IReadOnlyList<DTOFieldModel> QueryFieldsForOwnerId => _model.Properties.GetNestedCompositionalOwnerIdFields(AggregateOwnerDomainModel).ToList();

    public IReadOnlyList<ImplementationStrategyTemplatesExtensions.EntityNestedCompositionalIdAttribute> CompositeToOwnerIdAttributes =>
        TargetDomainModel.GetNestedCompositionalOwnerIdAttributes(AggregateOwnerDomainModel, _activeTemplate.ExecutionContext).ToList();

    public string OwnerToCompositeNavigationPropertyName =>
        AggregateOwnerDomainModel.GetNestedCompositeAssociation(TargetDomainModel).Name.ToCSharpIdentifier(CapitalizationBehaviour.AsIs);

    public string PagedResultInterfaceName => _activeTemplate.GetTypeName(TemplateRoles.Repository.Interface.PagedList);

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

    public IReadOnlyCollection<CSharpStatement> Get_InitialAutoFixture_TestDataStatements(bool includeVarKeyword)
    {
        var statements = new List<CSharpStatement>();

        statements.Add($"{(includeVarKeyword ? "var " : string.Empty)}fixture = new Fixture();");
        statements.AddRange(GetDomainEventBaseAutoFixtureRegistrationStatements());

        return statements;
    }

    public IReadOnlyCollection<CSharpStatement> Get_SingleDomainEntity_TestDataStatements(
        QueryTargetDomain queryTargetDomain,
        QueryTestDataReturn dataReturn,
        bool includeVarKeyword,
        bool nestedEntitiesAreEmpty)
    {
        var statements = new List<CSharpStatement>();

        var entityVarName = queryTargetDomain switch
        {
            QueryTargetDomain.Aggregate => "existingEntity",
            QueryTargetDomain.NestedEntity => "existingOwnerEntity",
            _ => throw new ArgumentOutOfRangeException(nameof(queryTargetDomain), queryTargetDomain, null)
        };
        var targetDomainClassType = queryTargetDomain switch
        {
            QueryTargetDomain.Aggregate => TargetDomainTypeName,
            QueryTargetDomain.NestedEntity => AggregateOwnerDomainTypeName,
            _ => throw new ArgumentOutOfRangeException(nameof(queryTargetDomain), queryTargetDomain, null)
        };

        statements.Add(string.Empty);
        statements.Add($"{(includeVarKeyword ? "var " : string.Empty)}{entityVarName} = fixture.Create<{targetDomainClassType}>();");

        switch (dataReturn)
        {
            case QueryTestDataReturn.QueryAndAggregateWithNestedEntityDomain:
                AddQueryWithIdentityTestData();
                break;
            case QueryTestDataReturn.AggregateDomain:
                statements.Add($"yield return new object[] {{ {entityVarName} }};");
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(dataReturn), dataReturn, null);
        }

        return statements;

        void AddQueryWithIdentityTestData()
        {
            switch (queryTargetDomain)
            {
                case QueryTargetDomain.Aggregate:
                    if (QueryIdFields.Count == 1)
                    {
                        statements.Add(
                            $"fixture.Customize<{QueryTypeName}>(comp => comp.With(x => x.{QueryIdFields[0].Name.ToCSharpIdentifier()}, {entityVarName}.{DomainIdAttributes[0].IdName.ToCSharpIdentifier()}));");
                    }
                    else
                    {
                        var fluent = new CSharpMethodChainStatement("comp").WithoutSemicolon();
                        statements.Add(new CSharpInvocationStatement($"fixture.Customize<{QueryTypeName}>")
                            .AddArgument(new CSharpLambdaBlock("comp").WithExpressionBody(fluent)));

                        for (var i = 0; i < QueryIdFields.Count; i++)
                        {
                            var queryIdField = QueryIdFields[i];
                            var domainIdAttribute = DomainIdAttributes[i];
                            fluent.AddChainStatement($"With(x => x.{queryIdField.Name.ToCSharpIdentifier()}, {entityVarName}.{domainIdAttribute.IdName.ToCSharpIdentifier()})");
                        }
                    }

                    break;
                case QueryTargetDomain.NestedEntity:
                    {
                        var fluent = new CSharpMethodChainStatement("comp").WithoutSemicolon();
                        statements.Add(new CSharpInvocationStatement($"fixture.Customize<{QueryTypeName}>")
                            .AddArgument(new CSharpLambdaBlock("comp").WithExpressionBody(fluent)));

                        for (var index = 0; index < DomainIdAttributes.Count; index++)
                        {
                            var idAttribute = DomainIdAttributes[index];
                            var idField = QueryFieldsForOwnerId[index];
                            fluent.AddChainStatement($"With(x => x.{idField.Name.ToCSharpIdentifier()}, {entityVarName}.{idAttribute.IdName.ToCSharpIdentifier()})");
                        }

                        if (nestedEntitiesAreEmpty)
                        {
                            statements.Add(
                                $"fixture.Customize<{AggregateOwnerDomainTypeName}>(comp => comp.With(p => p.{OwnerToCompositeNavigationPropertyName}, new List<{TargetDomainTypeName}>()));");
                        }

                        for (var i = 0; i < QueryIdFields.Count; i++)
                        {
                            var queryIdField = QueryIdFields[i];
                            var domainIdAttribute = DomainIdAttributes[i];
                            fluent.AddChainStatement($"With(x => x.{queryIdField.Name.ToCSharpIdentifier()}, {entityVarName}.{domainIdAttribute.IdName.ToCSharpIdentifier()})");
                        }
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(queryTargetDomain), queryTargetDomain, null);
            }

            statements.Add($"{(includeVarKeyword ? "var " : string.Empty)}testQuery = fixture.Create<{QueryTypeName}>();");
            statements.Add($"yield return new object[] {{ testQuery, {entityVarName} }};");
        }
    }

    public IReadOnlyCollection<CSharpStatement> Get_ProduceEntityOwnerAndCompositeAndQuery_TestDataStatements()
    {
        var statements = new List<CSharpStatement>();
        var targetDomainClassType = AggregateOwnerDomainModel.Name.ToPascalCase();

        statements.Add($"var existingOwnerEntity = fixture.Create<{targetDomainClassType}>();");
        statements.Add($"var expectedEntity = existingOwnerEntity.{OwnerToCompositeNavigationPropertyName}.First();");

        var fluent = new CSharpMethodChainStatement("comp").WithoutSemicolon();
        statements.Add(new CSharpInvocationStatement($"fixture.Customize<{QueryTypeName}>")
            .AddArgument(new CSharpLambdaBlock("comp").WithExpressionBody(fluent)));

        for (var index = 0; index < DomainIdAttributes.Count; index++)
        {
            var idAttribute = DomainIdAttributes[index];
            var idField = QueryFieldsForOwnerId[index];
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
        statements.AddRange(GetDomainEventBaseAutoFixtureRegistrationStatements());
        statements.Add(
            $"fixture.Customize<{AggregateOwnerDomainTypeName}>(comp => comp.With(p => p.{OwnerToCompositeNavigationPropertyName}, new List<{TargetDomainTypeName}>()));");
        statements.Add($"var existingOwnerEntity = fixture.Create<{AggregateOwnerDomainTypeName}>();");

        var fluent = new CSharpMethodChainStatement("comp").WithoutSemicolon();
        statements.Add(new CSharpInvocationStatement($"fixture.Customize<{QueryTypeName}>")
            .AddArgument(new CSharpLambdaBlock("comp").WithExpressionBody(fluent)));

        for (var index = 0; index < DomainIdAttributes.Count; index++)
        {
            var idAttribute = DomainIdAttributes[index];
            var idField = QueryFieldsForOwnerId[index];
            fluent.AddChainStatement($"With(p => p.{idField.Name.ToCSharpIdentifier()}, existingOwnerEntity.{idAttribute.IdName.ToCSharpIdentifier()})");
        }

        statements.Add($"var testQuery = fixture.Create<{QueryTypeName}>();");

        return statements;
    }

    public IReadOnlyCollection<CSharpStatement> Get_ManyAggregateDomainEntities_TestDataStatements(int? numberOfItems = null)
    {
        var statements = new List<CSharpStatement>();

        statements.Add($"yield return new object[] {{ fixture.CreateMany<{TargetDomainTypeName}>({(numberOfItems.HasValue ? numberOfItems.Value : string.Empty)}).ToList() }};");

        return statements;
    }

    public IReadOnlyCollection<CSharpStatement> GetDomainEventBaseAutoFixtureRegistrationStatements()
    {
        if (!HasDomainEventBaseName())
        {
            return ArraySegment<CSharpStatement>.Empty;
        }

        var statements = new List<CSharpStatement>();

        statements.Add($@"fixture.Register<{DomainEventBaseName}>(() => null!);");

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

    public IReadOnlyCollection<CSharpStatement> GetDomainRepositoryFindAllMockingStatements(
        string entitiesVarName,
        string repositoryVarName,
        int? pageNo = null,
        int? pageSize = null)
    {
        var statements = new List<CSharpStatement>();
        var pagination = pageNo.HasValue && pageSize.HasValue ? $"{pageNo}, {pageSize}, " : string.Empty;
        statements.Add(
            $"{repositoryVarName}.FindAllAsync({pagination}CancellationToken.None).Returns({GetFromResultExpression(entitiesVarName, TargetDomainTypeName, TargetDomainModel, true, true)});");
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

        return new[] { new CSharpStatement(string.Empty), inv };
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
        return GetAssertionComparingHandlerResultsWithExpectedResults(entitiesVarName, TargetDomainModel);
    }

    public IReadOnlyCollection<CSharpStatement> Get_AggregateOwner_AssertionComparingHandlerResultsWithExpectedResults(string entitiesVarName)
    {
        return GetAssertionComparingHandlerResultsWithExpectedResults(entitiesVarName, AggregateOwnerDomainModel);
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

    public IReadOnlyCollection<CSharpStatement> GetDomainAggregateRepositoryFindByIdMockingStatements(
        string queryVarName,
        string entityVarName,
        MockRepositoryResponse response)
    {
        return GetDomainRepositoryFindByIdMockingStatements(queryVarName, entityVarName, response, DomainAggregateRepositoryVarName, TargetDomainTypeName, TargetDomainModel,
            QueryIdFields);
    }

    public IReadOnlyCollection<CSharpStatement> GetDomainAggregateOwnerRepositoryFindByIdMockingStatements(
        string queryVarName,
        string entityVarName,
        MockRepositoryResponse response)
    {
        return GetDomainRepositoryFindByIdMockingStatements(queryVarName, entityVarName, response, DomainAggregateOwnerRepositoryVarName, AggregateOwnerDomainTypeName,
            AggregateOwnerDomainModel, QueryFieldsForOwnerId);
    }

    private IReadOnlyCollection<CSharpStatement> GetDomainRepositoryFindByIdMockingStatements(
        string queryVarName,
        string entityVarName,
        MockRepositoryResponse response,
        string repositoryVarName,
        string domainTypeName,
        ClassModel domainModel,
        IReadOnlyList<DTOFieldModel> queryIdFields)
    {
        var statements = new List<CSharpStatement>();
        var returns = response switch
        {
            MockRepositoryResponse.ReturnDomainVariable =>
                $".Returns({GetFromResultExpression(entityVarName ?? throw new ArgumentNullException(nameof(entityVarName)), TargetDomainTypeName, TargetDomainModel, true, false)})",
            MockRepositoryResponse.ReturnDefault =>
                $".Returns({GetFromResultExpression("default", domainTypeName, domainModel, false, false)})",
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

    public IReadOnlyCollection<CSharpStatement> GetPaginationQuerySetup(string queryVarName, int pageNo, int pageSize)
    {
        var statements = new List<CSharpStatement>();
        statements.Add($"{queryVarName}.PageNo = {pageNo};");
        statements.Add($"{queryVarName}.PageSize = {pageSize};");
        return statements;
    }

    public IReadOnlyCollection<CSharpStatement> GetMockedPaginatedResults(string outputVarName, string inputVarName)
    {
        var statements = new List<CSharpStatement>();
        statements.Add($"var {outputVarName} = Substitute.For<{PagedResultInterfaceName}<{TargetDomainTypeName}>>();");
        statements.Add($"{outputVarName}.GetEnumerator().Returns(c => {inputVarName}.GetEnumerator());");
        return statements;
    }

    private IReadOnlyCollection<(string Type, string Name)> GetHandlerConstructorParameters()
    {
        var ctor = ((ICSharpFileBuilderTemplate)QueryHandlerTemplate).CSharpFile.Classes.First(x => x.HasMetadata("handler")).Constructors.First();
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

    private string GetFromResultExpression(string returnExpression, string domainTypeName, ClassModel domainModel, bool preferImplicitGenerics, bool isList)
    {
        var entityTypeName = domainTypeName;
        if (_activeTemplate.TryGetTypeName(TemplateRoles.Domain.Entity.Interface, domainModel, out var interfaceTypeName))
        {
            entityTypeName = interfaceTypeName;
        }

        var performCast = entityTypeName != domainTypeName;
        if (performCast && isList)
        {
            _activeTemplate.AddUsing("System.Linq");
            return $"Task.FromResult({returnExpression}.Cast<{entityTypeName}>().ToList())";
        }

        if (performCast || !preferImplicitGenerics)
        {
            return $"Task.FromResult<{entityTypeName}>({returnExpression})";
        }

        return $"Task.FromResult({returnExpression})";
    }
}