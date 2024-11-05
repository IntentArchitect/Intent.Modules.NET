using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CosmosDB.Domain.Entities;
using CosmosDB.Domain.Repositories;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace CosmosDB.Application.Departments.CreateDepartment
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class CreateDepartmentCommandHandler : IRequestHandler<CreateDepartmentCommand, Guid>
    {
        private readonly IDepartmentRepository _departmentRepository;

        [IntentManaged(Mode.Merge)]
        public CreateDepartmentCommandHandler(IDepartmentRepository departmentRepository)
        {
            _departmentRepository = departmentRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<Guid> Handle(CreateDepartmentCommand request, CancellationToken cancellationToken)
        {
            var newDepartment = new Department
            {
                UniversityId = request.UniversityId,
                Name = request.Name,
            };

            _departmentRepository.Add(newDepartment);
            await _departmentRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
            return newDepartment.Id;
        }
    }
}