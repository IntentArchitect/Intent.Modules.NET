using System;
using System.Collections.Generic;
using Intent.Engine;
using Intent.Modules.AspNetCore.Identity.AccountController.Templates.AccountEmailSenderInterface;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.DependencyInjection;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.AspNetCore.Identity.AccountController.Templates.AccountEmailSender
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    partial class AccountEmailSenderTemplate : CSharpTemplateBase<object>
    {
        public const string TemplateId = "Intent.AspNetCore.Identity.AccountController.AccountEmailSender";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public AccountEmailSenderTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
        }

        public override void BeforeTemplateExecution()
        {
            ExecutionContext.EventDispatcher.Publish(ContainerRegistrationRequest.ToRegister(this).ForInterface(GetTemplate<AccountEmailSenderInterfaceTemplate>(AccountEmailSenderInterfaceTemplate.TemplateId)));
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        protected override CSharpFileConfig DefineFileConfig()
        {
            return new CSharpFileConfig(
                className: $"AccountEmailSender",
                @namespace: $"{this.GetNamespace()}",
                relativeLocation: $"{this.GetFolderPath()}");
        }
    }
}