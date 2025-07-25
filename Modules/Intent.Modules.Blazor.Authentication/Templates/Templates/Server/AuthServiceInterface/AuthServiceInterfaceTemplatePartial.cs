using System;
using System.Collections.Generic;
using Intent.Engine;
using Intent.Modules.Blazor.Authentication.Settings;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.Blazor.Authentication.Templates.Templates.Server.AuthServiceInterface
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class AuthServiceInterfaceTemplate : CSharpTemplateBase<object>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.Blazor.Authentication.Templates.Server.AuthServiceInterfaceTemplate";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public AuthServiceInterfaceTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddUsing("System.Threading.Tasks")
                .AddInterface($"IAuthService", @interface =>
                {
                    @interface.AddMethod("Task", "Login", method =>
                    {
                        method.Async();
                        method.AddParameter("string", "email");
                        method.AddParameter("string", "password");
                        method.AddParameter("bool", "rememberMe");
                        method.AddParameter("string", "returnUrl");
                    });

                    if (!outputTarget.ExecutionContext.Settings.GetAuthenticationType().Authentication().IsOidc())
                    {
                        @interface.AddMethod("Task<string>", "ConfirmEmail", method =>
                        {
                            method.Async();
                            method.AddParameter("string?", "userId");
                            method.AddParameter("string?", "code");
                        });

                        @interface.AddMethod("Task", "ForgotPassword", method =>
                        {
                            method.Async();
                            method.AddParameter("string", "email");
                        });

                        @interface.AddMethod("Task", "Register", method =>
                        {
                            method.Async();
                            method.AddParameter("string", "email");
                            method.AddParameter("string", "password");
                            method.AddParameter("string", "returnUrl");
                        });

                        @interface.AddMethod("Task", "ResendEmailConfirmation", method =>
                        {
                            method.Async();
                            method.AddParameter("string", "email");
                        });

                        @interface.AddMethod("Task", "ResetPassword", method =>
                        {
                            method.Async();
                            method.AddParameter("string", "email");
                            method.AddParameter("string", "code");
                            method.AddParameter("string", "password");
                        });
                    }
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