using System;
using System.Collections.Generic;
using Intent.Engine;
using Intent.Modules.AspNetCore.Swashbuckle.Security.Settings;
using Intent.Modules.AspNetCore.Swashbuckle.Settings;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.AspNetCore.Swashbuckle.Security.Templates.AuthorizeCheckOperationFilter
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    partial class AuthorizeCheckOperationFilterTemplate : CSharpTemplateBase<object>
    {
        [IntentManaged(Mode.Fully)]
        public const string TemplateId = "Intent.AspNetCore.Swashbuckle.Security.AuthorizeCheckOperationFilter";

        [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
        public AuthorizeCheckOperationFilterTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
        }

        public string GetAuthorizationSchemeName()
        {
            return ExecutionContext.Settings.GetSwaggerSettings().Authentication().Value;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        protected override CSharpFileConfig DefineFileConfig()
        {
            return new CSharpFileConfig(
                className: $"AuthorizeCheckOperationFilter",
                @namespace: $"{this.GetNamespace()}",
                relativeLocation: $"{this.GetFolderPath()}");
        }
    }
}