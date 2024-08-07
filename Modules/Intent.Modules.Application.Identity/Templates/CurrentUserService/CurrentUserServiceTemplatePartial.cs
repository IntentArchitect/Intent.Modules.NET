using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Modules.Application.Identity.Settings;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.DependencyInjection;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.Application.Identity.Templates.CurrentUserService;

[IntentManaged(Mode.Fully, Body = Mode.Merge)]
public partial class CurrentUserServiceTemplate : CSharpTemplateBase<object>, ICSharpFileBuilderTemplate
{
    public const string TemplateId = "Intent.Application.Identity.CurrentUserService";

    [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
    public CurrentUserServiceTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
    {
        FulfillsRole("Security.CurrentUserService");

        CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
            .AddUsing("System.Threading.Tasks")
            .AddClass($"CurrentUserService")
            .OnBuild(file =>
            {
                var priClass = file.Classes.First();
                priClass.ImplementsInterface(this.GetCurrentUserServiceInterfaceName());
                priClass.AddConstructor();
                string userIdType = ExecutionContext.Settings.GetIdentitySettings().UserIdType().ToCSharpType();
                priClass.AddProperty($"{this.UseType(userIdType)}?", "UserId");
                priClass.AddProperty("string?", "UserName");
                priClass.AddMethod($"Task<bool>", "AuthorizeAsync", method =>
                {
                    method.Async();
                    method.AddParameter("string", "policy");
                    method.AddStatement("return await Task.FromResult(true);");
                });
                priClass.AddMethod($"Task<bool>", "IsInRoleAsync", method =>
                {
                    method.Async();
                    method.AddParameter("string", "role");
                    method.AddStatement("return await Task.FromResult(true);");
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