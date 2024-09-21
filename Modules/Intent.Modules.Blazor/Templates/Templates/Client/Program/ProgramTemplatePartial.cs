using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.Common.Configuration;
using Intent.Modules.Common.CSharp.AppStartup;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Configuration;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.CSharp.VisualStudio;
using Intent.Modules.Common.VisualStudio;
using Intent.Modules.Constants;
using Intent.Modules.VisualStudio.Projects.Api;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.Blazor.Templates.Templates.Client.Program
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge, Signature = Mode.Ignore)]
    public partial class ProgramTemplate : CSharpTemplateBase<object>, IBlazorProgramTemplate
    {
        public const string TemplateId = "Intent.Blazor.Templates.Client.ProgramTemplate";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public ProgramTemplate(IOutputTarget outputTarget, object? model = null) : base(TemplateId, outputTarget, model)
        {
            FulfillsRole(TemplateRoles.Blazor.WebAssembly.Program);
            AddNugetDependency(NuGetPackages.MicrosoftAspNetCoreComponentsWebAssembly(outputTarget));

            var useTopLevelStatements = OutputTarget.GetProject().InternalElement.AsCSharpProjectNETModel()?.GetNETSettings()?.UseTopLevelStatements() == true;

            ProgramFile = new BlazorProgramFile(this, useTopLevelStatements);
            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddUsing("System")
                .AddUsing("System.Net.Http")
                .AddUsing("System.Threading.Tasks")
                .AddUsing("Microsoft.Extensions.Configuration")
                .AddUsing("Microsoft.Extensions.DependencyInjection")
                .AddUsing("Microsoft.AspNetCore.Components.WebAssembly.Hosting");

            if (useTopLevelStatements)
            {
                CSharpFile
                    .AddTopLevelStatements(tls =>
                    {
                        AddMainStatements(tls);

                        tls.AddLocalMethod("Task", "LoadAppSettings", method =>
                        {
                            method.Async()
                                .Static()
                                .AddParameter("WebAssemblyHostBuilder", "builder");

                            AddLoadAppSettingsStatements(method);
                        });
                    });
            }
            else
            {
                CSharpFile
                    .AddClass($"Program", @class =>
                    {
                        @class.AddMethod("Task", "Main", method =>
                        {
                            method.Async()
                                .Static()
                                .AddParameter("string[]", "args");
                            AddMainStatements(method);
                        });

                        @class.AddMethod("Task", "LoadAppSettings", method =>
                        {
                            method.Async()
                                .Static()
                                .AddParameter("WebAssemblyHostBuilder", "builder");

                            AddLoadAppSettingsStatements(method);
                        });
                    });
            }

            ExecutionContext.EventDispatcher.Subscribe<HostingSettingsCreatedEvent>(x =>
            {
                var profileName = OutputTarget.GetProject().ApplicationName() + ".Client";
                ExecutionContext.EventDispatcher.Publish(new LaunchProfileRegistrationRequest
                {
                    ForProjectWithRole = "Distribution",
                    Name = profileName,
                    CommandName = "Project",
                    LaunchBrowser = true,
                    ApplicationUrl = $"https://localhost:{x.SslPort}/",
                    LaunchUrl = $"https://localhost:{x.SslPort}",
                    PublishAllPorts = false,
                    EnvironmentVariables = new Dictionary<string, string> { { "ASPNETCORE_ENVIRONMENT", "Development" } }
                });
            });
        }

        private void AddMainStatements(IHasCSharpStatements hasStatements)
        {
            hasStatements.AddStatement("var builder = WebAssemblyHostBuilder.CreateDefault(args);");
            if (OutputTarget.GetMaxNetAppVersion().Major <= 7)
            {
                hasStatements.AddStatement("builder.RootComponents.Add<App>(\"#app\");");
                hasStatements.AddStatement("builder.RootComponents.Add<HeadOutlet>(\"head::after\");");
            }

            hasStatements.AddStatement("await LoadAppSettings(builder);");
            hasStatements.AddStatement("builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });");
            hasStatements.AddStatement("builder.Services.AddClientServices(builder.Configuration);");
            hasStatements.AddStatement("await builder.Build().RunAsync();", stmt => stmt.AddMetadata("run-builder", "true"));
        }

        private static void AddLoadAppSettingsStatements(IHasCSharpStatements hasStatements)
        {
            hasStatements.AddStatement("var configProxy = new HttpClient() { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) };");
            hasStatements.AddStatement("using var response = await configProxy.GetAsync(\"appsettings.json\");");
            hasStatements.AddStatement("using var stream = await response.Content.ReadAsStreamAsync();");
            hasStatements.AddStatement("builder.Configuration.AddJsonStream(stream);");
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

        public IBlazorProgramFile ProgramFile { get; }
    }
}