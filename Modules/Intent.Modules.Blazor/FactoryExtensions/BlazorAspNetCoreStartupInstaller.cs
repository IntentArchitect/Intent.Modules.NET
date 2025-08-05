using System;
using System.Linq;
using Intent.Engine;
using Intent.Exceptions;
using Intent.Modules.Blazor.Settings;
using Intent.Modules.Blazor.Templates;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp;
using Intent.Modules.Common.CSharp.AppStartup;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.Plugins;
using Intent.Modules.Common.VisualStudio;
using Intent.Plugins.FactoryExtensions;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.FactoryExtension", Version = "1.0")]

namespace Intent.Modules.Blazor.FactoryExtensions
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class BlazorAspNetCoreStartupInstaller : FactoryExtensionBase
    {
        public override string Id => "Intent.Blazor.BlazorAspNetCoreStartupInstaller";

        [IntentManaged(Mode.Ignore)]
        public override int Order => 0;

        protected override void OnBeforeTemplateRegistrations(IApplication application)
        {
            base.OnBeforeTemplateRegistrations(application);
            CheckBlazorWasmInstalled(application);
            ConfigureRazorWeaving(application);
        }

        private void CheckBlazorWasmInstalled(IApplication application)
        {
            if (!application.GetSettings().GetBlazor().RenderMode().IsInteractiveServer() && !application.InstalledModules.Any(m => m.ModuleId == "Intent.Blazor.Wasm"))
            {
                throw new FriendlyException($"For Render Module : {application.GetSettings().GetBlazor().RenderMode().AsEnum()}, please install the `Intent.Blazor.Wasm` module");
            }
        }

        private void ConfigureRazorWeaving(IApplication application)
        {
            application.ConfigureRazor(configurator =>
            {
                configurator.AddTagNameAttributeMatch("link", "href");
                configurator.AddTagNameAttributeMatch("script", "src");
            });
        }

        [IntentIgnore]
        protected override void OnAfterTemplateRegistrations(IApplication application)
        {
            base.OnAfterTemplateRegistrations(application);
            RegisterStartup(application);
        }

        private void RegisterStartup(IApplication application)
        {
            var startup = application.FindTemplateInstance<IAppStartupTemplate>(IAppStartupTemplate.RoleName);
            startup?.AddNugetDependency(NugetPackages.MicrosoftAspNetCoreComponentsWebAssemblyServer(startup.OutputTarget));

            startup?.CSharpFile.AfterBuild(file =>
            {
                startup.StartupFile.ConfigureServices((statements, context) =>
                {
                    var addRazorComponents = new CSharpMethodChainStatement($"{context.Services}.AddRazorComponents()");
                    if (startup.ExecutionContext.GetSettings().GetBlazor().RenderMode().IsInteractiveWebAssembly() || startup.ExecutionContext.GetSettings().GetBlazor().RenderMode().IsInteractiveAuto())
                    {
                        addRazorComponents.AddChainStatement("AddInteractiveWebAssemblyComponents()");
                    }
                    else if (startup.ExecutionContext.GetSettings().GetBlazor().RenderMode().IsInteractiveServer() || startup.ExecutionContext.GetSettings().GetBlazor().RenderMode().IsInteractiveAuto())
                    {
                        addRazorComponents.AddChainStatement("AddInteractiveServerComponents()");
                    }
                    addRazorComponents.WithSemicolon();
                    statements.AddStatement(addRazorComponents);
                });

                startup.StartupFile.ConfigureApp((statements, context) =>
                {
                    var ifDevelopmentStatement = (CSharpIfStatement)statements
                        .FindStatement(m => m is CSharpIfStatement cif && cif.GetText("").Contains("env.IsDevelopment()"));

                    if (ifDevelopmentStatement is not null)
                    {
                        ifDevelopmentStatement.InsertBelow(new CSharpElseStatement(), statement =>
                        {
                            var elseStatement = (CSharpElseStatement)statement;

                            elseStatement.AddStatement("app.UseExceptionHandler(\"/Error\");");
                            elseStatement.AddStatement("// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.");
                            elseStatement.AddStatement("app.UseHsts();");
                        });
                    }
                    else
                    {
                        statements.FindStatement(m => m.Text.StartsWith("app.UseExceptionHandler"))?.Remove();
                        var position = statements.FindStatement(m => m.Text.StartsWith("app.UseHttpsRedirection"));
                        var ifDevStatement = new CSharpIfStatement("!app.Environment.IsDevelopment()");
                        position.InsertBelow(ifDevStatement, statement => 
                        {
                            var ifs = (CSharpIfStatement)statement;
                            ifs.AddStatement("app.UseExceptionHandler(\"/Error\");");
                            ifs.AddStatement("// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.");
                            ifs.AddStatement("app.UseHsts();");
                        });
                        if (!startup.ExecutionContext.GetSettings().GetBlazor().RenderMode().IsInteractiveServer())
                        {
                            var elseStatement = new CSharpElseStatement();
                            elseStatement.AddStatement("app.UseWebAssemblyDebugging();");
                            ifDevStatement.InsertBelow(elseStatement);
                        }
                    }

                    statements.FindStatement(m => m.Text.StartsWith("app.UseEndpoints"))?
                        .InsertAbove("app.UseStaticFiles();")
                        .InsertAbove("app.UseAntiforgery();");
                });

                startup.StartupFile.ConfigureEndpoints((statements, context) =>
                {
                    
                    var addRazorComponents = new CSharpMethodChainStatement($"{context.Endpoints}.MapRazorComponents<{startup.GetAppRazorTemplateName()}>()");

                    if (startup.ExecutionContext.GetSettings().GetBlazor().RenderMode().IsInteractiveWebAssembly() || startup.ExecutionContext.GetSettings().GetBlazor().RenderMode().IsInteractiveAuto())
                    {
                        addRazorComponents.AddChainStatement("AddInteractiveWebAssemblyRenderMode()");
                    }
                    else if (startup.ExecutionContext.GetSettings().GetBlazor().RenderMode().IsInteractiveServer() || startup.ExecutionContext.GetSettings().GetBlazor().RenderMode().IsInteractiveAuto())
                    {
                        addRazorComponents.AddChainStatement("AddInteractiveServerRenderMode()");
                    }
                    if (!startup.ExecutionContext.GetSettings().GetBlazor().RenderMode().IsInteractiveServer())
                    {
                        addRazorComponents.AddChainStatement($"AddAdditionalAssemblies(typeof({startup.GetClientImportsRazorTemplateName()}).Assembly)");                            
                    }
                    addRazorComponents.WithSemicolon();
                    statements.AddStatement(addRazorComponents);
                });
            });
        }
    }
}