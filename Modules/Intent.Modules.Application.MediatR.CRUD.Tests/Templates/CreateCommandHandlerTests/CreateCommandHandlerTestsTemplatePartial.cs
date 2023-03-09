using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Modelers.Domain.Api;
using Intent.Modelers.Services.CQRS.Api;
using Intent.Modules.Application.MediatR.CRUD.Tests.Templates.RepositoryExtensions;
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

namespace Intent.Modules.Application.MediatR.CRUD.Tests.Templates.CreateCommandHandlerTests;

[IntentManaged(Mode.Fully, Body = Mode.Merge)]
public partial class CreateCommandHandlerTestsTemplate : CSharpTemplateBase<CommandModel>, ICSharpFileBuilderTemplate
{
    public const string TemplateId = "Intent.Application.MediatR.CRUD.Tests.CreateCommandHandlerTests";

    [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
    public CreateCommandHandlerTestsTemplate(IOutputTarget outputTarget, CommandModel model) : base(TemplateId, outputTarget, model)
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

        CSharpFile = new CSharpFile($"{this.GetNamespace()}", $"{this.GetFolderPath()}")
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

                var repoExtTemplate = ExecutionContext.FindTemplateInstance<IClassProvider>(TemplateDependency.OnTemplate(RepositoryExtensionsTemplate.TemplateId));
                if (repoExtTemplate != null)
                {
                    file.AddUsing(repoExtTemplate.Namespace);
                }

                var domainElement = Model.Mapping.Element.AsClassModel();
                var domainElementName = domainElement.Name.ToPascalCase();

                var priClass = file.Classes.First();
                priClass.AddMethod("Task", $"Handle_WithValidCommand_Adds{domainElementName}ToRepository", method =>
                {
                    method.Async();
                    method.AddAttribute("Theory");
                    method.AddAttribute("MemberData(nameof(GetTestData))");
                    method.AddParameter(GetTypeName(Model.InternalElement), "testCommand");
                    method.AddStatements($@"
        // Arrange
        var expected{domainElementName} = CreateExpected{domainElementName}(testCommand);

        {GetTypeName(domainElement.InternalElement)} added{domainElementName} = null;
        var repository = Substitute.For<{this.GetEntityRepositoryInterfaceName(domainElement)}>();
        repository.OnAdd(ent => added{domainElementName} = ent);
        repository.OnSave(() => added{domainElementName}.Id = expected{domainElementName}.Id);

        var sut = new {this.GetCommandHandlerName(Model)}(repository);

        // Act
        var result = await sut.Handle(testCommand, CancellationToken.None);

        // Assert
        result.Should().Be(expected{domainElementName}.Id);
        expected{domainElementName}.Should().BeEquivalentTo(added{domainElementName});");
                });

                priClass.AddMethod("IEnumerable<object[]>", "GetTestData", method =>
                {
                    method.Static();
                    method.AddStatements($@"
        var fixture = new Fixture();
        yield return new object[] {{ fixture.Create<{GetTypeName(Model.InternalElement)}>() }};");
                    method.AddStatement("");

                    foreach (var property in Model.Properties
                                 .Where(p => p.Mapping?.Element?.AsAssociationEndModel()?.Element?.AsClassModel()?.IsAggregateRoot() == false))
                    {
                        method.AddStatements($@"
        fixture = new Fixture();
        fixture.Customize<{GetTypeName(Model.InternalElement)}>(comp => comp.Without(x => x.{property.Name}));
        yield return new object[] {{ fixture.Create<{GetTypeName(Model.InternalElement)}>() }};");
                        method.AddStatement("");
                    }
                });

                this.AddDtoToDomainMappingMethods(priClass, Model, domainElement);
            });
    }

    [IntentManaged(Mode.Fully)] public CSharpFile CSharpFile { get; }

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