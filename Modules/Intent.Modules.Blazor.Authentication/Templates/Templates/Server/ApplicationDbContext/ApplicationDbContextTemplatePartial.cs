using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Modules.Blazor.Authentication.Settings;
using Intent.Modules.Blazor.Authentication.Templates.Templates.Server.ApplicationUser;
using Intent.Modules.Blazor.Settings;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.Common.VisualStudio;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.Blazor.Authentication.Templates.Templates.Server.ApplicationDbContext
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class ApplicationDbContextTemplate : CSharpTemplateBase<object>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.Blazor.Authentication.Templates.Server.ApplicationDbContextTemplate";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public ApplicationDbContextTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            AddNugetDependency(NugetPackages.MicrosoftAspNetCoreIdentityEntityFrameworkCore(outputTarget));
            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddUsing("Microsoft.EntityFrameworkCore")
                .AddClass($"ApplicationDbContext", @class =>
                {
                    AddNugetDependency(NugetPackages.MicrosoftEntityFrameworkCoreSqlServer(outputTarget));

                    @class.WithBaseType($"{UseType("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityDbContext")}<{GetTypeName(ApplicationUserTemplate.TemplateId)}>");
                    @class.AddConstructor(ctor =>
                    {
                        ctor.AddParameter("DbContextOptions", "options");
                        ctor.CallsBase(c => c.AddArgument("options"));
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
            if (ExecutionContext.InstalledModules.Any(im => im.ModuleId == "Intent.AspNetCore.Identity"))
            {
                return false;
            }
            return base.CanRunTemplate() && ExecutionContext.GetSettings().GetBlazor().Authentication().IsAspnetcoreIdentity();
        }
    }
}