using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace FluentValidationTest.Domain.Entities.ValidationScenarios.ConstructorOperationConstraints
{
    public class ConstructedConstrainedEntity
    {
        public ConstructedConstrainedEntity(string title, string code)
        {
        }

        /// <summary>
        /// Required by Entity Framework.
        /// </summary>
        protected ConstructedConstrainedEntity()
        {
            Title = null!;
            Code = null!;
        }

        public Guid Id { get; set; }

        public string Title { get; set; }

        public string Code { get; set; }

        public string? OptionalComment { get; set; }

        public void Rename(string newTitle, string newCode)
        {
            // TODO: Implement Rename (ConstructedConstrainedEntity) functionality
            throw new NotImplementedException("Replace with your implementation...");
        }
    }
}