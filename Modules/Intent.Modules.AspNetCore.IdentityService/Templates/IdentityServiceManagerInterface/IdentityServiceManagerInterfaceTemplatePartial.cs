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

namespace Intent.Modules.AspNetCore.IdentityService.Templates.IdentityServiceManagerInterface
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class IdentityServiceManagerInterfaceTemplate : CSharpTemplateBase<object>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.AspNetCore.IdentityService.IdentityServiceManagerInterface";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public IdentityServiceManagerInterfaceTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddUsing($"{this.GetNamespace().Replace(".Interfaces","")}.Identity")
                .AddUsing("System.Security.Claims")
                .AddInterface($"IIdentityServiceManager", @interface =>
                {
                    @interface.AddGenericParameter("TUser", out var tUser)
                    .AddGenericTypeConstraint(tUser, c => c
                            .AddType("class")
                            .AddType("new()"));

                    @interface
                    .AddMethod("Task", "RegisterAsync", method =>
                    {
                        method.Async();
                        method.AddParameter("RegisterRequestDto", "registration");
                    })
                    .AddMethod("Task", "LoginAsync", method =>
                    {
                        method.Async();
                        method.AddParameter("LoginRequestDto", "login");
                        method.AddParameter("bool?", "useCookies");
                        method.AddParameter("bool?", "useSessionCookies");
                    })
                    .AddMethod("ClaimsPrincipal", "RefreshAsync", method =>
                    {
                        method.Async();
                        method.AddParameter("RefreshRequestDto", "refreshRequest");
                    })
                    .AddMethod("string", "ConfirmEmailAsync", method =>
                    {
                        method.Async();
                        method.AddParameter("string", "userId");
                        method.AddParameter("string", "code");
                        method.AddParameter("string?", "changedEmail");
                    })
                    .AddMethod("bool", "ResendConfirmationEmailAsync", method =>
                    {
                        method.Async();
                        method.AddParameter("ResendConfirmationEmailRequestDto", "resendRequest");
                    })
                    .AddMethod("Task", "ForgotPasswordAsync", method =>
                    {
                        method.Async();
                        method.AddParameter("ForgotPasswordRequestDto", "resetRequest");
                    })
                    .AddMethod("Task", "ResetPasswordAsync", method =>
                    {
                        method.Async();
                        method.AddParameter("ResetPasswordRequestDto", "resetPasswordRequest");
                    })
                    .AddMethod("TwoFactorResponseDto", "UpdateTwoFactorAsync", method =>
                    {
                        method.Async();
                        method.AddParameter("TwoFactorRequestDto", "tfaRequest");
                    })
                    .AddMethod("InfoResponseDto", "GetInfoAsync", method =>
                    {
                        method.Async();
                    })
                    .AddMethod("InfoResponseDto", "UpdateInfoAsync", method =>
                    {
                        method.Async();
                        method.AddParameter("InfoRequestDto", "infoRequest");
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