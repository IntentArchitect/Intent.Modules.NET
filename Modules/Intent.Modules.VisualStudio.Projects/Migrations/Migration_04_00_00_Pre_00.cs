using System;
using System.IO;
using System.Linq;
using Intent.Modelers.CodebaseStructure.Api;
using Intent.Modules.Common.CSharp.Api;
using Intent.Modules.Common.Types.Api;
using Intent.Modules.VisualStudio.Projects.Api;
using Intent.Persistence.V2;
using Intent.Plugins;
using Intent.RoslynWeaver.Attributes;
using Intent.Utils;
using IElementPersistable = Intent.Persistence.V2.IElementPersistable;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Migrations.OnVersionMigration", Version = "1.0")]

namespace Intent.Modules.VisualStudio.Projects.Migrations
{
    public class Migration_04_00_00_Pre_00 : IModuleMigration
    {
        private readonly IPersistenceLoader _persistenceLoader;
        private const string OldFolderElementTypeId = "3407a825-1331-4f3f-89a4-901903ed97ce";
        private const string OldFolderStereotypeId = "61132551-825d-4c6e-b9f1-0f95249d08d4";
        private const string OldFolderStereotypeIsNamespacePropertyId = "4bfbefd0-d22e-4ca0-9723-92ae273feca4";
        private const string NewFolderStereotypeIsNamespacePropertyId = "96df2fa6-7361-4e43-9acf-dbcea23b650a";
        private const string CommonPackageName = "Intent.Common.CSharp";
        private const string CommonPackageId = "730e1275-0c32-44f7-991a-9619d07ca68d";
        private const string VsPackageName = "Intent.VisualStudio.Projects";
        private const string VsPackageId = "a0636ab7-d3a1-430b-9609-11a18aa3cc7f";

        public Migration_04_00_00_Pre_00(IPersistenceLoader persistenceLoader)
        {
            _persistenceLoader = persistenceLoader;
        }

        [IntentFully]
        public string ModuleId => "Intent.VisualStudio.Projects";
        [IntentFully]
        public string ModuleVersion => "4.0.0-pre.0";

        public void Up()
        {
            var application = _persistenceLoader.LoadCurrentApplication();
            var designer = application.GetDesigner(ApiMetadataDesignerExtensions.CodebaseStructureDesignerId);

            var packages = designer.GetPackages().ToArray();

            foreach (var package in designer.GetPackages())
            {
                var vsElement = package.Classes.Add(
                    id: Guid.NewGuid().ToString(),
                    specializationType: VisualStudioSolutionModel.SpecializationType,
                    specializationTypeId: VisualStudioSolutionModel.SpecializationTypeId,
                    name: package.Name,
                    parentId: package.Id);

                if (package.Stereotypes.TryGet(VisualStudioSolutionModelStereotypeExtensions.VisualStudioSolutionOptions.DefinitionId, out var packageStereotype))
                {
                    var elementStereotype = vsElement.Stereotypes.Add(
                        definitionId: packageStereotype.DefinitionId,
                        name: packageStereotype.Name,
                        definitionPackageId: packageStereotype.DefinitionPackageId,
                        definitionPackageName: packageStereotype.DefinitionPackageName);

                    foreach (var property in packageStereotype.Properties)
                    {
                        elementStereotype.Properties.Add(
                            id: property.DefinitionId,
                            name: property.Name,
                            value: property.Value);
                    }

                    package.Stereotypes.Remove(packageStereotype);
                }


                foreach (var element in package.Classes)
                {
                    UpgradeElement(
                        element: element,
                        packageId: package.Id,
                        vsElementId: vsElement.Id);
                }

                package.Save();
            }

            var designerReference = application.GetDesignerReference(ApiMetadataDesignerExtensions.CodebaseStructureDesignerId);
            var applicationDirectory = Path.GetDirectoryName(application.AbsolutePath);

            var oldRelativeLocation = designerReference.RelativeLocation;
            var oldAbsolutePath = Path.Join(applicationDirectory, oldRelativeLocation);
            var oldDirectoryName = Path.GetDirectoryName(oldAbsolutePath);
            var oldFileName = Path.GetFileName(oldAbsolutePath);

            var newRelativeLocation = oldRelativeLocation.Replace("Visual Studio", "Codebase Structure");
            var newAbsolutePath = Path.Join(applicationDirectory, newRelativeLocation);
            var newDirectoryName = Path.GetDirectoryName(newAbsolutePath);
            var newFileName = oldFileName.Replace("Visual Studio", "Codebase Structure");

            designerReference.RelativeLocation = newRelativeLocation;

            Directory.Move(oldDirectoryName, newDirectoryName);
            File.Move(Path.Join(newDirectoryName, oldFileName), Path.Join(newDirectoryName, newFileName));

            application.SaveAllChanges();

            if (Directory.Exists(oldDirectoryName))
            {
                Directory.Delete(oldDirectoryName, recursive: true);
            }
        }

        private static void UpgradeElement(IElementPersistable element, string packageId, string vsElementId)
        {
            if (element.ParentFolderId == packageId &&
                element.SpecializationTypeId != VisualStudioSolutionModel.SpecializationTypeId)
            {
                element.ParentFolderId = vsElementId;
            }

            if (element.SpecializationTypeId == OldFolderElementTypeId)
            {
                element.SpecializationTypeId = FolderModel.SpecializationTypeId;

                foreach (var stereotype in element.Stereotypes)
                {
                    if (stereotype.DefinitionId != OldFolderStereotypeId)
                    {
                        continue;
                    }

                    stereotype.DefinitionId = FolderModelStereotypeExtensions.FolderOptions.DefinitionId;
                    stereotype.DefinitionPackageId = CommonPackageId;
                    stereotype.DefinitionPackageName = CommonPackageName;

                    foreach (var property in stereotype.Properties)
                    {
                        if (property.DefinitionId != OldFolderStereotypeIsNamespacePropertyId)
                        {
                            continue;
                        }

                        property.DefinitionId = NewFolderStereotypeIsNamespacePropertyId;
                    }
                }
            }

            foreach (var childElement in element.ChildElements)
            {
                UpgradeElement(
                    element: childElement,
                    packageId: packageId,
                    vsElementId: vsElementId);
            }
        }

        public void Down()
        {
            var application = _persistenceLoader.LoadCurrentApplication();
            application.Modules.Remove("Intent.Modelers.CodebaseStructure");

            var designer = application.GetDesigner(ApiMetadataDesignerExtensions.CodebaseStructureDesignerId);
            designer.DesignerSettingsReferences.Remove("ba6abc27-ed11-4aae-a405-13fe7491636b");
            var packages = designer.GetPackages().ToArray();

            foreach (var package in designer.GetPackages())
            {
                var vsElements = package.Classes.Where(x => x.SpecializationTypeId == VisualStudioSolutionModel.SpecializationTypeId).ToArray();
                if (vsElements.Length != 1)
                {
                    Logging.Log.Warning($"Skipped running Visual Studio Designer downgrade on package '{package.Name}' as it has {vsElements.Length} count of Visual Studio elements while only a count of 1 is supported.");
                    continue;
                }

                var vsElement = vsElements[0];

                if (vsElement.Stereotypes.TryGet(VisualStudioSolutionModelStereotypeExtensions.VisualStudioSolutionOptions.DefinitionId, out var elementStereotype))
                {
                    var packageStereotype = package.Stereotypes.Add(
                        definitionId: elementStereotype.DefinitionId,
                        name: elementStereotype.Name,
                        definitionPackageId: elementStereotype.DefinitionPackageId,
                        definitionPackageName: elementStereotype.DefinitionPackageName);

                    foreach (var property in elementStereotype.Properties)
                    {
                        packageStereotype.Properties.Add(
                            id: property.DefinitionId,
                            name: property.Name,
                            value: property.Value);
                    }

                    package.Stereotypes.Remove(elementStereotype);
                }

                package.References.Remove("525868ff-adc0-4ab6-a90e-352288da941f");

                foreach (var element in package.Classes)
                {
                    DowngradeElement(
                        element: element,
                        packageId: package.Id,
                        vsElementId: vsElement.Id);
                }

                package.Classes.Remove(vsElement);

                package.Save();
            }

            designer.Save();

            var designerReference = application.GetDesignerReference(ApiMetadataDesignerExtensions.CodebaseStructureDesignerId);
            var applicationDirectory = Path.GetDirectoryName(application.AbsolutePath);

            var oldRelativeLocation = designerReference.RelativeLocation;
            var oldAbsolutePath = Path.Join(applicationDirectory, oldRelativeLocation);
            var oldDirectoryName = Path.GetDirectoryName(oldAbsolutePath);
            var oldFileName = Path.GetFileName(oldAbsolutePath);

            var newRelativeLocation = oldRelativeLocation.Replace("Codebase Structure", "Visual Studio");
            var newAbsolutePath = Path.Join(applicationDirectory, newRelativeLocation);
            var newDirectoryName = Path.GetDirectoryName(newAbsolutePath);
            var newFileName = oldFileName.Replace("Codebase Structure", "Visual Studio");

            designerReference.RelativeLocation = newRelativeLocation;

            Directory.Move(oldDirectoryName, newDirectoryName);
            File.Move(Path.Join(newDirectoryName, oldFileName), Path.Join(newDirectoryName, newFileName));

            application.SaveAllChanges();

            if (Directory.Exists(oldDirectoryName))
            {
                Directory.Delete(oldDirectoryName, recursive: true);
            }
        }

        private static void DowngradeElement(IElementPersistable element, string packageId, string vsElementId)
        {
            if (element.ParentFolderId == vsElementId &&
                element.SpecializationTypeId != VisualStudioSolutionModel.SpecializationTypeId)
            {
                element.ParentFolderId = packageId;
            }

            if (element.SpecializationTypeId == FolderModel.SpecializationTypeId)
            {
                element.SpecializationTypeId = OldFolderElementTypeId;

                foreach (var stereotype in element.Stereotypes)
                {
                    if (stereotype.DefinitionId != FolderModelStereotypeExtensions.FolderOptions.DefinitionId)
                    {
                        continue;
                    }

                    stereotype.DefinitionId = OldFolderStereotypeId;
                    stereotype.DefinitionPackageId = VsPackageId;
                    stereotype.DefinitionPackageName = VsPackageName;

                    foreach (var property in stereotype.Properties)
                    {
                        if (property.DefinitionId != NewFolderStereotypeIsNamespacePropertyId)
                        {
                            continue;
                        }

                        property.DefinitionId = OldFolderStereotypeIsNamespacePropertyId;
                    }
                }
            }

            foreach (var childElement in element.ChildElements)
            {
                DowngradeElement(
                    element: childElement,
                    packageId: packageId,
                    vsElementId: vsElementId);
            }
        }
    }
}