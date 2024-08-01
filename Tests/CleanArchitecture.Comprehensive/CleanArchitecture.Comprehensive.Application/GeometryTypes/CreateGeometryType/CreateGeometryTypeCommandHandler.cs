using System;
using System.Threading;
using System.Threading.Tasks;
using CleanArchitecture.Comprehensive.Domain.Entities.Geometry;
using CleanArchitecture.Comprehensive.Domain.Repositories.Geometry;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace CleanArchitecture.Comprehensive.Application.GeometryTypes.CreateGeometryType
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class CreateGeometryTypeCommandHandler : IRequestHandler<CreateGeometryTypeCommand, Guid>
    {
        private readonly IGeometryTypeRepository _geometryTypeRepository;

        [IntentManaged(Mode.Merge)]
        public CreateGeometryTypeCommandHandler(IGeometryTypeRepository geometryTypeRepository)
        {
            _geometryTypeRepository = geometryTypeRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<Guid> Handle(CreateGeometryTypeCommand request, CancellationToken cancellationToken)
        {
            var geometryType = new GeometryType
            {
                Point = request.Point
            };

            _geometryTypeRepository.Add(geometryType);
            await _geometryTypeRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
            return geometryType.Id;
        }
    }
}