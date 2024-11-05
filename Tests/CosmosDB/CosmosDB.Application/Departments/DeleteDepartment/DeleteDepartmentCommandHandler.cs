using System;
using System.Threading;
using System.Threading.Tasks;
using CosmosDB.Domain.Common.Exceptions;
using CosmosDB.Domain.Repositories;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace CosmosDB.Application.Departments.DeleteDepartment
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class DeleteDepartmentCommandHandler : IRequestHandler<DeleteDepartmentCommand>
    {
        private readonly IDepartmentRepository _departmentRepository;

        [IntentManaged(Mode.Merge)]
        public DeleteDepartmentCommandHandler(IDepartmentRepository departmentRepository)
        {
            _departmentRepository = departmentRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task Handle(DeleteDepartmentCommand request, CancellationToken cancellationToken)
        {
            var existingDepartment = await _departmentRepository.FindByIdAsync(request.Id, cancellationToken);
            if (existingDepartment is null)
            {
                throw new NotFoundException($"Could not find Department '{request.Id}'");
            }

            _departmentRepository.Remove(existingDepartment);
        }
    }
}