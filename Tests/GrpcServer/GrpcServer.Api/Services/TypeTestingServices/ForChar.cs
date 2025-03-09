using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
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
    public class ForChar : Protos.Services.TypeTestingServices.ForChar.ForCharBase
    {
        private readonly IForCharService _appService;
        private readonly IEventBus _eventBus;
        private readonly IUnitOfWork _unitOfWork;

        public ForChar(IForCharService appService, IEventBus eventBus, IUnitOfWork unitOfWork)
        {
            _appService = appService ?? throw new ArgumentNullException(nameof(appService));
            _eventBus = eventBus ?? throw new ArgumentNullException(nameof(eventBus));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        public override async Task<StringValue> Operation(ForCharOperationRequest request, ServerCallContext context)
        {
            char result;

            using (var transaction = new TransactionScope(TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }, TransactionScopeAsyncFlowOption.Enabled))
            {
                result = await _appService.Operation(request.Param[0], context.CancellationToken);
                await _unitOfWork.SaveChangesAsync(context.CancellationToken);
                transaction.Complete();
            }
            await _eventBus.FlushAllAsync(context.CancellationToken);
            return new StringValue { Value = char.ToString(result) };
        }

        public override async Task<StringValue> OperationCollection(
            ForCharOperationCollectionRequest request,
            ServerCallContext context)
        {
            List<char> result;

            using (var transaction = new TransactionScope(TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }, TransactionScopeAsyncFlowOption.Enabled))
            {
                result = await _appService.OperationCollection(request.Param.ToList(), context.CancellationToken);
                await _unitOfWork.SaveChangesAsync(context.CancellationToken);
                transaction.Complete();
            }
            await _eventBus.FlushAllAsync(context.CancellationToken);
            return new StringValue { Value = new string(result.ToArray()) };
        }

        public override async Task<StringValue?> OperationNullable(
            ForCharOperationNullableRequest request,
            ServerCallContext context)
        {
            char? result;

            using (var transaction = new TransactionScope(TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }, TransactionScopeAsyncFlowOption.Enabled))
            {
                result = await _appService.OperationNullable(request.Param?[0], context.CancellationToken);
                await _unitOfWork.SaveChangesAsync(context.CancellationToken);
                transaction.Complete();
            }
            await _eventBus.FlushAllAsync(context.CancellationToken);
            return result.HasValue ? new StringValue { Value = char.ToString(result.Value) } : null;
        }

        public override async Task<StringValue?> OperationNullableCollection(
            ForCharOperationNullableCollectionRequest request,
            ServerCallContext context)
        {
            List<char>? result;

            using (var transaction = new TransactionScope(TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }, TransactionScopeAsyncFlowOption.Enabled))
            {
                result = await _appService.OperationNullableCollection(request.Param?.ToList(), context.CancellationToken);
                await _unitOfWork.SaveChangesAsync(context.CancellationToken);
                transaction.Complete();
            }
            await _eventBus.FlushAllAsync(context.CancellationToken);
            return result != null ? new StringValue { Value = new string(result.ToArray()) } : null;
        }
    }
}