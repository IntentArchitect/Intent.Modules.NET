using Intent.RoslynWeaver.Attributes;

[assembly: IntentTagModeImplicit]

namespace EntityFrameworkCore.CosmosDb.TestApplication.Domain.Entities.Polymorphic
{
    [DefaultIntentManaged(Mode.Fully, Targets = Targets.Methods, Body = Mode.Ignore, AccessModifiers = AccessModifiers.Public)]
    public class Poly_ConcreteB : Poly_BaseClassNonAbstract
    {
        public string ConcreteField { get; set; }
    }
}