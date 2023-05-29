using System;
using System.Collections.Generic;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;
using MongoDb.TestApplication.Application.Common.Mappings;
using MongoDb.TestApplication.Domain.Entities.Indexes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace MongoDb.TestApplication.Application.TextIndexEntities
{
    public class TextIndexEntityDto : IMapFrom<TextIndexEntity>
    {
        public TextIndexEntityDto()
        {
            Id = null!;
            FullText = null!;
            SomeField = null!;
        }

        public string Id { get; set; }
        public string FullText { get; set; }
        public string SomeField { get; set; }

        public static TextIndexEntityDto Create(string id, string fullText, string someField)
        {
            return new TextIndexEntityDto
            {
                Id = id,
                FullText = fullText,
                SomeField = someField
            };
        }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<TextIndexEntity, TextIndexEntityDto>();
        }
    }
}