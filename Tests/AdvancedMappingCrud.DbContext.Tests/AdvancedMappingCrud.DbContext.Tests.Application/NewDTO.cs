using System;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace AdvancedMappingCrud.DbContext.Tests.Application
{
    public class NewDTO
    {
        public NewDTO()
        {
            Line1 = null!;
            Line2 = null!;
            City = null!;
        }

        public Guid Id { get; set; }
        public string Line1 { get; set; }
        public string Line2 { get; set; }
        public string City { get; set; }
        public int Postal { get; set; }

        public static NewDTO Create(Guid id, string line1, string line2, string city, int postal)
        {
            return new NewDTO
            {
                Id = id,
                Line1 = line1,
                Line2 = line2,
                City = city,
                Postal = postal
            };
        }
    }
}