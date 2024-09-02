using System;
using AdvancedMappingCrud.RichDomain.Tests.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace AdvancedMappingCrud.RichDomain.Tests.Application.Products.DoManual
{
    public class DoManualCommand : IRequest, ICommand
    {
        public DoManualCommand(Guid id)
        {
            Id = id;
        }

        public Guid Id { get; set; }
    }
}