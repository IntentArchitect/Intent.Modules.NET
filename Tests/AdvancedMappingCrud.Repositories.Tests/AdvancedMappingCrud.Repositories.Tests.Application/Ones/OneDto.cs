using System;
using System.Collections.Generic;
using AdvancedMappingCrud.Repositories.Tests.Application.Common.Mappings;
using AdvancedMappingCrud.Repositories.Tests.Domain.Entities.NullableNested;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Application.Ones
{
    public class OneDto : IMapFrom<One>
    {
        public OneDto()
        {
            OneName1 = null!;
            Fives = null!;
            FourName4 = null!;
            ThreeName3 = null!;
            TwoName2 = null!;
        }

        public Guid Id { get; set; }
        public string OneName1 { get; set; }
        public List<OneFiveDto> Fives { get; set; }
        public string FourName4 { get; set; }
        public string ThreeName3 { get; set; }
        public string TwoName2 { get; set; }
        public int OneAge1 { get; set; }
        public int FourAge4 { get; set; }
        public int ThreeAge3 { get; set; }
        public int TwoAge2 { get; set; }

        public static OneDto Create(
            Guid id,
            string oneName1,
            List<OneFiveDto> fives,
            string fourName4,
            string threeName3,
            string twoName2,
            int oneAge1,
            int fourAge4,
            int threeAge3,
            int twoAge2)
        {
            return new OneDto
            {
                Id = id,
                OneName1 = oneName1,
                Fives = fives,
                FourName4 = fourName4,
                ThreeName3 = threeName3,
                TwoName2 = twoName2,
                OneAge1 = oneAge1,
                FourAge4 = fourAge4,
                ThreeAge3 = threeAge3,
                TwoAge2 = twoAge2
            };
        }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<One, OneDto>()
                .ForMember(d => d.OneName1, opt => opt.MapFrom(src => src.OneName))
                .ForMember(d => d.Fives, opt => opt.MapFrom(src => src.Fives))
                .ForMember(d => d.FourName4, opt => opt.MapFrom(src => src.Four != null ? src.Four!.FourName : null))
                .ForMember(d => d.ThreeName3, opt => opt.MapFrom(src => src.Four != null && src.Four.Three != null ? src.Four!.Three!.ThreeName : null))
                .ForMember(d => d.TwoName2, opt => opt.MapFrom(src => src.Two.TwoName))
                .ForMember(d => d.OneAge1, opt => opt.MapFrom(src => src.OneAge))
                .ForMember(d => d.FourAge4, opt => opt.MapFrom(src => src.Four != null ? src.Four!.FourAge : (int?)null))
                .ForMember(d => d.ThreeAge3, opt => opt.MapFrom(src => src.Four != null && src.Four.Three != null ? src.Four!.Three!.ThreeAge : (int?)null))
                .ForMember(d => d.TwoAge2, opt => opt.MapFrom(src => src.Two.TwoAge));
        }
    }
}