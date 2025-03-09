using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;
using Google.Protobuf;
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
    public class ForBinary : Protos.Services.TypeTestingServices.ForBinary.ForBinaryBase
    {
        private readonly IForBinaryService _appService;
        private readonly IEventBus _eventBus;
        private readonly IUnitOfWork _unitOfWork;

        public ForBinary(IForBinaryService appService, IEventBus eventBus, IUnitOfWork unitOfWork)
        {
            _appService = appService ?? throw new ArgumentNullException(nameof(appService));
            _eventBus = eventBus ?? throw new ArgumentNullException(nameof(eventBus));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        public override async Task<BytesValue> Operation(ForBinaryOperationRequest request, ServerCallContext context)
        {
            byte[] result;

            using (var transaction = new TransactionScope(TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }, TransactionScopeAsyncFlowOption.Enabled))
            {
                result = await _appService.Operation(request.Param.ToByteArray(), context.CancellationToken);
                await _unitOfWork.SaveChangesAsync(context.CancellationToken);
                transaction.Complete();
            }
            await _eventBus.FlushAllAsync(context.CancellationToken);
            return new BytesValue { Value = ByteString.CopyFrom(result) };
        }

        public override async Task<ListOfBytes> OperationCollection(
            ForBinaryOperationCollectionRequest request,
            ServerCallContext context)
        {
            List<byte[]> result;

            using (var transaction = new TransactionScope(TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }, TransactionScopeAsyncFlowOption.Enabled))
            {
                result = await _appService.OperationCollection(request.Param.Select(x => x.ToByteArray()).ToList(), context.CancellationToken);
                await _unitOfWork.SaveChangesAsync(context.CancellationToken);
                transaction.Complete();
            }
            await _eventBus.FlushAllAsync(context.CancellationToken);
            return ListOfBytes.Create(result);
        }

        public override async Task<BytesValue?> OperationNullable(
            ForBinaryOperationNullableRequest request,
            ServerCallContext context)
        {
            byte[]? result;

            using (var transaction = new TransactionScope(TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }, TransactionScopeAsyncFlowOption.Enabled))
            {
                result = await _appService.OperationNullable(request.Param?.ToByteArray(), context.CancellationToken);
                await _unitOfWork.SaveChangesAsync(context.CancellationToken);
                transaction.Complete();
            }
            await _eventBus.FlushAllAsync(context.CancellationToken);
            return result != null ? new BytesValue { Value = ByteString.CopyFrom(result) } : null;
        }

        public override async Task<ListOfBytes?> OperationNullableCollection(
            ForBinaryOperationNullableCollectionRequest request,
            ServerCallContext context)
        {
            List<byte[]>? result;

            using (var transaction = new TransactionScope(TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }, TransactionScopeAsyncFlowOption.Enabled))
            {
                result = await _appService.OperationNullableCollection(request.Param?.Items.Select(x => x.ToByteArray()).ToList(), context.CancellationToken);
                await _unitOfWork.SaveChangesAsync(context.CancellationToken);
                transaction.Complete();
            }
            await _eventBus.FlushAllAsync(context.CancellationToken);
            return ListOfBytes.Create(result);
        }
    }
}