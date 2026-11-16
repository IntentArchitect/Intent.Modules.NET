using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace EntityFrameworkCore.SqlServer.EF11.Domain.TPH.Polymorphic
{
    public class TPH_Poly_SecondLevel
    {
        public TPH_Poly_SecondLevel()
        {
            SecondField = null!;
        }

        public Guid Id { get; set; }

        public string SecondField { get; set; }

        public virtual ICollection<TPH_Poly_BaseClassNonAbstract> BaseClassNonAbstracts { get; set; } = [];
    }
}