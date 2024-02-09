using Intent.RoslynWeaver.Attributes;
using MediatR;
using Redis.Om.Repositories.Application.Common.Interfaces;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace Redis.Om.Repositories.Application.DerivedTypes.DeleteDerivedType
{
    public class DeleteDerivedTypeCommand : IRequest, ICommand
    {
        public DeleteDerivedTypeCommand(string id)
        {
            Id = id;
        }

        public string Id { get; set; }
    }
}