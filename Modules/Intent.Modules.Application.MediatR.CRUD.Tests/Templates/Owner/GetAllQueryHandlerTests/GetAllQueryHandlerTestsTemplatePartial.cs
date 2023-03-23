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

namespace Intent.Modules.Application.MediatR.CRUD.Tests.Templates.Owner.GetAllQueryHandlerTests;

[IntentManaged(Mode.Fully, Body = Mode.Merge)]
public partial class GetAllQueryHandlerTestsTemplate : CSharpTemplateBase<QueryModel>, ICSharpFileBuilderTemplate
{
    public const string TemplateId = "Intent.Application.MediatR.CRUD.Tests.Owner.GetAllQueryHandlerTests";

    [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
    public GetAllQueryHandlerTestsTemplate(IOutputTarget outputTarget, QueryModel model) : base(TemplateId, outputTarget, model)
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
                var domainElement = dtoModel.Mapping.Element.AsClassModel();
                var domainElementName = domainElement.Name.ToPascalCase();
                var domainElementPluralName = domainElementName.Pluralize();

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
                    method.AddStatements($@"var fixture = new Fixture();");

                    this.RegisterDomainEventBaseFixture(method, domainElement);

                    method.AddStatement($@"yield return new object[] {{ fixture.CreateMany<{GetTypeName(domainElement.InternalElement)}>().ToList() }};");

                    method.AddStatement($@"yield return new object[] {{ fixture.CreateMany<{GetTypeName(domainElement.InternalElement)}>(0).ToList() }};");
                });

                priClass.AddMethod("Task", $"Handle_WithValidQuery_Retrieves{domainElementPluralName}", method =>
                {
                    method.Async();
                    method.AddAttribute("Theory");
                    method.AddAttribute("MemberData(nameof(GetSuccessfulResultTestData))");
                    method.AddParameter($"List<{GetTypeName(domainElement.InternalElement)}>", "testEntities");
                    method.AddStatements($@"
        // Arrange
        var testQuery = new {GetTypeName(Model.InternalElement)}();
        var repository = Substitute.For<{this.GetEntityRepositoryInterfaceName(domainElement)}>();
        repository.FindAllAsync(CancellationToken.None).Returns(Task.FromResult(testEntities));

        var sut = new {this.GetQueryHandlerName(Model)}(repository, _mapper);

        // Act
        var result = await sut.Handle(testQuery, CancellationToken.None);

        // Assert
        {this.GetAssertionClassName(domainElement)}.AssertEquivalent(result, testEntities);");
                });
            })
            .OnBuild(file =>
            {
                AddAssertionMethods();
            }, 3);
    }

    private void AddAssertionMethods()
    {
        var dtoModel = Model.TypeReference.Element.AsDTOModel();

        if (dtoModel?.Mapping?.Element?.IsClassModel() != true)
        {
            return;
        }

        var domainElement = dtoModel.Mapping.Element.AsClassModel();
        var template = ExecutionContext.FindTemplateInstance<ICSharpFileBuilderTemplate>(
            TemplateDependency.OnModel(AssertionClassTemplate.TemplateId, domainElement));
        if (template == null)
        {
            return;
        }

        template.AddAssertionMethods(template.CSharpFile.Classes.First(), domainElement, dtoModel, true);
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