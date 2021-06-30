using System.Collections.Generic;
using Intent.Engine;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;


[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.Security.JWT.Templates.ConfigurationJWTAuthentication
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    partial class ConfigurationJWTAuthenticationTemplate : CSharpTemplateBase<object, JWTAuthenticationDecorator>
    {
        [IntentManaged(Mode.Fully)]
        public const string TemplateId = "Intent.Security.JWT.ConfigurationJWTAuthentication";

        [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
        public ConfigurationJWTAuthenticationTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            AddNugetDependency(NugetPackages.MicrosoftAspNetCoreAuthenticationJwtBearer);
        }

        protected override CSharpFileConfig DefineFileConfig()
        {
            return new CSharpFileConfig(
                className: $"JWTAuthenticationConfiguration",
                @namespace: $"{this.GetNamespace()}",
                relativeLocation: $"{this.GetFolderPath()}");
        }
    }
}