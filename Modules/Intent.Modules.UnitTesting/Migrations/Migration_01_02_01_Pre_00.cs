#nullable enable
using System.Linq;
using Intent.Persistence;
using Intent.Plugins;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Migrations.OnVersionMigration", Version = "1.0")]

namespace Intent.Modules.UnitTesting.Migrations
{
    /// <summary>
    /// <c>unit-test-guidelines</c> became a skill in this version, and its template's role moved from
    /// <c>AI.Context.Instructions</c> to <c>AI.Context.Skills</c>. A template output that is already placed stays in
    /// its folder on upgrade, so without this move the skill would be written to
    /// <c>.agents/instructions/unit-test-guidelines/SKILL.md</c>, where harnesses don't look for skills.
    /// </summary>
    public class Migration_01_02_01_Pre_00 : IModuleMigration
    {
        private const string CodebaseStructureDesignerId = "0701433c-36c0-4569-b1f4-9204986b587d";
        private const string OutputAnchorSpecializationId = "025e933b-b602-4b6d-95ab-0ec36ae940da";
        private const string TemplateOutputSpecializationId = "d421c322-7a51-4094-89fa-e5d8a0a97b27";
        private const string SkillsAnchorName = "AI.Context.Skills";
        private const string SkillTemplateOutputName = "Intent.UnitTesting.UnitTestInstructionTemplate";

        private readonly IPersistenceLoader _persistenceLoader;

        public Migration_01_02_01_Pre_00(IPersistenceLoader persistenceLoader)
        {
            _persistenceLoader = persistenceLoader;
        }

        [IntentFully]
        public string ModuleId => "Intent.UnitTesting";
        [IntentFully]
        public string ModuleVersion => "1.2.1-pre.0";

        public void Up()
        {
            var designer = _persistenceLoader.LoadCurrentApplication().GetDesigner(CodebaseStructureDesignerId);
            if (designer == null)
            {
                return;
            }

            foreach (var package in designer.GetPackages())
            {
                var skillsFolder = package.GetElementsOfType(OutputAnchorSpecializationId)
                    .FirstOrDefault(x => x.Name == SkillsAnchorName)?
                    .Parent;
                if (skillsFolder == null)
                {
                    continue;
                }

                var outputs = package.GetElementsOfType(TemplateOutputSpecializationId)
                    .Where(x => x.Name == SkillTemplateOutputName && x.ParentFolderId != skillsFolder.Id)
                    .ToArray();
                if (outputs.Length == 0)
                {
                    continue;
                }

                foreach (var output in outputs)
                {
                    MoveTo(output, skillsFolder, package);
                }

                package.Save();
            }
        }

        public void Down()
        {
        }

        private static void MoveTo(IElementPersistable output, IElementPersistable targetFolder, IPackageModelPersistable package)
        {
            if (output.Parent != null)
            {
                output.Parent.ChildElements.Remove(output);
            }
            else
            {
                package.Classes.Remove(output);
            }

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
