using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Modelers.Domain.Api;
using Intent.Modelers.Services.CQRS.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;
using Intent.Modules.Constants;
using Intent.Modules.Entities.Repositories.Api.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.Application.MediatR.CRUD.Tests.Templates.CreateAggregateRootCommandHandlerTests
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class CreateAggregateRootCommandHandlerTestsTemplate : CSharpTemplateBase<CommandModel>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.Application.MediatR.CRUD.Tests.CreateAggregateRootCommandHandlerTests";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public CreateAggregateRootCommandHandlerTestsTemplate(IOutputTarget outputTarget, CommandModel model) : base(TemplateId, outputTarget, model)
        {
            AddNugetDependency(NugetPackages.AutoFixture);
            AddNugetDependency(NugetPackages.FluentAssertions);
            AddNugetDependency(NugetPackages.MicrosoftNetTestSdk);
            AddNugetDependency(NugetPackages.NSubstitute);
            AddNugetDependency(NugetPackages.Xunit);
            AddNugetDependency(NugetPackages.XunitRunnerVisualstudio);
            AddTypeSource(TemplateFulfillingRoles.Domain.Entity.Primary);
            
            CSharpFile = new CSharpFile($"{this.GetNamespace(additionalFolders: Model.GetConceptName())}", $"{this.GetFolderPath(additionalFolders: Model.GetConceptName())}")
                .AddClass($"{Model.Name}HandlerTests")
                .OnBuild(file =>
                {
                    var domainElement = Model.Mapping.Element.AsClassModel();
                    
                    var priClass = file.Classes.First();
                    priClass.AddMethod("Task", $"Handle_WithValidCommand_Adds{domainElement.Name.ToPascalCase()}ToRepository", method =>
                    {
                        method.AddParameter(GetTypeName(model), "testCommand");
                        method.AddStatements($@"
        // Arrange
        var expectedAggregateRoot = CreateExpectedAggregateRoot(testCommand);

        var repository = Substitute.For<{this.GetEntityRepositoryInterfaceName(domainElement)}>();
        {GetTypeName(domainElement.InternalElement)} addedAggregateRoot = null;
        repository.When(x => x.Add(Arg.Any<AggregateRoot>())).Do(ci => addedAggregateRoot = ci.Arg<AggregateRoot>());
        repository.UnitOfWork.When(async x => await x.SaveChangesAsync(CancellationToken.None)).Do(_ => addedAggregateRoot.Id = expectedAggregateRoot.Id);

        var sut = new CreateAggregateRootCommandHandler(repository);

        // Act
        var result = await sut.Handle(testCommand, CancellationToken.None);

        // Assert
        result.Should().Be(expectedAggregateRoot.Id);
        expectedAggregateRoot.Should().BeEquivalentTo(addedAggregateRoot);");
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
}