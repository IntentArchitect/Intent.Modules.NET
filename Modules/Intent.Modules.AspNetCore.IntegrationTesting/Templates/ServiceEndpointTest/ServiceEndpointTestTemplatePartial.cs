using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Intent.Engine;
using Intent.Modelers.Services.Api;
using Intent.Modules.AspNetCore.IntegrationTesting.Settings;
using Intent.Modules.AspNetCore.IntegrationTesting.Templates.DtoContract;
using Intent.Modules.AspNetCore.IntegrationTesting.Templates.HttpClientRequestException;
using Intent.Modules.AspNetCore.IntegrationTesting.Templates.ProxyServiceContract;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.Metadata.WebApi.Models;
using Intent.Modules.Metadata.WebApi.Stereotypes;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.AspNetCore.IntegrationTesting.Templates.ServiceEndpointTest
{
    [IntentManaged(Mode.Ignore)]
    public partial class ServiceEndpointTestTemplate : CSharpTemplateBase<IHttpEndpointModel>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.AspNetCore.IntegrationTesting.ServiceEndpointTest";

        public ServiceEndpointTestTemplate(IOutputTarget outputTarget, IHttpEndpointModel model) : base(TemplateId, outputTarget, model)
        {
            AddNugetDependency(NugetPackages.AutoFixture(outputTarget));
            AddTypeSource(ProxyServiceContractTemplate.TemplateId);
            AddTypeSource(DtoContractTemplate.TemplateId);

            string apiVersions = GetSanitizedApiVersions(model);

            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath(Model.InternalElement.ParentElement.Name))
                .AddClass($"{Model.Name}Tests{apiVersions}", @class =>
                {
                    @class.AddAttribute(CSharpIntentManagedAttribute.Merge().WithSignatureFully());
                    if (this.ExecutionContext.Settings.GetIntegrationTestSettings().ContainerIsolation().IsSharedContainer())
                    {
                        @class.AddAttribute("Collection", a => a.AddArgument($"\"SharedContainer\""));
                    }
                    @class.WithBaseType(this.GetBaseIntegrationTestName());
                    @class.AddConstructor(ctor =>
                    {
                        ctor.AddParameter(this.GetIntegrationTestWebAppFactoryName(), "factory");
                        ctor.CallsBase(c => c.AddArgument("factory"));
                    });
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

        private static string GetSanitizedApiVersions(IHttpEndpointModel model)
        {
            string apiVersions = string.Empty;
            if (model.InternalElement.TryGetApiVersion(out var apiVersion))
            {
                var invalidChars = Path.GetInvalidFileNameChars().Append('.').Append('_');
                apiVersions = $"_{string.Join("_", apiVersion.ApplicableVersions.Select(v => new string(v.Version.Where(c => !invalidChars.Contains(c)).ToArray())))}";
            }

            return apiVersions;
        }
    }
}