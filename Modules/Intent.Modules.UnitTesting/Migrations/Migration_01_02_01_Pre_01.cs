#nullable enable
using System.Collections.Generic;
using System.Linq;
using Intent.Persistence;
using Intent.Plugins;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Migrations.OnVersionMigration", Version = "1.0")]

namespace Intent.Modules.UnitTesting.Migrations
{
    /// <summary>
    /// An application can target several agent harnesses (<c>.claude</c>, <c>.codex</c>, <c>.github</c>, ...), each with
    /// its own skills folder and its own <c>unit-test-guidelines</c> output. <see cref="Migration_01_02_01_Pre_00"/> moved
    /// every one of those outputs into the first skills folder it found, and two outputs in one folder fail the Software
    /// Factory with "Duplicate Template Output found at same location". This migration gives each skills folder one
    /// output: it moves the extra outputs into the skills folders that have none, and removes any left over.
    /// </summary>
    public class Migration_01_02_01_Pre_01 : IModuleMigration
    {
        private const string CodebaseStructureDesignerId = "0701433c-36c0-4569-b1f4-9204986b587d";
        private const string OutputAnchorSpecializationId = "025e933b-b602-4b6d-95ab-0ec36ae940da";
        private const string TemplateOutputSpecializationId = "d421c322-7a51-4094-89fa-e5d8a0a97b27";
        private const string SkillsAnchorName = "AI.Context.Skills";
        private const string SkillTemplateOutputName = "Intent.UnitTesting.UnitTestInstructionTemplate";

        private readonly IPersistenceLoader _persistenceLoader;

        public Migration_01_02_01_Pre_01(IPersistenceLoader persistenceLoader)
        {
            _persistenceLoader = persistenceLoader;
        }

        [IntentFully]
        public string ModuleId => "Intent.UnitTesting";
        [IntentFully]
        public string ModuleVersion => "1.2.1-pre.1";

        public void Up()
        {
            var designer = _persistenceLoader.LoadCurrentApplication().GetDesigner(CodebaseStructureDesignerId);
            if (designer == null)
            {
                return;
            }

            foreach (var package in designer.GetPackages())
            {
                var skillsFolders = package.GetElementsOfType(OutputAnchorSpecializationId)
                    .Where(x => x.Name == SkillsAnchorName)
                    .Select(x => x.Parent)
                    .OfType<IElementPersistable>()
                    .ToArray();
                if (skillsFolders.Length < 2)
                {
                    continue;
                }

                var outputs = package.GetElementsOfType(TemplateOutputSpecializationId)
                    .Where(x => x.Name == SkillTemplateOutputName)
                    .ToArray();
                var skillsFolderIds = skillsFolders.Select(x => x.Id).ToHashSet();

                // Every output after the first in a skills folder is a duplicate.
                var surplus = outputs
                    .Where(x => x.ParentFolderId != null && skillsFolderIds.Contains(x.ParentFolderId))
                    .GroupBy(x => x.ParentFolderId)
                    .SelectMany(x => x.Skip(1))
                    .ToArray();
                if (surplus.Length == 0)
                {
                    continue;
                }

                var occupiedFolderIds = outputs.Select(x => x.ParentFolderId).ToHashSet();
                var freeFolders = new Queue<IElementPersistable>(skillsFolders.Where(x => !occupiedFolderIds.Contains(x.Id)));

                foreach (var output in surplus)
                {
                    if (freeFolders.Count > 0)
                    {
                        MoveTo(output, freeFolders.Dequeue(), package);
                    }
                    else
                    {
                        Remove(output, package);
                    }
                }

                package.Save();
            }
        }

        public void Down()
        {
        }

        private static void Remove(IElementPersistable output, IPackageModelPersistable package)
        {
            if (output.Parent != null)
            {
                output.Parent.ChildElements.Remove(output);
            }
            else
            {
                package.Classes.Remove(output);
            }
        }

        private static void MoveTo(IElementPersistable output, IElementPersistable targetFolder, IPackageModelPersistable package)
        {
            // The persistence API has no move, so the element is removed and added again under the target folder.
            // Intent Architect gives the re-added output a new id; nothing references a template output by id.
            Remove(output, package);

            var moved = targetFolder.ChildElements.Add(
                id: output.Id,
                specializationType: output.SpecializationType,
                specializationTypeId: output.SpecializationTypeId,
                name: output.Name,
                parentId: targetFolder.Id,
                externalReference: output.ExternalReference);

            foreach (var stereotype in output.Stereotypes.Where(s => !moved.Stereotypes.Exists(s.DefinitionId)))
            {
                moved.Stereotypes.Add(
                    definitionId: stereotype.DefinitionId,
                    name: stereotype.Name,
                    definitionPackageId: stereotype.DefinitionPackageId,
                    definitionPackageName: stereotype.DefinitionPackageName,
                    configure: copy =>
                    {
                        copy.AddedByDefault = stereotype.AddedByDefault;
                        foreach (var property in stereotype.Properties)
                        {
                            copy.Properties.Add(property.DefinitionId, property.Name, property.Value, p => p.IsActive = property.IsActive);
                        }
                    });
            }

            foreach (var entry in output.Metadata)
            {
                if (moved.Metadata.Exists(entry.Key))
                {
                    moved.Metadata.Update(entry.Key, entry.Value);
                }
                else
                {
                    moved.Metadata.Add(entry.Key, entry.Value);
                }
            }
        }
    }
}
