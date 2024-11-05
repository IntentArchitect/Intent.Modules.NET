using System;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace CosmosDB.PrivateSetters.Application.Departments.UpdateDepartment
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class UpdateDepartmentCommandHandler : IRequestHandler<UpdateDepartmentCommand>
    {
        [IntentManaged(Mode.Merge)]
        public UpdateDepartmentCommandHandler()
        {
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public async Task Handle(UpdateDepartmentCommand request, CancellationToken cancellationToken)
        {
            // TODO: Implement Handle (UpdateDepartmentCommandHandler) functionality
            throw new NotImplementedException("Your implementation here...");
        }
    }
}