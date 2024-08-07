using System;
using System.Collections.Generic;
using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.Integration.HttpClients.Settings;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.Integration.HttpClients.Templates.AuthorizationHeaderProviderInterface
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class AuthorizationHeaderProviderInterfaceTemplate : CSharpTemplateBase<object>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.Integration.HttpClients.AuthorizationHeaderProviderInterface";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public AuthorizationHeaderProviderInterfaceTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddInterface($"IAuthorizationHeaderProvider", @interface =>
                {
                    @interface.AddMethod("string?", "GetAuthorizationHeader", method =>
                    {
                    });
                });
        }

        public override bool CanRunTemplate()
        {
            return base.CanRunTemplate() && ExecutionContext.Settings.GetIntegrationHttpClientSettings().AuthorizationSetup().IsAuthorizationHeaderProvider();
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