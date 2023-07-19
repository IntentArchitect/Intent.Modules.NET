using System;
using System.Collections.Generic;
using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.AspNetCore.Identity.AccountController.Templates.AccountController
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    partial class AccountControllerTemplate : CSharpTemplateBase<object>
    {
        public const string TemplateId = "Intent.AspNetCore.Identity.AccountController.AccountController";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public AccountControllerTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            AddNugetDependency(NugetPackages.IdentityModel);
            AddNugetDependency(NugetPackages.MicrosoftAspNetCoreAuthenticationJwtBearer(OutputTarget));
        }

        public override void BeforeTemplateExecution()
        {
            var randomBytesBuffer = new byte[32];
            Random.Shared.NextBytes(randomBytesBuffer);

            this.ApplyAppSetting("JwtToken", new
            {
                Issuer = OutputTarget.ExecutionContext.GetApplicationConfig().Name,
                Audience = OutputTarget.ExecutionContext.GetApplicationConfig().Name,
                SigningKey = Convert.ToBase64String(randomBytesBuffer),
                AuthTokenExpiryMinutes = 120,
                RefreshTokenExpiryMinutes = 3
            });
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        protected override CSharpFileConfig DefineFileConfig()
        {
            return new CSharpFileConfig(
                className: $"AccountController",
                @namespace: $"{this.GetNamespace()}",
                relativeLocation: $"{this.GetFolderPath()}");
        }
    }
}