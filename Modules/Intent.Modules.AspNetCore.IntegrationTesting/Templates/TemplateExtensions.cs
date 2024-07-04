using System.Collections.Generic;
using Intent.Modelers.Services.Api;
using Intent.Modules.AspNetCore.IntegrationTesting.Templates.BaseIntegrationTest;
using Intent.Modules.AspNetCore.IntegrationTesting.Templates.ContainerCosmosClientProvider;
using Intent.Modules.AspNetCore.IntegrationTesting.Templates.CosmosContainerFixture;
using Intent.Modules.AspNetCore.IntegrationTesting.Templates.DtoContract;
using Intent.Modules.AspNetCore.IntegrationTesting.Templates.EFContainerFixture;
using Intent.Modules.AspNetCore.IntegrationTesting.Templates.EnumContract;
using Intent.Modules.AspNetCore.IntegrationTesting.Templates.HttpClient;
using Intent.Modules.AspNetCore.IntegrationTesting.Templates.HttpClientRequestException;
using Intent.Modules.AspNetCore.IntegrationTesting.Templates.IntegrationTestWebAppFactory;
using Intent.Modules.AspNetCore.IntegrationTesting.Templates.JsonResponse;
using Intent.Modules.AspNetCore.IntegrationTesting.Templates.MongoDbContainerFixture;
using Intent.Modules.AspNetCore.IntegrationTesting.Templates.PagedResult;
using Intent.Modules.AspNetCore.IntegrationTesting.Templates.ProblemDetailsWithErrors;
using Intent.Modules.AspNetCore.IntegrationTesting.Templates.ProxyServiceContract;
using Intent.Modules.AspNetCore.IntegrationTesting.Templates.ServiceEndpointTest;
using Intent.Modules.AspNetCore.IntegrationTesting.Templates.SharedContainerFixture;
using Intent.Modules.Common.Templates;
using Intent.Modules.Common.Types.Api;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.TemplateExtensions", Version = "1.0")]

namespace Intent.Modules.AspNetCore.IntegrationTesting.Templates
{
    public static class TemplateExtensions
    {
        public static string GetBaseIntegrationTestName(this IIntentTemplate template)
        {
            return template.GetTypeName(BaseIntegrationTestTemplate.TemplateId);
        }

        public static string GetContainerCosmosClientProviderName(this IIntentTemplate template)
        {
            return template.GetTypeName(ContainerCosmosClientProviderTemplate.TemplateId);
        }

        public static string GetCosmosContainerFixtureName(this IIntentTemplate template)
        {
            return template.GetTypeName(CosmosContainerFixtureTemplate.TemplateId);
        }

        public static string GetDtoContractName<T>(this IIntentTemplate<T> template) where T : DTOModel
        {
            return template.GetTypeName(DtoContractTemplate.TemplateId, template.Model);
        }

        public static string GetDtoContractName(this IIntentTemplate template, DTOModel model)
        {
            return template.GetTypeName(DtoContractTemplate.TemplateId, model);
        }

        public static string GetEFContainerFixtureName(this IIntentTemplate template)
        {
            return template.GetTypeName(EFContainerFixtureTemplate.TemplateId);
        }

        public static string GetEnumContractName<T>(this IIntentTemplate<T> template) where T : EnumModel
        {
            return template.GetTypeName(EnumContractTemplate.TemplateId, template.Model);
        }

        public static string GetEnumContractName(this IIntentTemplate template, EnumModel model)
        {
            return template.GetTypeName(EnumContractTemplate.TemplateId, model);
        }

        public static string GetHttpClientName<T>(this IIntentTemplate<T> template) where T : ServiceModel
        {
            return template.GetTypeName(HttpClientTemplate.TemplateId, template.Model);
        }

        public static string GetHttpClientName(this IIntentTemplate template, ServiceModel model)
        {
            return template.GetTypeName(HttpClientTemplate.TemplateId, model);
        }

        public static string GetHttpClientRequestExceptionName(this IIntentTemplate template)
        {
            return template.GetTypeName(HttpClientRequestExceptionTemplate.TemplateId);
        }
        public static string GetIntegrationTestWebAppFactoryName(this IIntentTemplate template)
        {
            return template.GetTypeName(IntegrationTestWebAppFactoryTemplate.TemplateId);
        }

        public static string GetJsonResponseName(this IIntentTemplate template)
        {
            return template.GetTypeName(JsonResponseTemplate.TemplateId);
        }

        public static string GetMongoDbContainerFixtureName(this IIntentTemplate template)
        {
            return template.GetTypeName(MongoDbContainerFixtureTemplate.TemplateId);
        }

        public static string GetPagedResultName(this IIntentTemplate template)
        {
            return template.GetTypeName(PagedResultTemplate.TemplateId);
        }

        public static string GetProblemDetailsWithErrorsName(this IIntentTemplate template)
        {
            return template.GetTypeName(ProblemDetailsWithErrorsTemplate.TemplateId);
        }

        public static string GetProxyServiceContractName<T>(this IIntentTemplate<T> template) where T : ServiceModel
        {
            return template.GetTypeName(ProxyServiceContractTemplate.TemplateId, template.Model);
        }

        public static string GetProxyServiceContractName(this IIntentTemplate template, ServiceModel model)
        {
            return template.GetTypeName(ProxyServiceContractTemplate.TemplateId, model);
        }

        public static string GetServiceEndpointTestName<T>(this IIntentTemplate<T> template) where T : ServiceModel
        {
            return template.GetTypeName(ServiceEndpointTestTemplate.TemplateId, template.Model);
        }

        public static string GetServiceEndpointTestName(this IIntentTemplate template, ServiceModel model)
        {
            return template.GetTypeName(ServiceEndpointTestTemplate.TemplateId, model);
        }

        public static string GetSharedContainerFixtureName(this IIntentTemplate template)
        {
            return template.GetTypeName(SharedContainerFixtureTemplate.TemplateId);
        }

    }
}