using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AdvancedMappingCrud.DbContext.ProjectTo.Tests.Application.Common.Interfaces;
using AdvancedMappingCrud.DbContext.ProjectTo.Tests.Domain.Common;
using AdvancedMappingCrud.DbContext.ProjectTo.Tests.Domain.Common.Exceptions;
using Intent.RoslynWeaver.Attributes;
using MediatR;
using Microsoft.EntityFrameworkCore;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace AdvancedMappingCrud.DbContext.ProjectTo.Tests.Application.ManyToMany.UpdateProductItem
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class UpdateProductItemCommandHandler : IRequestHandler<UpdateProductItemCommand>
    {
        private readonly IApplicationDbContext _dbContext;

        [IntentManaged(Mode.Merge)]
        public UpdateProductItemCommandHandler(IApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task Handle(UpdateProductItemCommand request, CancellationToken cancellationToken)
        {
            var productItem = await _dbContext.ProductItem.SingleOrDefaultAsync(x => x.Id == request.Id, cancellationToken);
            if (productItem is null)
            {
                throw new NotFoundException($"Could not find ProductItem '{request.Id}'");
            }

            var existingTags = await _dbContext.Tag.Where(x => request.TagIds.Contains(x.Id)).ToListAsync(cancellationToken);
            var existingCategories = await _dbContext.Category.Where(x => request.CategoryIds.Contains(x.Id)).ToListAsync(cancellationToken);
            productItem.Name = request.Name;
            productItem.Categories = UpdateHelper.SynchronizeCollection(productItem.Categories, existingCategories, (e, d) => e.Id == d.Id);
            productItem.Tags = UpdateHelper.SynchronizeCollection(productItem.Tags, existingTags, (e, d) => e.Id == d.Id);
        }
    }
}