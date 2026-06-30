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
        // The "Codebase Structure" designer (formerly "Visual Studio"). Migrations that mutate it must load
        // the application via IPersistenceLoader and use the Intent.Persistence.V2 API (see
        // Intent.Modules.VisualStudio.Projects Migration_04_00_00_Pre_00).
        private const string CodebaseStructureDesignerId = "0701433c-36c0-4569-b1f4-9204986b587d";

        private const string CSharpProjectSpecializationType = "C# Project (.NET)";
        private const string CSharpProjectSpecializationId = "8e9e6693-2888-4f48-a0d6-0f163baab740";

        // 025e933b is the Codebase Structure "Output Anchor" element type. A template generates into the
        // project that owns an anchor whose name matches the template's Role; a single "Stubs" anchor serves
        // both stub templates ("Stubs.Configuration" and "Stubs.HttpClientStub").
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
                // package.Classes is a FLAT list of every element in the package, so anchors/projects can be
                // filtered and parent-walked directly.
                var elements = package.Classes;

                // Locate the Infrastructure project by walking up from its existing "Infrastructure" output
                // anchor to the owning project, falling back to the conventionally named "*.Infrastructure"
                // project if no anchor is present yet.
                var infrastructureAnchor = elements.FirstOrDefault(e =>
                    e.SpecializationTypeId == OutputAnchorSpecializationId && e.Name == InfrastructureAnchorName);
                var infrastructureProject = infrastructureAnchor is not null
                    ? elements.FirstOrDefault(e => e.SpecializationTypeId == CSharpProjectSpecializationId && e.Id == infrastructureAnchor.ParentFolderId)
                    : null;
                infrastructureProject ??= elements.FirstOrDefault(e =>
                    e.SpecializationTypeId == CSharpProjectSpecializationId &&
                    e.Name.EndsWith(".Infrastructure", StringComparison.OrdinalIgnoreCase));

                if (infrastructureProject is null)
                {
                    continue;
                }

                var stubProjectName = $"{infrastructureProject.Name}.Stubs";

                // Idempotent: don't re-create if the stub project already exists.
                if (elements.Any(e => e.SpecializationTypeId == CSharpProjectSpecializationId &&
                        e.Name.Equals(stubProjectName, StringComparison.OrdinalIgnoreCase)))
                {
                    continue;
                }

                // Create the stub project beside Infrastructure (same solution folder) — never at the solution root.
                var stubProject = package.Classes.Add(
                    id: Guid.NewGuid().ToString(),
                    specializationType: CSharpProjectSpecializationType,
                    specializationTypeId: CSharpProjectSpecializationId,
                    name: stubProjectName,
                    parentId: infrastructureProject.ParentFolderId);

                // Inherit Infrastructure's .NET settings (SDK, Target Framework, Implicit Usings, ...) so the
                // stub project resolves a framework for the stub templates' NuGet/version resolution.
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

                // A single "Stubs" output anchor inside the stub project. The stub templates' Roles
                // ("Stubs.Configuration", "Stubs.HttpClientStub") bind to it, so the install places their
                // Template Outputs inside this (frameworked) project rather than at the solution root.
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
