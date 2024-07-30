using System;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace OpenApiImporterTest.Application.Accounts
{
    public class UpdateAccountCommand
    {
        public UpdateAccountCommand()
        {
            Name = null!;
        }

        public string Name { get; set; }
        public Guid Id { get; set; }

        public static UpdateAccountCommand Create(string name, Guid id)
        {
            return new UpdateAccountCommand
            {
                Name = name,
                Id = id
            };
        }
    }
}