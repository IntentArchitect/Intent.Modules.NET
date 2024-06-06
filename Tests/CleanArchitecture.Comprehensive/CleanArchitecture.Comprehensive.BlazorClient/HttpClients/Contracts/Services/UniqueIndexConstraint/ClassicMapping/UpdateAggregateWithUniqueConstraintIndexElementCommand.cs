using System.ComponentModel.DataAnnotations;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.Blazor.HttpClients.DtoContract", Version = "2.0")]

namespace CleanArchitecture.Comprehensive.BlazorClient.HttpClients.Contracts.Services.UniqueIndexConstraint.ClassicMapping
{
    public class UpdateAggregateWithUniqueConstraintIndexElementCommand
    {
        public UpdateAggregateWithUniqueConstraintIndexElementCommand()
        {
            SingleUniqueField = null!;
            CompUniqueFieldA = null!;
            CompUniqueFieldB = null!;
        }

        public Guid Id { get; set; }
        [Required(ErrorMessage = "Single unique field is required.")]
        [MaxLength(256, ErrorMessage = "Single unique field must be 256 or less characters.")]
        public string SingleUniqueField { get; set; }
        [Required(ErrorMessage = "Comp unique field a is required.")]
        [MaxLength(256, ErrorMessage = "Comp unique field a must be 256 or less characters.")]
        public string CompUniqueFieldA { get; set; }
        [Required(ErrorMessage = "Comp unique field b is required.")]
        [MaxLength(256, ErrorMessage = "Comp unique field b must be 256 or less characters.")]
        public string CompUniqueFieldB { get; set; }

        public static UpdateAggregateWithUniqueConstraintIndexElementCommand Create(
            Guid id,
            string singleUniqueField,
            string compUniqueFieldA,
            string compUniqueFieldB)
        {
            return new UpdateAggregateWithUniqueConstraintIndexElementCommand
            {
                Id = id,
                SingleUniqueField = singleUniqueField,
                CompUniqueFieldA = compUniqueFieldA,
                CompUniqueFieldB = compUniqueFieldB
            };
        }
    }
}