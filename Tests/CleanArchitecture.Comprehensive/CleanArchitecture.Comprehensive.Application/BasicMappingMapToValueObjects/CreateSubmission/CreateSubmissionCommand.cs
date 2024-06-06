using System;
using System.Collections.Generic;
using CleanArchitecture.Comprehensive.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace CleanArchitecture.Comprehensive.Application.BasicMappingMapToValueObjects.CreateSubmission
{
    public class CreateSubmissionCommand : IRequest<Guid>, ICommand
    {
        public CreateSubmissionCommand(string submissionType, List<CreateSubmissionItemDto> items)
        {
            SubmissionType = submissionType;
            Items = items;
        }

        public string SubmissionType { get; set; }
        public List<CreateSubmissionItemDto> Items { get; set; }
    }
}