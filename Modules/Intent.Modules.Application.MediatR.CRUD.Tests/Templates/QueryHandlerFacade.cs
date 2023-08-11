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

    public string DomainClassRepositoryName => _activeTemplate.GetTypeName(TemplateFulfillingRoles.Repository.Interface.Entity, DomainClassModel);
    public string DomainRepositoryVarName => GetHandlerConstructorParameters().First(p => p.Type == DomainClassRepositoryName).Name;
    public string DomainEventBaseName => _activeTemplate.TryGetTypeName("Intent.DomainEvents.DomainEventBase", out var domainEventBaseName) ? domainEventBaseName : null;
    public string QueryTypeName => _activeTemplate.GetTypeName(QueryModelsTemplate.TemplateId, _model);
    public string DomainClassTypeName => _activeTemplate.GetTypeName(TemplateFulfillingRoles.Domain.Entity.Primary, DomainClassModel);
    public string QueryHandlerTypeName => _activeTemplate.GetQueryHandlerName(_model);
    
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

    public IReadOnlyCollection<CSharpStatement> GetInitialAutoFixtureStatements()
    {
        var statements = new List<CSharpStatement>();

        statements.Add("var fixture = new Fixture();");
        statements.AddRange(GetDomainEventBaseAutoFixtureRegistrationStatements());

        return statements;
    }
    
    public IReadOnlyCollection<CSharpStatement> GetManyDomainEntityTestData(int? numberOfItems = null)
    {
        var statements = new List<CSharpStatement>();

        statements.Add($"yield return new object[] {{ fixture.CreateMany<{DomainClassTypeName}>({(numberOfItems.HasValue ? numberOfItems.Value : string.Empty)}).ToList() }};");

        return statements;
    }
    
    public IReadOnlyCollection<CSharpStatement> GetDomainEventBaseAutoFixtureRegistrationStatements()
    {
        if (!HasDomainEventBaseName())
        {
            return ArraySegment<CSharpStatement>.Empty;
        }

        var statements = new List<CSharpStatement>();

        statements.Add($@"fixture.Register<{DomainEventBaseName}>(() => null);");

        statements.Add(
            $@"fixture.Customize<{_activeTemplate.GetTypeName(TemplateFulfillingRoles.Domain.Entity.Primary, DomainClassModel)}>(comp => comp.Without(x => x.DomainEvents));");

        return statements;
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
    
    public IReadOnlyCollection<CSharpStatement> GetDomainRepositoryFindAllMockingStatements(string entitiesVarName)
    {
        var statements = new List<CSharpStatement>();
        statements.Add($"{DomainRepositoryVarName}.FindAllAsync(CancellationToken.None).Returns(Task.FromResult({entitiesVarName}));");
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
    
    public IReadOnlyCollection<CSharpStatement> GetAssertionComparingHandlerResultsWithExpectedResults(string entitiesVarName)
    {
        var inv = new CSharpInvocationStatement($"{_activeTemplate.GetAssertionClassName(DomainClassModel)}.AssertEquivalent")
            .AddArgument("results")
            .AddArgument(entitiesVarName);
        return new[] { inv };
    }

    public IReadOnlyCollection<CSharpStatement> GetSingleDomainEntityTestData()
    {
        var statements = new List<CSharpStatement>();

        statements.Add($"var existingEntity = fixture.Create<{DomainClassTypeName}>();");
        statements.Add($"var testQuery = fixture.Create<{QueryTypeName}>();");
        if (QueryIdFields.Count == 1)
        {
            statements.Add(
                $"fixture.Customize<{QueryTypeName}>(comp => comp.With(x => x.{QueryIdFields[0].Name.ToCSharpIdentifier()}, existingEntity.{DomainIdAttributes[0].IdName.ToCSharpIdentifier()}));");
        }
        else
        {
            var fluent = new CSharpMethodChainStatement("comp").WithoutSemicolon();
            statements.Add(new CSharpInvocationStatement($"fixture.Customize<{QueryTypeName}>")
                .AddArgument(new CSharpLambdaBlock("comp").WithExpressionBody(fluent)));

            for (var i = 0; i < QueryIdFields.Count; i++)
            {
                var commandIdField = QueryIdFields[i];
                var domainIdAttribute = DomainIdAttributes[i];
                fluent.AddChainStatement($"With(x => x.{commandIdField.Name.ToCSharpIdentifier()}, existingEntity.{domainIdAttribute.IdName.ToCSharpIdentifier()})");
            }
        }
        statements.Add($"yield return new object[] {{ testQuery, existingEntity }};");
        
        return statements;
    }
    
    public enum MockRepositoryResponse
    {
        ReturnDomainVariable,
        ReturnDefault
    }

    public IReadOnlyCollection<CSharpStatement> GetDomainRepositoryFindByIdMockingStatements(string queryVarName, string entityVarName, MockRepositoryResponse response)
    {
        var statements = new List<CSharpStatement>();
        var returns = response switch
        {
            MockRepositoryResponse.ReturnDomainVariable => $".Returns(Task.FromResult({entityVarName ?? throw new ArgumentNullException(nameof(entityVarName))}))",
            MockRepositoryResponse.ReturnDefault => $".Returns(Task.FromResult<{DomainClassTypeName}>(default))",
            _ => throw new ArgumentOutOfRangeException(nameof(response), response, null)
        };
        statements.Add($"{DomainRepositoryVarName}.FindByIdAsync({GetQueryIdKeyExpression(queryVarName)}){returns};");
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
    
    private string GetQueryIdKeyExpression(string queryVarName)
    {
        if (QueryIdFields.Count == 1)
        {
            return $"{queryVarName}.{QueryIdFields[0].Name.ToCSharpIdentifier()}";
        }

        return $"({string.Join(", ", QueryIdFields.Select(idField => $"{queryVarName}.{idField.Name.ToCSharpIdentifier()}"))})";
    }
}