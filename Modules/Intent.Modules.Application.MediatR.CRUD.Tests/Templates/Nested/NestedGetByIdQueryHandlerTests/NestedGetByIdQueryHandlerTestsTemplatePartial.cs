using System;
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

        AddTypeSource(TemplateFulfillingRoles.Application.Contracts.Dto);

        Facade = new QueryHandlerFacade(this, model);
        
        CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
            .AddClass($"{Model.Name}HandlerTests")
            .AfterBuild(file =>
            {
                AddUsingDirectives(file);
                Facade.AddHandlerConstructorMockUsings();

                var priClass = file.Classes.First();
                priClass.AddConstructor(ctor =>
                {
                    ctor.AddStatements(Facade.GetAutoMapperProfilesAndAddBackendField(priClass));
                });

                priClass.AddMethod("IEnumerable<object[]>", "GetSuccessfulResultTestData", method =>
                {
                    method.Static();

                    method.AddStatements(Facade.Get_InitialAutoFixture_TestDataStatements(
                        targetDomainModel: Facade.AggregateOwnerDomainModel, 
                        includeVarKeyword: true));
                    method.AddStatements(Facade.Get_ProduceEntityOwnerAndCompositeAndQuery_TestDataStatements());
                });

                priClass.AddMethod("Task", $"Handle_WithValidQuery_Retrieves{Facade.TargetDomainTypeName}", method =>
                {
                    method.Async();
                    method.AddAttribute("Theory");
                    method.AddAttribute("MemberData(nameof(GetSuccessfulResultTestData))");
                    method.AddParameter(Facade.QueryTypeName, "testQuery");
                    method.AddParameter(Facade.AggregateOwnerDomainTypeName, "existingOwnerEntity");
                    method.AddParameter(Facade.TargetDomainTypeName, "existingEntity");

                    method.AddStatement("// Arrange");
                    method.AddStatements(Facade.GetQueryHandlerConstructorParameterMockStatements());
                    method.AddStatements(Facade.GetDomainAggregateOwnerRepositoryFindByIdMockingStatements("testQuery", "existingOwnerEntity", QueryHandlerFacade.MockRepositoryResponse.ReturnDomainVariable));
                    method.AddStatements(Facade.GetQueryHandlerConstructorSutStatement());

                    method.AddStatement(string.Empty);
                    method.AddStatement("// Act");
                    method.AddStatements(Facade.GetSutHandleInvocationStatement("testQuery"));
                    
                    method.AddStatement(string.Empty);
                    method.AddStatement("// Assert");
                    method.AddStatements(Facade.Get_AggregateOwner_AssertionComparingHandlerResultsWithExpectedResults($"existingEntity"));
                });

                priClass.AddMethod("Task", "Handle_WithInvalidIdQuery_ThrowsNotFoundException", method =>
                {
                    method.Async();
                    method.AddAttribute("Fact");
                    
                    method.AddStatements("// Arrange");
                    method.AddStatements(Facade.GetNewAggregateOwnerWithoutCompositesStatements());
                    method.AddStatements(Facade.GetQueryHandlerConstructorParameterMockStatements());
                    method.AddStatements(Facade.GetDomainAggregateOwnerRepositoryFindByIdMockingStatements("testQuery", "existingOwnerEntity", QueryHandlerFacade.MockRepositoryResponse.ReturnDomainVariable));
                    method.AddStatement(string.Empty);
                    method.AddStatements(Facade.GetQueryHandlerConstructorSutStatement());
                    
                    method.AddStatement(string.Empty);
                    method.AddStatements("// Act");
                    method.AddStatements(Facade.GetSutHandleInvocationActLambdaStatement("testQuery"));
                    
                    method.AddStatement(string.Empty);
                    method.AddStatement("// Assert");
                    method.AddStatements(Facade.GetThrowsExceptionAssertionStatement(this.GetNotFoundExceptionName()));
                });
                
                AddAssertionMethods();
            });
    }
    
    private QueryHandlerFacade Facade { get; }

    private static void AddUsingDirectives(CSharpFile file)
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

        template.AddAssertionMethods(template.CSharpFile.Classes.First(), nestedDomainElement, dtoModel, false);
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