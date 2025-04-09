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
    public class Mutation2Endpoint : Endpoint<Mutation2RequestModel>
    {
        private readonly IServiceDispatchService _appService;
        private readonly IUnitOfWork _unitOfWork;

        public Mutation2Endpoint(IServiceDispatchService appService, IUnitOfWork unitOfWork)
        {
            _appService = appService ?? throw new ArgumentNullException(nameof(appService));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        public override void Configure()
        {
            Post("mutation-with-param");
            Description(b =>
            {
                b.WithTags("ServiceDispatchService");
                b.Accepts<Mutation2RequestModel>(MediaTypeNames.Application.Json);
                b.Produces(StatusCodes.Status201Created);
                b.ProducesProblemDetails();
                b.ProducesProblemDetails(StatusCodes.Status500InternalServerError);
            });
            AllowAnonymous();
            Options(x => x.WithVersionSet(">>Api Version<<").MapToApiVersion(new ApiVersion(1.0)));
        }

        public override async Task HandleAsync(Mutation2RequestModel req, CancellationToken ct)
        {
            using (var transaction = new TransactionScope(TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }, TransactionScopeAsyncFlowOption.Enabled))
            {
                _appService.Mutation2(req.Param);
                await _unitOfWork.SaveChangesAsync(ct);
                transaction.Complete();
            }
            await SendResultAsync(TypedResults.Created(string.Empty, (string?)null));
        }
    }

    public class Mutation2RequestModel
    {
        [FromQuery]
        public string Param { get; set; }
    }
}