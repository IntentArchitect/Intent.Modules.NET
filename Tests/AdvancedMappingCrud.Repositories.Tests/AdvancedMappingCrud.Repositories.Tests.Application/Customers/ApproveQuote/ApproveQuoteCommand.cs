using System;
using AdvancedMappingCrud.Repositories.Tests.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Application.Customers.ApproveQuote
{
    public class ApproveQuoteCommand : IRequest, ICommand
    {
        public ApproveQuoteCommand(Guid quoteId)
        {
            QuoteId = quoteId;
        }

        public Guid QuoteId { get; set; }
    }
}