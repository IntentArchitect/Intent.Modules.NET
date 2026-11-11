using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace EntityFrameworkCore.SqlServer.EF11.Domain.TPC.Polymorphic
{
    public class TPC_Poly_SecondLevel
    {
        public TPC_Poly_SecondLevel()
        {
            SecondField = null!;
        }

        public Guid Id { get; set; }

        public string SecondField { get; set; }

        public virtual ICollection<TPC_Poly_BaseClassNonAbstract> BaseClassNonAbstracts { get; set; } = [];
    }
}