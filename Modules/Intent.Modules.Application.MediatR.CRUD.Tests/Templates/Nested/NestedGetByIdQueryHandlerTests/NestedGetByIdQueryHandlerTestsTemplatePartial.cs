using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Modelers.Domain.Api;
using Intent.Modelers.Services.Api;
using Intent.Modelers.Services.CQRS.Api;
using Intent.Modules.Application.MediatR.CRUD.CrudStrategies;
using Intent.Modules.Application.MediatR.CRUD.Tests.Templates.Assertions.AssertionClass;
using Intent.Modules.Application.MediatR.Templates;
using Intent.Modules.Application.MediatR.Templates.QueryModels;
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

namespace Intent.Modules.Application.MediatR.CRUD.Tests.Templates.Nested.NestedGetByIdQueryHandlerTests;

[IntentManaged(Mode.Fully, Body = Mode.Merge)]
public partial class NestedGetByIdQueryHandlerTestsTemplate : CSharpTemplateBase<QueryModel>, ICSharpFileBuilderTemplate
{
    public const string TemplateId = "Intent.Application.MediatR.CRUD.Tests.Nested.NestedGetByIdQueryHandlerTests";

    [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
    public NestedGetByIdQueryHandlerTestsTemplate(IOutputTarget outputTarget, QueryModel model) : base(TemplateId, outputTarget, model)
    {
        AddNugetDependency(NugetPackages.AutoFixture);
        AddNugetDependency(NugetPackages.FluentAssertions);
        AddNugetDependency(NugetPackages.MicrosoftNetTestSdk);
        AddNugetDependency(NugetPackages.NSubstitute);
        AddNugetDependency(NugetPackages.Xunit);
        AddNugetDependency(NugetPackages.XunitRunnerVisualstudio);

        AddTypeSource(TemplateFulfillingRoles.Domain.Entity.Primary);
        AddTypeSource(QueryModelsTemplate.TemplateId);
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
                file.AddUsing("AutoMapper");

                var dtoModel = Model.TypeReference.Element.AsDTOModel();
                var nestedDomainElement = Model.Mapping.Element.AsClassModel();
                var nestedDomainElementName = nestedDomainElement.Name.ToPascalCase();
                var nestedDomainElementIdName = nestedDomainElement.GetEntityIdAttribute(ExecutionContext).IdName;
                var ownerDomainElement = nestedDomainElement.GetNestedCompositionalOwner();
                var ownerDomainElementIdName = ownerDomainElement.GetEntityIdAttribute(ExecutionContext).IdName;
                var nestedOwnerIdField = Model.Properties.GetNestedCompositionalOwnerIdField(ownerDomainElement);
                var nestedOwnerIdFieldName = nestedOwnerIdField.Name;
                var queryIdFieldName = Model.Properties.GetEntityIdField(nestedDomainElement).Name;
                var nestedAssociationName = ownerDomainElement.GetNestedCompositeAssociation(nestedDomainElement).Name.ToCSharpIdentifier();

                var priClass = file.Classes.First();
                priClass.AddField("IMapper", "_mapper", prop => prop.PrivateReadOnly());
                priClass.AddConstructor(ctor =>
                {
                    ctor.AddStatement(new CSharpInvocationStatement("var mapperConfiguration = new MapperConfiguration")
                        .AddArgument(new CSharpLambdaBlock("config")
                            .AddStatement($"config.AddMaps(typeof({this.GetQueryHandlerName(Model)}));"))
                        .WithArgumentsOnNewLines());
                    ctor.AddStatement("_mapper = mapperConfiguration.CreateMapper();");
                });

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
            .With(x => x.{queryIdFieldName}, expectedEntity.Id)
            .With(x => x.{nestedOwnerIdFieldName}, existingOwnerEntity.{ownerDomainElementIdName}));
        var testCommand = fixture.Create<{GetTypeName(Model.InternalElement)}>();
        yield return new object[] {{ testCommand, existingOwnerEntity, expectedEntity }};");
                });

                priClass.AddMethod("Task", $"Handle_WithValidQuery_Retrieves{nestedDomainElementName}", method =>
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
        repository.FindByIdAsync(testQuery.{nestedOwnerIdFieldName}, CancellationToken.None).Returns(Task.FromResult(existingOwnerEntity));

        var sut = new {this.GetQueryHandlerName(Model)}(repository, _mapper);

        // Act
        var result = await sut.Handle(testQuery, CancellationToken.None);

        // Assert
        {this.GetAssertionClassName(ownerDomainElement)}.AssertEquivalent(existingEntity, result);");
                });

                priClass.AddMethod("Task", "Handle_WithInvalidIdQuery_ReturnsEmptyResult", method =>
                {
                    method.Async();
                    method.AddAttribute("Fact");
                    method.AddStatements($@"
        // Arrange
        var fixture = new Fixture();");
                    this.RegisterDomainEventBaseFixture(method);
                    method.AddStatements($@"
        fixture.Customize<{GetTypeName(ownerDomainElement.InternalElement)}>(comp => comp.With(p => p.{nestedAssociationName}, new List<{GetTypeName(nestedDomainElement.InternalElement)}>()));
        var existingOwnerEntity = fixture.Create<{GetTypeName(ownerDomainElement.InternalElement)}>();
        var testQuery = fixture.Create<{GetTypeName(Model.InternalElement)}>();
        
        var repository = Substitute.For<{this.GetEntityRepositoryInterfaceName(ownerDomainElement)}>();
        repository.FindByIdAsync(testQuery.{nestedOwnerIdFieldName}, CancellationToken.None).Returns(Task.FromResult(existingOwnerEntity));

        var sut = new {this.GetQueryHandlerName(Model)}(repository, _mapper);
        
        // Act
        var result = await sut.Handle(testQuery, CancellationToken.None);
        
        // Assert
        result.Should().Be(null);");
                });
            })
            .OnBuild(file =>
            {
                AddAssertionMethods();
            }, 5);
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

        template.AddAssertionMethods(template.CSharpFile.Classes.First(), nestedDomainElement, dtoModel);
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