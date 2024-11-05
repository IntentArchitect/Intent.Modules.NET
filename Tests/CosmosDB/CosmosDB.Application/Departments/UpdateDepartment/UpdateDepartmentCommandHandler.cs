using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CosmosDB.Domain.Common.Exceptions;
using CosmosDB.Domain.Repositories;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace CosmosDB.Application.Departments.UpdateDepartment
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class UpdateDepartmentCommandHandler : IRequestHandler<UpdateDepartmentCommand>
    {
        private readonly IDepartmentRepository _departmentRepository;

        [IntentManaged(Mode.Merge)]
        public UpdateDepartmentCommandHandler(IDepartmentRepository departmentRepository)
        {
            _departmentRepository = departmentRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task Handle(UpdateDepartmentCommand request, CancellationToken cancellationToken)
        {
            var existingDepartment = await _departmentRepository.FindByIdAsync(request.Id, cancellationToken);
            if (existingDepartment is null)
            {
                throw new NotFoundException($"Could not find Department '{request.Id}'");
            }

            existingDepartment.UniversityId = request.UniversityId;
            existingDepartment.Name = request.Name;

            _departmentRepository.Update(existingDepartment);
        }
    }
}