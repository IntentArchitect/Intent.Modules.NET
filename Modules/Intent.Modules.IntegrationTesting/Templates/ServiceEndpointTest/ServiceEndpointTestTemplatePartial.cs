using System;
using System.Collections.Generic;
using Intent.Engine;
using Intent.Modelers.Services.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.Metadata.WebApi.Models;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.IntegrationTesting.Templates.ServiceEndpointTest
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class ServiceEndpointTestTemplate : CSharpTemplateBase<IHttpEndpointModel>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.IntegrationTesting.ServiceEndpointTest";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public ServiceEndpointTestTemplate(IOutputTarget outputTarget, IHttpEndpointModel model) : base(TemplateId, outputTarget, model)
        {
            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath(Model.InternalElement.ParentElement.Name))
                .AddClass($"{Model.Name}Tests", @class =>
                {
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
    }
}