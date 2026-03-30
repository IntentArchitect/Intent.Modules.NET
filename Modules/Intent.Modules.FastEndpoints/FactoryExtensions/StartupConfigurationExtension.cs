using System;
using System.Linq;
using Intent.Engine;
using Intent.Exceptions;
using Intent.IArchitect.Common.Exceptions;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.AppStartup;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Plugins;
using Intent.Modules.Constants;
using Intent.Modules.FastEndpoints.Templates;
using Intent.Plugins.FactoryExtensions;
using Intent.RoslynWeaver.Attributes;
using FriendlyException = Intent.Exceptions.FriendlyException;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.FactoryExtension", Version = "1.0")]

namespace Intent.Modules.FastEndpoints.FactoryExtensions
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class StartupConfigurationExtension : FactoryExtensionBase
    {
        public override string Id => "Intent.FastEndpoints.StartupConfigurationExtension";

        [IntentManaged(Mode.Ignore)]
        public override int Order => 0;

        protected override void OnAfterTemplateRegistrations(IApplication application)
        {
            var startupTemplate = application.FindTemplateInstance<IAppStartupTemplate>(IAppStartupTemplate.RoleName);
            var programTemplate = application.FindTemplateInstance<IProgramTemplate>(TemplateRoles.Distribution.WebApi.Program);

            startupTemplate.AddUsing("Mode = Intent.RoslynWeaver.Attributes.Mode");

            if (HasIncompatibleSwashbuckleInstalled(startupTemplate))
            {
                throw new FriendlyException("FastEndpoints is incompatible with `Intent.Swashbuckle.AspNetCore` v6+ and `Intent.AspNetCore.Swashbuckle.Security` v5+. Please revert to a compatible Swashbuckle module versions, or switch to an alternative OpenAPI documentation module such as `Intent.AspNetCore.Scalar`.");
            }

            startupTemplate?.CSharpFile.OnBuild(_ =>
            {
                var startup = startupTemplate.StartupFile;

                startupTemplate.AddUsing("FastEndpoints");
                startup.AddServiceConfiguration(c =>
                {
                    var statement = new CSharpInvocationStatement($"{c.Services}.AddFastEndpoints");
                    if (programTemplate is not null)
                    {
                        statement.AddArgument(new CSharpLambdaBlock("opt")
                            .AddStatement("opt.Assemblies = [typeof(Program).Assembly];"));
                    }
                    statement.AddMetadata("fast-endpoints", "main");
                    return statement;
                });

                if (HasSwashbuckleInstalled(startupTemplate))
                {
                    startupTemplate.AddNugetDependency(NugetPackages.FastEndpointsSwaggerSwashbuckle(startupTemplate.OutputTarget));
                    startupTemplate.AddUsing("FastEndpoints.ApiExplorer");
                    startup.AddServiceConfiguration(c => $"{c.Services}.AddFastEndpointsApiExplorer();");
                }

                startup.AddUseEndpointsStatement(
                    create: context => new CSharpInvocationStatement($"{context.Endpoints}.MapFastEndpoints")
                    .AddArgument(new CSharpLambdaBlock("c")
                        .WithExpressionBody(
                            new CSharpAssignmentStatement("c.Endpoints.Configurator",
                                new CSharpLambdaBlock("ep")
                                    .AddStatement($"ep.PostProcessors(0, new {startupTemplate.GetExceptionProcessorTemplateName()}());"))))
                    .AddMetadata("configure-endpoints-map-fast-endpoints", true),
                    configure: (s, _) => s.AddMetadata("configure-endpoints-controllers-generic", true));
            }, 10);
        }

        private static bool HasSwashbuckleInstalled(ICSharpTemplate template)
        {
            return template.ExecutionContext.FindTemplateInstances("Distribution.SwashbuckleConfiguration")?.Any() == true;
        }

        private static bool HasIncompatibleSwashbuckleInstalled(ICSharpTemplate template)
        {
            return template.ExecutionContext.InstalledModules.Any(m => m.ModuleId == "Intent.Swashbuckle.AspNetCore"
                && GetMajorVersion(m.Version) >= 6) ||
                template.ExecutionContext.InstalledModules.Any(m => m.ModuleId == "Intent.AspNetCore.Swashbuckle.Security"
                && GetMajorVersion(m.Version) >= 5);
        }

        private static int GetMajorVersion(string version)
        {
            if (string.IsNullOrWhiteSpace(version))
            {
                return 0;
            }

            var parts = version.Split('.');
            if (parts.Length > 0 && int.TryParse(parts[0], out var majorVersion))
            {
                return majorVersion;
            }

            return 0;
        }
    }

}