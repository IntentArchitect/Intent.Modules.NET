using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Intent.Engine;
using Intent.Metadata;
using Intent.Modules.Common.VisualStudio;
using Intent.Persistence;
using Intent.Plugins;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Migrations.OnInstallMigration", Version = "1.0")]

namespace Intent.Modules.UnitTesting.Migrations
{
    public class OnInstallMigration : IModuleOnInstallMigration
    {
        private const string VisualStudioDesignerId = "0701433c-36c0-4569-b1f4-9204986b587d";
        private const string VisualStudioSolutionPackageSpecializationId = "07e7b690-a59d-4b72-8440-4308a121d32c";

        private const string SolutionFolderSpecializationId = "0dc2b846-c968-49eb-99b7-8776919313a8";
        private const string SolutionFolderSpecializationType = "Solution Folder";

        private const string CSharpProjectSpecializationId = "8e9e6693-2888-4f48-a0d6-0f163baab740";
        private const string CSharpProjectSpecializationType = "C# Project (.NET)";

        private const string RoleSpecializationId = "025e933b-b602-4b6d-95ab-0ec36ae940da";
        private const string RoleSpecializationType = "Role";
        private const string UnitTestsRoleName = "UnitTests";

        private const string NetSettingsStereoTypeId = "a490a23f-5397-40a1-a3cb-6da7e0b467c0";
        private const string NetSettingsStereoType = ".NET Settings";

        private const string VSProjectsPackageId = "a0636ab7-d3a1-430b-9609-11a18aa3cc7f";
        private const string VSProjectsPackageName = "Intent.VisualStudio.Projects";

        private const string TargetFrameworkPropertyId = "d53ab03c-90cf-4b6a-b733-73b6983ab603";
        private const string TargetFrameworkPropertyName = "Target Framework";

        private const string SDKPropertyId = "1f0cdbc4-7a18-40a5-aeca-90452cce4fcf";
        private const string SDKPropertyName = "SDK";
        private const string SDKPropertyValue = "Microsoft.NET.Sdk";


        private readonly IApplicationConfigurationProvider _configurationProvider;
        private readonly IPersistenceLoader _persistenceLoader;

        public OnInstallMigration(IApplicationConfigurationProvider configurationProvider, IPersistenceLoader persistenceLoader)
        {
            _configurationProvider = configurationProvider;
            _persistenceLoader = persistenceLoader;
        }

        [IntentFully]
        public string ModuleId => "Intent.UnitTesting";

        public void OnInstall()
        {
            var app = _persistenceLoader.LoadApplication(_configurationProvider.GetApplicationConfig().FilePath);
            var designer = app.GetDesigner(VisualStudioDesignerId);
            var packages = designer.GetPackages().Where(x => x.SpecializationTypeId == VisualStudioSolutionPackageSpecializationId);

            // should only be one, but just going to go for each one
            foreach (var package in packages)
            {
                var testFolder = CreateTestFolder(package);
                var unitTestProject = CreateUnitTestProject(package, testFolder);
                AddUnitTestRole(package, unitTestProject);

                package.Save();
            }
        }
        private static IElementPersistable CreateTestFolder(IPackageModelPersistable package)
        {
            var testFolderName = "5 - Tests";
            var elements = package.GetElementsOfType(SolutionFolderSpecializationId);

            if (!elements.Any(x => x.Name == testFolderName))
            {
                package.Classes.Add(Guid.NewGuid().ToString(), SolutionFolderSpecializationType, SolutionFolderSpecializationId, testFolderName, package.Id);
            }

            return package.GetElementsOfType(SolutionFolderSpecializationId).First(n => n.Name == testFolderName);
        }

        private static ExistingProjectSettings GetApplicationProjectSettings(IPackageModelPersistable package)
        {
            var applicationRole = package.GetElementsOfType(RoleSpecializationId).FirstOrDefault(r => r.Name == "Application");

            var applicationProject = package.GetElementsOfType(CSharpProjectSpecializationId).FirstOrDefault(x => x.ChildElements.Any(c => c.Id == applicationRole?.Id));
            if (applicationRole != null && applicationProject != null)
            {
                var applicationNetSettings = applicationProject.Stereotypes.FirstOrDefault(s => s.DefinitionId == NetSettingsStereoTypeId);
 
                return new ExistingProjectSettings(applicationProject.Name, applicationNetSettings);
            }

            return new ExistingProjectSettings($"{package.Name}.Application", null);
        }

        private static IElementPersistable CreateUnitTestProject(IPackageModelPersistable package, IElementPersistable testFolder)
        {
            var applicationProjectSettings = GetApplicationProjectSettings(package);

            if (package.GetElementsOfType(CSharpProjectSpecializationId).All(p => p.Name != $"{applicationProjectSettings.ProjectName}.UnitTests"))
            {
                var testProject = package.Classes.Add(Guid.NewGuid().ToString(), CSharpProjectSpecializationType, CSharpProjectSpecializationId,
                    $"{applicationProjectSettings.ProjectName}.UnitTests", testFolder.Id);

                if (applicationProjectSettings.DotNetSettingsStereotype != null)
                {
                    testProject.Stereotypes.Add(applicationProjectSettings.DotNetSettingsStereotype.DefinitionId, applicationProjectSettings.DotNetSettingsStereotype.Name, applicationProjectSettings.DotNetSettingsStereotype.DefinitionPackageId, applicationProjectSettings.DotNetSettingsStereotype.DefinitionPackageName,
                        stereo =>
                        {
                            foreach (var property in applicationProjectSettings.DotNetSettingsStereotype.Properties)
                            {
                                stereo.Properties.Add(property.DefinitionId, property.Name, property.Value);
                            }
                        });
                }
                else
                {
                    testProject.Stereotypes.Add(NetSettingsStereoTypeId, NetSettingsStereoType, VSProjectsPackageId, VSProjectsPackageName,
                        stereo =>
                        {
                            stereo.Properties.Add(TargetFrameworkPropertyId, TargetFrameworkPropertyName, ".NET 8.0");
                            stereo.Properties.Add(SDKPropertyId, SDKPropertyName, SDKPropertyValue);
                        });

                }
            }

            return package.GetElementsOfType(CSharpProjectSpecializationId).FirstOrDefault(p => p.Name == $"{applicationProjectSettings.ProjectName}.UnitTests");
        }

        private static void AddUnitTestRole(IPackageModelPersistable package, IElementPersistable unitTestProject)
        {
            var unitTestRole = package.GetElementsOfType(RoleSpecializationId).FirstOrDefault(r => r.Name == UnitTestsRoleName);

            if (unitTestRole != null)
            {
                return;
            }

            unitTestProject.ChildElements.Add(Guid.NewGuid().ToString(), RoleSpecializationType, RoleSpecializationId, UnitTestsRoleName, unitTestProject.Id);
        }

        private record ExistingProjectSettings(string ProjectName, IStereotypePersistable DotNetSettingsStereotype);
    }
}