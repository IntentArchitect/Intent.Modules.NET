using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Persistence;
using Intent.Plugins;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Migrations.OnInstallMigration", Version = "1.0")]

namespace Intent.Modules.AspNetCore.IntegrationTesting.Migrations
{
    public class OnInstallMigration : IModuleOnInstallMigration
    {
        private const string VisualStudioDesignerId = "0701433c-36c0-4569-b1f4-9204986b587d";
        private const string VisualStudioSolutionPackageSpecializationId = "07e7b690-a59d-4b72-8440-4308a121d32c";
        private const string VisualStudioSolutionSpecializationId = "769a45a2-119f-434f-8c27-bd4a399b915c";

        private const string CSharpProjectSpecializationId = "8e9e6693-2888-4f48-a0d6-0f163baab740";
        private const string CSharpProjectSpecializationType = "C# Project (.NET)";

        private const string RoleSpecializationId = "025e933b-b602-4b6d-95ab-0ec36ae940da";
        private const string RoleSpecializationType = "Role";
        private const string TestsRoleName = "IntegrationTests";

        private const string NetSettingsStereoTypeId = "a490a23f-5397-40a1-a3cb-6da7e0b467c0";
        private const string NetSettingsStereoType = ".NET Settings";

        private const string VsProjectsPackageId = "a0636ab7-d3a1-430b-9609-11a18aa3cc7f";
        private const string VsProjectsPackageName = "Intent.VisualStudio.Projects";

        private const string SdkPropertyId = "1f0cdbc4-7a18-40a5-aeca-90452cce4fcf";
        private const string SdkPropertyValue = "Microsoft.NET.Sdk";

        private readonly IApplicationConfigurationProvider _configurationProvider;
        private readonly IPersistenceLoader _persistenceLoader;

        public OnInstallMigration(IApplicationConfigurationProvider configurationProvider, IPersistenceLoader persistenceLoader)
        {
            _configurationProvider = configurationProvider;
            _persistenceLoader = persistenceLoader;
        }

        [IntentFully]
        public string ModuleId => "Intent.AspNetCore.IntegrationTesting";

        public void OnInstall()
        {
            var app = _persistenceLoader.LoadCurrentApplication();
            var designer = app.GetDesigner(VisualStudioDesignerId);
            if (designer == null)
            {
                return;
            }

            var packages = designer.GetPackages()
                .Where(x => x.SpecializationTypeId == VisualStudioSolutionPackageSpecializationId)
                .ToArray();

            if (packages.Length == 0 ||
                packages.SelectMany(x => x.GetElementsOfType(RoleSpecializationId)).Any(x => x.Name == TestsRoleName) ||
                !TryGetDefaultSolution(packages, out var defaultSolution))
            {
                return;
            }

            var applicationProject = GetProjectWithRole(packages, "Application");
            var testProjectName = applicationProject != null
                ? $"{applicationProject.Name}.{TestsRoleName}"
                : $"{defaultSolution.Name}.Application.{TestsRoleName}";
            var targetPackage = applicationProject?.Package ?? defaultSolution.Package;

            var testProject =
                packages.SelectMany(p => p.GetElementsOfType(CSharpProjectSpecializationId)).FirstOrDefault(p => p.Name.Equals(testProjectName, StringComparison.OrdinalIgnoreCase)) ??
                targetPackage.Classes.Add(
                   id: Guid.NewGuid().ToString(),
                   specializationType: CSharpProjectSpecializationType,
                   specializationTypeId: CSharpProjectSpecializationId,
                   name: testProjectName,
                   parentId: applicationProject?.ParentFolderId ?? defaultSolution.Id);

            testProject.ChildElements.Add(
                id: Guid.NewGuid().ToString(),
                specializationType: RoleSpecializationType,
                specializationTypeId: RoleSpecializationId,
                name: TestsRoleName,
                parentId: testProject.Id);

            if (testProject.Stereotypes.All(s => s.DefinitionId != NetSettingsStereoTypeId))
            {
                testProject.Stereotypes.Add(
                    definitionId: NetSettingsStereoTypeId,
                    name: NetSettingsStereoType,
                    definitionPackageId: VsProjectsPackageId,
                    definitionPackageName: VsProjectsPackageName,
                    configure: stereotype =>
                    {
                        var sourceStereotype =
                            applicationProject?.Stereotypes?.FirstOrDefault(s => s.DefinitionId == NetSettingsStereoTypeId) ??
                            targetPackage.GetElementsOfType(CSharpProjectSpecializationId)
                                .SelectMany(x => x.Stereotypes)
                                .FirstOrDefault(x => x.DefinitionId == NetSettingsStereoTypeId && x.Properties.Any(y => y.DefinitionId == SdkPropertyId && y.Value == SdkPropertyValue));
                        if (sourceStereotype == null)
                        {
                            return;
                        }

                        foreach (var property in sourceStereotype.Properties)
                        {
                            stereotype.Properties.Add(property.DefinitionId, property.Name, property.Value);
                        }
                    });
            }

            targetPackage.Save();
        }

        /// <summary>
        /// The "Codebase Structure" designer replaced the Visual Studio designer and changed the solutions from packages to elements.
        /// </summary>
        /// <param name="packages"></param>
        /// <param name="defaultSolution"></param>
        /// <returns></returns>
        private bool TryGetDefaultSolution(IReadOnlyList<IPackageModelPersistable> packages, out (string Name, string Id, IPackageModelPersistable Package) defaultSolution)
        {
            if (!_configurationProvider.GetInstalledModules().Any(x => string.Equals(x.ModuleId, "Intent.Modelers.CodebaseStructure", StringComparison.OrdinalIgnoreCase)))
            {
                defaultSolution = (packages[0].Name, packages[0].Id, packages[0]);
                return true;
            }

            var element = packages.SelectMany(x => x.GetElementsOfType(VisualStudioSolutionSpecializationId)).FirstOrDefault();
            if (element == null)
            {
                defaultSolution = default;
                return false;
            }

            defaultSolution = (element.Name, element.Id, element.Package);
            return true;
        }

        private static IElementPersistable? GetProjectWithRole(IReadOnlyCollection<IPackageModelPersistable> packages, string roleName)
        {
            var roleElement = packages
                .SelectMany(p => p.GetElementsOfType(RoleSpecializationId))
                .FirstOrDefault(x => x.Name == roleName);
            if (roleElement == null)
            {
                return null;
            }

            return packages
                .SelectMany(p => p.GetElementsOfType(CSharpProjectSpecializationId))
                .FirstOrDefault(p => p.ChildElements.Any(c => c.Id == roleElement.Id));
        }
    }
}