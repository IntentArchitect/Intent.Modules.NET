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

namespace Intent.Modules.AspNetCore.IntegrationTesting.Templates.SharedContainerFixture
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class SharedContainerFixtureTemplate : CSharpTemplateBase<object>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.AspNetCore.IntegrationTesting.SharedContainerFixture";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public SharedContainerFixtureTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddClass($"SharedContainerFixture", @class =>
                {
                    @class
                        .AddAttribute("CollectionDefinition", a => a.AddArgument("\"SharedContainer\""))
                        .ImplementsInterface($"ICollectionFixture<{this.GetIntegrationTestWebAppFactoryName()}>");
                });
        }

        public override bool CanRunTemplate()
        {
            return base.CanRunTemplate() && this.ExecutionContext.Settings.GetIntegrationTestSettings().ContainerIsolation().IsSharedContainer();
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