using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace EntityFrameworkCore.SqlServer.EF11.Domain.TPC.Polymorphic
{
    public class TPC_Poly_TopLevel
    {
        public TPC_Poly_TopLevel()
        {
            TopField = null!;
        }

        public Guid Id { get; set; }

        public string TopField { get; set; }
    }
}