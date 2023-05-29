using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace MongoDb.TestApplication.Application.CompoundIndexEntities
{
    public class CompoundIndexEntityUpdateDto
    {
        public CompoundIndexEntityUpdateDto()
        {
            Id = null!;
            SomeField = null!;
            CompoundOne = null!;
            CompoundTwo = null!;
        }

        public string Id { get; set; }
        public string SomeField { get; set; }
        public string CompoundOne { get; set; }
        public string CompoundTwo { get; set; }

        public static CompoundIndexEntityUpdateDto Create(string id, string someField, string compoundOne, string compoundTwo)
        {
            return new CompoundIndexEntityUpdateDto
            {
                Id = id,
                SomeField = someField,
                CompoundOne = compoundOne,
                CompoundTwo = compoundTwo
            };
        }
    }
}