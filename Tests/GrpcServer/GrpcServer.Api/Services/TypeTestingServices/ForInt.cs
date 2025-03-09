using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using GrpcServer.Api.Protos.Messages;
using GrpcServer.Api.Protos.Services.TypeTestingServices;
using GrpcServer.Application.Common.Eventing;
using GrpcServer.Application.Common.Validation;
using GrpcServer.Application.Interfaces.TypeTestingServices;
using GrpcServer.Domain.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using Microsoft.AspNetCore.Authorization;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.AspNetCore.Grpc.TraditionalService", Version = "1.0")]

namespace GrpcServer.Api.Services.TypeTestingServices
{
    [Authorize]
    public class ForInt : Protos.Services.TypeTestingServices.ForInt.ForIntBase
    {
        private readonly IForIntService _appService;
        private readonly IEventBus _eventBus;
        private readonly IUnitOfWork _unitOfWork;

        public ForInt(IForIntService appService, IEventBus eventBus, IUnitOfWork unitOfWork)
        {
            _appService = appService ?? throw new ArgumentNullException(nameof(appService));
            _eventBus = eventBus ?? throw new ArgumentNullException(nameof(eventBus));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        public override async Task<Int32Value> Operation(ForIntOperationRequest request, ServerCallContext context)
        {
            int result;

            using (var transaction = new TransactionScope(TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }, TransactionScopeAsyncFlowOption.Enabled))
            {
                result = await _appService.Operation(request.Param, context.CancellationToken);
                await _unitOfWork.SaveChangesAsync(context.CancellationToken);
                transaction.Complete();
            }
            await _eventBus.FlushAllAsync(context.CancellationToken);
            return new Int32Value { Value = result };
        }

        public override async Task<ListOfInt32> OperationCollection(
            ForIntOperationCollectionRequest request,
            ServerCallContext context)
        {
            List<int> result;

            using (var transaction = new TransactionScope(TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }, TransactionScopeAsyncFlowOption.Enabled))
            {
                result = await _appService.OperationCollection(request.Param.ToList(), context.CancellationToken);
                await _unitOfWork.SaveChangesAsync(context.CancellationToken);
                transaction.Complete();
            }
            await _eventBus.FlushAllAsync(context.CancellationToken);
            return ListOfInt32.Create(result);
        }

        public override async Task<Int32Value?> OperationNullable(
            ForIntOperationNullableRequest request,
            ServerCallContext context)
        {
            int? result;

            using (var transaction = new TransactionScope(TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }, TransactionScopeAsyncFlowOption.Enabled))
            {
                result = await _appService.OperationNullable(request.Param, context.CancellationToken);
                await _unitOfWork.SaveChangesAsync(context.CancellationToken);
                transaction.Complete();
            }
            await _eventBus.FlushAllAsync(context.CancellationToken);
            return result.HasValue ? new Int32Value { Value = result.Value } : null;
        }

        public override async Task<ListOfInt32?> OperationNullableCollection(
            ForIntOperationNullableCollectionRequest request,
            ServerCallContext context)
        {
            List<int>? result;

            using (var transaction = new TransactionScope(TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }, TransactionScopeAsyncFlowOption.Enabled))
            {
                result = await _appService.OperationNullableCollection(request.Param?.Items.ToList(), context.CancellationToken);
                await _unitOfWork.SaveChangesAsync(context.CancellationToken);
                transaction.Complete();
            }
            await _eventBus.FlushAllAsync(context.CancellationToken);
            return ListOfInt32.Create(result);
        }
    }
}