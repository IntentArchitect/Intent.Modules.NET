using System;
using System.Threading;
using System.Threading.Tasks;
using AdvancedMappingCrud.RichDomain.SeparatedEntityState.Tests.Domain.Common.Exceptions;
using AdvancedMappingCrud.RichDomain.SeparatedEntityState.Tests.Domain.Repositories;
using AdvancedMappingCrud.RichDomain.SeparatedEntityState.Tests.Domain.Services;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace AdvancedMappingCrud.RichDomain.SeparatedEntityState.Tests.Application.Products.DoManual
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class DoManualCommandHandler : IRequestHandler<DoManualCommand>
    {
        private readonly IProductRepository _productRepository;
        private readonly ICategoriesService _categoriesService;
        private readonly ISecondService _secondService;

        [IntentManaged(Mode.Merge)]
        public DoManualCommandHandler(IProductRepository productRepository,
            ICategoriesService categoriesService,
            ISecondService secondService)
        {
            _productRepository = productRepository;
            _categoriesService = categoriesService;
            _secondService = secondService;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task Handle(DoManualCommand request, CancellationToken cancellationToken)
        {
            var entity = await _productRepository.FindByIdAsync(request.Id, cancellationToken);
            if (entity is null)
            {
                throw new NotFoundException($"Could not find Product '{request.Id}'");
            }

            await entity.DoManualAsync(_categoriesService, cancellationToken);
            entity.Another(_secondService);
        }
    }
}