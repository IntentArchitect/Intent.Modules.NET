using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Modelers.Domain.Api;
using Intent.Modelers.Services.CQRS.Api;
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

namespace Intent.Modules.Application.MediatR.CRUD.Tests.Templates.DeleteCommandHandlerTests;

[IntentManaged(Mode.Fully, Body = Mode.Merge)]
public partial class DeleteCommandHandlerTestsTemplate : CSharpTemplateBase<CommandModel>, ICSharpFileBuilderTemplate
{
    public const string TemplateId = "Intent.Application.MediatR.CRUD.Tests.DeleteCommandHandlerTests";

    [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
    public DeleteCommandHandlerTestsTemplate(IOutputTarget outputTarget, CommandModel model) : base(TemplateId, outputTarget, model)
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
        AddTypeSource("Intent.DomainEvents.DomainEventBase");

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

                var priClass = file.Classes.First();
                priClass.AddMethod("Task", $"Handle_WithValidCommand_Deletes{domainElementName}FromRepository", method =>
                {
                    method.Async();
                    method.AddAttribute("Fact");
                    method.AddStatements($@"
        // Arrange
        var fixture = new Fixture();
        var testCommand = fixture.Create<{GetTypeName(Model.InternalElement)}>();

        var repository = Substitute.For<{this.GetEntityRepositoryInterfaceName(domainElement)}>();
        var existing{domainElementName} = GetExisting{domainElementName}(testCommand);
        repository.FindByIdAsync(testCommand.Id).Returns(Task.FromResult(existing{domainElementName}));

        var sut = new {this.GetCommandHandlerName(Model)}(repository);

        // Act
        await sut.Handle(testCommand, CancellationToken.None);

        // Assert
        repository.Received(1).Remove(Arg.Is<{GetTypeName(domainElement.InternalElement)}>(p => p.Id == testCommand.Id));");
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
        repository.FindByIdAsync(testCommand.Id, CancellationToken.None).Returns(Task.FromResult<{GetTypeName(domainElement.InternalElement)}>(default));
        repository.When(x => x.Remove(null)).Throw(new ArgumentNullException());

        var sut = new {this.GetCommandHandlerName(Model)}(repository);

        // Act
        // Assert
        await Assert.ThrowsAsync<ArgumentNullException>(async () =>
        {{
            await sut.Handle(testCommand, CancellationToken.None);
        }});");
                });

                priClass.AddMethod(GetTypeName(domainElement.InternalElement), $"GetExisting{domainElementName}", method =>
                {
                    method.Static();
                    method.Private();
                    method.AddParameter(GetTypeName(Model.InternalElement), "testCommand");
                    method.AddStatement($@"var fixture = new Fixture();");

                    if (TryGetTypeName("Intent.DomainEvents.DomainEventBase", out var domainEventBaseName))
                    {
                        method.AddStatement($"fixture.Register<{domainEventBaseName}>(() => null);");
                    }

                    method.AddStatements($@"
        fixture.Customize<{GetTypeName(domainElement.InternalElement)}>(comp => comp.With(x => x.Id, testCommand.Id));
        var existing{domainElementName} = fixture.Create<{GetTypeName(domainElement.InternalElement)}>();
        return existing{domainElementName};");
                });
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