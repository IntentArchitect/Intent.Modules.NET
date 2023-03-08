using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Modelers.Domain.Api;
using Intent.Modelers.Services.CQRS.Api;
using Intent.Modules.Application.MediatR.CRUD.CrudStrategies;
using Intent.Modules.Application.MediatR.CRUD.Tests.Templates.Extensions.RepositoryExtensions;
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

                var nestedDomainElement = Model.Mapping.Element.AsClassModel();
                var nestedDomainElementName = nestedDomainElement.Name.ToPascalCase();
                var ownerDomainElement = nestedDomainElement.GetNestedCompositionalOwner();
                var ownerIdField = Model.Properties.GetNestedCompositionalOwnerIdField(ownerDomainElement);
                var ownerIdFieldName = ownerIdField.Name.ToPascalCase();

                var priClass = file.Classes.First();
                priClass.AddMethod("Task", $"Handle_WithValidCommand_Adds{nestedDomainElementName}ToRepository", method =>
                {
                    method.Async();
                    method.AddAttribute("Theory");
                    method.AddAttribute("MemberData(nameof(GetValidTestData))");
                    method.AddParameter(GetTypeName(ownerDomainElement.InternalElement), "owner");
                    method.AddParameter(GetTypeName(Model.InternalElement), "testCommand");
                    method.AddStatements($@"
        // Arrange
        var expected{nestedDomainElementName} = CreateExpected{nestedDomainElementName}(testCommand);

        {GetTypeName(nestedDomainElement.InternalElement)} added{nestedDomainElementName} = null;
        var repository = Substitute.For<{this.GetEntityRepositoryInterfaceName(ownerDomainElement)}>();
        repository.FindByIdAsync(testCommand.{ownerIdFieldName}, CancellationToken.None).Returns(Task.FromResult(owner));
        ");

                    var nestedEntityIdName = nestedDomainElement.GetEntityIdAttribute().IdName;
                    var associationPropertyName = ownerDomainElement.GetNestedCompositeAssociation(nestedDomainElement).Name.ToCSharpIdentifier(CapitalizationBehaviour.AsIs);
                    method.AddInvocationStatement("repository.OnSave", stmt => stmt
                        .AddArgument(new CSharpLambdaBlock("()")
                            .AddStatement($@"
        added{nestedDomainElementName} = owner.{associationPropertyName}.Single(p => p.{nestedEntityIdName} == default);
        added{nestedDomainElementName}.{nestedEntityIdName} = expected{nestedDomainElementName}.{nestedEntityIdName};"))
                        .WithArgumentsOnNewLines());

                    method.AddStatements($@"
        var sut = new {this.GetCommandHandlerName(Model)}(repository);

        // Act
        var result = await sut.Handle(testCommand, CancellationToken.None);

        // Assert
        result.Should().Be(expected{nestedDomainElementName}.Id);
        expected{nestedDomainElementName}.Should().BeEquivalentTo(added{nestedDomainElementName});");
                });

                priClass.AddMethod("IEnumerable<object[]>", "GetValidTestData", method =>
                {
                    var entityIdName = ownerDomainElement.GetEntityIdAttribute().IdName;

                    method.Static();
                    method.AddStatements($@"var fixture = new Fixture();");

                    this.RegisterDomainEventBaseFixture(method);

                    method.AddStatements($@"
        var owner = fixture.Create<{GetTypeName(ownerDomainElement.InternalElement)}>();
        var command = fixture.Create<{GetTypeName(Model.InternalElement)}>();
        command.{ownerIdFieldName} = owner.{entityIdName};
        yield return new object[] {{ owner, command }};");

                    foreach (var property in Model.Properties
                                 .Where(p => p.TypeReference.IsNullable && p.Mapping?.Element?.AsAssociationEndModel()?.Element?.AsClassModel()?.IsAggregateRoot() == false))
                    {
                        method.AddStatement("");
                        method.AddStatements($@"fixture = new Fixture();");

                        this.RegisterDomainEventBaseFixture(method);

                        method.AddStatement($@"fixture.Customize<{GetTypeName(Model.InternalElement)}>(comp => comp.Without(x => x.{property.Name}));");
                        method.AddStatements($@"
        owner = fixture.Create<{GetTypeName(ownerDomainElement.InternalElement)}>();
        command = fixture.Create<{GetTypeName(Model.InternalElement)}>();
        command.{ownerIdFieldName} = owner.{entityIdName};
        yield return new object[] {{ owner, command }};");
                    }
                });

                this.AddDtoToDomainMappingMethods(priClass, Model, nestedDomainElement);
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