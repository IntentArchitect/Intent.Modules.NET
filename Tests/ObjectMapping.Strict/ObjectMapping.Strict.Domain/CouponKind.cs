using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DomainEnum", Version = "1.0")]

namespace ObjectMapping.Strict.Domain
{
    public enum CouponKind
    {
        Percentage,
        FixedAmount
    }
}