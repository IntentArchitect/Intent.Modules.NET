using System;
using System.Collections.Generic;
using Intent.Engine;
using Intent.Modules.AspNetCore.Identity;
using Intent.Modules.AspNetCore.IdentityService.Templates.EmailSenderOptions;
using Intent.Modules.AspNetCore.IdentityService.Templates.IdentityEmailSenderInterface;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.DependencyInjection;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.AspNetCore.IdentityService.Templates.IdentityEmailSender
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class IdentityEmailSenderTemplate : CSharpTemplateBase<object>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.AspNetCore.IdentityService.IdentityEmailSender";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public IdentityEmailSenderTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddUsing("Microsoft.AspNetCore.Identity")
                .AddUsing("Microsoft.Extensions.Logging")
                .AddUsing("Microsoft.Extensions.Options")
                .AddUsing("System.Net.Mail")
                .AddUsing("System.Net")
                .AddClass($"IdentityEmailSender", @class =>
                {
                    GetTypeName(EmailSenderOptionsTemplate.TemplateId);
                    @class.ImplementsInterface(GetFullyQualifiedTypeName(IdentityEmailSenderInterfaceTemplate.TemplateId));

                    @class.AddField("EmailSenderOptions", "_options", c => c.PrivateReadOnly());

                    @class.AddConstructor(c =>
                    {
                        c.AddParameter("IOptions<EmailSenderOptions>", "options");
                        c.AddParameter("ILogger<IdentityEmailSender>", "logger", p => p.IntroduceReadonlyField());
                        c.AddStatement("_options = options.Value;");
                    });

                    @class.AddMethod("Task", "SendConfirmationLinkAsync", m =>
                    {
                        m.Async();
                        m.AddParameter(this.GetIdentityUserClass(), "user");
                        m.AddParameter("string", "email");
                        m.AddParameter("string", "confirmationLink");

                        m.AddStatement("string subject = \"Confirm your email\";");
                        m.AddStatement("string body = $\"Hello {user.UserName},\\n\\nPlease confirm your email by clicking this link:\\n{confirmationLink}\";");
                        m.AddStatement("await SendEmailAsync(email, subject, body);");
                    });

                    @class.AddMethod("Task", "SendPasswordResetCodeAsync", m =>
                    {
                        m.Async();
                        m.AddParameter(this.GetIdentityUserClass(), "user");
                        m.AddParameter("string", "email");
                        m.AddParameter("string", "resetCode");

                        m.AddStatement("string subject = \"Reset your password - Code\";");
                        m.AddStatement("string body = $\"Hello {user.UserName},\\n\\nUse this code to reset your password:\\n{resetCode}\";");
                        m.AddStatement("await SendEmailAsync(email, subject, body);");
                    });

                    @class.AddMethod("Task", "SendPasswordResetLinkAsync", m =>
                    {
                        m.Async();
                        m.AddParameter(this.GetIdentityUserClass(), "user");
                        m.AddParameter("string", "email");
                        m.AddParameter("string", "resetLink");

                        m.AddStatement("string subject = \"Reset your password\";");
                        m.AddStatement("string body = $\"Hello {user.UserName},\\n\\nClick this link to reset your password:\\n{resetLink}\";");
                        m.AddStatement("await SendEmailAsync(email, subject, body);");
                    });

                    @class.AddMethod("Task", "SendEmailAsync", m =>
                    {
                        m.Async();
                        m.Private();
                        m.AddParameter("string", "toEmail");
                        m.AddParameter("string", "subject");
                        m.AddParameter("string", "body");

                        m.AddObjectInitializerBlock("using var client = new SmtpClient(_options.SmtpHost, _options.SmtpPort)", c =>
                        {
                            c.AddObjectInitStatement("Credentials", "new NetworkCredential(_options.Username, _options.Password)");
                            c.AddObjectInitStatement("EnableSsl", "_options.UseSsl");
                            c.WithSemicolon();
                        });

                        m.AddObjectInitializerBlock("var message = new MailMessage", c =>
                        {
                            c.AddObjectInitStatement("From", "new MailAddress(_options.SenderEmail, _options.SenderName)");
                            c.AddObjectInitStatement("Subject", "subject");
                            c.AddObjectInitStatement("Body", "body");
                            c.AddObjectInitStatement("IsBodyHtml", "false");
                            c.WithSemicolon();
                        });

                        m.AddStatement("message.To.Add(toEmail);");

                        m.AddTryBlock(t =>
                        {
                            t.AddStatement("await client.SendMailAsync(message);");
                            t.AddStatement("_logger.LogInformation(\"Email sent to {Email} with subject '{Subject}'\", toEmail, subject);");
                        }).AddCatchBlock("SmtpException", "ex", c =>
                        {
                            c.AddStatement("_logger.LogError(ex, \"Failed to send email to {Email}\", toEmail);");
                            c.AddStatement("throw;");
                        });
                    });
                });
        }

        public override void BeforeTemplateExecution()
        {
            base.BeforeTemplateExecution();
            if (!CanRunTemplate()) return;
            ExecutionContext.EventDispatcher.Publish(ContainerRegistrationRequest.ToRegister(this).ForInterface(GetTemplate<IdentityEmailSenderInterfaceTemplate>(IdentityEmailSenderInterfaceTemplate.TemplateId)).WithSingletonLifeTime());
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