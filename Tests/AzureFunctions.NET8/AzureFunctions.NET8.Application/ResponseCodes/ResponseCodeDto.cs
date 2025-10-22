using System;
using AutoMapper;
using AzureFunctions.NET8.Application.Common.Mappings;
using AzureFunctions.NET8.Domain.Entities;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace AzureFunctions.NET8.Application.ResponseCodes
{
    public class ResponseCodeDto : IMapFrom<ResponseCode>
    {
        public ResponseCodeDto()
        {
            Name = null!;
        }

        public Guid Id { get; set; }
        public string Name { get; set; }

        public static ResponseCodeDto Create(Guid id, string name)
        {
            return new ResponseCodeDto
            {
                Id = id,
                Name = name
            };
        }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<ResponseCode, ResponseCodeDto>();
        }
    }
}