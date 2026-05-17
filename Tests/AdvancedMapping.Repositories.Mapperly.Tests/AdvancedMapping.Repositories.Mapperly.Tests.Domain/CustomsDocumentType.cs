using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DomainEnum", Version = "1.0")]

namespace AdvancedMapping.Repositories.Mapperly.Tests.Domain
{
    public enum CustomsDocumentType
    {
        SAD500 = 1,
        MRN = 2
    }
}