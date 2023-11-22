using System;
using System.Collections.Generic;
using Intent.Engine;
using Intent.Modules.Blazor.HttpClients.AccountController.Templates.AccountServiceHttpClient;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.Blazor.HttpClients.AccountController.Templates.AccountServiceInterface
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class AccountServiceInterfaceTemplate : CSharpTemplateBase<object>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.Blazor.HttpClients.AccountController.AccountServiceInterfaceTemplate";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public AccountServiceInterfaceTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddUsing("System.Threading.Tasks")
                .AddUsing("System.Threading")
                .AddInterface($"IAccountService", @class =>
                {
                    var implementationTemplate = GetTemplate<ICSharpFileBuilderTemplate>(AccountServiceHttpClientTemplate.TemplateId);
                    AddUsing(implementationTemplate.Namespace);

                    @class.AddMethod("Task", "Register", method =>
                    {
                        method.AddParameter("RegisterDto", "dto")
                            .AddParameter("CancellationToken", "cancellationToken", p => p.WithDefaultValue("default"));
                    });

                    @class.AddMethod("Task<TokenResultDto>", "Login", method =>
                    {
                        method.AddParameter("LoginDto", "dto")
                            .AddParameter("CancellationToken", "cancellationToken", p => p.WithDefaultValue("default"));
                    });

                    @class.AddMethod("Task<TokenResultDto>", "Refresh", method =>
                    {
                        method
                            .AddParameter("string", "refreshToken")
                            .AddParameter("CancellationToken", "cancellationToken", p => p.WithDefaultValue("default"));
                    });

                    @class.AddMethod("Task", "ConfirmEmail", method =>
                    {
                        method.AddParameter("ConfirmEmailDto", "dto")
                            .AddParameter("CancellationToken", "cancellationToken", p => p.WithDefaultValue("default"));
                    });

                    @class.AddMethod("Task", "Logout", method =>
                    {
                        method.AddParameter("CancellationToken", "cancellationToken", p => p.WithDefaultValue("default"));
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