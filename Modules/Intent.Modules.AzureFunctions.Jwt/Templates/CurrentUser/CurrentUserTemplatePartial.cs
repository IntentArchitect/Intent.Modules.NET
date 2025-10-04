using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Modules.Application.Identity.Templates;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.AzureFunctions.Jwt.Templates.CurrentUser
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class CurrentUserTemplate : CSharpTemplateBase<object>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.AzureFunctions.Jwt.CurrentUser";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public CurrentUserTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddUsing("System.Linq")
                .AddUsing("System.Security.Claims")
                .AddClass($"CurrentUser", @class =>
                {
                    @class.ImplementsInterface(this.GetCurrentUserInterfaceName());
                    
                    @class.AddConstructor(ctor =>
                    {
                        ctor.AddParameter("ClaimsPrincipal", "principal", param =>
                        {
                            param.IntroduceProperty(prop => prop.ReadOnly());
                        });
                        ctor.AddParameter("string?", "accessToken", param =>
                        {
                            param.IntroduceProperty(prop => prop.ReadOnly());
                        });
                    });

                    @class.AddProperty("string?", "Id", prop =>
                    {
                        prop.Getter.WithExpressionImplementation(@"Principal.FindFirst(ClaimTypes.NameIdentifier)?.Value");
                        prop.WithoutSetter();
                    });

                    @class.AddProperty("string?", "Name", prop =>
                    {
                        prop.Getter.WithExpressionImplementation(@"Principal.FindFirst(ClaimTypes.Name)?.Value");
                        prop.WithoutSetter();
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