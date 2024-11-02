using System.Transactions;
using AzureFunctions.NET8.Application.Customers;
using AzureFunctions.NET8.Application.Interfaces.Queues.Bindings;
using AzureFunctions.NET8.Domain.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using Microsoft.Azure.Functions.Worker;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.AzureFunctions.AzureFunctionClass", Version = "2.0")]

namespace AzureFunctions.NET8.Api.Queues.Bindings.BindingService
{
    public class BindingTest
    {
        private readonly IBindingService _appService;
        private readonly IUnitOfWork _unitOfWork;

        public BindingTest(IBindingService appService, IUnitOfWork unitOfWork)
        {
            _appService = appService ?? throw new ArgumentNullException(nameof(appService));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        [Function("Queues_Bindings_BindingService_BindingTest")]
        [QueueOutput("out-queue")]
        public async Task<CustomerDto> Run(
            [QueueTrigger("in-queue")] CustomerDto dto,
            CancellationToken cancellationToken = default)
        {
            using (var transaction = new TransactionScope(TransactionScopeOption.Required,
                new TransactionOptions() { IsolationLevel = IsolationLevel.ReadCommitted }, TransactionScopeAsyncFlowOption.Enabled))
            {
                var result = await _appService.BindingTest(dto, cancellationToken);
                await _unitOfWork.SaveChangesAsync(cancellationToken);
                transaction.Complete();
                return result;
            }
        }
    }
}