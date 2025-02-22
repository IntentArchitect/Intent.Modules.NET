using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Configuration;
using Intent.Modules.Common.CSharp.VisualStudio;
using Intent.Modules.Common.Templates;
using Intent.Modules.Common.VisualStudio;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.ProjectItemTemplate.Partial", Version = "1.0")]

namespace Intent.Modules.AspNetCore.Docker.Templates.Dockerfile
{
    [IntentManaged(Mode.Merge)]
    partial class DockerfileTemplate : IntentTemplateBase<object>, IHasNugetDependencies
    {
        private string _defaultLaunchUrlPath;
        private readonly string _sdkVersion;

        [IntentManaged(Mode.Fully)]
        public const string TemplateId = "Intent.AspNetCore.Docker.Dockerfile";

        [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
        public DockerfileTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            ExecutionContext.EventDispatcher.Subscribe<DefaultLaunchUrlPathRequest>(Handle);
            Project = outputTarget.GetProject();

            _sdkVersion = Project.TryGetMaxNetAppVersion(out var netVersion) switch
            {
                false when Project.IsNetCore2App() => "2.1",
                false when Project.IsNetCore3App() => "3.1",
                true => netVersion.ToString(),
                _ => throw new InvalidOperationException(
                    "Project .NET version not supported by this Docker module, only .NET Core and .NET 5+ are supported.")
            };
        }

        public ICSharpProject Project { get; }

        public override void BeforeTemplateExecution()
        {
            base.BeforeTemplateExecution();

            var environmentVariables = new Dictionary<string, string>
            {
                { "ASPNETCORE_HTTPS_PORTS", "8081" },
                { "ASPNETCORE_HTTP_PORTS", "8080" }
            };
            ExecutionContext.EventDispatcher.Publish(new LaunchProfileRegistrationRequest
            {
                Name = "Docker",
                CommandName = "Docker",
                LaunchBrowser = true,
                LaunchUrl = $"{{Scheme}}://{{ServiceHost}}:{{ServicePort}}{AddDefaultLaunchUrl()}",
                EnvironmentVariables = environmentVariables,
                PublishAllPorts = true,
                UseSsl = true,
            });
            ExecutionContext.EventDispatcher.Publish(new AddProjectPropertyEvent(Project, "DockerDefaultTargetOS", "Linux"));

            //Make sure the file exists
            ExecutionContext.EventDispatcher.Publish(new AddUserSecretsEvent(Project, new Dictionary<string, string>()));
        }

        public IEnumerable<INugetPackageInfo> GetNugetDependencies()
        {
            yield return NugetPackages.MicrosoftVisualStudioAzureContainersToolsTargets;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public override ITemplateFileConfig GetTemplateFileConfig()
        {
            return new TemplateFileConfig(
                fileName: $"Dockerfile",
                fileExtension: "",
                overwriteBehaviour: OverwriteBehaviour.OverwriteDisabled
            );
        }

        private void Handle(DefaultLaunchUrlPathRequest request)
        {
            _defaultLaunchUrlPath = request.UrlPath;
        }

        private string AddDefaultLaunchUrl()
        {
            if (string.IsNullOrWhiteSpace(_defaultLaunchUrlPath))
            {
                return string.Empty;
            }

            return $"/{_defaultLaunchUrlPath}";
        }

        private string GetRuntime()
        {
            return $"mcr.microsoft.com/dotnet/aspnet:{_sdkVersion}";
        }

        private string GetSdk()
        {
            return $"mcr.microsoft.com/dotnet/sdk:{_sdkVersion}";
        }
    }
}