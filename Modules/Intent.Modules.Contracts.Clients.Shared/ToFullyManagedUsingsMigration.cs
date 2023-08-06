using System;
using Intent.Modules.Common.Templates;

namespace Intent.Modules.Contracts.Clients.Shared;

public class ToFullyManagedUsingsMigration : ITemplateMigration
{
    private readonly int _version;

    public static RoslynMergeConfig GetConfig(string id, int version)
    {
        return new RoslynMergeConfig(new TemplateMetadata(id, $"{version:D}.0"), new ToFullyManagedUsingsMigration(version));
    }

    private ToFullyManagedUsingsMigration(int version)
    {
        _version = version;
    }

    public string Execute(string currentText)
    {
        var index = currentText.IndexOf("[assembly: IntentTemplate(", StringComparison.InvariantCulture);
        if (index < 0)
        {
            return currentText;
        }

        return $"{currentText[..index]}[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]{Environment.NewLine}{currentText[index..]}";
    }

    public TemplateMigrationCriteria Criteria => TemplateMigrationCriteria.Upgrade(_version - 1, _version);
}