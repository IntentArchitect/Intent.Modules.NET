using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Modules.Blazor.Authentication.FactoryExtensions;
using Intent.Modules.Blazor.Authentication.Settings;
using Intent.Modules.Blazor.Authentication.Templates.Templates.Server.ApplicationUser;
using Intent.Modules.Blazor.Settings;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.Blazor.Authentication.Templates.Templates.Server.IdentityUserAccessor
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class IdentityUserAccessorTemplate : CSharpTemplateBase<object>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.Blazor.Authentication.Templates.Server.IdentityUserAccessorTemplate";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public IdentityUserAccessorTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddUsing("Microsoft.AspNetCore.Identity")
                .AddUsing("Microsoft.AspNetCore.Http")
                .AddUsing("System.Threading.Tasks")
                .AddClass($"IdentityUserAccessor", @class =>
                {
                    var identityUserName = string.Empty;
                    if (ExecutionContext.InstalledModules.Any(im => im.ModuleId == "Intent.AspNetCore.Identity"))
                    {
                        identityUserName = IdentityHelperExtensions.GetIdentityUserClass(this);
                    }
                    else
                    {
                        identityUserName = GetTypeName(ApplicationUserTemplate.TemplateId);
                    }
                    @class.Internal().Sealed();
                    @class.AddConstructor(ctor =>
                    {

                        ctor.AddParameter($"UserManager<{identityUserName}>", "userManager", param =>
                        {
                            param.IntroduceReadonlyField();
                        });
                        ctor.AddParameter("IdentityRedirectManager", "redirectManager", param =>
                        {
                            param.IntroduceReadonlyField();
                        });
                    });

                    @class.AddMethod($"Task<{identityUserName}>", "GetRequiredUserAsync", getRequiredUserAsync =>
                    {
                        getRequiredUserAsync.Async();

                        getRequiredUserAsync.AddParameter("HttpContext", "context");

                        getRequiredUserAsync.AddStatement("var user = await _userManager.GetUserAsync(context.User);");

                        getRequiredUserAsync.AddIfStatement("user is null", @if =>
                        {
                            @if.AddStatement("_redirectManager.RedirectToWithStatus(\"Account/InvalidUser\", $\"Error: Unable to load user with ID '{_userManager.GetUserId(context.User)}'.\", context);");
                        });

                        getRequiredUserAsync.AddReturn("user");
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

        public override bool CanRunTemplate()
        {
            return base.CanRunTemplate() && ExecutionContext.GetSettings().GetBlazor().Authentication().IsAspnetcoreIdentity();
        }
    }
}