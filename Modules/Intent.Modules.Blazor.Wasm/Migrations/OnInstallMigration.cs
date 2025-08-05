using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Intent.Engine;
using Intent.IArchitect.Agent.Persistence.Model;
using Intent.IArchitect.Agent.Persistence.Model.Common;
using Intent.Metadata;
using Intent.Plugins;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Migrations.OnInstallMigration", Version = "1.0")]

namespace Intent.Modules.Blazor.Wasm.Migrations
{
    public class OnInstallMigration : IModuleOnInstallMigration
    {
        private const string VisualStudioDesignerId = "0701433c-36c0-4569-b1f4-9204986b587d";
        private const string VisualStudioSolutionPackageSpecializationId = "07e7b690-a59d-4b72-8440-4308a121d32c";
        private const string RoleSpecializationId = "025e933b-b602-4b6d-95ab-0ec36ae940da";
        private const string TemplateOutputRoleSpecializationId = "d421c322-7a51-4094-89fa-e5d8a0a97b27";
        private const string ProjectSpecializationId = "8e9e6693-2888-4f48-a0d6-0f163baab740";
        private readonly IApplicationConfigurationProvider _configurationProvider;
        private readonly IMetadataInstaller _metadataInstaller;

        public OnInstallMigration(IApplicationConfigurationProvider configurationProvider, IMetadataInstaller metadataInstaller)
        {
            _configurationProvider = configurationProvider;
            _metadataInstaller = metadataInstaller;
        }


        [IntentFully]
        public string ModuleId => "Intent.Blazor.Wasm";

        public void OnInstall()
        {
            var app = ApplicationPersistable.Load(_configurationProvider.GetApplicationConfig().FilePath);
            if (app == null)
                return;

            var designer = app.GetDesigner(VisualStudioDesignerId);
            var packages = designer.GetPackages().Where(x => x.SpecializationTypeId == VisualStudioSolutionPackageSpecializationId);

            foreach (var package in packages)
            {
                var elements = package.GetElementsOfType(RoleSpecializationId);
                var blazorElement = elements.FirstOrDefault(x => x.Name == "Blazor");
                if (blazorElement == null)
                {
                    continue;
                }
                if (!IsClientProject(blazorElement))
                {
                    package.RemoveElement(blazorElement);
                    var templateRoles = package.GetElementsOfType(TemplateOutputRoleSpecializationId);
                    foreach (var templateRole in templateRoles)
                    {
                        if (templateRole.Name.StartsWith("Intent.Blazor.Templates.Client") || templateRole.Name.StartsWith("Intent.Blazor.HttpClients") || templateRole.Name.StartsWith("Intent.Modules.Blazor.Templates.Templates.Client"))
                        {
                            templateRole.Delete();
                        }
                    }
                }
                package.Save();
            }

            var visualStudioMetadataInstallationFile = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "../content", "visual-studio.metadata.config");
            if (File.Exists(visualStudioMetadataInstallationFile))
            {
                _metadataInstaller.InstallFromFile(app.AbsolutePath, visualStudioMetadataInstallationFile,
                    fieldDictionary: new Dictionary<string, string>
                    {
                        ["application.name"] = _configurationProvider.GetApplicationConfig().Name,
                        ["solution.name"] = _configurationProvider.GetSolutionConfig().SolutionName
                    }, s => { return Task.CompletedTask; });
            }
            else
            {
                throw new Exception("File not found: " + visualStudioMetadataInstallationFile);
            }
        }

        private bool IsClientProject(ElementPersistable blazorElement)
        {
            ElementPersistable parent = blazorElement.Parent as ElementPersistable;
            while (parent != null && parent.SpecializationTypeId != ProjectSpecializationId)// Project
            {
                parent = parent.Parent as ElementPersistable;
            }
            if (parent == null)
            {
                throw new Exception("Unable to find Project related to Blazor Role");
            }
            if (parent.Name.EndsWith(".Client"))
            {
                return true;
            }
            return false;
        }
    }
}