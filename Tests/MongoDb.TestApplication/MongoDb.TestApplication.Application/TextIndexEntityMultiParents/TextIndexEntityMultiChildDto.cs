using System;
using System.Collections.Generic;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;
using MongoDb.TestApplication.Application.Common.Mappings;
using MongoDb.TestApplication.Domain.Entities.Indexes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace MongoDb.TestApplication.Application.TextIndexEntityMultiParents
{
    public class TextIndexEntityMultiChildDto : IMapFrom<TextIndexEntityMultiChild>
    {
        public TextIndexEntityMultiChildDto()
        {
            FullText = null!;
        }

        public string FullText { get; set; }
        public Guid Id { get; set; }

        public static TextIndexEntityMultiChildDto Create(string fullText, Guid id)
        {
            return new TextIndexEntityMultiChildDto
            {
                FullText = fullText,
                Id = id
            };
        }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<TextIndexEntityMultiChild, TextIndexEntityMultiChildDto>();
        }
    }
}