using System;
using System.Collections.Generic;
using AdvancedMappingCrud.Repositories.Tests.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Application.Customers.CreateFuneralCoverQuote
{
    public class CreateFuneralCoverQuoteCommand : IRequest, ICommand
    {
        public CreateFuneralCoverQuoteCommand(string refNo,
            Guid personId,
            string? personEmail,
            List<CreateFuneralCoverQuoteCommandQuoteLinesDto> quoteLines)
        {
            RefNo = refNo;
            PersonId = personId;
            PersonEmail = personEmail;
            QuoteLines = quoteLines;
        }

        public string RefNo { get; set; }
        public Guid PersonId { get; set; }
        public string? PersonEmail { get; set; }
        public List<CreateFuneralCoverQuoteCommandQuoteLinesDto> QuoteLines { get; set; }
    }
}