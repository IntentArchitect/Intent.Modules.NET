using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace OpenApiImporterTest.Application.Players
{
    public class MaxBetResultViewModel
    {
        public MaxBetResultViewModel()
        {
        }

        public int RejectReason { get; set; }
        public decimal Amount { get; set; }

        public static MaxBetResultViewModel Create(int rejectReason, decimal amount)
        {
            return new MaxBetResultViewModel
            {
                RejectReason = rejectReason,
                Amount = amount
            };
        }
    }
}