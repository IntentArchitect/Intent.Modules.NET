using System;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace EntityFrameworkCore.SqlServer.TestApplication.Domain.Entities.TPT.Polymorphic
{
    [DefaultIntentManaged(Mode.Fully, Targets = Targets.Methods, Body = Mode.Ignore, AccessModifiers = AccessModifiers.Public)]
    public class TPT_Poly_BaseClassNonAbstract : TPT_Poly_RootAbstract
    {
        public string BaseField { get; set; }

        public Guid? Poly_SecondLevelId { get; set; }
    }
}