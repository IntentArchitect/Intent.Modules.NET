using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Modelers.Domain.Api;
using Intent.Modelers.Services.Api;
using Intent.Modelers.Services.CQRS.Api;
using Intent.Modules.Application.MediatR.CRUD.CrudStrategies;
using Intent.Modules.Application.MediatR.Templates;
using Intent.Modules.Application.MediatR.Templates.CommandModels;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.Constants;
using Intent.Modules.Entities.Settings;
using Intent.Modules.Modelers.Domain.Settings;

namespace Intent.Modules.Application.MediatR.CRUD.Tests.Templates;

internal enum CommandTargetDomain
{
    Aggregate,
    NestedEntity
}

internal enum CommandTestDataReturn
{
    CommandAndAggregateDomain,
    CommandAndAggregateWithNestedEntityDomain
}

internal class CommandHandlerFacade
{
    private readonly ICSharpFileBuilderTemplate _activeTemplate;
    private readonly CommandModel _model;

    public CommandHandlerFacade(ICSharpFileBuilderTemplate activeTemplate, CommandModel model)
    {
        _activeTemplate = activeTemplate;
        _model = model;

        TargetDomainModel = model.GetClassModel();
        CommandIdFields = model.Properties.GetEntityIdFields(TargetDomainModel, activeTemplate.ExecutionContext);
        TargetDomainIdAttributes = TargetDomainModel.GetEntityPkAttributes(activeTemplate.ExecutionContext).ToList();
        SingularTargetDomainName = TargetDomainModel.Name.ToPascalCase();
        PluralTargetDomainName = TargetDomainModel.Name.Pluralize().ToPascalCase();
        SingularCommandName = model.Name.ToPascalCase();
    }

    private CSharpTemplateBase<CommandModel> _commandHandlerTemplate;

    public CSharpTemplateBase<CommandModel> CommandHandlerTemplate
    {
        get
        {
            if (_commandHandlerTemplate is not null)
            {
                return _commandHandlerTemplate;
            }

            _commandHandlerTemplate = _activeTemplate.GetCommandHandlerTemplate(_model, trackDependency: true)
                                      ?? throw new Exception($"Could not find {nameof(CommandHandlerTemplate)} for CommandModel Id = {_model.Id} / Name = {_model.Name}");

            return _commandHandlerTemplate;
        }
    }

    private CommandModelsTemplate _commandModelsTemplate;
    public CommandModelsTemplate CommandModelsTemplate
    {
        get
        {
            if (_commandModelsTemplate is not null)
            {
                return _commandModelsTemplate;
            }

            if (!_activeTemplate.TryGetTemplate<CommandModelsTemplate>(CommandModelsTemplate.TemplateId, _model, out var foundCommandModelsTemplate))
            {
                throw new Exception($"Could not find {nameof(CommandModelsTemplate)} for CommandModel Id = {_model.Id} / Name = {_model.Name}");
            }

            _commandModelsTemplate = foundCommandModelsTemplate;

            return _commandModelsTemplate;
        }
    }

    public ClassModel TargetDomainModel { get; }
    public string SingularCommandName { get; }
    public string SingularTargetDomainName { get; }
    public string PluralTargetDomainName { get; }
    public IReadOnlyList<DTOFieldModel> CommandIdFields { get; }
    public IReadOnlyList<ImplementationStrategyTemplatesExtensions.EntityIdAttribute> TargetDomainIdAttributes { get; }

    public string DomainAggregateRepositoryTypeName => _activeTemplate.GetTypeName(TemplateRoles.Repository.Interface.Entity, TargetDomainModel);
    public string DomainAggregateOwnerRepositoryTypeName => _activeTemplate.GetTypeName(TemplateRoles.Repository.Interface.Entity, AggregateOwnerDomain);
    public string DomainAggregateRepositoryVarName => GetHandlerConstructorParameters().First(p => p.Type == DomainAggregateRepositoryTypeName).Name;
    public string DomainAggregateOwnerRepositoryVarName => GetHandlerConstructorParameters().First(p => p.Type == DomainAggregateOwnerRepositoryTypeName).Name;
    public string DomainEventBaseName => _activeTemplate.TryGetTypeName("Intent.DomainEvents.DomainEventBase", out var domainEventBaseName) ? domainEventBaseName : null;
    public string CommandTypeName => _activeTemplate.GetTypeName(CommandModelsTemplate.TemplateId, _model);
    public string TargetDomainTypeName => _activeTemplate.GetTypeName(TemplateRoles.Domain.Entity.Primary, TargetDomainModel);
    public ClassModel AggregateOwnerDomain => TargetDomainModel.GetNestedCompositionalOwner();
    public string AggregateOwnerDomainTypeName => _activeTemplate.GetTypeName(TemplateRoles.Domain.Entity.Primary, AggregateOwnerDomain);
    public string SingularAggregateOwnerDomainName => AggregateOwnerDomain.Name.ToPascalCase();
    public IReadOnlyList<DTOFieldModel> CommandFieldsForOwnerId => _model.Properties.GetNestedCompositionalOwnerIdFields(AggregateOwnerDomain).ToList();

    public IReadOnlyList<ImplementationStrategyTemplatesExtensions.EntityNestedCompositionalIdAttribute> CompositeToOwnerIdAttributes =>
        TargetDomainModel.GetNestedCompositionalOwnerIdAttributes(AggregateOwnerDomain, _activeTemplate.ExecutionContext).ToList();

    public string OwnerToCompositeNavigationPropertyName =>
        AggregateOwnerDomain.GetNestedCompositeAssociation(TargetDomainModel).Name.ToCSharpIdentifier(CapitalizationBehaviour.AsIs);

    public bool HasIdReturnTypeOnCommand()
    {
        return _model.TypeReference.Element is not null;
    }

    public bool HasDomainEventBaseName()
    {
        return DomainEventBaseName is not null;
    }

    public void AddHandlerConstructorMockUsings()
    {
        foreach (var declareUsing in CommandHandlerTemplate.DeclareUsings())
        {
            _activeTemplate.AddUsing(declareUsing);
        }
    }

    public IReadOnlyCollection<CSharpStatement> Get_ProduceSingleCommand_TestDataStatements()
    {
        var statements = new List<CSharpStatement>();

        statements.Add("var fixture = new Fixture();");
        statements.Add($@"yield return new object[] {{ fixture.Create<{CommandTypeName}>() }};");

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
        statements.Add(new CSharpInvocationStatement($"fixture.Customize<{CommandTypeName}>")
            .AddArgument(new CSharpLambdaBlock("comp").WithExpressionBody(fluent)));

        for (var index = 0; index < TargetDomainIdAttributes.Count; index++)
        {
            var idAttribute = TargetDomainIdAttributes[index];
            var idField = CommandFieldsForOwnerId[index];
            fluent.AddChainStatement($"With(p => p.{idField.Name.ToCSharpIdentifier()}, existingOwnerEntity.{idAttribute.IdName.ToCSharpIdentifier()})");
        }

        statements.Add($"var testCommand = fixture.Create<{CommandTypeName}>();");

        return statements;
    }

    public IReadOnlyCollection<CSharpStatement> Get_ProduceCommandWithNullableFields_TestDataStatements()
    {
        var statements = new List<CSharpStatement>();
        foreach (var property in _model.Properties
                     .Where(p => p.TypeReference.IsNullable && p.Mapping?.Element?.AsAssociationEndModel()?.Element?.AsClassModel()?.IsAggregateRoot() == false))
        {
            statements.Add(string.Empty);
            statements.Add("fixture = new Fixture();");
            statements.Add($"fixture.Customize<{CommandTypeName}>(comp => comp.Without(x => x.{property.Name}));");
            statements.Add($"yield return new object[] {{ fixture.Create<{CommandTypeName}>() }};");
        }

        return statements;
    }

    public IReadOnlyCollection<CSharpStatement> Get_ProduceSingleCommandAndEntity_TestDataStatements(
        CommandTargetDomain commandTargetDomain,
        CommandTestDataReturn dataReturn)
    {
        var statements = new List<CSharpStatement>();

        var targetDomainClassType = commandTargetDomain switch
        {
            CommandTargetDomain.Aggregate => TargetDomainTypeName,
            CommandTargetDomain.NestedEntity => AggregateOwnerDomainTypeName,
            _ => throw new ArgumentOutOfRangeException(nameof(commandTargetDomain), commandTargetDomain, null)
        };

        statements.Add("var fixture = new Fixture();");
        statements.AddRange(GetDomainEventBaseAutoFixtureRegistrationStatements());

        AddAggregateDomainTestData();
        AddCommandWithIdentityTestData();

        switch (dataReturn)
        {
            case CommandTestDataReturn.CommandAndAggregateDomain:
                var varName = commandTargetDomain switch
                {
                    CommandTargetDomain.Aggregate => "existingEntity",
                    CommandTargetDomain.NestedEntity => "existingOwnerEntity"
                };
                statements.Add(
                    $"yield return new object[] {{ testCommand, {varName} }};");
                break;
            case CommandTestDataReturn.CommandAndAggregateWithNestedEntityDomain:
                statements.Add($"yield return new object[] {{ testCommand, existingOwnerEntity, existingEntity }};");
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(dataReturn), dataReturn, null);
        }

        return statements;

        void AddAggregateDomainTestData()
        {
            var variableName = commandTargetDomain switch { CommandTargetDomain.Aggregate => "existingEntity", CommandTargetDomain.NestedEntity => "existingOwnerEntity" };

            statements.Add($"var {variableName} = fixture.Create<{targetDomainClassType}>();");
            
            if (commandTargetDomain is not CommandTargetDomain.NestedEntity)
            {
                return;
            }
            
            statements.Add($"var existingEntity = existingOwnerEntity.{OwnerToCompositeNavigationPropertyName}.First();");
            for (var index = 0; index < CompositeToOwnerIdAttributes.Count; index++)
            {
                var ownerIdAttribute = CompositeToOwnerIdAttributes[index];
                if (ownerIdAttribute.Attribute is not null)
                {
                    continue;
                }
                var nestedIdAttribute = TargetDomainIdAttributes[index];
                statements.Add($"existingEntity.{ownerIdAttribute.IdName.ToCSharpIdentifier()} = existingOwnerEntity.{nestedIdAttribute.IdName.ToCSharpIdentifier()};");
            }
        }

        void AddCommandWithIdentityTestData()
        {
            switch (commandTargetDomain)
            {
                case CommandTargetDomain.Aggregate:
                    if (CommandIdFields.Count == 1)
                    {
                        statements.Add(
                            $"fixture.Customize<{CommandTypeName}>(comp => comp.With(x => x.{CommandIdFields[0].Name.ToCSharpIdentifier()}, existingEntity.{TargetDomainIdAttributes[0].IdName.ToCSharpIdentifier()}));");
                    }
                    else
                    {
                        var fluent = new CSharpMethodChainStatement("comp").WithoutSemicolon();
                        statements.Add(new CSharpInvocationStatement($"fixture.Customize<{CommandTypeName}>")
                            .AddArgument(new CSharpLambdaBlock("comp").WithExpressionBody(fluent)));

                        for (var i = 0; i < CommandIdFields.Count; i++)
                        {
                            var commandIdField = CommandIdFields[i];
                            var domainIdAttribute = TargetDomainIdAttributes[i];
                            fluent.AddChainStatement(
                                $"With(x => x.{commandIdField.Name.ToCSharpIdentifier()}, existingEntity.{domainIdAttribute.IdName.ToCSharpIdentifier()})");
                        }
                    }

                    break;
                case CommandTargetDomain.NestedEntity:
                {
                    var fluent = new CSharpMethodChainStatement("comp").WithoutSemicolon();
                    statements.Add(new CSharpInvocationStatement($"fixture.Customize<{CommandTypeName}>")
                        .AddArgument(new CSharpLambdaBlock("comp").WithExpressionBody(fluent)));

                    for (var index = 0; index < TargetDomainIdAttributes.Count; index++)
                    {
                        var idAttribute = TargetDomainIdAttributes[index];
                        var idField = CommandFieldsForOwnerId[index];
                        fluent.AddChainStatement($"With(x => x.{idField.Name.ToCSharpIdentifier()}, existingOwnerEntity.{idAttribute.IdName.ToCSharpIdentifier()})");
                    }
                    
                    for (var i = 0; i < CommandIdFields.Count; i++)
                    {
                        var commandIdField = CommandIdFields[i];
                        var domainIdAttribute = TargetDomainIdAttributes[i];
                        fluent.AddChainStatement(
                            $"With(x => x.{commandIdField.Name.ToCSharpIdentifier()}, existingEntity.{domainIdAttribute.IdName.ToCSharpIdentifier()})");
                    }
                }
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(commandTargetDomain), commandTargetDomain, null);
            }

            statements.Add($"var testCommand = fixture.Create<{CommandTypeName}>();");
        }
    }

    public IReadOnlyCollection<CSharpStatement> Get_ProduceCommandWithNullableFields_ProduceSingleEntity_TestDataStatements(
        CommandTargetDomain commandTargetDomain)
    {
        var statements = new List<CSharpStatement>();
        var entityVarName = commandTargetDomain switch
        {
            CommandTargetDomain.Aggregate => "existingEntity",
            CommandTargetDomain.NestedEntity => "existingOwnerEntity"
        };
        var targetDomainClassType = commandTargetDomain switch
        {
            CommandTargetDomain.Aggregate => TargetDomainTypeName,
            CommandTargetDomain.NestedEntity => AggregateOwnerDomainTypeName
        };

        foreach (var property in GetNullableAttributes())
        {
            statements.Add(string.Empty);
            statements.Add("fixture = new Fixture();");
            statements.AddRange(GetDomainEventBaseAutoFixtureRegistrationStatements());
            statements.Add($"{entityVarName} = fixture.Create<{targetDomainClassType}>();");

            AddCommandWithIdentityTestData(property);

            statements.Add($"yield return new object[] {{ testCommand, {entityVarName} }};");
        }

        return statements;

        IEnumerable<DTOFieldModel> GetNullableAttributes()
        {
            return _model.Properties
                .Where(p => p.TypeReference.IsNullable && p.Mapping?.Element?.AsAssociationEndModel()?.Element?.AsClassModel()?.IsAggregateRoot() == false);
        }

        void AddCommandWithIdentityTestData(DTOFieldModel property)
        {
            switch (commandTargetDomain)
            {
                case CommandTargetDomain.Aggregate:
                    if (CommandIdFields.Count == 1)
                    {
                        statements.Add(
                            $"fixture.Customize<{CommandTypeName}>(comp => comp.Without(x => x.{property.Name}).With(x => x.{CommandIdFields[0].Name.ToCSharpIdentifier()}, {entityVarName}.{TargetDomainIdAttributes[0].IdName.ToCSharpIdentifier()}));");
                    }
                    else
                    {
                        var fluent = new CSharpMethodChainStatement("comp").WithoutSemicolon();
                        statements.Add(new CSharpInvocationStatement($"fixture.Customize<{CommandTypeName}>")
                            .AddArgument(new CSharpLambdaBlock("comp").WithExpressionBody(fluent)));

                        fluent.AddChainStatement($"Without(x => x.{property.Name})");

                        for (var i = 0; i < CommandIdFields.Count; i++)
                        {
                            var commandIdField = CommandIdFields[i];
                            var domainIdAttribute = TargetDomainIdAttributes[i];
                            fluent.AddChainStatement(
                                $"With(x => x.{commandIdField.Name.ToCSharpIdentifier()}, {entityVarName}.{domainIdAttribute.IdName.ToCSharpIdentifier()})");
                        }
                    }

                    break;
                case CommandTargetDomain.NestedEntity:
                {
                    var fluent = new CSharpMethodChainStatement("comp").WithoutSemicolon();
                    statements.Add(new CSharpInvocationStatement($"fixture.Customize<{CommandTypeName}>")
                        .AddArgument(new CSharpLambdaBlock("comp").WithExpressionBody(fluent)));

                    fluent.AddChainStatement($"Without(x => x.{property.Name})");

                    for (var i = 0; i < CommandIdFields.Count; i++)
                    {
                        var commandIdField = CommandIdFields[i];
                        var domainIdAttribute = TargetDomainIdAttributes[i];
                        fluent.AddChainStatement($"With(x => x.{commandIdField.Name.ToCSharpIdentifier()}, {entityVarName}.{domainIdAttribute.IdName.ToCSharpIdentifier()})");
                    }
                }
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(commandTargetDomain), commandTargetDomain, null);
            }

            statements.Add($"testCommand = fixture.Create<{CommandTypeName}>();");
        }
    }

    public IReadOnlyCollection<CSharpStatement> GetCommandHandlerConstructorParameterMockStatements()
    {
        return GetHandlerConstructorParameters()
            .Select(param => new CSharpStatement($"var {param.Name.ToLocalVariableName()} = Substitute.For<{param.Type}>();")
                .AddMetadata("type", param.Type))
            .ToArray();
    }

    public IReadOnlyCollection<CSharpStatement> GetAggregateDomainRepositoryUnitOfWorkMockingStatements()
    {
        return GetDomainRepositoryUnitOfWorkMockingStatements(
            idAttribute => $"expected{SingularTargetDomainName}{idAttribute.IdName.ToPascalCase()}",
            TargetDomainIdAttributes,
            null,
            GetDoExpressionForAggregate(),
            DomainAggregateRepositoryVarName,
            true);

        CSharpStatement GetDoExpressionForAggregate()
        {
            if (TargetDomainIdAttributes.Count == 1)
            {
                return
                    $@"_ => added{SingularTargetDomainName}.{TargetDomainIdAttributes[0].IdName.ToPascalCase()} = expected{SingularTargetDomainName}{TargetDomainIdAttributes[0].IdName.ToPascalCase()}";
            }

            var block = new CSharpLambdaBlock("_");
            foreach (var idAttribute in TargetDomainIdAttributes)
            {
                block.AddStatements(
                    $"added{SingularTargetDomainName}.{idAttribute.IdName.ToPascalCase()} = expected{SingularTargetDomainName}{idAttribute.IdName.ToPascalCase()};");
            }

            return block;
        }
    }


    public IReadOnlyCollection<CSharpStatement> GetAggregateOwnerDomainRepositoryUnitOfWorkMockingStatements(string aggregateOwnerEntityVarName, string commandVarName)
    {
        return GetDomainRepositoryUnitOfWorkMockingStatements(
            idAttr => $"expected{idAttr.IdName.ToPascalCase()}",
            CompositeToOwnerIdAttributes,
            GetInitializationStatements(),
            GetDoExpressionForAggregateOwner(),
            DomainAggregateOwnerRepositoryVarName,
            false);

        IReadOnlyCollection<CSharpStatement> GetInitializationStatements()
        {
            var associationPropertyName = AggregateOwnerDomain.GetNestedCompositeAssociation(TargetDomainModel).Name.ToCSharpIdentifier(CapitalizationBehaviour.AsIs);
            return new CSharpStatement[]
            {
                $"var {PluralTargetDomainName.ToCamelCase()}Snapshot = {aggregateOwnerEntityVarName}.{associationPropertyName}.ToArray();"
            };
        }

        CSharpStatement GetDoExpressionForAggregateOwner()
        {
            var block = new CSharpLambdaBlock("_");
            var associationPropertyName = AggregateOwnerDomain.GetNestedCompositeAssociation(TargetDomainModel).Name.ToCSharpIdentifier(CapitalizationBehaviour.AsIs);
            block.AddStatement($@"added{SingularTargetDomainName} = {aggregateOwnerEntityVarName}.{associationPropertyName}.Except({PluralTargetDomainName.ToCamelCase()}Snapshot).Single();");

            for (var index = 0; index < TargetDomainIdAttributes.Count; index++)
            {
                var idAttribute = TargetDomainIdAttributes[index];
                var ownerIdAttribute = CompositeToOwnerIdAttributes[index];
                block.AddStatement($@"added{SingularTargetDomainName}.{idAttribute.IdName.ToCSharpIdentifier()} = expected{ownerIdAttribute.IdName.ToPascalCase()};");
            }
            
            for (var index = 0; index < CompositeToOwnerIdAttributes.Count; index++)
            {
                var idAttribute = CompositeToOwnerIdAttributes[index];
                if (idAttribute.Attribute is not null)
                {
                    continue;
                }
                var idField = CommandFieldsForOwnerId[index];
                block.AddStatement($@"added{SingularTargetDomainName}.{idAttribute.IdName} = {commandVarName}.{idField.Name.ToPascalCase()};");
            }

            return block;
        }
    }
    
    private IReadOnlyCollection<CSharpStatement> GetDomainRepositoryUnitOfWorkMockingStatements(
        Func<ImplementationStrategyTemplatesExtensions.IEntityId, string> expectedDomainVarName,
        IReadOnlyList<ImplementationStrategyTemplatesExtensions.IEntityId> entityIdList,
        IReadOnlyCollection<CSharpStatement> initializationStatements,
        CSharpStatement doExpression,
        string repositoryVarName,
        bool includeOnAddAssignment)
    {
        var statements = new List<CSharpStatement>();
        if (HasIdReturnTypeOnCommand())
        {
            foreach (var idAttribute in entityIdList)
            {
                statements.Add($@"var {expectedDomainVarName(idAttribute)} = new Fixture().Create<{idAttribute.Type}>();");
            }
        }

        statements.Add($@"{_activeTemplate.GetTypeName(TemplateRoles.Domain.Entity.Primary, TargetDomainModel)} added{SingularTargetDomainName} = null;");

        if (initializationStatements is not null)
        {
            statements.AddRange(initializationStatements);
        }

        if (includeOnAddAssignment)
        {
            var castExpression = _activeTemplate.ExecutionContext.Settings.GetDomainSettings().CreateEntityInterfaces()
                ? $"({_activeTemplate.GetTypeName(TemplateRoles.Domain.Entity.Primary, TargetDomainModel)})"
                : string.Empty;
            statements.Add($"{repositoryVarName}.OnAdd(ent => added{SingularTargetDomainName} = {castExpression}ent);");
        }

        if (HasIdReturnTypeOnCommand())
        {
            statements.Add(new CSharpMethodChainStatement($"{repositoryVarName}.UnitOfWork") { BeforeSeparator = CSharpCodeSeparatorType.NewLine }
                .WithoutSemicolon()
                .AddChainStatement(new CSharpInvocationStatement("When")
                    .WithoutSemicolon()
                    .AddArgument("async x => await x.SaveChangesAsync(CancellationToken.None)")
                )
                .AddChainStatement(new CSharpInvocationStatement("Do")
                    .AddArgument(doExpression)
                )
            );
        }

        return statements;
    }

    public IReadOnlyCollection<CSharpStatement> GetCommandHandlerConstructorSutStatement()
    {
        var inv = new CSharpInvocationStatement($"var sut = new {CommandHandlerTemplate.GetCommandHandlerName(_model)}");
        foreach (var param in GetHandlerConstructorParameters())
        {
            inv.AddArgument(param.Name.ToLocalVariableName());
        }

        return new[] { new CSharpStatement(string.Empty), inv };
    }

    public IReadOnlyCollection<CSharpStatement> GetSutHandleInvocationStatement(string commandVarName)
    {
        var inv = new CSharpInvocationStatement(HasIdReturnTypeOnCommand()
                ? "var result = await sut.Handle"
                : "await sut.Handle")
            .AddArgument(commandVarName)
            .AddArgument("CancellationToken.None");
        return new[] { inv };
    }

    public IReadOnlyCollection<CSharpStatement> GetSutHandleInvocationActLambdaStatement(string commandVarName)
    {
        var inv = new CSharpInvocationStatement("var act = async () => await sut.Handle")
            .AddArgument(commandVarName)
            .AddArgument("CancellationToken.None");
        return new[] { inv };
    }

    public IReadOnlyCollection<CSharpStatement> GetAggregateDomainRepositorySaveChangesAssertionStatement()
    {
        var statements = new List<CSharpStatement>();
        if (HasIdReturnTypeOnCommand())
        {
            statements.Add($"result.Should().Be(expected{SingularTargetDomainName}{TargetDomainIdAttributes.First().IdName.ToPascalCase()});");
            statements.Add($"await {DomainAggregateRepositoryVarName}.UnitOfWork.Received(1).SaveChangesAsync();");
        }

        return statements;
    }

    public IReadOnlyCollection<CSharpStatement> GetAggregateOwnerDomainRepositorySaveChangesAssertionStatement(string entityOwnerVarName)
    {
        var statements = new List<CSharpStatement>();
        if (HasIdReturnTypeOnCommand())
        {
            statements.Add($"result.Should().Be(expected{CompositeToOwnerIdAttributes[0].IdName.ToPascalCase()});");
        }
        else
        {
            var associationPropertyName = AggregateOwnerDomain.GetNestedCompositeAssociation(TargetDomainModel).Name.ToCSharpIdentifier(CapitalizationBehaviour.AsIs);
            statements.Add($"added{SingularTargetDomainName} = {entityOwnerVarName}.{associationPropertyName}.Single(p => {GetDomainIdKeyComparisonExpression("p", "default")});");
        }

        statements.Add($"await {DomainAggregateOwnerRepositoryVarName}.UnitOfWork.Received(1).SaveChangesAsync();");
        return statements;
    }

    public IReadOnlyCollection<CSharpStatement> GetCommandCompareToNewAddedDomainAssertionStatement(string commandVarName)
    {
        return GetCommandCompareToExistingDomainAssertionStatement(TargetDomainModel, commandVarName, $"added{SingularTargetDomainName}");
    }

    public IReadOnlyCollection<CSharpStatement> GetCommandCompareToNewAddedDomainFromOwnerAssertionStatement(string commandVarName)
    {
        return GetCommandCompareToExistingDomainAssertionStatement(AggregateOwnerDomain, commandVarName, $"added{SingularTargetDomainName}");
    }

    public IReadOnlyCollection<CSharpStatement> GetCommandCompareToExistingDomainAssertionStatement(string commandVarName, string entityVarName)
    {
        return GetCommandCompareToExistingDomainAssertionStatement(TargetDomainModel, commandVarName, entityVarName);
    }

    public IReadOnlyCollection<CSharpStatement> GetCommandCompareToExistingDomainFromOwnerAssertionStatement(string commandVarName, string entityVarName)
    {
        return GetCommandCompareToExistingDomainAssertionStatement(AggregateOwnerDomain, commandVarName, entityVarName);
    }

    private IReadOnlyCollection<CSharpStatement> GetCommandCompareToExistingDomainAssertionStatement(ClassModel model, string commandVarName, string entityVarName)
    {
        var inv = new CSharpInvocationStatement($"{_activeTemplate.GetAssertionClassName(model)}.AssertEquivalent")
            .AddArgument(commandVarName)
            .AddArgument(entityVarName);
        return new[] { inv };
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

    public enum MockRepositoryResponse
    {
        ReturnDomainVariable,
        ReturnDefault
    }

    public IReadOnlyCollection<CSharpStatement> GetAggregateDomainRepositoryFindByIdMockingStatements(string commandVarName, string entityVarName, MockRepositoryResponse response)
    {
        return GetDomainRepositoryFindByIdMockingStatements(
            commandVarName,
            entityVarName,
            response,
            TargetDomainTypeName,
            TargetDomainModel,
            DomainAggregateRepositoryVarName,
            CommandIdFields);
    }

    public IReadOnlyCollection<CSharpStatement> GetAggregateOwnerDomainRepositoryFindByIdMockingStatements(string commandVarName, string entityVarName,
        MockRepositoryResponse response)
    {
        return GetDomainRepositoryFindByIdMockingStatements(
            commandVarName,
            entityVarName,
            response,
            AggregateOwnerDomainTypeName,
            AggregateOwnerDomain,
            DomainAggregateOwnerRepositoryVarName,
            CommandFieldsForOwnerId);
    }

    private IReadOnlyCollection<CSharpStatement> GetDomainRepositoryFindByIdMockingStatements(
        string commandVarName,
        string entityVarName,
        MockRepositoryResponse response,
        string targetType,
        ClassModel domainModel,
        string repositoryVarName,
        IReadOnlyList<DTOFieldModel> commandIdFields)
    {
        var statements = new List<CSharpStatement>();
        var returns = response switch
        {
            MockRepositoryResponse.ReturnDomainVariable =>
                $".Returns({GetFromResultExpression(entityVarName ?? throw new ArgumentNullException(nameof(entityVarName)), targetType, domainModel, true, false)})",
            MockRepositoryResponse.ReturnDefault =>
                $".Returns({GetFromResultExpression("default", targetType, domainModel, false, false)})",
            _ => throw new ArgumentOutOfRangeException(nameof(response), response, null)
        };
        statements.Add($"{repositoryVarName}.FindByIdAsync({GetCommandIdKeysList(commandVarName, commandIdFields)}, CancellationToken.None)!{returns};");
        return statements;
    }

    public IReadOnlyCollection<CSharpStatement> GetDomainAggegrateRepositoryRemovedAssertionStatement(string commandVarName)
    {
        return GetDomainRepositoryRemovedAssertionStatement(commandVarName, DomainAggregateRepositoryVarName);
    }

    public IReadOnlyCollection<CSharpStatement> GetDomainAggregateOwnerRepositoryRemovedAssertionStatement(string commandVarName)
    {
        return GetDomainRepositoryRemovedAssertionStatement(commandVarName, DomainAggregateOwnerRepositoryVarName);
    }

    private IReadOnlyCollection<CSharpStatement> GetDomainRepositoryRemovedAssertionStatement(string commandVarName, string repoVarName)
    {
        var statements = new List<CSharpStatement>();

        statements.Add($"{repoVarName}.Received(1).Remove(Arg.Is<{TargetDomainTypeName}>(p => {GetCommandAndDomainIdKeyComparisonExpression(commandVarName, "p")}));");

        return statements;
    }

    public IReadOnlyCollection<CSharpStatement> GetNewCommandAutoFixtureInlineStatements(string commandVarName)
    {
        var statements = new List<CSharpStatement>();
        statements.Add($"var fixture = new Fixture();");
        statements.Add($"var {commandVarName} = fixture.Create<{CommandTypeName}>();");
        return statements;
    }

    public IReadOnlyCollection<CSharpStatement> GetThrowsExceptionAssertionStatement(string exceptionTypeName)
    {
        var statements = new List<CSharpStatement>();
        statements.Add($"await act.Should().ThrowAsync<{exceptionTypeName}>();");
        return statements;
    }

    public IReadOnlyCollection<CSharpStatement> GetNestedEntityRemovedFromOwningAggregateAssertionStatements(string ownerEntityVarName, string commandVarName)
    {
        var statements = new List<CSharpStatement>();
        statements.Add(
            $"{ownerEntityVarName}.{OwnerToCompositeNavigationPropertyName}.Should().NotContain(p => {GetCommandAndDomainIdKeyComparisonExpression(commandVarName, "p")});");
        return statements;
    }

    private IReadOnlyCollection<(string Type, string Name)> GetHandlerConstructorParameters()
    {
        var ctor = ((ICSharpFileBuilderTemplate)CommandHandlerTemplate).CSharpFile.Classes.First(x => x.HasMetadata("handler")).Constructors.First();
        return ctor.Parameters
            .Select(param => (param.Type, param.Name.ToLocalVariableName()))
            .ToArray();
    }

    private string GetCommandIdKeysList(string commandVarName, IReadOnlyList<DTOFieldModel> commandIdFields)
    {
        var left = commandIdFields.Count > 1 ? "(" : string.Empty;
        var right = commandIdFields.Count > 1 ? ")" : string.Empty;

        return $"{left}{string.Join(", ", commandIdFields.Select(idField => $"{commandVarName}.{idField.Name.ToCSharpIdentifier()}"))}{right}";
    }

    private string GetCommandAndDomainIdKeyComparisonExpression(string commandVarName, string entityVarName)
    {
        return string.Join(" && ", Enumerable.Range(0, CommandIdFields.Count)
            .Select(index =>
            {
                var commandIdField = CommandIdFields[index];
                var domainIdField = TargetDomainIdAttributes[index];
                return $"{commandVarName}.{commandIdField.Name.ToCSharpIdentifier()} == {entityVarName}.{domainIdField.IdName.ToCSharpIdentifier()}";
            }));
    }

    private string GetDomainIdKeyComparisonExpression(string entityVarName, string expressionToCompareTo)
    {
        return string.Join(" && ", Enumerable.Range(0, TargetDomainIdAttributes.Count)
            .Select(index =>
            {
                var domainIdField = TargetDomainIdAttributes[index];
                return $"{entityVarName}.{domainIdField.IdName.ToCSharpIdentifier()} == {expressionToCompareTo}";
            }));
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