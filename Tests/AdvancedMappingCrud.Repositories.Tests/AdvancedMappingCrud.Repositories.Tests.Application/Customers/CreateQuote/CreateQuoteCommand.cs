using System;
using AdvancedMappingCrud.Repositories.Tests.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Application.Customers.CreateQuote
{
    public class CreateQuoteCommand : IRequest, ICommand
    {
        public CreateQuoteCommand(string refNo, Guid personId, string? personEmail)
        {
            RefNo = refNo;
            PersonId = personId;
            PersonEmail = personEmail;
        }

        public string RefNo { get; set; }
        public Guid PersonId { get; set; }
        public string? PersonEmail { get; set; }
    }
}