using System;
using System.Collections.Generic;
using Intent.Engine;
using Intent.Modules.Application.Identity.Settings;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.Application.Identity.Templates.CurrentUserServiceInterface
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public partial class CurrentUserServiceInterfaceTemplate : CSharpTemplateBase<object>, ICSharpFileBuilderTemplate
    {
        [IntentManaged(Mode.Fully)]
        public const string TemplateId = "Intent.Application.Identity.CurrentUserServiceInterface";

        [IntentManaged(Mode.Ignore, Signature = Mode.Fully)]
        public CurrentUserServiceInterfaceTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddUsing("System.Threading.Tasks")
                .AddInterface($"ICurrentUserService", @interface =>
                {
                    string userIdType = ExecutionContext.Settings.GetIdentitySettings().UserIdType().ToCSharpType();
                    @interface.AddProperty($"{this.UseType(userIdType)}?", "UserId", p => p.ReadOnly());
                    @interface.AddProperty("string?", "UserName", p => p.ReadOnly());
                    @interface.AddMethod("Task<bool>", "IsInRoleAsync", method =>
                    {
                        method.AddParameter("string", "role");
                    });
                    @interface.AddMethod("Task<bool>", "AuthorizeAsync", method =>
                    {
                        method.AddParameter("string", "policy");
                    });
                });
        }

        [IntentManaged(Mode.Fully)]
        public CSharpFile CSharpFile { get; }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        protected override CSharpFileConfig DefineFileConfig()
        {
            return new CSharpFileConfig(
                className: $"ICurrentUserService",
                @namespace: $"{OutputTarget.GetNamespace()}");
        }

        [IntentManaged(Mode.Fully)]
        public override string TransformText()
        {
            return CSharpFile.ToString();
        }

    }
}