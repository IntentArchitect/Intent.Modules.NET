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

namespace Intent.Modules.Application.MediatR.CRUD.Tests.Templates.Owner.CreateCommandHandlerTests;

[IntentManaged(Mode.Fully, Body = Mode.Merge)]
public partial class CreateCommandHandlerTestsTemplate : CSharpTemplateBase<CommandModel>, ICSharpFileBuilderTemplate
{
    public const string TemplateId = "Intent.Application.MediatR.CRUD.Tests.Owner.CreateCommandHandlerTests";

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
                var domainIdAttr = domainElement.GetEntityIdAttribute(ExecutionContext);

                var priClass = file.Classes.First();

                priClass.AddMethod("IEnumerable<object[]>", "GetSuccessfulResultTestData", method =>
                {
                    method.Static();
                    method.AddStatements($@"
        var fixture = new Fixture();
        yield return new object[] {{ fixture.Create<{GetTypeName(Model.InternalElement)}>() }};");

                    foreach (var property in Model.Properties
                                 .Where(p => p.TypeReference.IsNullable && p.Mapping?.Element?.AsAssociationEndModel()?.Element?.AsClassModel()?.IsAggregateRoot() == false))
                    {
                        method.AddStatement("");
                        method.AddStatements($@"
        fixture = new Fixture();
        fixture.Customize<{GetTypeName(Model.InternalElement)}>(comp => comp.Without(x => x.{property.Name}));
        yield return new object[] {{ fixture.Create<{GetTypeName(Model.InternalElement)}>() }};");
                    }
                });

                priClass.AddMethod("Task", $"Handle_WithValidCommand_Adds{domainElementName}ToRepository", method =>
                {
                    var hasIdReturnType = model.TypeReference.Element != null;
                    
                    method.Async();
                    method.AddAttribute("Theory");
                    method.AddAttribute("MemberData(nameof(GetSuccessfulResultTestData))");
                    method.AddParameter(GetTypeName(Model.InternalElement), "testCommand");
                    method.AddStatements($@"
        // Arrange");
                    if (hasIdReturnType)
                    {
                        method.AddStatement($@"var expected{domainElementName}Id = new Fixture().Create<{domainIdAttr.Type}>();");
                    }

                    method.AddStatements($@"{GetTypeName(domainElement.InternalElement)} added{domainElementName} = null;
        var repository = Substitute.For<{this.GetEntityRepositoryInterfaceName(domainElement)}>();
        repository.OnAdd(ent => added{domainElementName} = ent);");
                    if (hasIdReturnType)
                    {
                        method.AddStatement(new CSharpMethodChainStatement("repository.UnitOfWork") { BeforeSeparator = CSharpCodeSeparatorType.NewLine }
                            .WithoutSemicolon()
                            .AddChainStatement(new CSharpInvocationStatement("When")
                                .WithoutSemicolon()
                                .AddArgument("async x => await x.SaveChangesAsync(CancellationToken.None)")
                            )
                            .AddChainStatement(new CSharpInvocationStatement("Do")
                                .AddArgument($@"_ => added{domainElementName}.{domainIdAttr.IdName} = expected{domainElementName}Id")
                            )
                        );
                    }

                    method.AddStatements($@"var sut = new {this.GetCommandHandlerName(Model)}(repository);

        // Act
        var result = await sut.Handle(testCommand, CancellationToken.None);

        // Assert");
                    if (hasIdReturnType)
                    {
                        method.AddStatements($@"result.Should().Be(expected{domainElementName}Id);
        await repository.UnitOfWork.Received(1).SaveChangesAsync();");
                    }

                    method.AddStatement($@"{this.GetAssertionClassName(domainElement)}.AssertEquivalent(testCommand, added{domainElementName});");
                });
            })
            .OnBuild(file =>
            {
                AddAssertionMethods();
            }, 1);
    }

    private void AddAssertionMethods()
    {
        if (Model?.Mapping?.Element?.IsClassModel() != true)
        {
            return;
        }

        var domainModel = Model.Mapping.Element.AsClassModel();
        var template = ExecutionContext.FindTemplateInstance<ICSharpFileBuilderTemplate>(
            TemplateDependency.OnModel(AssertionClassTemplate.TemplateId, domainModel));
        if (template == null)
        {
            return;
        }

        template.AddAssertionMethods(template.CSharpFile.Classes.First(), Model, domainModel);
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