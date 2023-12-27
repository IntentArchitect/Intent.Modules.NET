using System;
using System.Collections.Generic;
using AdvancedMappingCrud.Repositories.Tests.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Application.Customers.UpdateCorporateFuneralCoverQuote
{
    public class UpdateCorporateFuneralCoverQuoteCommand : IRequest, ICommand
    {
        public UpdateCorporateFuneralCoverQuoteCommand(string refNo,
            Guid personId,
            string? personEmail,
            List<CreateFuneralCoverQuoteCommandQuoteLinesDto> quoteLines,
            string corporate,
            string registration,
            Guid id,
            decimal amount)
        {
            RefNo = refNo;
            PersonId = personId;
            PersonEmail = personEmail;
            QuoteLines = quoteLines;
            Corporate = corporate;
            Registration = registration;
            Id = id;
            Amount = amount;
        }

        public string RefNo { get; set; }
        public Guid PersonId { get; set; }
        public string? PersonEmail { get; set; }
        public List<CreateFuneralCoverQuoteCommandQuoteLinesDto> QuoteLines { get; set; }
        public string Corporate { get; set; }
        public string Registration { get; set; }
        public Guid Id { get; set; }
        public decimal Amount { get; set; }
    }
}