using System;
using System.Collections.Generic;
using Intent.Engine;
using Intent.Modules.AspNetCore.IntegrationTesting;
using Intent.Modules.AspNetCore.IntegrationTesting.Templates;
using Intent.Modules.AspNetCore.IntegrationTesting.Templates.DtoContract;
using Intent.Modules.AspNetCore.IntegrationTesting.Templates.ProxyServiceContract;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.AspNetCore.IntegrationTests.CRUD.Templates.TestDataFactory
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class TestDataFactoryTemplate : CSharpTemplateBase<object>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.AspNetCore.IntegrationTests.CRUD.TestDataFactory";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public TestDataFactoryTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            AddNugetDependency(NugetPackages.AutoFixture(outputTarget));

            AddTypeSource(ProxyServiceContractTemplate.TemplateId);
            AddTypeSource(DtoContractTemplate.TemplateId);

            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddClass($"TestDataFactory", @class =>
                {
                    @class.AddField(UseType("AutoFixture.Fixture"), "_fixture", f => f.PrivateReadOnly());
                    @class.AddConstructor(ctor =>
                    {
                        ctor.AddParameter(this.GetIntegrationTestWebAppFactoryName(), "factory", param => param.IntroduceReadonlyField());
                        ctor.AddStatement("_fixture = new Fixture();");
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