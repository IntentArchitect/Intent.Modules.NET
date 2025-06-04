using AspNetCoreCleanArchitecture.Sample.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace AspNetCoreCleanArchitecture.Sample.Application.Buyers.ActivateBuyer
{
    public class ActivateBuyerCommand : IRequest, ICommand
    {
        public ActivateBuyerCommand(Guid id, bool isActive)
        {
            Id = id;
            IsActive = isActive;
        }

        public Guid Id { get; set; }
        public bool IsActive { get; set; }
    }
}