using System;
using System.Collections.Generic;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;
using MongoDb.TestApplication.Application.Common.Mappings;
using MongoDb.TestApplication.Domain.Entities.Indexes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace MongoDb.TestApplication.Application.TextIndexEntitySingleParents
{
    public class TextIndexEntitySingleChildDto : IMapFrom<TextIndexEntitySingleChild>
    {
        public TextIndexEntitySingleChildDto()
        {
            FullText = null!;
        }

        public string FullText { get; set; }
        public Guid Id { get; set; }

        public static TextIndexEntitySingleChildDto Create(string fullText, Guid id)
        {
            return new TextIndexEntitySingleChildDto
            {
                FullText = fullText,
                Id = id
            };
        }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<TextIndexEntitySingleChild, TextIndexEntitySingleChildDto>();
        }
    }
}