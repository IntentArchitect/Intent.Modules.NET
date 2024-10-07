using System;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;
using CleanArchitecture.Comprehensive.Application.Common.Eventing;
using CleanArchitecture.Comprehensive.Application.Interfaces.ServiceDispatch;
using CleanArchitecture.Comprehensive.Domain.Common.Interfaces;
using FastEndpoints;
using Intent.RoslynWeaver.Attributes;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Mode = Intent.RoslynWeaver.Attributes.Mode;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.FastEndpoints.EndpointTemplate", Version = "1.0")]

namespace CleanArchitecture.Comprehensive.Api.FastEndpoints.ServiceDispatch.ServiceDispatchWithoutImplementation
{
    public class MutationNoImpl8Endpoint : EndpointWithoutRequest
    {
        private readonly IServiceDispatchWithoutImplementationService _appService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly Application.Common.Eventing.IEventBus _eventBus;

        public MutationNoImpl8Endpoint(IServiceDispatchWithoutImplementationService appService,
            IUnitOfWork unitOfWork,
            Application.Common.Eventing.IEventBus eventBus)
        {
            _appService = appService ?? throw new ArgumentNullException(nameof(appService));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _eventBus = eventBus ?? throw new ArgumentNullException(nameof(eventBus));
        }

        public override void Configure()
        {
            Post("api/service-dispatch-without-implementation/mutation");
            Description(b =>
            {
                b.WithTags("ServiceDispatchWithoutImplementation");
                b.Produces(StatusCodes.Status201Created);
                b.ProducesProblemDetails(StatusCodes.Status500InternalServerError);
            });
        }

        public override async Task HandleAsync(CancellationToken ct)
        {
            using (var transaction = new TransactionScope(TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }, TransactionScopeAsyncFlowOption.Enabled))
            {
                _appService.MutationNoImpl8();
                await _unitOfWork.SaveChangesAsync(ct);
                transaction.Complete();
            }
            await _eventBus.FlushAllAsync(ct);
            await SendResultAsync(TypedResults.Created(string.Empty, (string?)null));
        }
    }
}