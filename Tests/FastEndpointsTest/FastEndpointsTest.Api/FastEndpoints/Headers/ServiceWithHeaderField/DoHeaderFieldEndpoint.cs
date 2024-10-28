using System;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;
using Asp.Versioning;
using FastEndpoints;
using FastEndpoints.AspVersioning;
using FastEndpointsTest.Application.Interfaces.Headers;
using FastEndpointsTest.Domain.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Mode = Intent.RoslynWeaver.Attributes.Mode;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.FastEndpoints.EndpointTemplate", Version = "1.0")]

namespace FastEndpointsTest.Api.FastEndpoints.Headers.ServiceWithHeaderField
{
    public class DoHeaderFieldEndpoint : Endpoint<DoHeaderFieldRequestModel>
    {
        private readonly IServiceWithHeaderFieldService _appService;
        private readonly IUnitOfWork _unitOfWork;

        public DoHeaderFieldEndpoint(IServiceWithHeaderFieldService appService, IUnitOfWork unitOfWork)
        {
            _appService = appService ?? throw new ArgumentNullException(nameof(appService));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        public override void Configure()
        {
            Post("do-header-field");
            Description(b =>
            {
                b.WithTags("ServiceWithHeaderField");
                b.Accepts<DoHeaderFieldRequestModel>();
                b.Produces(StatusCodes.Status201Created);
                b.ProducesProblemDetails();
                b.ProducesProblemDetails(StatusCodes.Status500InternalServerError);
            });
            AllowAnonymous();
            Options(x => x.WithVersionSet(">>Api Version<<").MapToApiVersion(new ApiVersion(1.0)));
        }

        public override async Task HandleAsync(DoHeaderFieldRequestModel req, CancellationToken ct)
        {
            using (var transaction = new TransactionScope(
                TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }, TransactionScopeAsyncFlowOption.Enabled))
            {
                await _appService.DoHeaderField(req.Param, ct);
                await _unitOfWork.SaveChangesAsync(ct);
                transaction.Complete();
            }
            await SendResultAsync(TypedResults.Created(string.Empty, (string?)null));
        }
    }

    public class DoHeaderFieldRequestModel
    {
        [FromHeader("X-HEADER-FIELD")]
        public string Param { get; set; }
    }
}