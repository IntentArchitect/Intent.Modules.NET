using System.Collections.Generic;
using Intent.Modelers.Services.Api;
using Intent.Modules.Common.Templates;
using Intent.Modules.Common.Types.Api;
using Intent.Modules.IntegrationTesting.Templates.BaseIntegrationTest;
using Intent.Modules.IntegrationTesting.Templates.CosmosContainerFixture;
using Intent.Modules.IntegrationTesting.Templates.DtoContract;
using Intent.Modules.IntegrationTesting.Templates.EFContainerFixture;
using Intent.Modules.IntegrationTesting.Templates.EnumContract;
using Intent.Modules.IntegrationTesting.Templates.HttpClient;
using Intent.Modules.IntegrationTesting.Templates.HttpClientRequestException;
using Intent.Modules.IntegrationTesting.Templates.IntegrationTestWebAppFactory;
using Intent.Modules.IntegrationTesting.Templates.JsonResponse;
using Intent.Modules.IntegrationTesting.Templates.PagedResult;
using Intent.Modules.IntegrationTesting.Templates.ProxyServiceContract;
using Intent.Modules.IntegrationTesting.Templates.ServiceEndpointTest;
using Intent.Modules.IntegrationTesting.Templates.SharedContainerFixture;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.TemplateExtensions", Version = "1.0")]

namespace Intent.Modules.IntegrationTesting.Templates
{
    public static class TemplateExtensions
    {
        public static string GetBaseIntegrationTestName(this IIntentTemplate template)
        {
            return template.GetTypeName(BaseIntegrationTestTemplate.TemplateId);
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

        public static string GetPagedResultName(this IIntentTemplate template)
        {
            return template.GetTypeName(PagedResultTemplate.TemplateId);
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