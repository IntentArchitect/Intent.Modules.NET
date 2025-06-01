using System;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;
using Asp.Versioning;
using FastEndpoints;
using FastEndpoints.AspVersioning;
using FastEndpointsTest.Application.Interfaces;
using FastEndpointsTest.Domain.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Mode = Intent.RoslynWeaver.Attributes.Mode;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.FastEndpoints.EndpointTemplate", Version = "1.0")]

namespace FastEndpointsTest.Api.FastEndpoints.SimpleProductsService
{
    public class DeleteSimpleProductEndpoint : Endpoint<DeleteSimpleProductRequestModel>
    {
        private readonly ISimpleProductsService _appService;
        private readonly IUnitOfWork _unitOfWork;

        public DeleteSimpleProductEndpoint(ISimpleProductsService appService, IUnitOfWork unitOfWork)
        {
            _appService = appService ?? throw new ArgumentNullException(nameof(appService));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        public override void Configure()
        {
            Delete("api/simple-products/{id}");
            Description(b =>
            {
                b.WithTags("SimpleProductsService");
                b.Accepts<DeleteSimpleProductRequestModel>();
                b.Produces(StatusCodes.Status200OK);
                b.ProducesProblemDetails();
                b.ProducesProblemDetails(StatusCodes.Status500InternalServerError);
            });
            AllowAnonymous();
            Options(x => x.WithVersionSet(">>Api Version<<").MapToApiVersion(new ApiVersion(1.0)));
        }

        public override async Task HandleAsync(DeleteSimpleProductRequestModel req, CancellationToken ct)
        {
            using (var transaction = new TransactionScope(TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }, TransactionScopeAsyncFlowOption.Enabled))
            {
                await _appService.DeleteSimpleProduct(req.Id, ct);
                await _unitOfWork.SaveChangesAsync(ct);
                transaction.Complete();
            }
            await SendResultAsync(TypedResults.Ok());
        }
    }

    public class DeleteSimpleProductRequestModel
    {
        public Guid Id { get; set; }
    }
}