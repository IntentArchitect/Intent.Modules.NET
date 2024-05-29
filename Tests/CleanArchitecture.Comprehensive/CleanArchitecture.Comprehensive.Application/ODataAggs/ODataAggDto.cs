using System;
using AutoMapper;
using CleanArchitecture.Comprehensive.Application.Common.Mappings;
using CleanArchitecture.Comprehensive.Domain.Entities.ODataQuery;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace CleanArchitecture.Comprehensive.Application.ODataAggs
{
    public class ODataAggDto : IMapFrom<ODataAgg>
    {
        public ODataAggDto()
        {
            Name = null!;
        }

        public Guid Id { get; set; }
        public string Name { get; set; }

        public static ODataAggDto Create(Guid id, string name)
        {
            return new ODataAggDto
            {
                Id = id,
                Name = name
            };
        }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<ODataAgg, ODataAggDto>();
        }
    }
}