using NSubstitute;
using ObjectMappingTest.Application.Products.GetDigitalProductById;
using ObjectMappingTest.Domain.Common.Exceptions;
using ObjectMappingTest.Domain.Entities;
using ObjectMappingTest.Domain.Repositories;

namespace ObjectMappingTest.Tests;

public class ProductHandlerTests
{
    [Fact]
    public async Task GetDigitalProductByIdHandler_ProductExists_ReturnsMappedDto()
    {
        var product = new DigitalProduct
        {
            Id = Guid.NewGuid(),
            Name = "C# Deep Dive",
            Price = 29.99m,
            DownloadUrl = "https://downloads.example.com/csharp-deep-dive.pdf",
        };
        var repository = Substitute.For<IDigitalProductRepository>();
        repository.FindByIdAsync(product.Id, Arg.Any<CancellationToken>()).Returns(product);
        var handler = new GetDigitalProductByIdHandler(repository);

        var dto = await handler.Handle(new GetDigitalProductById(product.Id), CancellationToken.None);

        Assert.Equal(product.Id, dto.Id);
        Assert.Equal(product.DownloadUrl, dto.DownloadUrl);
        await repository.Received(1).FindByIdAsync(product.Id, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task GetDigitalProductByIdHandler_ProductMissing_ThrowsNotFoundException()
    {
        var productId = Guid.NewGuid();
        var repository = Substitute.For<IDigitalProductRepository>();
        repository.FindByIdAsync(productId, Arg.Any<CancellationToken>()).Returns((DigitalProduct?)null);
        var handler = new GetDigitalProductByIdHandler(repository);

        await Assert.ThrowsAsync<NotFoundException>(
            () => handler.Handle(new GetDigitalProductById(productId), CancellationToken.None));
    }
}
