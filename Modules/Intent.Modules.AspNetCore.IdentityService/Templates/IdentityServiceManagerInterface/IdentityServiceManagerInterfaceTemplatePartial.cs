using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Exceptions;
using Intent.Modelers.Services.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.Constants;
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
                .AddUsing("System.Security.Claims")
                .AddInterface($"IIdentityServiceManager", @interface =>
                {
                    var dtoModels = this.ExecutionContext.MetadataManager.GetDesigner(this.ExecutionContext.GetApplicationConfig().Id, Designers.Services).GetDTOModels();
                    var loginRequestDto = dtoModels.FirstOrDefault(x => x.Name == "LoginRequestDto");

                    if (loginRequestDto is null)
                    {
                        var package = this.ExecutionContext.MetadataManager.GetDesigner(this.ExecutionContext.GetApplicationConfig().Id, Designers.Services).Packages.FirstOrDefault();
                        if (package is null)
                        {
                            throw new Exception("No package found. Please create a package and uninstall and re-install the Intent.AspNetCore.IdentityService module.");
                        }
                        throw new ElementException(package, "No LoginRequestDto found. Please uninstall and re-install the Intent.AspNetCore.IdentityService module.");
                    }

                    var typeName = GetFullyQualifiedTypeName("Intent.Application.Dtos.DtoModel", loginRequestDto);
                    UseType(typeName);

                    @interface
                    .AddMethod("Task", "Register", method =>
                    {
                        method.Async();
                        method.AddParameter("RegisterRequestDto", "registration");
                    })
                    .AddMethod("AccessTokenResponseDto", "Login", method =>
                    {
                        method.Async();
                        method.AddParameter("LoginRequestDto", "login");
                        method.AddParameter("bool?", "useCookies");
                        method.AddParameter("bool?", "useSessionCookies");
                    })
                    .AddMethod("AccessTokenResponseDto", "Refresh", method =>
                    {
                        method.Async();
                        method.AddParameter("RefreshRequestDto", "refreshRequest");
                    })
                    .AddMethod("string", "ConfirmEmail", method =>
                    {
                        method.Async();
                        method.AddParameter("string", "userId");
                        method.AddParameter("string", "code");
                        method.AddParameter("string?", "changedEmail");
                    })
                    .AddMethod("bool", "ResendConfirmationEmail", method =>
                    {
                        method.Async();
                        method.AddParameter("ResendConfirmationEmailRequestDto", "resendRequest");
                    })
                    .AddMethod("Task", "ForgotPassword", method =>
                    {
                        method.Async();
                        method.AddParameter("ForgotPasswordRequestDto", "resetRequest");
                    })
                    .AddMethod("Task", "ResetPassword", method =>
                    {
                        method.Async();
                        method.AddParameter("ResetPasswordRequestDto", "resetPasswordRequest");
                    })
                    .AddMethod("TwoFactorResponseDto", "UpdateTwoFactor", method =>
                    {
                        method.Async();
                        method.AddParameter("TwoFactorRequestDto", "tfaRequest");
                    })
                    .AddMethod("InfoResponseDto", "GetInfo", method =>
                    {
                        method.Async();
                    })
                    .AddMethod("InfoResponseDto", "UpdateInfo", method =>
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