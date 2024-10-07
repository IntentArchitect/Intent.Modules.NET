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

namespace CleanArchitecture.Comprehensive.Api.FastEndpoints.ServiceDispatch.ServiceDispatchService
{
    public class Query8AsyncEndpoint : Endpoint<Query8AsyncRequestModel, string>
    {
        private readonly IServiceDispatchService _appService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly Application.Common.Eventing.IEventBus _eventBus;

        public Query8AsyncEndpoint(IServiceDispatchService appService,
            IUnitOfWork unitOfWork,
            Application.Common.Eventing.IEventBus eventBus)
        {
            _appService = appService ?? throw new ArgumentNullException(nameof(appService));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _eventBus = eventBus ?? throw new ArgumentNullException(nameof(eventBus));
        }

        public override void Configure()
        {
            Post("api/service-dispatch/query-async-with-param");
            Description(b =>
            {
                b.WithTags("ServiceDispatchService");
                b.Accepts<Query8AsyncRequestModel>();
                b.Produces<string>(StatusCodes.Status201Created);
                b.ProducesProblemDetails();
                b.ProducesProblemDetails(StatusCodes.Status500InternalServerError);
            });
        }

        public override async Task HandleAsync(Query8AsyncRequestModel req, CancellationToken ct)
        {
            var result = default(string);

            using (var transaction = new TransactionScope(TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }, TransactionScopeAsyncFlowOption.Enabled))
            {
                result = await _appService.Query8Async(req.Param, ct);
                await _unitOfWork.SaveChangesAsync(ct);
                transaction.Complete();
            }
            await _eventBus.FlushAllAsync(ct);
            await SendResultAsync(TypedResults.Created(string.Empty, result));
        }
    }

    public class Query8AsyncRequestModel
    {
        [FromQueryParams]
        public string Param { get; set; }
    }
}