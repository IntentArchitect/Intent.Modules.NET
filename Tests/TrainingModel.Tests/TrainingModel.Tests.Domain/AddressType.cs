using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DomainEnum", Version = "1.0")]

namespace TrainingModel.Tests.Domain
{
    public enum AddressType
    {
        Delivery = 1,
        Billing = 2
    }
}