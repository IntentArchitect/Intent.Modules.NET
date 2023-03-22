using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Modelers.Domain.Api;
using Intent.Modelers.Services.Api;
using Intent.Modelers.Services.CQRS.Api;
using Intent.Modules.Application.MediatR.CRUD.CrudStrategies;
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

namespace Intent.Modules.Application.MediatR.CRUD.Tests.Templates.Nested.NestedUpdateCommandHandlerTests
{
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
                    priClass.AddMethod("Task", "Handle_WithValidCommand_UpdatesExistingEntity", method =>
                    {
                        method.Async();
                        method.AddAttribute("Theory");
                        method.AddAttribute("MemberData(nameof(GetValidTestData))");
                        method.AddParameter(GetTypeName(Model.InternalElement), "testCommand");
                        method.AddParameter(GetTypeName(ownerDomainElement.InternalElement), "owner");
                        method.AddStatements($@"
        // Arrange
        var expectedNestedEntity = CreateExpected{nestedDomainElementName}(testCommand);
        
        var repository = Substitute.For<{this.GetEntityRepositoryInterfaceName(ownerDomainElement)}>();
        repository.FindByIdAsync(testCommand.{nestedOwnerIdFieldName}, CancellationToken.None).Returns(Task.FromResult(owner));
        
        var sut = new {this.GetCommandHandlerName(Model)}(repository);

        // Act
        await sut.Handle(testCommand, CancellationToken.None);
        
        // Assert
        owner.{nestedAssociationName}.Should().Contain(p => p.{nestedDomainElementIdName} == testCommand.{commandIdFieldName}).Which.Should().BeEquivalentTo(expectedNestedEntity);");
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
        // Assert
        await Assert.ThrowsAsync<InvalidOperationException>(async () =>
        {{
            await sut.Handle(testCommand, CancellationToken.None);
        }});");
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
        // Assert
        await Assert.ThrowsAsync<InvalidOperationException>(async () =>
        {{
            await sut.Handle(testCommand, CancellationToken.None);
        }});");
                    });

                    priClass.AddMethod("IEnumerable<object[]>", "GetValidTestData", method =>
                    {
                        method.Static();
                        method.AddStatement($@"var fixture = new Fixture();");
                        this.RegisterDomainEventBaseFixture(method);
                        method.AddStatements($@"
        var testCommand = fixture.Create<{GetTypeName(Model.InternalElement)}>();
        var owner = fixture.Create<{GetTypeName(ownerDomainElement.InternalElement)}>();
        testCommand.{nestedOwnerIdFieldName} = owner.{ownerDomainElementIdName};
        owner.{nestedAssociationName}.Add(CreateExpected{nestedDomainElementName}(testCommand));
        yield return new object[] {{ testCommand, owner }};");

                        foreach (var property in Model.Properties
                                     .Where(p => p.TypeReference.IsNullable && p.Mapping?.Element?.AsAssociationEndModel()?.Element?.AsClassModel()?.IsAggregateRoot() == false))
                        {
                            method.AddStatement("");
                            method.AddStatement($@"fixture = new Fixture();");
                            this.RegisterDomainEventBaseFixture(method);
                            method.AddStatements($@"
        fixture.Customize<{GetTypeName(Model.InternalElement)}>(comp => comp.Without(x => x.{property.Name}));
        testCommand = fixture.Create<{GetTypeName(Model.InternalElement)}>();
        owner = fixture.Create<{GetTypeName(ownerDomainElement.InternalElement)}>();
        testCommand.{nestedOwnerIdFieldName} = owner.{ownerDomainElementIdName};
        owner.{nestedAssociationName}.Add(CreateExpected{nestedDomainElementName}(testCommand));
        yield return new object[] {{ testCommand, owner }};");
                        }
                    });

                    this.AddAssertionMethods(priClass, Model, nestedDomainElement, false);
                });
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
}