using System;
using System.Collections.Generic;
using CleanArchitecture.Comprehensive.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace CleanArchitecture.Comprehensive.Application.BasicMappingMapToValueObjects.UpdateSubmission
{
    public class UpdateSubmissionCommand : IRequest, ICommand
    {
        public UpdateSubmissionCommand(Guid id, string submissionType, List<UpdateSubmissionItemDto> items)
        {
            Id = id;
            SubmissionType = submissionType;
            Items = items;
        }

        public Guid Id { get; set; }
        public string SubmissionType { get; set; }
        public List<UpdateSubmissionItemDto> Items { get; set; }
    }
}