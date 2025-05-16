using System.Diagnostics;
using System.Linq;
using Intent.Engine;
using Intent.IArchitect.Agent.Persistence.Model;
using Intent.IArchitect.Agent.Persistence.Model.Common;
using Intent.Plugins;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Migrations.OnUninstallMigration", Version = "1.0")]

namespace Intent.Modules.UnitTesting.Migrations
{
    public class OnUninstallMigration : IModuleOnUninstallMigration
    {
        private const string VisualStudioDesignerId = "0701433c-36c0-4569-b1f4-9204986b587d";
        private const string VisualStudioSolutionPackageSpecializationId = "07e7b690-a59d-4b72-8440-4308a121d32c";

        private const string CSharpProjectSpecializationId = "8e9e6693-2888-4f48-a0d6-0f163baab740";
        private const string SolutionFolderSpecializationId = "0dc2b846-c968-49eb-99b7-8776919313a8";

        private const string RoleSpecializationId = "025e933b-b602-4b6d-95ab-0ec36ae940da";

        private readonly IApplicationConfigurationProvider _configurationProvider;

        public OnUninstallMigration(IApplicationConfigurationProvider configurationProvider)
        {
            _configurationProvider = configurationProvider;
        }

        [IntentFully]
        public string ModuleId => "Intent.UnitTesting";

        public void OnUninstall()
        {
            var app = ApplicationPersistable.Load(_configurationProvider.GetApplicationConfig().FilePath);
            var designer = app.GetDesigner(VisualStudioDesignerId);
            var packages = designer.GetPackages().Where(x => x.SpecializationTypeId == VisualStudioSolutionPackageSpecializationId);

            // should only be one, but just going to go for each one
            foreach (var package in packages)
            {
                var applicationProjectName = GetExistingApplicationProjectName(package);
                var unitTestProject = package.GetElementsOfType(CSharpProjectSpecializationId).FirstOrDefault(p => p.Name == $"{applicationProjectName}.UnitTests");
                var parentFolderId = unitTestProject.ParentFolderId;

                bool savePackage = false;
                if (unitTestProject != null)
                {
                    unitTestProject.ChildElements.RemoveRange(0, unitTestProject.ChildElements.Count);
                    package.RemoveElement(unitTestProject);
                    savePackage = true;
                }

                // if no projects which have the folder as a parent
                if (!package.GetElementsOfType(CSharpProjectSpecializationId).Any(p => p.ParentFolderId == parentFolderId))
                {
                    var folder = package.GetElementsOfType(SolutionFolderSpecializationId).FirstOrDefault(s => s.Id == parentFolderId && s.ChildElements.Count == 0);

                    if (folder != null)
                    {
                        package.RemoveElement(folder);
                        savePackage = true;
                    }
                }

                if (savePackage)
                {
                    package.Save(true);
                }
            }
        }

        private static string GetExistingApplicationProjectName(PackageModelPersistable package)
        {
            var applicationRole = package.GetElementsOfType(RoleSpecializationId).FirstOrDefault(r => r.Name == "Application");

            var applicationProjectName = $"{package.Name}.Application";

            if (applicationRole != null && package.TryGetElementById(applicationRole.Parent.Id, out var applicationProjectInterface))
            {
                if (applicationProjectInterface is ElementPersistable applicationProject)
                {
                    applicationProjectName = applicationProject.Name;
                }
            }

            return applicationProjectName;
        }
    }
}