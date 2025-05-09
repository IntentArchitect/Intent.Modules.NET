using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Intent.Engine;
using Intent.IArchitect.Agent.Persistence.Model;
using Intent.IArchitect.Agent.Persistence.Model.Common;
using Intent.Metadata;
using Intent.Modules.Common.VisualStudio;
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

        private readonly IApplicationConfigurationProvider _configurationProvider;

        public OnInstallMigration(IApplicationConfigurationProvider configurationProvider)
        {
            _configurationProvider = configurationProvider;
        }

        [IntentFully]
        public string ModuleId => "Intent.UnitTesting";

        public void OnInstall()
        {
            var app = ApplicationPersistable.Load(_configurationProvider.GetApplicationConfig().FilePath);
            var designer = app.GetDesigner(VisualStudioDesignerId);
            var packages = designer.GetPackages().Where(x => x.SpecializationTypeId == VisualStudioSolutionPackageSpecializationId);

            // should only be one, but just going to go for each one
            foreach (var package in packages)
            {
                var testFolder = CreateTestFolder(package);
                var unitTestProject = CreateUnitTestProject(package, testFolder);
                AddUnitTestRole(package, unitTestProject);

                package.Save(true);
            }
        }
        private static ElementPersistable CreateTestFolder(PackageModelPersistable package)
        {
            var testFolderName = "5 - Tests";
            var elements = package.GetElementsOfType(SolutionFolderSpecializationId);

            if (!elements.Any(x => x.Name == testFolderName))
            {
                package.AddElement(new ElementPersistable
                {
                    Name = testFolderName,
                    Display = testFolderName,
                    IsAbstract = false,
                    SortChildren = SortChildrenOptions.SortByTypeAndName,
                    IsMapped = false,
                    ParentFolderId = package.Id,
                    PackageId = package.Id,
                    PackageName = package.Name,
                    SpecializationTypeId = SolutionFolderSpecializationId,
                    SpecializationType = SolutionFolderSpecializationType,
                    Package = package,
                    Id = Guid.NewGuid().ToString()
                });
            }

            return package.GetElementsOfType(SolutionFolderSpecializationId).First(n => n.Name == testFolderName);
        }

        private static ExistingProjectSettings GetExistingProjectSettings(PackageModelPersistable package)
        {
            var applicationRole = package.GetElementsOfType(RoleSpecializationId).FirstOrDefault(r => r.Name == "Application");

            var applicationProjectName = $"{package.Name}.Application";
            var targetFramework = string.Empty;

            if (applicationRole != null && package.TryGetElementById(applicationRole.Parent.Id, out var applicationProjectInterface))
            {
                if (applicationProjectInterface is ElementPersistable applicationProject)
                {
                    applicationProjectName = applicationProject.Name;

                    var applicationNetSettings = applicationProject.Stereotypes.FirstOrDefault(s => s.DefinitionId == NetSettingsStereoTypeId);
                    if (applicationNetSettings != null)
                    {
                        var applicationTargetFramework = applicationNetSettings.Properties.FirstOrDefault(s => s.DefinitionId == TargetFrameworkPropertyId);

                        if (applicationTargetFramework != null)
                        {
                            targetFramework = applicationTargetFramework.Value;
                        }
                    }
                }
            }

            return new ExistingProjectSettings(applicationProjectName, targetFramework);
        }

        private static ElementPersistable CreateUnitTestProject(PackageModelPersistable package, ElementPersistable testFolder)
        {
            var applicationProjectSettings = GetExistingProjectSettings(package);

            if (!package.GetElementsOfType(CSharpProjectSpecializationId).Any(p => p.Name == $"{applicationProjectSettings.ProjectName}.UnitTests"))
            {
                var projectElement = new ElementPersistable
                {
                    Name = $"{applicationProjectSettings.ProjectName}.UnitTests",
                    Display = $"{applicationProjectSettings.ProjectName}.UnitTests",
                    IsAbstract = false,
                    SortChildren = SortChildrenOptions.SortByTypeAndName,
                    IsMapped = false,
                    ParentFolderId = testFolder.Id,
                    Package = package,
                    SpecializationTypeId = CSharpProjectSpecializationId,
                    SpecializationType = CSharpProjectSpecializationType,
                    Id = Guid.NewGuid().ToString(),
                };

                if (!string.IsNullOrWhiteSpace(applicationProjectSettings.FrameworkVersion))
                {
                    var testNetSettings = new StereotypePersistable
                    {
                        DefinitionId = NetSettingsStereoTypeId,
                        Name = NetSettingsStereoType,
                        DefinitionPackageId = VSProjectsPackageId,
                        DefinitionPackageName = VSProjectsPackageName,
                        Properties =
                        [
                            new StereotypePropertyPersistable
                        {
                            DefinitionId = TargetFrameworkPropertyId,
                            Name = TargetFrameworkPropertyName,
                            Value = applicationProjectSettings.FrameworkVersion,
                            IsActive = true
                        }
                        ]
                    };

                    projectElement.Stereotypes.Add(testNetSettings);
                }

                testFolder.AddElement(projectElement);
            }

            return package.GetElementsOfType(CSharpProjectSpecializationId).FirstOrDefault(p => p.Name == $"{applicationProjectSettings.ProjectName}.UnitTests");
        }

        private static void AddUnitTestRole(PackageModelPersistable package, ElementPersistable unitTestProject)
        {
            var unitTestRole = package.GetElementsOfType(RoleSpecializationId).FirstOrDefault(r => r.Name == UnitTestsRoleName);

            if (unitTestRole != null)
            {
                return;
            }

            unitTestRole = new ElementPersistable
            {
                Name = UnitTestsRoleName,
                Display = UnitTestsRoleName,
                IsAbstract = false,
                IsMapped = false,
                ParentFolderId = unitTestProject.Id,
                Package = package,
                SpecializationTypeId = RoleSpecializationId,
                SpecializationType = RoleSpecializationType,
                Id = Guid.NewGuid().ToString()
            };

            unitTestProject.AddElement(unitTestRole);
        }

        private record ExistingProjectSettings(string ProjectName, string FrameworkVersion);
    }
}