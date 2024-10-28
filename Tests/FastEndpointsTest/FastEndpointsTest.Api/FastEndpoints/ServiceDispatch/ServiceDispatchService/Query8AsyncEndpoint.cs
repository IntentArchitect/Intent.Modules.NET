using System;
using System.Net.Mime;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;
using Asp.Versioning;
using FastEndpoints;
using FastEndpoints.AspVersioning;
using FastEndpointsTest.Application.Interfaces.ServiceDispatch;
using FastEndpointsTest.Domain.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Mode = Intent.RoslynWeaver.Attributes.Mode;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.FastEndpoints.EndpointTemplate", Version = "1.0")]

namespace FastEndpointsTest.Api.FastEndpoints.ServiceDispatch.ServiceDispatchService
{
    public class Query8AsyncEndpoint : Endpoint<Query8AsyncRequestModel, string>
    {
        private readonly IServiceDispatchService _appService;
        private readonly IUnitOfWork _unitOfWork;

        public Query8AsyncEndpoint(IServiceDispatchService appService, IUnitOfWork unitOfWork)
        {
            _appService = appService ?? throw new ArgumentNullException(nameof(appService));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        public override void Configure()
        {
            Post("query-async-with-param");
            Description(b =>
            {
                b.WithTags("ServiceDispatchService");
                b.Accepts<Query8AsyncRequestModel>(MediaTypeNames.Application.Json);
                b.Produces<string>(StatusCodes.Status201Created);
                b.ProducesProblemDetails();
                b.ProducesProblemDetails(StatusCodes.Status500InternalServerError);
            });
            AllowAnonymous();
            Options(x => x.WithVersionSet(">>Api Version<<").MapToApiVersion(new ApiVersion(1.0)));
        }

        public override async Task HandleAsync(Query8AsyncRequestModel req, CancellationToken ct)
        {
            var result = default(string);

            using (var transaction = new TransactionScope(
                TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }, TransactionScopeAsyncFlowOption.Enabled))
            {
                result = await _appService.Query8Async(req.Param, ct);
                await _unitOfWork.SaveChangesAsync(ct);
                transaction.Complete();
            }
            await SendResultAsync(TypedResults.Created(string.Empty, result));
        }
    }

    public class Query8AsyncRequestModel
    {
        [FromQueryParams]
        public string Param { get; set; }
    }
}