using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CleanArchitecture.TestApplication.Domain.Entities;
using CleanArchitecture.TestApplication.Domain.Repositories;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "1.0")]

namespace CleanArchitecture.TestApplication.Application.VariantTypesClasses.CreateVariantTypesClass
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class CreateVariantTypesClassCommandHandler : IRequestHandler<CreateVariantTypesClassCommand, Guid>
    {
        private readonly IVariantTypesClassRepository _variantTypesClassRepository;

        [IntentManaged(Mode.Ignore)]
        public CreateVariantTypesClassCommandHandler(IVariantTypesClassRepository variantTypesClassRepository)
        {
            _variantTypesClassRepository = variantTypesClassRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<Guid> Handle(CreateVariantTypesClassCommand request, CancellationToken cancellationToken)
        {
            var newVariantTypesClass = new VariantTypesClass
            {
                StrCollection = request.StrCollection.ToList(),
                IntCollection = request.IntCollection.ToList(),
                StrNullCollection = request.StrNullCollection?.ToList(),
                IntNullCollection = request.IntNullCollection?.ToList(),
                NullStr = request.NullStr,
                NullInt = request.NullInt,
            };

            _variantTypesClassRepository.Add(newVariantTypesClass);
            await _variantTypesClassRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
            return newVariantTypesClass.Id;
        }
    }
}