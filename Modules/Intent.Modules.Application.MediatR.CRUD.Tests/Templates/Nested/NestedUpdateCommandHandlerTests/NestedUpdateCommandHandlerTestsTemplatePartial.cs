using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Modelers.Domain.Api;
using Intent.Modelers.Services.Api;
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

namespace Intent.Modules.Application.MediatR.CRUD.Tests.Templates.Nested.NestedUpdateCommandHandlerTests;

[IntentManaged(Mode.Fully, Body = Mode.Merge)]
public partial class NestedUpdateCommandHandlerTestsTemplate : CSharpTemplateBase<CommandModel>, ICSharpFileBuilderTemplate
{
    public const string TemplateId = "Intent.Application.MediatR.CRUD.Tests.Nested.NestedUpdateCommandHandlerTests";

    [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
    public NestedUpdateCommandHandlerTestsTemplate(IOutputTarget outputTarget, CommandModel model) : base(TemplateId, outputTarget, model)
    {
        AddNugetDependency(NugetPackages.AutoFixture);
        AddNugetDependency(NugetPackages.FluentAssertions);
        AddNugetDependency(NugetPackages.MicrosoftNetTestSdk);
        AddNugetDependency(NugetPackages.NSubstitute);
        AddNugetDependency(NugetPackages.Xunit);
        AddNugetDependency(NugetPackages.XunitRunnerVisualstudio);

        AddTypeSource(TemplateFulfillingRoles.Domain.Entity.Primary);
        AddTypeSource(CommandModelsTemplate.TemplateId);
        AddTypeSource(TemplateFulfillingRoles.Application.Contracts.Dto);

        CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
            .AddClass($"{Model.Name}HandlerTests")
            .OnBuild(file =>
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

                var dtoModel = Model.TypeReference.Element.AsDTOModel();
                var nestedDomainElement = Model.Mapping.Element.AsClassModel();
                var nestedDomainElementName = nestedDomainElement.Name.ToPascalCase();
                var nestedDomainElementIdName = nestedDomainElement.GetEntityIdAttribute(ExecutionContext).IdName;
                var ownerDomainElement = nestedDomainElement.GetNestedCompositionalOwner();
                var ownerDomainElementIdName = ownerDomainElement.GetEntityIdAttribute(ExecutionContext).IdName;
                var nestedOwnerIdField = Model.Properties.GetNestedCompositionalOwnerIdField(ownerDomainElement);
                var nestedOwnerIdFieldName = nestedOwnerIdField.Name;
                var commandIdFieldName = Model.Properties.GetEntityIdField(nestedDomainElement).Name;
                var nestedAssociationName = ownerDomainElement.GetNestedCompositeAssociation(nestedDomainElement).Name.ToCSharpIdentifier();

                var priClass = file.Classes.First();
                
                priClass.AddMethod("IEnumerable<object[]>", "GetSuccessfulResultTestData", method =>
                {
                    method.Static();
                    method.AddStatements($@"
        var fixture = new Fixture();");
                    this.RegisterDomainEventBaseFixture(method, ownerDomainElement);
                    method.AddStatements($@"
        var existingOwnerEntity = fixture.Create<{GetTypeName(ownerDomainElement.InternalElement)}>();
        var expectedEntity = existingOwnerEntity.{nestedAssociationName}.First();
        fixture.Customize<{GetTypeName(Model.InternalElement)}>(comp => comp
            .With(x => x.{commandIdFieldName}, expectedEntity.Id)
            .With(x => x.{nestedOwnerIdFieldName}, existingOwnerEntity.{ownerDomainElementIdName}));
        var testCommand = fixture.Create<{GetTypeName(Model.InternalElement)}>();
        yield return new object[] {{ testCommand, existingOwnerEntity, expectedEntity }};");
                });
                
                priClass.AddMethod("Task", "Handle_WithValidCommand_UpdatesExistingEntity", method =>
                {
                    method.Async();
                    method.AddAttribute("Theory");
                    method.AddAttribute("MemberData(nameof(GetSuccessfulResultTestData))");
                    method.AddParameter(GetTypeName(Model.InternalElement), "testQuery");
                    method.AddParameter(GetTypeName(ownerDomainElement.InternalElement), "existingOwnerEntity");
                    method.AddParameter(GetTypeName(nestedDomainElement.InternalElement), "existingEntity");
                    method.AddStatements($@"
        // Arrange
        var repository = Substitute.For<{this.GetEntityRepositoryInterfaceName(ownerDomainElement)}>();
        repository.FindByIdAsync(testCommand.{nestedOwnerIdFieldName}, CancellationToken.None).Returns(Task.FromResult(owner));
        
        var sut = new {this.GetCommandHandlerName(Model)}(repository);

        // Act
        await sut.Handle(testCommand, CancellationToken.None);
        
        // Assert
        {this.GetAssertionClassName(ownerDomainElement)}.AssertEquivalent(testCommand, existingEntity);");
                });

                priClass.AddMethod("Task", $"Handle_WithInvalid{nestedOwnerIdFieldName}Command_ReturnsNotFound", method =>
                {
                    method.Async();
                    method.AddAttribute("Fact");
                    method.AddStatements($@"
        // Arrange
        var fixture = new Fixture();");
                    this.RegisterDomainEventBaseFixture(method);
                    method.AddStatements($@"
        var testCommand = fixture.Create<{GetTypeName(Model.InternalElement)}>();
        
        var repository = Substitute.For<{this.GetEntityRepositoryInterfaceName(ownerDomainElement)}>();
        repository.FindByIdAsync(testCommand.{nestedOwnerIdFieldName}, CancellationToken.None).Returns(Task.FromResult<{GetTypeName(ownerDomainElement.InternalElement)}>(null));
        
        var sut = new {this.GetCommandHandlerName(Model)}(repository);

        // Act
        var act = async () => await sut.Handle(testCommand, CancellationToken.None);
        
        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>();");
                });

                priClass.AddMethod("Task", "Handle_WithInvalidIdCommand_ReturnsNotFound", method =>
                {
                    method.Async();
                    method.AddAttribute("Fact");
                    method.AddStatements($@"
        // Arrange
        var fixture = new Fixture();");
                    this.RegisterDomainEventBaseFixture(method);
                    method.AddStatements($@"
        var testCommand = fixture.Create<{GetTypeName(Model.InternalElement)}>();
        var owner = fixture.Create<{GetTypeName(ownerDomainElement.InternalElement)}>();
        testCommand.{nestedOwnerIdFieldName} = owner.{ownerDomainElementIdName};
        
        var repository = Substitute.For<{this.GetEntityRepositoryInterfaceName(ownerDomainElement)}>();
        repository.FindByIdAsync(testCommand.{nestedOwnerIdFieldName}, CancellationToken.None).Returns(Task.FromResult(owner));
        
        var sut = new {this.GetCommandHandlerName(Model)}(repository);

        // Act
        var act = async () => await sut.Handle(testCommand, CancellationToken.None);
        
        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>();");
                });
            })
            .OnBuild(file =>
            {
                AddAssertionMethods();
            }, 6);
    }
    
    private void AddAssertionMethods()
    {
        if (Model?.Mapping?.Element?.IsClassModel() != true)
        {
            return;
        }

        var dtoModel = Model.TypeReference.Element.AsDTOModel();
        var nestedDomainElement = Model.Mapping.Element.AsClassModel();
        var ownerDomainElement = nestedDomainElement.GetNestedCompositionalOwner();
        var template = ExecutionContext.FindTemplateInstance<ICSharpFileBuilderTemplate>(
            TemplateDependency.OnModel(AssertionClassTemplate.TemplateId, ownerDomainElement));
        if (template == null)
        {
            return;
        }

        template.AddAssertionMethods(template.CSharpFile.Classes.First(), Model, nestedDomainElement, false);
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