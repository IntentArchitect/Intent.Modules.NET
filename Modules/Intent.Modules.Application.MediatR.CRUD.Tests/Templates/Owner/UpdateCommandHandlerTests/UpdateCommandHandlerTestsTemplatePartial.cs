using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Modelers.Domain.Api;
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

                var domainElement = Model.Mapping.Element.AsClassModel();
                var domainElementName = domainElement.Name.ToPascalCase();
                var idFieldName = Model.Properties.GetEntityIdField(domainElement).Name.ToCSharpIdentifier();

                var priClass = file.Classes.First();
                priClass.AddMethod("Task", "Handle_WithValidCommand_UpdatesExistingEntity", method =>
                {
                    method.Async();
                    method.AddAttribute("Theory");
                    method.AddAttribute("MemberData(nameof(GetValidTestData))");
                    method.AddParameter(GetTypeName(Model.InternalElement), "testCommand");
                    method.AddParameter(GetTypeName(domainElement.InternalElement), "existingEntity");
                    method.AddStatements($@"
        // Arrange
        var expected{domainElementName} = CreateExpected{domainElementName}(testCommand);
        
        var repository = Substitute.For<{this.GetEntityRepositoryInterfaceName(domainElement)}>();
        repository.FindByIdAsync(testCommand.{idFieldName}, CancellationToken.None).Returns(Task.FromResult(existingEntity));
        
        var sut = new {this.GetCommandHandlerName(Model)}(repository);

        // Act
        await sut.Handle(testCommand, CancellationToken.None);
        
        // Assert
        expected{domainElementName}.Should().BeEquivalentTo(existingEntity);");
                });

                priClass.AddMethod("Task", "Handle_WithInvalidIdCommand_ReturnsNotFound", method =>
                {
                    method.Async();
                    method.AddAttribute("Fact");
                    method.AddStatements($@"
        // Arrange
        var fixture = new Fixture();
        var testCommand = fixture.Create<{GetTypeName(Model.InternalElement)}>();
        
        var repository = Substitute.For<{this.GetEntityRepositoryInterfaceName(domainElement)}>();
        repository.FindByIdAsync(testCommand.{idFieldName}, CancellationToken.None).Returns(Task.FromResult<{GetTypeName(domainElement.InternalElement)}>(null));
        
        var sut = new {this.GetCommandHandlerName(Model)}(repository);

        // Act
        // Assert
        await Assert.ThrowsAsync<NullReferenceException>(async () =>
        {{
            await sut.Handle(testCommand, CancellationToken.None);
        }});");
                });

                priClass.AddMethod("IEnumerable<object[]>", "GetValidTestData", method =>
                {
                    method.Static();
                    method.AddStatements($@"
        var fixture = new Fixture();
        var testCommand = fixture.Create<{GetTypeName(Model.InternalElement)}>();
        yield return new object[] {{ testCommand, CreateExpected{domainElementName}(testCommand) }};");

                    foreach (var property in Model.Properties
                                 .Where(p => p.TypeReference.IsNullable && p.Mapping?.Element?.AsAssociationEndModel()?.Element?.AsClassModel()?.IsAggregateRoot() == false))
                    {
                        method.AddStatement("");
                        method.AddStatements($@"
        fixture = new Fixture();
        fixture.Customize<{GetTypeName(Model.InternalElement)}>(comp => comp.Without(x => x.{property.Name}));
        testCommand = fixture.Create<{GetTypeName(Model.InternalElement)}>();
        yield return new object[] {{ testCommand, CreateExpected{domainElementName}(testCommand) }};");
                    }
                });

                this.AddDtoToDomainMappingMethods(priClass, Model, domainElement);
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