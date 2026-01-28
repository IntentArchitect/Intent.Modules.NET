using System;
using System.IO;
using System.Linq;
using Intent.Modules.Common.CSharp.Api;
using Intent.Modules.Common.Types.Api;
using Intent.Modules.VisualStudio.Projects.Api;
using Intent.Persistence;
using Intent.Plugins;
using Intent.RoslynWeaver.Attributes;
using IElementPersistable = Intent.Persistence.IElementPersistable;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Migrations.OnVersionMigration", Version = "1.0")]

namespace Intent.Modules.VisualStudio.Projects.Migrations
{
    public class Migration_04_00_00_Pre_00 : IModuleMigration
    {
        private readonly IPersistenceLoader _persistenceLoader;
        private const string DesignerId = "0701433c-36c0-4569-b1f4-9204986b587d";
        private const string OldFolderElementTypeId = "3407a825-1331-4f3f-89a4-901903ed97ce";
        private const string OldFolderStereotypeId = "61132551-825d-4c6e-b9f1-0f95249d08d4";
        private const string OldFolderStereotypeIsNamespacePropertyId = "4bfbefd0-d22e-4ca0-9723-92ae273feca4";
        private const string NewFolderStereotypeIsNamespacePropertyId = "96df2fa6-7361-4e43-9acf-dbcea23b650a";
        private const string CommonPackageName = "730e1275-0c32-44f7-991a-9619d07ca68d";
        private const string CommonPackageId = "730e1275-0c32-44f7-991a-9619d07ca68d";

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
            var designer = application.GetDesigner(DesignerId);

            var packages = designer.GetPackages().ToArray();

            foreach (var package in designer.GetPackages())
            {
                var vsElement = package.Classes.Add(
                    id: Guid.NewGuid().ToString(),
                    specializationType: VisualStudioSolutionModel.SpecializationType,
                    specializationTypeId: VisualStudioSolutionModel.SpecializationTypeId,
                    name: package.Name,
                    parentId: package.Id);

                var packageStereotype = package.Stereotypes.Get(VisualStudioSolutionModelStereotypeExtensions.VisualStudioSolutionOptions.DefinitionId);
                if (packageStereotype != null)
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
                    UpdateElement(
                        element: element,
                        packageId: package.Id,
                        vsElementId: vsElement.Id);
                }

                package.Save();
            }

            var designerReference = application.GetDesignerReference(DesignerId);
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
        }

        private static void UpdateElement(IElementPersistable element, string packageId, string vsElementId)
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
                UpdateElement(
                    element: childElement,
                    packageId: packageId,
                    vsElementId: vsElementId);
            }
        }

        public void Down()
        {
        }
    }
}