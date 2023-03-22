using System;
using System.Threading;
using System.Threading.Tasks;
using CleanArchitecture.TestApplication.Domain.Repositories;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "1.0")]

namespace CleanArchitecture.TestApplication.Application.VariantTypesClasses.DeleteVariantTypesClass
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class DeleteVariantTypesClassCommandHandler : IRequestHandler<DeleteVariantTypesClassCommand>
    {
        private readonly IVariantTypesClassRepository _variantTypesClassRepository;

        [IntentManaged(Mode.Ignore)]
        public DeleteVariantTypesClassCommandHandler(IVariantTypesClassRepository variantTypesClassRepository)
        {
            _variantTypesClassRepository = variantTypesClassRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<Unit> Handle(DeleteVariantTypesClassCommand request, CancellationToken cancellationToken)
        {
            var existingVariantTypesClass = await _variantTypesClassRepository.FindByIdAsync(request.Id, cancellationToken);
            _variantTypesClassRepository.Remove(existingVariantTypesClass);
            return Unit.Value;
        }
    }
}