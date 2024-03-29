using System;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace EntityFrameworkCore.SqlServer.TestApplication.Domain.Entities.TPH.Polymorphic
{
    [DefaultIntentManaged(Mode.Fully, Targets = Targets.Methods, Body = Mode.Ignore, AccessModifiers = AccessModifiers.Public)]
    public class TPH_Poly_BaseClassNonAbstract : TPH_Poly_RootAbstract
    {
        public string BaseField { get; set; }

        public Guid? Poly_SecondLevelId { get; set; }
    }
}