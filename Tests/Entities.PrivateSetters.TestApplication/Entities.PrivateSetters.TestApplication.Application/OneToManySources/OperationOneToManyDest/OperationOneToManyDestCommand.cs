using System;
using Entities.PrivateSetters.TestApplication.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace Entities.PrivateSetters.TestApplication.Application.OneToManySources.OperationOneToManyDest
{
    public class OperationOneToManyDestCommand : IRequest, ICommand
    {
        public OperationOneToManyDestCommand(Guid oneToManySourceid, Guid id, string attribute)
        {
            OneToManySourceid = oneToManySourceid;
            Id = id;
            Attribute = attribute;
        }

        public Guid OneToManySourceid { get; set; }
        public Guid Id { get; set; }
        public string Attribute { get; set; }
    }
}