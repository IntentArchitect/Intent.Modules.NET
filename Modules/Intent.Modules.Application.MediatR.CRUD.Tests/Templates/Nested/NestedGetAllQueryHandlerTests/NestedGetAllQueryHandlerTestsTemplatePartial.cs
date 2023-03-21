using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Modelers.Domain.Api;
using Intent.Modelers.Services.Api;
using Intent.Modelers.Services.CQRS.Api;
using Intent.Modules.Application.MediatR.CRUD.CrudStrategies;
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

namespace Intent.Modules.Application.MediatR.CRUD.Tests.Templates.Nested.NestedGetAllQueryHandlerTests
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class NestedGetAllQueryHandlerTestsTemplate : CSharpTemplateBase<QueryModel>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.Application.MediatR.CRUD.Tests.Nested.NestedGetAllQueryHandlerTests";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public NestedGetAllQueryHandlerTestsTemplate(IOutputTarget outputTarget, QueryModel model) : base(TemplateId, outputTarget, model)
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
                    var nestedDomainElement = dtoModel.Mapping.Element.AsClassModel();
                    var nestedDomainElementName = nestedDomainElement.Name.ToPascalCase();
                    var nestedDomainElementPluralName = nestedDomainElementName.Pluralize();
                    var ownerDomainElement = nestedDomainElement.GetNestedCompositionalOwner();
                    var ownerDomainElementIdName = ownerDomainElement.GetEntityIdAttribute().IdName;
                    var nestedOwnerIdField = Model.Properties.GetNestedCompositionalOwnerIdField(ownerDomainElement);
                    var nestedOwnerIdFieldName = nestedOwnerIdField.Name;
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

                    priClass.AddMethod("Task", $"Handle_WithValidQuery_Retrieves{nestedDomainElementPluralName}", method =>
                    {
                        method.Async();
                        method.AddAttribute("Theory");
                        method.AddAttribute("MemberData(nameof(GetTestData))");
                        method.AddParameter(GetTypeName(ownerDomainElement.InternalElement), "owner");
                        method.AddStatements($@"
        // Arrange
        var testQuery = new {GetTypeName(Model.InternalElement)}();
        testQuery.{nestedOwnerIdFieldName} = owner.{ownerDomainElementIdName};
        var repository = Substitute.For<{this.GetEntityRepositoryInterfaceName(ownerDomainElement)}>();
        repository.FindByIdAsync(testQuery.{nestedOwnerIdFieldName}, CancellationToken.None).Returns(Task.FromResult(owner));

        var sut = new {this.GetQueryHandlerName(Model)}(repository, _mapper);

        // Act
        var result = await sut.Handle(testQuery, CancellationToken.None);

        // Assert
        result.Should().BeEquivalentTo(owner.{nestedAssociationName}.Select(CreateExpected{dtoModel.InternalElement.Name.ToPascalCase()}));");
                    });

                    priClass.AddMethod("IEnumerable<object[]>", "GetTestData", method =>
                    {
                        method.Static();
                        method.AddStatements($@"var fixture = new Fixture();");
                        this.RegisterDomainEventBaseFixture(method);
                        method.AddStatement($"yield return new object[] {{ fixture.Create<{GetTypeName(ownerDomainElement.InternalElement)}>() }};");
                        method.AddStatement("");
                        method.AddStatement("fixture = new Fixture();");
                        this.RegisterDomainEventBaseFixture(method);
                        method.AddStatement($"fixture.Customize<{GetTypeName(ownerDomainElement.InternalElement)}>(comp => comp.With(p => p.{nestedAssociationName}, new List<{GetTypeName(nestedDomainElement.InternalElement)}>()));");
                        method.AddStatement($"yield return new object[] {{ fixture.Create<{GetTypeName(ownerDomainElement.InternalElement)}>() }};");
                    });

                    this.AddDomainToDtoMappingMethods(priClass, nestedDomainElement, dtoModel);
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