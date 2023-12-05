using System;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace EntityFrameworkCore.SqlServer.TestApplication.Domain.Entities.TPC.Polymorphic
{
    [DefaultIntentManaged(Mode.Fully, Targets = Targets.Methods, Body = Mode.Ignore, AccessModifiers = AccessModifiers.Public)]
    public class TPC_Poly_BaseClassNonAbstract : TPC_Poly_RootAbstract
    {
        public Guid Id { get; set; }

        public string BaseField { get; set; }

        public Guid? Poly_SecondLevelId { get; set; }
    }
}