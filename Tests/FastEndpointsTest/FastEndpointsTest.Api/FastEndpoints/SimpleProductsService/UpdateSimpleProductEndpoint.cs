using System;
using System.Net.Mime;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;
using Asp.Versioning;
using FastEndpoints;
using FastEndpoints.AspVersioning;
using FastEndpointsTest.Application.Common.Validation;
using FastEndpointsTest.Application.Interfaces;
using FastEndpointsTest.Application.SimpleProducts;
using FastEndpointsTest.Domain.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Mode = Intent.RoslynWeaver.Attributes.Mode;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.FastEndpoints.EndpointTemplate", Version = "1.0")]

namespace FastEndpointsTest.Api.FastEndpoints.SimpleProductsService
{
    public class UpdateSimpleProductEndpoint : Endpoint<SimpleProductUpdateDto>
    {
        private readonly ISimpleProductsService _appService;
        private readonly IValidationService _validationService;
        private readonly IUnitOfWork _unitOfWork;

        public UpdateSimpleProductEndpoint(ISimpleProductsService appService,
            IValidationService validationService,
            IUnitOfWork unitOfWork)
        {
            _appService = appService ?? throw new ArgumentNullException(nameof(appService));
            _validationService = validationService ?? throw new ArgumentNullException(nameof(validationService));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        public override void Configure()
        {
            Put("api/simple-products/{id}");
            Description(b =>
            {
                b.WithTags("SimpleProductsService");
                b.Accepts<SimpleProductUpdateDto>(MediaTypeNames.Application.Json);
                b.Produces(StatusCodes.Status204NoContent);
                b.ProducesProblemDetails();
                b.ProducesProblemDetails(StatusCodes.Status500InternalServerError);
            });
            AllowAnonymous();
            Options(x => x.WithVersionSet(">>Api Version<<").MapToApiVersion(new ApiVersion(1.0)));
        }

        public override async Task HandleAsync(SimpleProductUpdateDto req, CancellationToken ct)
        {
            await _validationService.Handle(req, ct);

            using (var transaction = new TransactionScope(TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }, TransactionScopeAsyncFlowOption.Enabled))
            {
                await _appService.UpdateSimpleProduct(req.Id, req, ct);
                await _unitOfWork.SaveChangesAsync(ct);
                transaction.Complete();
            }
            await Send.ResultAsync(TypedResults.NoContent());
        }
    }
}