using AspNetCoreCleanArchitecture.Sample.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace AspNetCoreCleanArchitecture.Sample.Application.Buyers.DeleteBuyer
{
    public class DeleteBuyerCommand : IRequest, ICommand
    {
        public DeleteBuyerCommand(Guid id)
        {
            Id = id;
        }

        public Guid Id { get; set; }
    }
}