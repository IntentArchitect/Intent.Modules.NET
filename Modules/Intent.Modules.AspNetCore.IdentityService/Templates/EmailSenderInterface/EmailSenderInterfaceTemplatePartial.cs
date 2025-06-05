using System;
using System.Collections.Generic;
using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.AspNetCore.IdentityService.Templates.EmailSenderInterface
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class EmailSenderInterfaceTemplate : CSharpTemplateBase<object>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.AspNetCore.IdentityService.EmailSenderInterface";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public EmailSenderInterfaceTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddInterface($"IEmailSender", @interface =>
                {
                    @interface.AddGenericParameter("TUser", out var tUser)
                        .AddGenericTypeConstraint(tUser, c => c
                        .AddType("class"));
                    @interface.AddMethod("Task", "SendConfirmationLinkAsync", method =>
                    {
                        method.Async();
                        method.AddParameter("TUser", "user");
                        method.AddParameter("string", "email");
                        method.AddParameter("string", "confirmationLink");
                    });

                    @interface.AddMethod("Task", "SendPasswordResetLinkAsync", method =>
                    {
                        method.Async();
                        method.AddParameter("TUser", "user");
                        method.AddParameter("string", "email");
                        method.AddParameter("string", "resetLink");
                    });

                    @interface.AddMethod("Task", "SendPasswordResetCodeAsync", method =>
                    {
                        method.Async();
                        method.AddParameter("TUser", "user");
                        method.AddParameter("string", "email");
                        method.AddParameter("string", "resetCode");
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