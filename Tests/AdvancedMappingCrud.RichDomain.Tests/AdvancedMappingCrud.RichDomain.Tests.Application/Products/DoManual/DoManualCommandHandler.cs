using System;
using System.Threading;
using System.Threading.Tasks;
using AdvancedMappingCrud.RichDomain.Tests.Domain.Common.Exceptions;
using AdvancedMappingCrud.RichDomain.Tests.Domain.Repositories;
using AdvancedMappingCrud.RichDomain.Tests.Domain.Services;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace AdvancedMappingCrud.RichDomain.Tests.Application.Products.DoManual
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class DoManualCommandHandler : IRequestHandler<DoManualCommand>
    {
        private readonly IProductRepository _productRepository;
        private readonly ICategoriesService _categoriesService;

        [IntentManaged(Mode.Merge)]
        public DoManualCommandHandler(IProductRepository productRepository, ICategoriesService categoriesService)
        {
            _productRepository = productRepository;
            _categoriesService = categoriesService;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task Handle(DoManualCommand request, CancellationToken cancellationToken)
        {
            var entity = await _productRepository.FindByIdAsync(request.Id, cancellationToken);
            if (entity is null)
            {
                throw new NotFoundException($"Could not find Product '{request.Id}'");
            }

            entity.DoManual(_categoriesService);
        }
    }
}