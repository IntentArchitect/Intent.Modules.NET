using System.Collections.Generic;
using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.Security.MSAL.Templates.ConfigurationMSALAuthentication
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    partial class ConfigurationMSALAuthenticationTemplate : CSharpTemplateBase<object, MSALAuthenticationDecorator>
    {
        [IntentManaged(Mode.Fully)]
        public const string TemplateId = "Intent.Security.MSAL.ConfigurationMSALAuthentication";

        [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
        public ConfigurationMSALAuthenticationTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            AddNugetDependency(NugetPackages.MicrosoftAspNetCoreAuthenticationJwtBearer);
            AddNugetDependency(NugetPackages.MicrosoftAspNetCoreAuthenticationOpenIdConnect);
            AddNugetDependency(NugetPackages.MicrosoftIdentityWeb);
            AddNugetDependency(NugetPackages.MicrosoftIdentityWebMicrosoftGraph);
            AddNugetDependency(NugetPackages.MicrosoftIdentityWebUI);
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        protected override CSharpFileConfig DefineFileConfig()
        {
            return new CSharpFileConfig(
                className: $"MSALAuthenticationConfiguration",
                @namespace: $"{this.GetNamespace()}",
                relativeLocation: $"{this.GetFolderPath()}");
        }
    }
}