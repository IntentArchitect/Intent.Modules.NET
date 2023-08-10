using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Modelers.Domain.Api;
using Intent.Modelers.Services.CQRS.Api;
using Intent.Modules.Application.MediatR.CRUD.CrudStrategies;
using Intent.Modules.Application.MediatR.Templates;
using Intent.Modules.Application.MediatR.Templates.CommandHandler;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.Constants;

namespace Intent.Modules.Application.MediatR.CRUD.Tests.Templates;

public class CommandHandlerFacade
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
        DomainClassName = DomainClassModel.Name.ToPascalCase();
        DomainAttributeId = DomainClassModel.GetEntityIdAttribute(activeTemplate.ExecutionContext);
    }

    public CommandHandlerTemplate CommandHandlerTemplate { get; }
    public ClassModel DomainClassModel { get; }
    public string DomainClassName { get; }
    public ImplementationStrategyTemplatesExtensions.EntityIdAttribute DomainAttributeId { get; }
    
    public string DomainClassRepositoryName => _activeTemplate.GetTypeName(TemplateFulfillingRoles.Repository.Interface.Entity, DomainClassModel);
    public string DomainRepositoryVarName => GetHandlerConstructorParameters().First(p => p.Type == DomainClassRepositoryName).Name;

    public bool HasCommandHandlerTemplate()
    {
        return CommandHandlerTemplate is not null;
    }

    public bool HasIdReturnTypeOnCommand()
    {
        return _model.TypeReference.Element is not null;
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

    public IReadOnlyCollection<CSharpStatement> GetRepositoryUnitOfWorkMockingStatements()
    {
        var statements = new List<CSharpStatement>();
        if (HasIdReturnTypeOnCommand())
        {
            statements.Add($@"var expected{DomainClassName}Id = new Fixture().Create<{DomainAttributeId.Type}>();");
        }
        
        statements.Add($@"{_activeTemplate.GetTypeName(DomainClassModel.InternalElement)} added{DomainClassName} = null;
        repository.OnAdd(ent => added{DomainClassName} = ent);");
        
        if (HasIdReturnTypeOnCommand())
        {
            statements.Add(new CSharpMethodChainStatement($"{DomainRepositoryVarName}.UnitOfWork") { BeforeSeparator = CSharpCodeSeparatorType.NewLine }
                .WithoutSemicolon()
                .AddChainStatement(new CSharpInvocationStatement("When")
                    .WithoutSemicolon()
                    .AddArgument("async x => await x.SaveChangesAsync(CancellationToken.None)")
                )
                .AddChainStatement(new CSharpInvocationStatement("Do")
                    .AddArgument($@"_ => added{DomainClassName}.{DomainAttributeId.IdName} = expected{DomainClassName}Id")
                )
            );
        }

        return statements;
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
            statements.Add($"result.Should().Be(expected{DomainClassName}Id);");
        }
        statements.Add($"await {DomainRepositoryVarName}.UnitOfWork.Received(1).SaveChangesAsync();");

        return statements;
    }

    public IReadOnlyCollection<CSharpStatement> GetDomainAssertionStatement(string commandVarName)
    {
        var inv = new CSharpInvocationStatement($"{_activeTemplate.GetAssertionClassName(DomainClassModel)}.AssertEquivalent")
            .AddArgument(commandVarName)
            .AddArgument($"added{DomainClassName}");
        return new[] { inv };
    }
}