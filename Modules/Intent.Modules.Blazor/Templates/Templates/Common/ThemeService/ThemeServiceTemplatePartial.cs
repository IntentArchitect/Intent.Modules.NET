using System;
using System.Collections.Generic;
using Intent.Engine;
using Intent.Modules.Blazor.Settings;
using Intent.Modules.Blazor.Templates.Templates.Client.Program;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.AppStartup;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.DependencyInjection;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Registrations;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;
using Intent.Utils;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.Blazor.Templates.Templates.Common.ThemeService
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class ThemeServiceTemplate : CSharpTemplateBase<object>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.Blazor.Templates.Common.ThemeServiceTemplate";
        private readonly IApplication _application;

        [IntentManaged(Mode.Merge, Body = Mode.Ignore)]
        public ThemeServiceTemplate(IOutputTarget outputTarget, IApplication application, object model = null) : base(TemplateId, outputTarget, model)
        {
            _application = application;

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

            if (isWasm)
            {
                RegisterInClientProgram();
            }

            RegisterInServerStartup();
        }

        private void RegisterInServerStartup()
        {
            var startup = _application.FindTemplateInstance<IAppStartupTemplate>(IAppStartupTemplate.RoleName);

            if (startup == null)
            {
                Logging.Log.Warning("Unable to install ThemeService. Startup class could not be found.");
                return;
            }

            startup.CSharpFile.AfterBuild(file =>
            {
                startup.StartupFile.ConfigureServices((statements, context) =>
                {
                    var addThemeService = new CSharpInvocationStatement($"{context.Services}.AddScoped<{startup.UseType(GetFullyQualifiedTypeName(Id))}>");
                    statements.AddStatement(addThemeService);
                });
            });
        }

        private void RegisterInClientProgram()
        {
            var program = _application.FindTemplateInstance<IBlazorProgramTemplate>(ProgramTemplate.TemplateId);

            if (program == null)
            {
                Logging.Log.Warning("Unable to install ThemeService. Program class could not be found.");
                return;
            }

            program.CSharpFile.AfterBuild(_ =>
            {
                program.ProgramFile.ConfigureMainStatementsBlock(main =>
                {
                    main.FindStatement(x => x.HasMetadata("run-builder"))
                        ?.InsertAbove(new CSharpInvocationStatement($"builder.Services.AddSingleton<{program.UseType(GetFullyQualifiedTypeName(Id))}>").SeparatedFromNext());
                });
            });
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

        public override bool CanRunTemplate()
        {
            return base.CanRunTemplate() && ExecutionContext.Settings.GetBlazor().EnableThemeToggle();
        }
    }
}