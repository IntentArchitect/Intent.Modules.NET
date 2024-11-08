using MediatR;
using MudBlazor.ExampleApp.Application.Products.GetProducts;
using MudBlazor.ExampleApp.Application.Products.GetProductsLookup;
using MudBlazor.ExampleApp.Client.HttpClients;
using MudBlazor.ExampleApp.Client.HttpClients.Common;
using MudBlazor.ExampleApp.Client.HttpClients.Contracts.Services.Products;

namespace MudBlazor.ExampleApp.Api
{
    public class ProductService : IProductsService
    {
        private readonly ISender _mediator;

        public ProductService(ISender mediatR)
        {
            _mediator = mediatR;
        }

        public Task<Guid> CreateProductAsync(CreateProductCommand command, CancellationToken cancellationToken = default)
        {
            return _mediator.Send<Guid>(new MudBlazor.ExampleApp.Application.Products.CreateProduct.CreateProductCommand(command.Name, command.Description, command.Price, command.ImageUrl), cancellationToken);
        }

        public Task DeleteProductAsync(Guid id, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
        }

        public Task<ProductDto> GetProductByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<PagedResult<ProductDto>> GetProductsAsync(int pageNo, int pageSize, string? orderBy, string? searchText, CancellationToken cancellationToken = default)
        {
            var emptyResult = new PagedResult<ProductDto>
            {
                Data = new List<ProductDto>(), // An empty list of ProductDto
                TotalCount = 0,                 // No items in total
                PageSize = pageSize,
                PageNumber = pageNo
            };

            return Task.FromResult(emptyResult);
        }

        public Task<List<ProductDto>> GetProductsLookupAsync(CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task UpdateProductAsync(Guid id, UpdateProductCommand command, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
    }
}
