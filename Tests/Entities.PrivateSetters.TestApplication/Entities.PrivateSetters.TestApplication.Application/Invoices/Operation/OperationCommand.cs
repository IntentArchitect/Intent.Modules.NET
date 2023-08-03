using System;
using System.Collections.Generic;
using Entities.PrivateSetters.TestApplication.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace Entities.PrivateSetters.TestApplication.Application.Invoices.Operation
{
    public class OperationCommand : IRequest, ICommand
    {
        public OperationCommand(DateTime date, List<OperationLineDataContractDto> lines, Guid id)
        {
            Date = date;
            Lines = lines;
            Id = id;
        }

        public DateTime Date { get; set; }
        public List<OperationLineDataContractDto> Lines { get; set; }
        public Guid Id { get; set; }
    }
}