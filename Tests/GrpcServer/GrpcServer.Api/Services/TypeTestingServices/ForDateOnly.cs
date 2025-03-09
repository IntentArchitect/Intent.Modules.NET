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
    public class ForDateOnly : Protos.Services.TypeTestingServices.ForDateOnly.ForDateOnlyBase
    {
        private readonly IForDateOnlyService _appService;
        private readonly IEventBus _eventBus;
        private readonly IUnitOfWork _unitOfWork;

        public ForDateOnly(IForDateOnlyService appService, IEventBus eventBus, IUnitOfWork unitOfWork)
        {
            _appService = appService ?? throw new ArgumentNullException(nameof(appService));
            _eventBus = eventBus ?? throw new ArgumentNullException(nameof(eventBus));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        public override async Task<Timestamp> Operation(ForDateOnlyOperationRequest request, ServerCallContext context)
        {
            DateOnly result;

            using (var transaction = new TransactionScope(TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }, TransactionScopeAsyncFlowOption.Enabled))
            {
                result = await _appService.Operation(DateOnly.FromDateTime(request.Param.ToDateTime()), context.CancellationToken);
                await _unitOfWork.SaveChangesAsync(context.CancellationToken);
                transaction.Complete();
            }
            await _eventBus.FlushAllAsync(context.CancellationToken);
            return Timestamp.FromDateTime(result.ToDateTime(TimeOnly.MinValue));
        }

        public override async Task<ListOfTimestamp> OperationCollection(
            ForDateOnlyOperationCollectionRequest request,
            ServerCallContext context)
        {
            List<DateOnly> result;

            using (var transaction = new TransactionScope(TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }, TransactionScopeAsyncFlowOption.Enabled))
            {
                result = await _appService.OperationCollection(request.Param.Select(x => DateOnly.FromDateTime(x.ToDateTime())).ToList(), context.CancellationToken);
                await _unitOfWork.SaveChangesAsync(context.CancellationToken);
                transaction.Complete();
            }
            await _eventBus.FlushAllAsync(context.CancellationToken);
            return ListOfTimestamp.Create(result.Select(x => Timestamp.FromDateTime(x.ToDateTime(TimeOnly.MinValue))));
        }

        public override async Task<Timestamp?> OperationNullable(
            ForDateOnlyOperationNullableRequest request,
            ServerCallContext context)
        {
            DateOnly? result;

            using (var transaction = new TransactionScope(TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }, TransactionScopeAsyncFlowOption.Enabled))
            {
                result = await _appService.OperationNullable(request.Param != null ? DateOnly.FromDateTime(request.Param.ToDateTime()) : null, context.CancellationToken);
                await _unitOfWork.SaveChangesAsync(context.CancellationToken);
                transaction.Complete();
            }
            await _eventBus.FlushAllAsync(context.CancellationToken);
            return result.HasValue ? Timestamp.FromDateTime(result.Value.ToDateTime(TimeOnly.MinValue)) : null;
        }

        public override async Task<ListOfTimestamp?> OperationNullableCollection(
            ForDateOnlyOperationNullableCollectionRequest request,
            ServerCallContext context)
        {
            List<DateOnly>? result;

            using (var transaction = new TransactionScope(TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }, TransactionScopeAsyncFlowOption.Enabled))
            {
                result = await _appService.OperationNullableCollection(request.Param?.Items.Select(x => DateOnly.FromDateTime(x.ToDateTime())).ToList(), context.CancellationToken);
                await _unitOfWork.SaveChangesAsync(context.CancellationToken);
                transaction.Complete();
            }
            await _eventBus.FlushAllAsync(context.CancellationToken);
            return ListOfTimestamp.Create(result?.Select(x => Timestamp.FromDateTime(x.ToDateTime(TimeOnly.MinValue))));
        }
    }
}