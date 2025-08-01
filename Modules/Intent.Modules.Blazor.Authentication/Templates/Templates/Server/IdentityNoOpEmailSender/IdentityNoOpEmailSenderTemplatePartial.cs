using Intent.Engine;
using Intent.Modules.Blazor.Authentication.Settings;
using Intent.Modules.Blazor.Authentication.Templates.Templates.Server.ApplicationUser;
using Intent.Modules.Blazor.Settings;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;
using System;
using System.Collections.Generic;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.Blazor.Authentication.Templates.Templates.Server.IdentityNoOpEmailSender
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class IdentityNoOpEmailSenderTemplate : CSharpTemplateBase<object>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.Blazor.Authentication.Templates.Server.IdentityNoOpEmailSenderTemplate";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public IdentityNoOpEmailSenderTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddUsing("Microsoft.AspNetCore.Identity")
                .AddUsing("Microsoft.AspNetCore.Identity.UI.Services")
                .AddUsing("System.Threading.Tasks")
                .AddClass($"IdentityNoOpEmailSender", @class =>
                {
                    @class.Internal().Sealed();
                    @class.ImplementsInterface($"{UseType("Microsoft.AspNetCore.Identity.IEmailSender")}<{GetTypeName(ApplicationUserTemplate.TemplateId)}>");

                    @class.AddField("IEmailSender", "emailSender", emailSender =>
                    {
                        emailSender.PrivateReadOnly();
                        emailSender.WithAssignment(new CSharpStatement("new NoOpEmailSender()"));
                    });

                    @class.AddMethod("Task", "SendConfirmationLinkAsync", sendConfirmationLinkAsync =>
                    {
                        sendConfirmationLinkAsync.Async();
                        sendConfirmationLinkAsync.AddParameter(GetTypeName(ApplicationUserTemplate.TemplateId), "user");
                        sendConfirmationLinkAsync.AddParameter("string", "email");
                        sendConfirmationLinkAsync.AddParameter("string", "confirmationLink");

                        sendConfirmationLinkAsync.AddStatement(new CSharpStatement("await emailSender.SendEmailAsync(email, \"Confirm your email\", $\"Please confirm your account by <a href='{confirmationLink}'>clicking here</a>.\");"));
                    });

                    @class.AddMethod("Task", "SendPasswordResetLinkAsync", sendPasswordResetLinkAsync =>
                    {
                        sendPasswordResetLinkAsync.Async();
                        sendPasswordResetLinkAsync.AddParameter(GetTypeName(ApplicationUserTemplate.TemplateId), "user");
                        sendPasswordResetLinkAsync.AddParameter("string", "email");
                        sendPasswordResetLinkAsync.AddParameter("string", "resetLink");

                        sendPasswordResetLinkAsync.AddStatement(new CSharpStatement("await emailSender.SendEmailAsync(email, \"Reset your password\", $\"Please reset your password by <a href='{resetLink}'>clicking here</a>.\");"));
                    });

                    @class.AddMethod("Task", "SendPasswordResetCodeAsync", sendPasswordResetCodeAsync =>
                    {
                        sendPasswordResetCodeAsync.Async();
                        sendPasswordResetCodeAsync.AddParameter(GetTypeName(ApplicationUserTemplate.TemplateId), "user");
                        sendPasswordResetCodeAsync.AddParameter("string", "email");
                        sendPasswordResetCodeAsync.AddParameter("string", "resetCode");

                        sendPasswordResetCodeAsync.AddStatement(new CSharpStatement("await emailSender.SendEmailAsync(email, \"Reset your password\", $\"Please reset your password using the following code: {resetCode}\");"));
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

        public override bool CanRunTemplate()
        {
            return base.CanRunTemplate() && ExecutionContext.GetSettings().GetBlazor().Authentication().IsAspnetcoreIdentity();
        }
    }
}