using System;
using System.Collections.Generic;
using Intent.Engine;
using Intent.Modules.AspNetCore.IdentityService.Templates.IdentityEmailSenderInterface;
using Intent.Modules.AspNetCore.IdentityService.Templates.IdentityServiceManagerInterface;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Configuration;
using Intent.Modules.Common.CSharp.DependencyInjection;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.AspNetCore.IdentityService.Templates.EmailSenderOptions
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class EmailSenderOptionsTemplate : CSharpTemplateBase<object>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.AspNetCore.IdentityService.EmailSenderOptions";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public EmailSenderOptionsTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddClass($"EmailSenderOptions", @class =>
                {
                    @class.AddProperty("string", "SmtpHost");
                    @class.AddProperty("int", "SmtpPort");
                    @class.AddProperty("string", "SenderEmail");
                    @class.AddProperty("string", "SenderName");
                    @class.AddProperty("string", "Username");
                    @class.AddProperty("string", "Password");
                    @class.AddProperty("bool", "UseSsl");
                });
        }

        public override void BeforeTemplateExecution()
        {
            ExecutionContext.EventDispatcher.Publish(new AppSettingRegistrationRequest("EmailSender:SmtpHost", "smtp.yourdomain.com"));
            ExecutionContext.EventDispatcher.Publish(new AppSettingRegistrationRequest("EmailSender:SmtpPort", "587"));
            ExecutionContext.EventDispatcher.Publish(new AppSettingRegistrationRequest("EmailSender:SenderEmail", "noreply@yourdomain.com"));
            ExecutionContext.EventDispatcher.Publish(new AppSettingRegistrationRequest("EmailSender:SenderName", "YourAppName"));
            ExecutionContext.EventDispatcher.Publish(new AppSettingRegistrationRequest("EmailSender:Username", "smtp-username"));
            ExecutionContext.EventDispatcher.Publish(new AppSettingRegistrationRequest("EmailSender:Password", "smtp-password"));
            ExecutionContext.EventDispatcher.Publish(new AppSettingRegistrationRequest("EmailSender:UseSsl", "true"));


            ExecutionContext.EventDispatcher.Publish(ServiceConfigurationRequest.ToRegister(extensionMethodName: "Configure<EmailSenderOptions>", ServiceConfigurationRequest.ParameterType.Configuration).HasDependency(this));
            base.BeforeTemplateExecution();
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