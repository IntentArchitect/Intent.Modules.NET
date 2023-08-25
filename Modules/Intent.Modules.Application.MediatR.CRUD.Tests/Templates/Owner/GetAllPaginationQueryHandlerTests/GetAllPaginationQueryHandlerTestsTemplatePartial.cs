using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Modelers.Domain.Api;
using Intent.Modelers.Services.Api;
using Intent.Modelers.Services.CQRS.Api;
using Intent.Modules.Application.MediatR.CRUD.Tests.Templates.Assertions.AssertionClass;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.Constants;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.Application.MediatR.CRUD.Tests.Templates.Owner.GetAllPaginationQueryHandlerTests
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class GetAllPaginationQueryHandlerTestsTemplate : CSharpTemplateBase<QueryModel>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.Application.MediatR.CRUD.Tests.Owner.GetAllPaginationQueryHandlerTests";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public GetAllPaginationQueryHandlerTestsTemplate(IOutputTarget outputTarget, QueryModel model) : base(TemplateId, outputTarget, model)
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
                            includeVarKeyword: true));
                        method.AddStatements(Facade.Get_ManyAggregateDomainEntities_TestDataStatements(5));
                        method.AddStatements(Facade.Get_ManyAggregateDomainEntities_TestDataStatements(0));
                    });
                    
                    priClass.AddMethod("Task", $"Handle_WithValidQuery_Retrieves{Facade.PluralTargetDomainName}", method =>
                    {
                        method.Async();
                        method.AddAttribute("Theory");
                        method.AddAttribute("MemberData(nameof(GetSuccessfulResultTestData))");
                        method.AddParameter($"List<{Facade.TargetDomainTypeName}>", "testEntities");

                        method.AddStatement("// Arrange");
                        method.AddStatements(Facade.GetNewQueryAutoFixtureInlineStatements("testQuery"));
                        method.AddStatements(Facade.GetPaginationQuerySetup("testQuery", pageNo: 1, pageSize: 5));
                        method.AddStatements(Facade.GetQueryHandlerConstructorParameterMockStatements());
                        method.AddStatements(Facade.GetMockedPaginatedResults("fetchedResults", "testEntities"));
                        method.AddStatements(Facade.GetDomainRepositoryFindAllMockingStatements(
                            entitiesVarName: "fetchedResults", 
                            repositoryVarName: Facade.DomainAggregateRepositoryVarName, 
                            pageNo: 1, 
                            pageSize: 5));
                        method.AddStatements(Facade.GetQueryHandlerConstructorSutStatement());

                        method.AddStatement(string.Empty);
                        method.AddStatement("// Act");
                        method.AddStatements(Facade.GetSutHandleInvocationStatement("testQuery"));

                        method.AddStatement(string.Empty);
                        method.AddStatement("// Assert");
                        method.AddStatements(Facade.Get_Aggregate_AssertionComparingHandlerResultsWithExpectedResults("fetchedResults"));
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
            if (!Model.IsPaginationQueryMapping())
            {
                return;
            }
            
            var domainModel = Model.GetPaginatedClassModel();
            var templateInstance = ExecutionContext.FindTemplateInstance<ICSharpFileBuilderTemplate>(
                TemplateDependency.OnModel(AssertionClassTemplate.TemplateId, domainModel));

            templateInstance?.AddPaginationAssertionMethod(templateInstance.CSharpFile.Classes.First(), Model, domainModel);
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
}