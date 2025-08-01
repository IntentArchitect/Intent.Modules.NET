using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Xml.Serialization;
using Intent.Engine;
using Intent.IArchitect.Agent.Persistence.Model.ApplicationTemplate;
using Intent.Metadata.Models;
using Intent.Modelers.Domain.Api;
using Intent.Modelers.Services.Api;
using Intent.Modules.AspNetCore.IntegrationTesting;
using Intent.Modules.AspNetCore.IntegrationTesting.Templates;
using Intent.Modules.AspNetCore.IntegrationTests.CRUD.FactoryExtensions.TestImplementations;
using Intent.Modules.AspNetCore.IntegrationTests.CRUD.Templates;
using Intent.Modules.AspNetCore.IntegrationTests.CRUD.Templates.TestDataFactory;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Mapping;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Plugins;
using Intent.Modules.Common.Templates;
using Intent.Modules.Integration.HttpClients.Shared.Templates;
using Intent.Modules.Metadata.WebApi.Models;
using Intent.Plugins.FactoryExtensions;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;
using static Intent.Modules.AspNetCore.IntegrationTests.CRUD.FactoryExtensions.EndpointTestImplementationFactoryExtension;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.FactoryExtension", Version = "1.0")]

namespace Intent.Modules.AspNetCore.IntegrationTests.CRUD.FactoryExtensions
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class EndpointTestImplementationFactoryExtension : FactoryExtensionBase
    {

        public override string Id => "Intent.AspNetCore.IntegrationTests.CRUD.EndpointTestImplementationFactoryExtension";
        private readonly IMetadataManager _metadataManager;

        public EndpointTestImplementationFactoryExtension(IMetadataManager metadataManager)
        {
            _metadataManager = metadataManager;
        }

        [IntentManaged(Mode.Ignore)]
        public override int Order => 0;

        protected override void OnAfterTemplateRegistrations(IApplication application)
        {
            var testDataTemplate = application.FindTemplateInstance<ICSharpFileBuilderTemplate>(TestDataFactoryTemplate.TemplateId);
            var crudMaps = CrudMapHelper.LoadCrudMaps(testDataTemplate, _metadataManager, application, out var cantImplementCrudMaps);

            PopulateTestDataFactory(testDataTemplate, crudMaps);
            GenerateCRUDTests(application, crudMaps);
            GenerateCRUDStubTests(application, cantImplementCrudMaps);
        }

        private void GenerateCRUDStubTests(IApplication application, List<CrudMap> cantImplementCrudMaps)
        {
            foreach (var crudTest in cantImplementCrudMaps)
            {
                var template = application.FindTemplateInstance<ICSharpFileBuilderTemplate>("Intent.AspNetCore.IntegrationTesting.ServiceEndpointTest", crudTest.Create.Id);
                template.AddNugetDependency(NugetPackages.AutoFixture(template.OutputTarget));
                DoNotImplementedTest(template, crudTest, crudTest.Create);
                template = application.FindTemplateInstance<ICSharpFileBuilderTemplate>("Intent.AspNetCore.IntegrationTesting.ServiceEndpointTest", crudTest.GetById.Id);
                DoNotImplementedTest(template, crudTest, crudTest.GetById);
                if (crudTest.Update != null)
                {
                    template = application.FindTemplateInstance<ICSharpFileBuilderTemplate>("Intent.AspNetCore.IntegrationTesting.ServiceEndpointTest", crudTest.Update.Id);
                    DoNotImplementedTest(template, crudTest, crudTest.Update);
                }
                if (crudTest.Delete != null)
                {
                    template = application.FindTemplateInstance<ICSharpFileBuilderTemplate>("Intent.AspNetCore.IntegrationTesting.ServiceEndpointTest", crudTest.Delete.Id);
                    DoNotImplementedTest(template, crudTest, crudTest.Delete);
                }
                if (crudTest.GetAll != null)
                {
                    template = application.FindTemplateInstance<ICSharpFileBuilderTemplate>("Intent.AspNetCore.IntegrationTesting.ServiceEndpointTest", crudTest.GetAll.Id);
                    DoNotImplementedTest(template, crudTest, crudTest.GetAll);
                }
                if (crudTest.DomainInvocations.Any())
                {
                    foreach (var domainInvocation in crudTest.DomainInvocations)
                    {
                        template = application.FindTemplateInstance<ICSharpFileBuilderTemplate>("Intent.AspNetCore.IntegrationTesting.ServiceEndpointTest", domainInvocation.Id);
                        DoNotImplementedTest(template, crudTest, domainInvocation);
                    }
                }
            }
        }

        private void DoNotImplementedTest(ICSharpFileBuilderTemplate template, CrudMap crudTest, IHttpEndpointModel operation)
        {
            template.CSharpFile.OnBuild(file =>
            {
                template.AddUsing("System.Net");
                template.AddUsing("AutoFixture");
                template.GetHttpClientRequestExceptionName();

                var @class = template.CSharpFile.Classes.First();

                @class.AddMethod("Task", $"{operation.Name}_Should{operation.Name}", method =>
                {

                    method
                        .Async()
                        .AddAttribute("Fact")
                        ;
                    AddRequirementTraits(crudTest, method, template);
                    method
                        .AddStatement("// Arrange", s => s.SeparatedFromPrevious())
                        .AddStatement($"var client = new {template.GetTypeName("Intent.AspNetCore.IntegrationTesting.HttpClient", crudTest.Proxy.Id)}(CreateClient());")
                        .AddStatement("// Act", s => s.SeparatedFromPrevious())
                        .AddStatement($"// Unable to generate test: Can't determine how to mock data for ({string.Join(",", crudTest.Dependencies.Where(d => d.CrudMap is null).Select(d => d.EntityName))})", s => s.SeparatedFromPrevious())
                        .AddStatement("// IntentInitialGen")
                        .AddStatement($"// TODO: Implement {method.Name} ({@class.Name}) functionality")
                        .AddStatement("""throw new NotImplementedException("Your implementation here...");""")
                        ;
                });
            });
        }


        private void GenerateCRUDTests(IApplication application, List<CrudMap> crudMaps)
        {
            foreach (var crudTest in crudMaps)
            {
                var template = application.FindTemplateInstance<ICSharpFileBuilderTemplate>("Intent.AspNetCore.IntegrationTesting.ServiceEndpointTest", crudTest.Create.Id);
                template.AddNugetDependency(NugetPackages.AutoFixture(template.OutputTarget));
                DoCreateTest(template, crudTest);
                template = application.FindTemplateInstance<ICSharpFileBuilderTemplate>("Intent.AspNetCore.IntegrationTesting.ServiceEndpointTest", crudTest.GetById.Id);
                DoGetByIdTest(template, crudTest);
                if (crudTest.Update != null)
                {
                    template = application.FindTemplateInstance<ICSharpFileBuilderTemplate>("Intent.AspNetCore.IntegrationTesting.ServiceEndpointTest", crudTest.Update.Id);
                    DoUpdateTest(template, crudTest);
                }
                if (crudTest.Delete != null)
                {
                    template = application.FindTemplateInstance<ICSharpFileBuilderTemplate>("Intent.AspNetCore.IntegrationTesting.ServiceEndpointTest", crudTest.Delete.Id);
                    DoDeleteTest(template, crudTest);
                }
                if (crudTest.GetAll != null)
                {
                    template = application.FindTemplateInstance<ICSharpFileBuilderTemplate>("Intent.AspNetCore.IntegrationTesting.ServiceEndpointTest", crudTest.GetAll.Id);
                    DoGetAllTest(template, crudTest);
                }
                if (crudTest.DomainInvocations.Any())
                {
                    foreach (var domainInvoation in crudTest.DomainInvocations)
                    {
                        template = application.FindTemplateInstance<ICSharpFileBuilderTemplate>("Intent.AspNetCore.IntegrationTesting.ServiceEndpointTest", domainInvoation.Id);
                        DoDomainInvocationTest(template, crudTest, domainInvoation);
                    }
                }
            }
        }

        private void DoCreateTest(ICSharpFileBuilderTemplate template, CrudMap crudTest)
        {
            template.CSharpFile.OnBuild(file =>
            {
                var @class = template.CSharpFile.Classes.First();
                var operation = crudTest.Create;

                @class.AddMethod("Task", $"{operation.Name}_Should{operation.Name}", method =>
                {
                    template.AddUsing("AutoFixture");
                    var sutId = $"{crudTest.Entity.Name.ToParameterName()}Id";
                    var dtoModel = crudTest.Create.Inputs.First(x => x.TypeReference?.Element.SpecializationTypeId == Constansts.DtoSpecializationType || x.TypeReference?.Element.SpecializationTypeId == Constansts.CommandSpecializationType);
                    var entityName = crudTest.Entity.Name.ToParameterName() == "client" ? "clientEntity" : crudTest.Entity.Name.ToParameterName();

                    var getByIdParams = crudTest.OwningAggregate is null ? sutId : $"{crudTest.OwningAggregate.AsArguments()}, {sutId}";


                    method
                        .Async()
                        .AddAttribute("Fact");
                    AddRequirementTraits(crudTest, method, template);
                    method
                        .AddStatement("// Arrange")
                        .AddStatement($"var client = new {template.GetTypeName("Intent.AspNetCore.IntegrationTesting.HttpClient", crudTest.Proxy.Id)}(CreateClient());")
                        .AddStatement($"var dataFactory = new TestDataFactory(WebAppFactory);", s => s.SeparatedFromPrevious());

                    if (crudTest.Dependencies.Any() || crudTest.OwningAggregate != null)
                    {
                        if (crudTest.OwningAggregate is null)
                        {
                            method.AddStatement($"await dataFactory.Create{crudTest.Entity.Name}Dependencies();");
                        }
                        else
                        {
                            method.AddStatement($"var {crudTest.OwningAggregate.VariableName()} = await dataFactory.Create{crudTest.Entity.Name}Dependencies();");
                        }
                    }

                    method
                        .AddStatement($"var command = dataFactory.CreateCommand<{template.GetTypeName(dtoModel.TypeReference)}>();", s => s.SeparatedFromPrevious())
                        .AddStatement("// Act", s => s.SeparatedFromPrevious());

                    string parameters = $"command";
                    if (crudTest.Create.Inputs.Count > 1 && crudTest.OwningAggregate != null)
                    {
                        parameters = $"{crudTest.OwningAggregate.AsArguments()}, {parameters}";
                    }

                    if (crudTest.ResponseDtoIdField is null)
                    {
                        method.AddStatement($"var {sutId} = await client.{crudTest.Create.Name}Async({parameters}, TestContext.Current.CancellationToken);");
                    }
                    else
                    {
                        method.AddStatement($"var createdDto = await client.{crudTest.Create.Name}Async({parameters}, TestContext.Current.CancellationToken);");
                        method.AddStatement($"var {sutId} = createdDto.{crudTest.ResponseDtoIdField};");
                    }
                    method
                        .AddStatement("// Assert", s => s.SeparatedFromPrevious())
                        .AddStatement($"var {entityName} = await client.{crudTest.GetById.Name}Async({getByIdParams}, TestContext.Current.CancellationToken);")
                        .AddStatement($"Assert.NotNull({entityName});");
                });

            });
        }

        private void DoGetByIdTest(ICSharpFileBuilderTemplate template, CrudMap crudTest)
        {
            template.CSharpFile.OnBuild(file =>
            {
                var @class = template.CSharpFile.Classes.First();
                var operation = crudTest.GetById;

                @class.AddMethod("Task", $"{operation.Name}_Should{operation.Name}", method =>
                {
                    template.AddUsing("AutoFixture");
                    var sutId = crudTest.OwningAggregate is null ? $"{crudTest.Entity.Name.ToParameterName()}Id" : $"ids.{crudTest.Entity.Name.ToPascalCase()}Id";
                    var createVarName = crudTest.OwningAggregate is null ? $"{crudTest.Entity.Name.ToParameterName()}Id" : "ids";
                    var getByIdParams = crudTest.OwningAggregate is null ? sutId : $"{crudTest.OwningAggregate.AsArguments(true)}, {sutId}";
                    var entityName = crudTest.Entity.Name.ToParameterName() == "client" ? "clientEntity" : crudTest.Entity.Name.ToParameterName();

                    method
                        .Async()
                        .AddAttribute("Fact");
                    AddRequirementTraits(crudTest, method, template);
                    method
                        .AddStatement("// Arrange")
                        .AddStatement($"var client = new {template.GetTypeName("Intent.AspNetCore.IntegrationTesting.HttpClient", crudTest.Proxy.Id)}(CreateClient());")

                        .AddStatement($"var dataFactory = new TestDataFactory(WebAppFactory);", s => s.SeparatedFromPrevious())
                        .AddStatement($"var {createVarName} = await dataFactory.Create{crudTest.Entity.Name}();")

                        .AddStatement("// Act", s => s.SeparatedFromPrevious())
                        .AddStatement($"var {entityName} = await client.{crudTest.GetById.Name}Async({getByIdParams}, TestContext.Current.CancellationToken);")

                        .AddStatement("// Assert", s => s.SeparatedFromPrevious())
                        .AddStatement($"Assert.NotNull({entityName});")
                        ;
                });
            });
        }

        private void DoGetAllTest(ICSharpFileBuilderTemplate template, CrudMap crudTest)
        {
            template.CSharpFile.OnBuild(file =>
            {
                var @class = template.CSharpFile.Classes.First();
                var operation = crudTest.GetAll!;

                @class.AddMethod("Task", $"{operation.Name}_Should{operation.Name}", method =>
                {
                    template.AddUsing("AutoFixture");
                    var dtoModel = crudTest.Create.Inputs.First();

                    method
                        .Async()
                        .AddAttribute("Fact");
                    AddRequirementTraits(crudTest, method, template);
                    method
                        .AddStatement("// Arrange")
                        .AddStatement($"var client = new {template.GetTypeName("Intent.AspNetCore.IntegrationTesting.HttpClient", crudTest.Proxy.Id)}(CreateClient());")

                        .AddStatement($"var dataFactory = new TestDataFactory(WebAppFactory);", s => s.SeparatedFromPrevious())
                        .AddStatement($"{(crudTest.OwningAggregate is not null ? "var ids = " : "")}await dataFactory.Create{crudTest.Entity.Name}();")
                        .AddStatement("// Act", s => s.SeparatedFromPrevious())
                        .AddStatement($"var {crudTest.Entity.Name.ToParameterName().Pluralize()} = await client.{crudTest.GetAll!.Name}Async({(crudTest.OwningAggregate is not null ? $"{crudTest.OwningAggregate.AsArguments(true)}, TestContext.Current.CancellationToken" : "TestContext.Current.CancellationToken")});")

                        .AddStatement("// Assert", s => s.SeparatedFromPrevious())
                        .AddStatement($"Assert.NotEmpty({crudTest.Entity.Name.ToParameterName().Pluralize()});")
                        ;
                });
            });
        }

        private void DoDeleteTest(ICSharpFileBuilderTemplate template, CrudMap crudTest)
        {
            template.CSharpFile.OnBuild(file =>
            {
                template.AddUsing("System.Net");
                template.AddUsing("AutoFixture");
                template.GetHttpClientRequestExceptionName();

                var @class = template.CSharpFile.Classes.First();
                var operation = crudTest.Delete!;

                @class.AddMethod("Task", $"{operation.Name}_Should{operation.Name}", method =>
                {
                    var sutId = crudTest.OwningAggregate is null ? $"{crudTest.Entity.Name.ToParameterName()}Id" : $"ids.{crudTest.Entity.Name.ToPascalCase()}Id";
                    var dtoModel = crudTest.Create.Inputs.First();

                    var createVarName = crudTest.OwningAggregate is null ? $"{crudTest.Entity.Name.ToParameterName()}Id" : "ids";
                    var deleteParams = crudTest.OwningAggregate is null ? sutId : $"{crudTest.OwningAggregate.AsArguments(true)}, {sutId}";
                    var getByIdParams = crudTest.OwningAggregate is null ? sutId : $"{crudTest.OwningAggregate.AsArguments(true)}, {sutId}";

                    method
                        .Async()
                        .AddAttribute("Fact");
                    AddRequirementTraits(crudTest, method, template);
                    method
                        .AddStatement("// Arrange")
                        .AddStatement($"var client = new {template.GetTypeName("Intent.AspNetCore.IntegrationTesting.HttpClient", crudTest.Proxy.Id)}(CreateClient());")
                        .AddStatement($"var dataFactory = new TestDataFactory(WebAppFactory);", s => s.SeparatedFromPrevious())
                        .AddStatement($"var {createVarName} = await dataFactory.Create{crudTest.Entity.Name}();")
                        .AddStatement("// Act", s => s.SeparatedFromPrevious())
                        .AddStatement($"await client.{crudTest.Delete!.Name}Async({deleteParams}, TestContext.Current.CancellationToken);")

                        .AddStatement("// Assert", s => s.SeparatedFromPrevious())
                        .AddStatement($"var exception = await Assert.ThrowsAsync<{template.GetHttpClientRequestExceptionName()}>(() => client.{crudTest.GetById.Name}Async({getByIdParams}, TestContext.Current.CancellationToken));")
                        .AddStatement($"Assert.Equal(HttpStatusCode.NotFound, exception.StatusCode);")
                        ;
                });
            });
        }

        private void DoUpdateTest(ICSharpFileBuilderTemplate template, CrudMap crudTest)
        {
            template.AddUsing("AutoFixture");
            template.CSharpFile.OnBuild(file =>
            {
                var @class = template.CSharpFile.Classes.First();
                var operation = crudTest.Update!;

                @class.AddMethod("Task", $"{operation.Name}_Should{operation.Name}", method =>
                {
                    var sutId = crudTest.OwningAggregate is null ? $"{crudTest.Entity.Name.ToParameterName()}Id" : $"ids.{crudTest.Entity.Name.ToPascalCase()}Id";
                    var updateDtoModel = crudTest.Update!.Inputs.First(x => x.TypeReference?.Element.SpecializationTypeId == Constansts.DtoSpecializationType || x.TypeReference?.Element.SpecializationTypeId == Constansts.CommandSpecializationType);
                    var getDtoModel = crudTest.GetById.ReturnType!;
                    var createVarName = crudTest.OwningAggregate is null ? $"{crudTest.Entity.Name.ToParameterName()}Id" : "ids";
                    var getByIdParams = crudTest.OwningAggregate is null ? sutId : $"{crudTest.OwningAggregate.AsArguments(true)}, {sutId}";
                    var entityName = crudTest.Entity.Name.ToParameterName() == "client" ? "clientEntity" : crudTest.Entity.Name.ToParameterName();

                    method
                        .Async()
                        .AddAttribute("Fact");
                    AddRequirementTraits(crudTest, method, template);
                    method
                        .AddStatement("// Arrange")
                        .AddStatement($"var client = new {template.GetTypeName("Intent.AspNetCore.IntegrationTesting.HttpClient", crudTest.Proxy.Id)}(CreateClient());")

                        .AddStatement($"var dataFactory = new TestDataFactory(WebAppFactory);", s => s.SeparatedFromPrevious())
                        .AddStatement($"var {$"{createVarName}"} = await dataFactory.Create{crudTest.Entity.Name}();")
                        .AddStatement($"var command = dataFactory.CreateCommand<{template.GetTypeName(updateDtoModel.TypeReference)}>();", s => s.SeparatedFromPrevious());


                    string parameters = $"{sutId}, command";
                    if (crudTest.Update.Inputs.Count > 2 && crudTest.OwningAggregate != null)
                    {
                        parameters = $"{crudTest.OwningAggregate.AsArguments(true)}, {parameters}";
                    }

                    SetDtoPKFieldIfPresent(method, updateDtoModel, GetDtoPkFieldName(operation), sutId);

                    method
                        .AddStatement("// Act", s => s.SeparatedFromPrevious())
                        .AddStatement($"await client.{crudTest.Update!.Name}Async({parameters}, TestContext.Current.CancellationToken);")
                        .AddStatement("// Assert", s => s.SeparatedFromPrevious())
                        .AddStatement($"var {entityName} = await client.{crudTest.GetById.Name}Async({getByIdParams}, TestContext.Current.CancellationToken);")
                        .AddStatement($"Assert.NotNull({entityName});")
                        ;

                    //Checking that at least 1 field changed, ideally a string field
                    var matchingFields = ((IElement)getDtoModel.Element).ChildElements.Select(c => c.Name)
                    .Intersect(
                    ((IElement)updateDtoModel.TypeReference.Element).ChildElements.Select(c => c.Name)).Where(x => !x.EndsWith("Id")).ToList();
                    if (matchingFields.Any())
                    {
                        var stringField = (((IElement)getDtoModel.Element).ChildElements).FirstOrDefault(x => matchingFields.Contains(x.Name) && x.TypeReference.HasStringType());
                        if (stringField != null)
                        {
                            method.AddStatement($"Assert.Equal(command.{stringField.Name}, {entityName}.{stringField.Name});");
                        }
                        else
                        {
                            method.AddStatement($"Assert.Equal(command.{matchingFields.First()}, {entityName}.{matchingFields.First()});");
                        }
                    }

                });
            });
        }

        private void SetDtoPKFieldIfPresent(CSharpClassMethod method, IHttpEndpointInputModel dto, string pkFieldName, string sutId)
        {
            if (((IElement)dto.TypeReference.Element).ChildElements.Any(c => c.Name == pkFieldName))
            {
                method
                    .AddStatement($"command.{pkFieldName} = {sutId};");
            }
        }

        private void DoDomainInvocationTest(ICSharpFileBuilderTemplate template, CrudMap crudTest, IHttpEndpointModel endpoint)
        {
            template.AddUsing("AutoFixture");
            template.CSharpFile.OnBuild(file =>
            {
                var @class = template.CSharpFile.Classes.First();
                var operation = endpoint;

                @class.AddMethod("Task", $"{operation.Name}_Should{operation.Name}", method =>
                {
                    var sutId = crudTest.OwningAggregate is null ? $"{crudTest.Entity.Name.ToParameterName()}Id" : $"ids.{crudTest.Entity.Name.ToPascalCase()}Id";
                    var invocationDtoModel = endpoint.Inputs.First(x => x.TypeReference?.Element.SpecializationTypeId == Constansts.DtoSpecializationType || x.TypeReference?.Element.SpecializationTypeId == Constansts.CommandSpecializationType);
                    var getDtoModel = crudTest.GetById.ReturnType!;
                    //var owningAggregateId = crudTest.OwningAggregate is null ? null : $"ids.{crudTest.OwningAggregate.Name.ToPascalCase()}Id";
                    var createVarName = crudTest.OwningAggregate is null ? $"{crudTest.Entity.Name.ToParameterName()}Id" : "ids";
                    var getByIdParams = crudTest.OwningAggregate is null ? sutId : $"{crudTest.OwningAggregate.AsArguments(true)}, {sutId}";
                    var entityName = crudTest.Entity.Name.ToParameterName() == "client" ? "clientEntity" : crudTest.Entity.Name.ToParameterName();

                    method
                        .Async()
                        .AddAttribute("Fact");
                    AddRequirementTraits(crudTest, method, template);
                    method
                        .AddStatement("// Arrange")
                        .AddStatement($"var client = new {template.GetTypeName("Intent.AspNetCore.IntegrationTesting.HttpClient", crudTest.Proxy.Id)}(CreateClient());")

                        .AddStatement($"var dataFactory = new TestDataFactory(WebAppFactory);", s => s.SeparatedFromPrevious())
                        .AddStatement($"var {$"{createVarName}"} = await dataFactory.Create{crudTest.Entity.Name}();")
                        .AddStatement($"var command = dataFactory.CreateCommand<{template.GetTypeName(invocationDtoModel.TypeReference)}>();", s => s.SeparatedFromPrevious());


                    string parameters = $"{sutId}, command";
                    if (endpoint.Inputs.Count > 2 && crudTest.OwningAggregate != null)
                    {
                        parameters = $"{crudTest.OwningAggregate.AsArguments(true)}, {parameters}";
                    }

                    SetDtoPKFieldIfPresent(method, invocationDtoModel, GetDtoPkFieldName(operation), sutId);

                    method
                        .AddStatement("// Act", s => s.SeparatedFromPrevious())
                        .AddStatement($"await client.{endpoint.Name}Async({parameters}, TestContext.Current.CancellationToken);")
                        .AddStatement("// Assert", s => s.SeparatedFromPrevious())
                        .AddStatement($"var {entityName} = await client.{crudTest.GetById.Name}Async({getByIdParams}, TestContext.Current.CancellationToken);")
                        .AddStatement($"Assert.NotNull({entityName});")
                        ;

                    //Checking that at least 1 field changed, ideally a string field
                    var matchingFields = ((IElement)getDtoModel.Element).ChildElements.Select(c => c.Name)
                    .Intersect(
                    ((IElement)invocationDtoModel.TypeReference.Element).ChildElements.Select(c => c.Name)).Where(x => !x.EndsWith("Id")).ToList();
                    if (matchingFields.Any())
                    {
                        var stringField = (((IElement)getDtoModel.Element).ChildElements).FirstOrDefault(x => matchingFields.Contains(x.Name) && x.TypeReference.HasStringType());
                        if (stringField != null)
                        {
                            method.AddStatement($"Assert.Equal(command.{stringField.Name}, {entityName}.{stringField.Name});");
                        }
                        else
                        {
                            method.AddStatement($"Assert.Equal(command.{matchingFields.First()}, {entityName}.{matchingFields.First()});");
                        }
                    }
                });
            });
        }

        private void AddRequirementTraits(CrudMap test, CSharpClassMethod method, ICSharpFileBuilderTemplate template)
        {
            if (test.Entity.InternalElement.RequiresCosmosDb((IntentTemplateBase)template))
            {
                AddRequirementTrait(method, "CosmosDB");
                method.WithComments("/// The Cosmos DB Linux Emulator Docker image does not run on Microsoft's CI environment (GitHub, Azure DevOps).\")] // https://github.com/Azure/azure-cosmos-db-emulator-docker/issues/45.");
            }
        }

        private static void AddRequirementTrait(CSharpClassMethod method, string requirement)
        {
            method.AddAttribute("Trait", attribute =>
            {
                attribute
                    .AddArgument("\"Requirement\"")
                    .AddArgument($"\"{requirement}\"");
            });
            method.WithComments(new[]
                {
                    "/// <summary>",
                    $"/// You can use this trait to filter this test out of your CI/CD if appropriate e.g. dotnet test --filter Requirement!=\"{requirement}\"",
                    "/// </summary>",
                });
        }

        private string GetDtoPkFieldName(IHttpEndpointModel operation)
        {

            var result = TestDataFactoryHelper.GetDtoPkFieldName(operation);
            return result?.ToPascalCase() ?? "Id";
        }

        private void PopulateTestDataFactory(ICSharpFileBuilderTemplate template, List<CrudMap> crudTests)
        {
            TestDataFactoryHelper.PopulateTestDataFactory(template, crudTests);
        }
    }
}