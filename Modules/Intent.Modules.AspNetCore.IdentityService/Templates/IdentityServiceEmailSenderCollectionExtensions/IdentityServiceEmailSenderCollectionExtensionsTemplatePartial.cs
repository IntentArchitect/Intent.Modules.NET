using System;
using System.Collections.Generic;
using Intent.Engine;
using Intent.Modules.AspNetCore.IdentityService.Templates.EmailSenderOptions;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.DependencyInjection;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.AspNetCore.IdentityService.Templates.IdentityServiceEmailSenderCollectionExtensions
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class IdentityServiceEmailSenderCollectionExtensionsTemplate : CSharpTemplateBase<object>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.AspNetCore.IdentityService.IdentityServiceEmailSenderCollectionExtensions";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public IdentityServiceEmailSenderCollectionExtensionsTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddUsing("Microsoft.Extensions.DependencyInjection")
                .AddUsing("Microsoft.Extensions.Configuration")
                .AddUsing("System")
                .AddClass($"IdentityServiceEmailSenderCollectionExtensions", @class =>
                {
                    @class.Static();
                    @class.AddMethod("void", "ConfigureIdentityEmailSender", method =>
                    {
                        method.Static();
                        method.AddParameter("IServiceCollection", "services", param =>
                        {
                            param.WithThisModifier();
                        });
                        method.AddParameter("IConfiguration", "configuration");
                        method.AddStatement($"services.Configure<{GetTypeName(EmailSenderOptionsTemplate.TemplateId)}>(configuration.GetSection(\"EmailSender\"));");
                    });
                });
        }

        public override void BeforeTemplateExecution()
        {
            ExecutionContext.EventDispatcher.Publish(ServiceConfigurationRequest.ToRegister(extensionMethodName: "ConfigureIdentityEmailSender", ServiceConfigurationRequest.ParameterType.Configuration).HasDependency(this));
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