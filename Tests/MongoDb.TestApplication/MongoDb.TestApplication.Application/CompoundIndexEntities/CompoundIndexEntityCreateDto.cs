using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace MongoDb.TestApplication.Application.CompoundIndexEntities
{
    public class CompoundIndexEntityCreateDto
    {
        public CompoundIndexEntityCreateDto()
        {
            SomeField = null!;
            CompoundOne = null!;
            CompoundTwo = null!;
        }

        public string SomeField { get; set; }
        public string CompoundOne { get; set; }
        public string CompoundTwo { get; set; }

        public static CompoundIndexEntityCreateDto Create(string someField, string compoundOne, string compoundTwo)
        {
            return new CompoundIndexEntityCreateDto
            {
                SomeField = someField,
                CompoundOne = compoundOne,
                CompoundTwo = compoundTwo
            };
        }
    }
}