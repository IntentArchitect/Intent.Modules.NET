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

namespace Intent.Modules.Blazor.JwtAuth.Templates.AuthServiceInterface
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class AuthServiceInterfaceTemplate : CSharpTemplateBase<object>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.Blazor.JwtAuth.AuthServiceInterface";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public AuthServiceInterfaceTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddInterface($"IAuthService", @interface =>
                {
                    @interface.AddMethod("void", "Register", method =>
                    {
                        method.Async();
                        method.AddParameter("string", "username");
                        method.AddParameter("string", "password");
                    });

                    @interface.AddMethod("bool", "Login", method =>
                    {
                        method.Async();
                        method.AddParameter("string", "username");
                        method.AddParameter("string", "password");
                    });

                    @interface.AddMethod("void", "Logout", method =>
                    {
                        method.Async();
                    });

                    @interface.AddMethod("string?", "GetAccessTokenAsync", method =>
                    {
                        method.Async();
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