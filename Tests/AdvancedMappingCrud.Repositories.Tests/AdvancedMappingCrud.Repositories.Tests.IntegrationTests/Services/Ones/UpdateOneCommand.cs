using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.DtoContract", Version = "2.0")]

namespace AdvancedMappingCrud.Repositories.Tests.IntegrationTests.Services.Ones
{
    public class UpdateOneCommand
    {
        public UpdateOneCommand()
        {
            OneName = null!;
            Two = null!;
        }

        public Guid Id { get; set; }
        public string OneName { get; set; }
        public UpdateOneTwoDto Two { get; set; }

        public static UpdateOneCommand Create(Guid id, string oneName, UpdateOneTwoDto two)
        {
            return new UpdateOneCommand
            {
                Id = id,
                OneName = oneName,
                Two = two
            };
        }
    }
}