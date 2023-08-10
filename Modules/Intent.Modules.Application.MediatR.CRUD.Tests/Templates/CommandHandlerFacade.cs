﻿using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Modelers.Domain.Api;
using Intent.Modelers.Services.Api;
using Intent.Modelers.Services.CQRS.Api;
using Intent.Modules.Application.MediatR.CRUD.CrudStrategies;
using Intent.Modules.Application.MediatR.Templates;
using Intent.Modules.Application.MediatR.Templates.CommandHandler;
using Intent.Modules.Application.MediatR.Templates.CommandModels;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.Constants;

namespace Intent.Modules.Application.MediatR.CRUD.Tests.Templates;

internal class CommandHandlerFacade
{
    private readonly ICSharpFileBuilderTemplate _activeTemplate;
    private readonly CommandModel _model;

    public CommandHandlerFacade(ICSharpFileBuilderTemplate activeTemplate, CommandModel model)
    {
        _activeTemplate = activeTemplate;
        _model = model;
        
        activeTemplate.TryGetTemplate<CommandHandlerTemplate>(CommandHandlerTemplate.TemplateId, model, out var foundCommandHandlerTemplate);
        CommandHandlerTemplate = foundCommandHandlerTemplate;
        
        DomainClassModel = model.Mapping.Element.AsClassModel();
        CommandIdFields = model.Properties.GetEntityIdFields(DomainClassModel);
        DomainIdAttributes = DomainClassModel.GetEntityIdAttributes(activeTemplate.ExecutionContext).ToList();
    }

    public CommandHandlerTemplate CommandHandlerTemplate { get; }
    public ClassModel DomainClassModel { get; }
    public IReadOnlyList<DTOFieldModel> CommandIdFields { get; }
    public IReadOnlyList<ImplementationStrategyTemplatesExtensions.EntityIdAttribute> DomainIdAttributes { get; }
    
    public string DomainClassRepositoryName => _activeTemplate.GetTypeName(TemplateFulfillingRoles.Repository.Interface.Entity, DomainClassModel);
    public string DomainRepositoryVarName => GetHandlerConstructorParameters().First(p => p.Type == DomainClassRepositoryName).Name;
    public string DomainEventBaseName => _activeTemplate.TryGetTypeName("Intent.DomainEvents.DomainEventBase", out var domainEventBaseName) ? domainEventBaseName : null;
    public string CommandTypeName => _activeTemplate.GetTypeName(CommandModelsTemplate.TemplateId, _model);
    public string DomainClassTypeName => _activeTemplate.GetTypeName(TemplateFulfillingRoles.Domain.Entity.Primary, DomainClassModel);
    
    public bool HasCommandHandlerTemplate()
    {
        return CommandHandlerTemplate is not null;
    }

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
            CommandHandlerTemplate.AddUsing(declareUsing);
        }
    }
    
    private IReadOnlyCollection<(string Type, string Name)> GetHandlerConstructorParameters()
    {
        if (!HasCommandHandlerTemplate())
        {
            return ArraySegment<(string Type, string Name)>.Empty;
        }
    
        var ctor = CommandHandlerTemplate.CSharpFile.Classes.First().Constructors.First();
        return ctor.Parameters
            .Select(param => (param.Type, param.Name.ToLocalVariableName()))
            .ToArray();
    }

    public IReadOnlyCollection<CSharpStatement> GetCommandHandlerConstructorParameterMockStatements()
    {
        if (!HasCommandHandlerTemplate())
        {
            return ArraySegment<CSharpStatement>.Empty;
        }
    
        return GetHandlerConstructorParameters()
            .Select(param => new CSharpStatement($"var {param.Name.ToLocalVariableName()} = Substitute.For<{param.Type}>();")
                .AddMetadata("type", param.Type))
            .ToArray();
    }

    public IReadOnlyCollection<CSharpStatement> GetDomainRepositoryUnitOfWorkMockingStatements()
    {
        var statements = new List<CSharpStatement>();
        if (HasIdReturnTypeOnCommand())
        {
            foreach (var idAttribute in DomainIdAttributes)
            {
                statements.Add($@"var expected{DomainClassTypeName}{idAttribute.IdName.ToPascalCase()} = new Fixture().Create<{idAttribute.Type}>();");
            }
        }
        
        statements.Add($@"{_activeTemplate.GetTypeName(TemplateFulfillingRoles.Domain.Entity.Primary, DomainClassModel)} added{DomainClassTypeName} = null;
        repository.OnAdd(ent => added{DomainClassTypeName} = ent);");
        
        if (HasIdReturnTypeOnCommand())
        {
            statements.Add(new CSharpMethodChainStatement($"{DomainRepositoryVarName}.UnitOfWork") { BeforeSeparator = CSharpCodeSeparatorType.NewLine }
                .WithoutSemicolon()
                .AddChainStatement(new CSharpInvocationStatement("When")
                    .WithoutSemicolon()
                    .AddArgument("async x => await x.SaveChangesAsync(CancellationToken.None)")
                )
                .AddChainStatement(new CSharpInvocationStatement("Do")
                    .AddArgument(GetDoExpression())
                )
            );
        }

        return statements;

        CSharpStatement GetDoExpression()
        {
            if (DomainIdAttributes.Count == 1)
            {
                return $@"_ => added{DomainClassTypeName}.{DomainIdAttributes[0].IdName.ToPascalCase()} = expected{DomainClassTypeName}{DomainIdAttributes[0].IdName.ToPascalCase()}";
            }

            var block = new CSharpLambdaBlock("_");
            foreach (var idAttribute in DomainIdAttributes)
            {
                block.AddStatements($"added{DomainClassTypeName}.{idAttribute.IdName.ToPascalCase()} = expected{DomainClassTypeName}{idAttribute.IdName.ToPascalCase()};");
            }
            return block;
        }
    }

    public IReadOnlyCollection<CSharpStatement> GetCommandHandlerConstructorSutStatement()
    {
        if (!HasCommandHandlerTemplate())
        {
            return ArraySegment<CSharpStatement>.Empty;
        }
        
        var inv = new CSharpInvocationStatement($"var sut = new {CommandHandlerTemplate.GetCommandHandlerName(_model)}");
        foreach (var param in GetHandlerConstructorParameters())
        {
            inv.AddArgument(param.Name.ToLocalVariableName());
        }

        return new[] { inv };
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

    public IReadOnlyCollection<CSharpStatement> GetRepositorySaveChangesAssertionStatement()
    {
        var statements = new List<CSharpStatement>();
        if (HasIdReturnTypeOnCommand())
        {
            statements.Add($"result.Should().Be(expected{DomainClassTypeName}{DomainIdAttributes.First().IdName.ToPascalCase()});");
        }
        statements.Add($"await {DomainRepositoryVarName}.UnitOfWork.Received(1).SaveChangesAsync();");

        return statements;
    }

    public IReadOnlyCollection<CSharpStatement> GetDomainAssertionStatement(string commandVarName)
    {
        var inv = new CSharpInvocationStatement($"{_activeTemplate.GetAssertionClassName(DomainClassModel)}.AssertEquivalent")
            .AddArgument(commandVarName)
            .AddArgument($"added{DomainClassTypeName}");
        return new[] { inv };
    }

    public IReadOnlyCollection<CSharpStatement> GetInitialCreateCommandAutoFixtureTestData()
    {
        var statements = new List<CSharpStatement>();

        statements.Add("var fixture = new Fixture();");
        statements.Add($@"yield return new object[] {{ fixture.Create<{CommandTypeName}>() }};");
        
        return statements;
    }

    public IReadOnlyCollection<CSharpStatement> GetCreateCommandWithNullableCompositePropertiesTestData()
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

    public IReadOnlyCollection<CSharpStatement> GetInitialCommandAndDomainEntityAutoFixtureTestData()
    {
        var statements = new List<CSharpStatement>();

        statements.Add("var fixture = new Fixture();");
        statements.AddRange(GetDomainEventBaseAutoFixtureRegistration());
        statements.Add($"var existingEntity = fixture.Create<{DomainClassTypeName}>();");
        
        if (CommandIdFields.Count == 1)
        {
            statements.Add($"fixture.Customize<{CommandTypeName}>(comp => comp.With(x => x.{CommandIdFields[0].Name.ToCSharpIdentifier()}, existingEntity.{DomainIdAttributes[0].IdName.ToCSharpIdentifier()}));");
        }
        else
        {
            var fluent = new CSharpMethodChainStatement("comp").WithoutSemicolon();
            statements.Add(new CSharpInvocationStatement($"fixture.Customize<{CommandTypeName}>")
                .AddArgument(new CSharpLambdaBlock("comp").WithExpressionBody(fluent)));
            
            for (var i = 0; i < CommandIdFields.Count; i++)
            {
                var commandIdField = CommandIdFields[i];
                var domainIdAttribute = DomainIdAttributes[i];
                fluent.AddChainStatement($"With(x => x.{commandIdField.Name.ToCSharpIdentifier()}, existingEntity.{domainIdAttribute.IdName.ToCSharpIdentifier()})");
            }
        }
        
        statements.Add($"var testCommand = fixture.Create<{CommandTypeName}>();");
        statements.Add($"yield return new object[] {{ testCommand, existingEntity }};");
        
        return statements;
    }
    
    public IReadOnlyCollection<CSharpStatement> GetDomainEventBaseAutoFixtureRegistration()
    {
        if (!HasDomainEventBaseName())
        {
            return ArraySegment<CSharpStatement>.Empty;
        }

        var statements = new List<CSharpStatement>();
        
        statements.Add($@"fixture.Register<{DomainEventBaseName}>(() => null);");

        statements.Add($@"fixture.Customize<{_activeTemplate.GetTypeName(TemplateFulfillingRoles.Domain.Entity.Primary, DomainClassModel)}>(comp => comp.Without(x => x.DomainEvents));");

        return statements;
    }

    public IReadOnlyCollection<CSharpStatement> GetDomainRepositoryFindByIdMockingStatements(string commandVarName, string entityVarName)
    {
        var statements = new List<CSharpStatement>();
        statements.Add($"repository.FindByIdAsync({GetCommandIdKeyExpression(commandVarName)}).Returns(Task.FromResult({entityVarName}));");
        return statements;
    }

    private string GetCommandIdKeyExpression(string commandVarName)
    {
        if (CommandIdFields.Count == 1)
        {
            return $"{commandVarName}.{CommandIdFields[0].Name.ToCSharpIdentifier()}";
        }

        return $"({string.Join(", ", CommandIdFields.Select(idField => $"{commandVarName}.{idField.Name.ToCSharpIdentifier()}"))})";
    }
}