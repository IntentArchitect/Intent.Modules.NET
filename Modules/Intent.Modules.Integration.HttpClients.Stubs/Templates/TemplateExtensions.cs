using System.Collections.Generic;
using Intent.Modules.Common.Templates;
using Intent.Modules.Integration.HttpClients.Shared.Templates;
using Intent.Modules.Integration.HttpClients.Stubs.Templates.HttpClientStub;
using Intent.Modules.Integration.HttpClients.Stubs.Templates.StubHttpClientConfiguration;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.TemplateExtensions", Version = "1.0")]

namespace Intent.Modules.Integration.HttpClients.Stubs.Templates
{
    public static class TemplateExtensions
    {
        public static string GetHttpClientStubName<T>(this IIntentTemplate<T> template) where T : IServiceProxyModel
        {
            return template.GetTypeName(HttpClientStubTemplate.TemplateId, template.Model);
        }

        public static string GetHttpClientStubName(this IIntentTemplate template, IServiceProxyModel model)
        {
            return template.GetTypeName(HttpClientStubTemplate.TemplateId, model);
        }

        public static string GetStubHttpClientConfigurationName(this IIntentTemplate template)
        {
            return template.GetTypeName(StubHttpClientConfigurationTemplate.TemplateId);
        }
    }
}
