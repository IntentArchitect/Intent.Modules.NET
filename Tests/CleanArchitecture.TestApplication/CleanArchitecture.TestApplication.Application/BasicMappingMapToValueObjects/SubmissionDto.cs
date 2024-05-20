using System;
using AutoMapper;
using CleanArchitecture.TestApplication.Application.Common.Mappings;
using CleanArchitecture.TestApplication.Domain.Entities.BasicMappingMapToValueObjects;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace CleanArchitecture.TestApplication.Application.BasicMappingMapToValueObjects
{
    public class SubmissionDto : IMapFrom<Submission>
    {
        public SubmissionDto()
        {
            SubmissionType = null!;
        }

        public Guid Id { get; set; }
        public string SubmissionType { get; set; }

        public static SubmissionDto Create(Guid id, string submissionType)
        {
            return new SubmissionDto
            {
                Id = id,
                SubmissionType = submissionType
            };
        }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<Submission, SubmissionDto>();
        }
    }
}