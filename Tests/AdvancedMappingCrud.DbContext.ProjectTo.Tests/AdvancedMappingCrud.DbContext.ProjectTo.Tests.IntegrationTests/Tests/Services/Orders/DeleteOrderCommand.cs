using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.DtoContract", Version = "2.0")]

namespace AdvancedMappingCrud.DbContext.ProjectTo.Tests.IntegrationTests.Tests.Services.Orders
{
    public class DeleteOrderCommand
    {
        public Guid Id { get; set; }

        public static DeleteOrderCommand Create(Guid id)
        {
            return new DeleteOrderCommand
            {
                Id = id
            };
        }
    }
}