#nullable enable
using System;
using System.Linq;
using Intent.Persistence.V2;
using Intent.Plugins;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Migrations.OnInstallMigration", Version = "1.0")]

namespace Intent.Modules.Integration.HttpClients.Stubs.Migrations
{
    public class OnInstallMigration : IModuleOnInstallMigration
    {
        private const string CodebaseStructureDesignerId = "0701433c-36c0-4569-b1f4-9204986b587d";

        private const string CSharpProjectSpecializationType = "C# Project (.NET)";
        private const string CSharpProjectSpecializationId = "8e9e6693-2888-4f48-a0d6-0f163baab740";

        // "Output Anchor" element type. Projects/folders/anchors are NESTED in the Codebase Structure tree, so
        // they must be located with GetElementsOfType(...) (walks the whole package, top-level AND nested).
        // package.Classes only returns the package's TOP-LEVEL elements and never sees a nested anchor — that was
        // the bug.
        private const string OutputAnchorSpecializationType = "Output Anchor";
        private const string OutputAnchorSpecializationId = "025e933b-b602-4b6d-95ab-0ec36ae940da";

        private const string InfrastructureAnchorName = "Infrastructure";
        private const string StubsAnchorName = "Stubs";

        private const string NetSettingsStereotypeId = "a490a23f-5397-40a1-a3cb-6da7e0b467c0";

        private const string OutputAnchorSettingsStereotypeId = "a23c7901-31a5-4cbf-b8bf-1be128977e6d";
        private const string OutputAnchorSettingsStereotypeName = "Output Anchor Settings";
        private const string CodebaseStructurePackageId = "525868ff-adc0-4ab6-a90e-352288da941f";
        private const string CodebaseStructurePackageName = "Intent.Modelers.CodebaseStructure";
        private const string CreateSubFoldersPropertyId = "8d79a350-96f1-4d50-bb0a-f65a5c53ec62";
        private const string CreateSubFoldersPropertyName = "Create Sub-Folders";

        private readonly IPersistenceLoader _persistenceLoader;

        public OnInstallMigration(IPersistenceLoader persistenceLoader)
        {
            _persistenceLoader = persistenceLoader;
        }

        [IntentFully]
        public string ModuleId => "Intent.Integration.HttpClients.Stubs";

        public void OnInstall()
        {
            var application = _persistenceLoader.LoadCurrentApplication();
            var designer = application.GetDesigner(CodebaseStructureDesignerId);
            if (designer is null)
            {
                return;
            }

            foreach (var package in designer.GetPackages())
            {
                // Find the Infrastructure anchor anywhere in the package, then the project that owns it.
                var infrastructureAnchor = package.GetElementsOfType(OutputAnchorSpecializationId)
                    .FirstOrDefault(e => e.Name == InfrastructureAnchorName);
                if (infrastructureAnchor is null)
                {
                    continue;
                }

                var infrastructureProject = package.GetElementsOfType(CSharpProjectSpecializationId)
                    .FirstOrDefault(p => p.Id == infrastructureAnchor.ParentFolderId);
                if (infrastructureProject is null)
                {
                    continue;
                }

                var stubProjectName = $"{infrastructureProject.Name}.Stubs";
                if (package.GetElementsOfType(CSharpProjectSpecializationId)
                        .Any(p => p.Name.Equals(stubProjectName, StringComparison.OrdinalIgnoreCase)))
                {
                    continue; // already created — idempotent
                }

                // Stub project beside Infrastructure (same parent folder), inheriting its .NET settings so it
                // resolves a target framework.
                var stubProject = package.Classes.Add(
                    id: Guid.NewGuid().ToString(),
                    specializationType: CSharpProjectSpecializationType,
                    specializationTypeId: CSharpProjectSpecializationId,
                    name: stubProjectName,
                    parentId: infrastructureProject.ParentFolderId);

                if (infrastructureProject.Stereotypes.TryGet(NetSettingsStereotypeId, out var sourceNetSettings) && sourceNetSettings is not null)
                {
                    var netSettings = stubProject.Stereotypes.Add(
                        definitionId: sourceNetSettings.DefinitionId,
                        name: sourceNetSettings.Name,
                        definitionPackageId: sourceNetSettings.DefinitionPackageId,
                        definitionPackageName: sourceNetSettings.DefinitionPackageName);
                    netSettings.AddedByDefault = true;
                    foreach (var property in sourceNetSettings.Properties)
                    {
                        netSettings.Properties.Add(property.DefinitionId, property.Name, property.Value);
                    }
                }

                // The "Stubs" anchor routes the stub templates (role "Stubs.*") into this project; Create
                // Sub-Folders lets the HttpClientStub template emit into its "HttpClients" sub-folder.
                var anchor = package.Classes.Add(
                    id: Guid.NewGuid().ToString(),
                    specializationType: OutputAnchorSpecializationType,
                    specializationTypeId: OutputAnchorSpecializationId,
                    name: StubsAnchorName,
                    parentId: stubProject.Id);
                var anchorSettings = anchor.Stereotypes.Add(
                    definitionId: OutputAnchorSettingsStereotypeId,
                    name: OutputAnchorSettingsStereotypeName,
                    definitionPackageId: CodebaseStructurePackageId,
                    definitionPackageName: CodebaseStructurePackageName);
                anchorSettings.AddedByDefault = true;
                anchorSettings.Properties.Add(CreateSubFoldersPropertyId, CreateSubFoldersPropertyName, "true");

                package.Save();
            }
        }
    }
}
