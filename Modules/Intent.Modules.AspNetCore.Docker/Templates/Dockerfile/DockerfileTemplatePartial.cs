using System.Collections.Generic;
using Intent.Modules.Common.Plugins;
using Intent.Modules.Common.Templates;
using Intent.Modules.Common.VisualStudio;
using Intent.Modules.Constants;
using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Configuration;
using Intent.Modules.Common.CSharp.VisualStudio;
using Intent.Templates;
using Intent.Utils;
using System;

namespace Intent.Modules.AspNetCore.Docker.Templates.DockerFile
{
    partial class DockerfileTemplate : IntentFileTemplateBase<object>, ITemplate, IHasNugetDependencies, ITemplateBeforeExecutionHook
    {
        public const string Identifier = "Intent.AspNetCore.Dockerfile";

        private string _defaultLaunchUrlPath;
        private readonly string _sdkVersion;

        public DockerfileTemplate(IProject project)
            : base(Identifier, project, null)
        {
            ExecutionContext.EventDispatcher.Subscribe<DefaultLaunchUrlPathRequest>(Handle);

            _sdkVersion = Project.TryGetMaxNetAppVersion(out var netVersion) switch
            {
                false when Project.IsNetCore2App() => "2.1",
                false when Project.IsNetCore3App() => "3.1",
                true => netVersion.ToString(),
                _ => throw new InvalidOperationException(
                    "Project .NET version not supported by this Docker module, only .NET Core and .NET 5+ are supported.")
            };
        }

        private void Handle(DefaultLaunchUrlPathRequest request)
        {
            _defaultLaunchUrlPath = request.UrlPath;
        }

        public IEnumerable<INugetPackageInfo> GetNugetDependencies()
        {
            yield return NugetPackages.MicrosoftVisualStudioAzureContainersToolsTargets;
        }

        public override ITemplateFileConfig GetTemplateFileConfig()
        {
            return new TemplateFileConfig(
                overwriteBehaviour: OverwriteBehaviour.OverwriteDisabled,
                codeGenType: CodeGenType.Basic,
                fileName: "Dockerfile",
                fileExtension: "",
                relativeLocation: ""
                );
        }

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
