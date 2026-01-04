using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AdvancedMappingCrud.DbContext.Tests.Application.Common.Interfaces;
using AdvancedMappingCrud.DbContext.Tests.Domain.Entities.ManyToMany;
using Intent.RoslynWeaver.Attributes;
using MediatR;
using Microsoft.EntityFrameworkCore;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace AdvancedMappingCrud.DbContext.Tests.Application.ManyToMany.CreateProductItem
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class CreateProductItemCommandHandler : IRequestHandler<CreateProductItemCommand>
    {
        private readonly IApplicationDbContext _dbContext;

        [IntentManaged(Mode.Merge)]
        public CreateProductItemCommandHandler(IApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task Handle(CreateProductItemCommand request, CancellationToken cancellationToken)
        {
            var existingTags = await _dbContext.Tags.Where(x => request.TagIds.Contains(x.Id)).ToListAsync(cancellationToken);
            var existingCategories = await _dbContext.Categories.Where(x => request.CategoryIds.Contains(x.Id)).ToListAsync(cancellationToken);
            var productItem = new ProductItem
            {
                Name = request.Name,
                Categories = existingCategories,
                Tags = existingTags
            };

            _dbContext.ProductItems.Add(productItem);
        }
    }
}