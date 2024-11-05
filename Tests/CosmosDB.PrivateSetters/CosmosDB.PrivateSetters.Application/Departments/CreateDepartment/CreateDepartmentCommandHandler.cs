using System;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace CosmosDB.PrivateSetters.Application.Departments.CreateDepartment
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class CreateDepartmentCommandHandler : IRequestHandler<CreateDepartmentCommand, Guid>
    {
        [IntentManaged(Mode.Merge)]
        public CreateDepartmentCommandHandler()
        {
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public async Task<Guid> Handle(CreateDepartmentCommand request, CancellationToken cancellationToken)
        {
            // TODO: Implement Handle (CreateDepartmentCommandHandler) functionality
            throw new NotImplementedException("Your implementation here...");
        }
    }
}