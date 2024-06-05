using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Configuration;
using Intent.Modules.Common.CSharp.DependencyInjection;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.CSharp.VisualStudio;
using Intent.Modules.Common.Templates;
using Intent.Modules.Common.VisualStudio;
using Intent.Modules.Constants;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.Blazor.Templates.Templates.Client.Program
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class ProgramTemplate : CSharpTemplateBase<object>, ICSharpFileBuilderTemplate
    {
        private readonly IList<ContainerRegistrationRequest> _containerRegistrationRequests = new List<ContainerRegistrationRequest>();
        private readonly IList<ServiceConfigurationRequest> _serviceConfigurationRequests = new List<ServiceConfigurationRequest>();

        public const string TemplateId = "Intent.Blazor.Templates.Client.ProgramTemplate";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public ProgramTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            FulfillsRole(TemplateRoles.Blazor.WebAssembly.Program);
            AddNugetDependency(NuGetPackages.MicrosoftAspNetCoreComponentsWebAssembly(outputTarget));
            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddUsing("Microsoft.AspNetCore.Components.Web")
                .AddUsing("Microsoft.AspNetCore.Components.WebAssembly.Hosting")
                .AddClass($"Program", @class =>
                {
                    @class.AddMethod("Task", "Main", method =>
                    {
                        method.Async()
                            .Static()
                            .AddParameter("string[]", "args");
                        method.AddStatement("var builder = WebAssemblyHostBuilder.CreateDefault(args);");
                        if (outputTarget.GetMaxNetAppVersion().Major <= 7)
                        {
                            method.AddStatement("builder.RootComponents.Add<App>(\"#app\");");
                            method.AddStatement("builder.RootComponents.Add<HeadOutlet>(\"head::after\");");
                        }

                        method.AddStatement("await LoadAppSettings(builder);");
                        method.AddStatement("builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });");
                        method.AddStatement("builder.Services.AddClientServices(builder.Configuration);");
                        method.AddStatement("await builder.Build().RunAsync();", stmt => stmt.AddMetadata("run-builder", "true"));
                    });

                    @class.AddMethod("Task", "LoadAppSettings", method =>
                    {
                        method.Async()
                            .Static()
                            .AddParameter("WebAssemblyHostBuilder", "builder");

                        method.AddStatement("var configProxy = new HttpClient() { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) };");
                        method.AddStatement("using var response = await configProxy.GetAsync(\"appsettings.json\");");
                        method.AddStatement("using var stream = await response.Content.ReadAsStreamAsync();");
                        method.AddStatement("builder.Configuration.AddJsonStream(stream);");
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