using System;
using System.Collections.Generic;
using Intent.Engine;
using Intent.Modules.Blazor.Settings;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.DependencyInjection;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.Blazor.Templates.Templates.Common.ThemeService
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class ThemeServiceTemplate : CSharpTemplateBase<object>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.Blazor.Templates.Common.ThemeServiceTemplate";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public ThemeServiceTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath(), this)
                .AddClass("ThemeService", @class =>
                {
                    @class.AddProperty("bool", "IsDark", prop =>
                    {
                        prop.PrivateSetter();
                        prop.WithInitialValue("true");
                    });

                    @class.AddCodeBlock($"public event {UseType("System.Action")}? OnChange;");

                    @class.AddMethod("void", "Toggle", method =>
                    {
                        method.AddStatement("IsDark = !IsDark;");
                        method.AddInvocationStatement("OnChange?.Invoke");
                    });

                    @class.AddMethod("void", "SetDark", method =>
                    {
                        method.AddParameter("bool", "isDark");
                        method.AddIfStatement("IsDark == isDark", @if =>
                        {
                            @if.AddStatement("return;");
                        });
                        method.AddStatement("IsDark = isDark;");
                        method.AddInvocationStatement("OnChange?.Invoke");
                    });
                });
        }

        [IntentManaged(Mode.Fully)]
        public CSharpFile CSharpFile { get; }

        public override void BeforeTemplateExecution()
        {
            var isWasm = ExecutionContext.GetSettings().GetBlazor().RenderMode().IsInteractiveWebAssembly();

            var request = ContainerRegistrationRequest.ToRegister(this);
            request = isWasm
                ? request.WithSingletonLifeTime()
                : request.WithPerServiceCallLifeTime();

            ExecutionContext.EventDispatcher.Publish(request);
        }

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