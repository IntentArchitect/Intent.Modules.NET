using System;
using Intent.RoslynWeaver.Attributes;
using MediatR;
using SharedKernel.Consumer.Tests.Application.Common.Interfaces;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace SharedKernel.Consumer.Tests.Application.Currencies.DeleteCurrency
{
    public class DeleteCurrencyCommand : IRequest, ICommand
    {
        public DeleteCurrencyCommand(Guid id)
        {
            Id = id;
        }

        public Guid Id { get; set; }
    }
}