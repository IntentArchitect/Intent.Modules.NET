using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Modelers.Domain.Api;
using Intent.Modelers.Services.CQRS.Api;
using Intent.Modules.Application.MediatR.CRUD.CrudStrategies;
using Intent.Modules.Application.MediatR.CRUD.Tests.Templates.Assertions.AssertionClass;
using Intent.Modules.Application.MediatR.CRUD.Tests.Templates.Extensions.RepositoryExtensions;
using Intent.Modules.Application.MediatR.Templates;
using Intent.Modules.Application.MediatR.Templates.CommandHandler;
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

namespace Intent.Modules.Application.MediatR.CRUD.Tests.Templates.Nested.NestedCreateCommandHandlerTests;

[IntentManaged(Mode.Fully, Body = Mode.Merge)]
public partial class NestedCreateCommandHandlerTestsTemplate : CSharpTemplateBase<CommandModel>, ICSharpFileBuilderTemplate
{
    public const string TemplateId = "Intent.Application.MediatR.CRUD.Tests.Nested.NestedCreateCommandHandlerTests";

    [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
    public NestedCreateCommandHandlerTestsTemplate(IOutputTarget outputTarget, CommandModel model) : base(TemplateId, outputTarget, model)
    {
        AddNugetDependency(NugetPackages.AutoFixture);
        AddNugetDependency(NugetPackages.FluentAssertions);
        AddNugetDependency(NugetPackages.MicrosoftNetTestSdk);
        AddNugetDependency(NugetPackages.NSubstitute);
        AddNugetDependency(NugetPackages.Xunit);
        AddNugetDependency(NugetPackages.XunitRunnerVisualstudio);

        AddTypeSource(TemplateRoles.Application.Contracts.Dto);

        CSharpFile = new CSharpFile($"{this.GetNamespace()}", $"{this.GetFolderPath()}")
            .AddClass($"{Model.Name}HandlerTests")
            .AfterBuild(file =>
            {
                var facade = new CommandHandlerFacade(this, model);

                AddUsingDirectives(file);
                facade.AddHandlerConstructorMockUsings();

                var priClass = file.Classes.First();

                priClass.AddMethod("IEnumerable<object[]>", "GetSuccessfulResultTestData", method =>
                {
                    method.Static();
                    method.AddStatements(facade.Get_ProduceSingleCommandAndEntity_TestDataStatements(
                        CommandTargetDomain.NestedEntity,
                        CommandTestDataReturn.CommandAndAggregateDomain));
                    method.AddStatements(facade.Get_ProduceCommandWithNullableFields_ProduceSingleEntity_TestDataStatements(
                        CommandTargetDomain.NestedEntity));
                });

                priClass.AddMethod("Task", $"Handle_WithValidCommand_Adds{facade.SingularTargetDomainName}To{facade.SingularAggregateOwnerDomainName}", method =>
                {
                    method.Async();
                    method.AddAttribute("Theory");
                    method.AddAttribute("MemberData(nameof(GetSuccessfulResultTestData))");
                    method.AddParameter(facade.CommandTypeName, "testCommand");
                    method.AddParameter(facade.AggregateOwnerDomainTypeName, "existingOwnerEntity");

                    method.AddStatement($@"// Arrange");
                    method.AddStatements(facade.GetCommandHandlerConstructorParameterMockStatements());
                    method.AddStatements(facade.GetAggregateOwnerDomainRepositoryFindByIdMockingStatements("testCommand", "existingOwnerEntity", CommandHandlerFacade.MockRepositoryResponse.ReturnDomainVariable));
                    method.AddStatements(facade.GetAggregateOwnerDomainRepositoryUnitOfWorkMockingStatements("existingOwnerEntity", "testCommand"));
                    method.AddStatements(facade.GetCommandHandlerConstructorSutStatement());

                    method.AddStatement(string.Empty);
                    method.AddStatement("// Act");
                    method.AddStatements(facade.GetSutHandleInvocationStatement("testCommand"));

                    method.AddStatement(string.Empty);
                    method.AddStatement("// Assert");
                    method.AddStatements(facade.GetAggregateOwnerDomainRepositorySaveChangesAssertionStatement("existingOwnerEntity"));
                    method.AddStatements(facade.GetCommandCompareToNewAddedDomainFromOwnerAssertionStatement("testCommand"));
                });

                AddAssertionMethods();
            });
    }

    private bool? _canRunTemplate;

    public override bool CanRunTemplate()
    {
        if (_canRunTemplate.HasValue)
        {
            return _canRunTemplate.Value;
        }

        var template = this.GetCommandHandlerTemplate(Model, trackDependency: false);
        if (template is null)
        {
            _canRunTemplate = false;
        }
        else if (StrategyFactory.GetMatchedCommandStrategy(template) is CreateImplementationStrategy strategy && strategy.IsMatch())
        {
            _canRunTemplate = Model.GetClassModel()?.IsAggregateRoot() == false && Model.GetClassModel().InternalElement.Package.HasStereotype("Relational Database");
        }
        else
        {
            _canRunTemplate = false;
        }

        return _canRunTemplate.Value;
    }


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

    private void AddAssertionMethods()
    {
        if (!Model.IsValidCommandMapping())
        {
            return;
        }

        var nestedDomainElement = Model.GetClassModel();
        var ownerDomainElement = nestedDomainElement.GetNestedCompositionalOwner();
        var template = ExecutionContext.FindTemplateInstance<ICSharpFileBuilderTemplate>(
            TemplateDependency.OnModel(AssertionClassTemplate.TemplateId, ownerDomainElement));
        if (template == null)
        {
            return;
        }

        template.AddAssertionMethods(template.CSharpFile.Classes.First(), Model, nestedDomainElement);
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