using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace Entities.PrivateSetters.MongoDb.Application
{
    public class CreateTagDto
    {
        public CreateTagDto()
        {
        }

        public string Name { get; set; } = null!;

        public static CreateTagDto Create(string name)
        {
            return new CreateTagDto
            {
                Name = name
            };
        }
    }
}