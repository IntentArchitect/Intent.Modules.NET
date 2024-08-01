using System;
using System.Threading;
using System.Threading.Tasks;
using CleanArchitecture.Comprehensive.Domain.Common.Exceptions;
using CleanArchitecture.Comprehensive.Domain.Repositories.Geometry;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace CleanArchitecture.Comprehensive.Application.GeometryTypes.DeleteGeometryType
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class DeleteGeometryTypeCommandHandler : IRequestHandler<DeleteGeometryTypeCommand>
    {
        private readonly IGeometryTypeRepository _geometryTypeRepository;

        [IntentManaged(Mode.Merge)]
        public DeleteGeometryTypeCommandHandler(IGeometryTypeRepository geometryTypeRepository)
        {
            _geometryTypeRepository = geometryTypeRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task Handle(DeleteGeometryTypeCommand request, CancellationToken cancellationToken)
        {
            var geometryType = await _geometryTypeRepository.FindByIdAsync(request.Id, cancellationToken);
            if (geometryType is null)
            {
                throw new NotFoundException($"Could not find GeometryType '{request.Id}'");
            }

            _geometryTypeRepository.Remove(geometryType);
        }
    }
}