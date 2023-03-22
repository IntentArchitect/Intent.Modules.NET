using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CleanArchitecture.TestApplication.Domain.Repositories;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "1.0")]

namespace CleanArchitecture.TestApplication.Application.VariantTypesClasses.UpdateVariantTypesClass
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class UpdateVariantTypesClassCommandHandler : IRequestHandler<UpdateVariantTypesClassCommand>
    {
        private readonly IVariantTypesClassRepository _variantTypesClassRepository;

        [IntentManaged(Mode.Ignore)]
        public UpdateVariantTypesClassCommandHandler(IVariantTypesClassRepository variantTypesClassRepository)
        {
            _variantTypesClassRepository = variantTypesClassRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<Unit> Handle(UpdateVariantTypesClassCommand request, CancellationToken cancellationToken)
        {
            var existingVariantTypesClass = await _variantTypesClassRepository.FindByIdAsync(request.Id, cancellationToken);
            existingVariantTypesClass.StrCollection = request.StrCollection.ToList();
            existingVariantTypesClass.IntCollection = request.IntCollection.ToList();
            existingVariantTypesClass.StrNullCollection = request.StrNullCollection?.ToList();
            existingVariantTypesClass.IntNullCollection = request.IntNullCollection?.ToList();
            existingVariantTypesClass.NullStr = request.NullStr;
            existingVariantTypesClass.NullInt = request.NullInt;
            return Unit.Value;
        }
    }
}