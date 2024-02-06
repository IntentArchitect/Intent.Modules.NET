using System;
using System.Collections.Generic;
using Intent.Engine;
using Intent.Modules.AspNetCore.IntegrationTesting.Settings;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.AspNetCore.IntegrationTesting.Templates.BaseIntegrationTest
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class BaseIntegrationTestTemplate : CSharpTemplateBase<object>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.AspNetCore.IntegrationTesting.BaseIntegrationTest";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public BaseIntegrationTestTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddClass($"BaseIntegrationTest", @class =>
                {
                    AddUsing("Microsoft.Extensions.DependencyInjection");
                    if (this.ExecutionContext.Settings.GetIntegrationTestSettings().ContainerIsolation().IsContainerPerTestClass())
                    {
                        @class.ImplementsInterface($"IClassFixture<{this.GetIntegrationTestWebAppFactoryName()}>");
                    }

                    @class.AddConstructor(ctor =>
                    {
                        ctor.AddParameter(this.GetIntegrationTestWebAppFactoryName(), "webAppFactory", p => p.IntroduceProperty(p => p.ReadOnly()));
                    });


                    @class.AddMethod("HttpClient", "CreateClient", method =>
                    {
                        method
                            .Protected()
                            .AddStatement("return WebAppFactory.CreateClient();");
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