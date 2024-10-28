using System;
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

namespace FastEndpointsTest.Api.FastEndpoints.ServiceDispatch.ServiceDispatchWithoutImplementation
{
    public class MutationNoImpl8Endpoint : EndpointWithoutRequest
    {
        private readonly IServiceDispatchWithoutImplementationService _appService;
        private readonly IUnitOfWork _unitOfWork;

        public MutationNoImpl8Endpoint(IServiceDispatchWithoutImplementationService appService, IUnitOfWork unitOfWork)
        {
            _appService = appService ?? throw new ArgumentNullException(nameof(appService));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        public override void Configure()
        {
            Post("mutation");
            Description(b =>
            {
                b.WithTags("ServiceDispatchWithoutImplementation");
                b.Produces(StatusCodes.Status201Created);
                b.ProducesProblemDetails(StatusCodes.Status500InternalServerError);
            });
            AllowAnonymous();
            Options(x => x.WithVersionSet(">>Api Version<<").MapToApiVersion(new ApiVersion(1.0)));
        }

        public override async Task HandleAsync(CancellationToken ct)
        {
            using (var transaction = new TransactionScope(
                TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }, TransactionScopeAsyncFlowOption.Enabled))
            {
                _appService.MutationNoImpl8();
                await _unitOfWork.SaveChangesAsync(ct);
                transaction.Complete();
            }
            await SendResultAsync(TypedResults.Created(string.Empty, (string?)null));
        }
    }
}