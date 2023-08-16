using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Modelers.Domain.Api;
using Intent.Modelers.Services.CQRS.Api;
using Intent.Modules.Application.MediatR.CRUD.CrudStrategies;
using Intent.Modules.Application.MediatR.CRUD.Tests.Templates.Assertions.AssertionClass;
using Intent.Modules.Application.MediatR.Templates;
using Intent.Modules.Application.MediatR.Templates.CommandModels;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.Constants;
using Intent.Modules.Entities.Repositories.Api.Templates;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.Application.MediatR.CRUD.Tests.Templates.Owner.UpdateCommandHandlerTests;

[IntentManaged(Mode.Fully, Body = Mode.Merge)]
public partial class UpdateCommandHandlerTestsTemplate : CSharpTemplateBase<CommandModel>, ICSharpFileBuilderTemplate
{
    public const string TemplateId = "Intent.Application.MediatR.CRUD.Tests.Owner.UpdateCommandHandlerTests";

    [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
    public UpdateCommandHandlerTestsTemplate(IOutputTarget outputTarget, CommandModel model) : base(TemplateId, outputTarget, model)
    {
        AddNugetDependency(NugetPackages.AutoFixture);
        AddNugetDependency(NugetPackages.FluentAssertions);
        AddNugetDependency(NugetPackages.MicrosoftNetTestSdk);
        AddNugetDependency(NugetPackages.NSubstitute);
        AddNugetDependency(NugetPackages.Xunit);
        AddNugetDependency(NugetPackages.XunitRunnerVisualstudio);

        AddTypeSource(TemplateFulfillingRoles.Application.Contracts.Dto);

        Facade = new CommandHandlerFacade(this, model);
        
        CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
            .AddClass($"{Model.Name}HandlerTests")
            .AfterBuild(file =>
            {
                AddUsingDirectives(file);
                Facade.AddHandlerConstructorMockUsings();

                var priClass = file.Classes.First();

                priClass.AddMethod("IEnumerable<object[]>", "GetSuccessfulResultTestData", method =>
                {
                    method.Static();
                    method.AddStatements(Facade.Get_ProduceSingleCommandAndAggregateEntity_TestDataStatements());
                    method.AddStatements(Facade.Get_ProduceCommandWithNullableFields_ProduceSingleAggregateEntity_TestDataStatements());
                });

                priClass.AddMethod("Task", "Handle_WithValidCommand_UpdatesExistingEntity", method =>
                {
                    method.Async();
                    method.AddAttribute("Theory");
                    method.AddAttribute("MemberData(nameof(GetSuccessfulResultTestData))");
                    method.AddParameter(Facade.CommandTypeName, "testCommand");
                    method.AddParameter(Facade.TargetDomainTypeName, "existingEntity");
                    
                    method.AddStatement("// Arrange");
                    method.AddStatements(Facade.GetCommandHandlerConstructorParameterMockStatements());
                    method.AddStatements(Facade.GetAggregateDomainRepositoryFindByIdMockingStatements("testCommand", "existingEntity", CommandHandlerFacade.MockRepositoryResponse.ReturnDomainVariable));
                    method.AddStatements(Facade.GetCommandHandlerConstructorSutStatement());
                    
                    method.AddStatement(string.Empty);
                    method.AddStatement("// Act");
                    method.AddStatements(Facade.GetSutHandleInvocationStatement("testCommand"));
                    
                    method.AddStatement(string.Empty);
                    method.AddStatement("// Assert");
                    method.AddStatements(Facade.GetCommandCompareToExistingDomainAssertionStatement("testCommand", "existingEntity"));
                });

                priClass.AddMethod("Task", "Handle_WithInvalidIdCommand_ReturnsNotFound", method =>
                {
                    method.Async();
                    method.AddAttribute("Fact");
                    method.AddStatement("// Arrange");
                    method.AddStatements(Facade.GetNewCommandAutoFixtureInlineStatements("testCommand"));
                    method.AddStatements(Facade.GetCommandHandlerConstructorParameterMockStatements());
                    method.AddStatements(Facade.GetAggregateDomainRepositoryFindByIdMockingStatements("testCommand", "", CommandHandlerFacade.MockRepositoryResponse.ReturnDefault));
                    method.AddStatement(string.Empty);
                    method.AddStatements(Facade.GetCommandHandlerConstructorSutStatement());

                    method.AddStatement(string.Empty);
                    method.AddStatement("// Act");
                    method.AddStatements(Facade.GetSutHandleInvocationActLambdaStatement("testCommand"));
                    
                    method.AddStatement(string.Empty);
                    method.AddStatement("// Assert");
                    method.AddStatements(Facade.GetThrowsExceptionAssertionStatement(this.GetNotFoundExceptionName()));
                });
                
                this.AddCommandAssertionMethods(Model);
            });
    }

    private CommandHandlerFacade Facade { get; }

    private static void AddUsingDirectives(CSharpFile file)
    {
        file.AddUsing("System");
        file.AddUsing("System.Collections.Generic");
        file.AddUsing("System.Linq");
        file.AddUsing("System.Threading");
        file.AddUsing("System.Threading.Tasks");
        file.AddUsing("AutoFixture");
        file.AddUsing("FluentAssertions");
        file.AddUsing("NSubstitute");
        file.AddUsing("Xunit");
    }

    [IntentManaged(Mode.Fully)]
    public CSharpFile CSharpFile { get; }

    [IntentManaged(Mode.Fully)]
    protected override CSharpFileConfig DefineFileConfig()
    {
        return CSharpFile.GetConfig();
    }

    [IntentManaged(Mode.Fully)]
    public override string TransformText()
    {
        return CSharpFile.ToString();
    }
}