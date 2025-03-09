using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;
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
    public class ForDictionary : Protos.Services.TypeTestingServices.ForDictionary.ForDictionaryBase
    {
        private readonly IForDictionaryService _appService;
        private readonly IEventBus _eventBus;
        private readonly IUnitOfWork _unitOfWork;

        public ForDictionary(IForDictionaryService appService, IEventBus eventBus, IUnitOfWork unitOfWork)
        {
            _appService = appService ?? throw new ArgumentNullException(nameof(appService));
            _eventBus = eventBus ?? throw new ArgumentNullException(nameof(eventBus));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        public override async Task<MapOfStringAndString> Operation(
            ForDictionaryOperationRequest request,
            ServerCallContext context)
        {
            Dictionary<string, string> result;

            using (var transaction = new TransactionScope(TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }, TransactionScopeAsyncFlowOption.Enabled))
            {
                result = await _appService.Operation(request.Param.ToDictionary(), context.CancellationToken);
                await _unitOfWork.SaveChangesAsync(context.CancellationToken);
                transaction.Complete();
            }
            await _eventBus.FlushAllAsync(context.CancellationToken);
            return MapOfStringAndString.Create(result);
        }

        public override async Task<ListOfMapOfStringAndString> OperationCollection(
            ForDictionaryOperationCollectionRequest request,
            ServerCallContext context)
        {
            List<Dictionary<string, string>> result;

            using (var transaction = new TransactionScope(TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }, TransactionScopeAsyncFlowOption.Enabled))
            {
                result = await _appService.OperationCollection(request.Param.Select(x => x.Items.ToDictionary()).ToList(), context.CancellationToken);
                await _unitOfWork.SaveChangesAsync(context.CancellationToken);
                transaction.Complete();
            }
            await _eventBus.FlushAllAsync(context.CancellationToken);
            return ListOfMapOfStringAndString.Create(result);
        }

        public override async Task<MapOfStringAndString?> OperationNullable(
            ForDictionaryOperationNullableRequest request,
            ServerCallContext context)
        {
            Dictionary<string, string>? result;

            using (var transaction = new TransactionScope(TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }, TransactionScopeAsyncFlowOption.Enabled))
            {
                result = await _appService.OperationNullable(request.Param?.Items.ToDictionary(), context.CancellationToken);
                await _unitOfWork.SaveChangesAsync(context.CancellationToken);
                transaction.Complete();
            }
            await _eventBus.FlushAllAsync(context.CancellationToken);
            return MapOfStringAndString.Create(result);
        }

        public override async Task<ListOfMapOfStringAndString?> OperationNullableCollection(
            ForDictionaryOperationNullableCollectionRequest request,
            ServerCallContext context)
        {
            List<Dictionary<string, string>>? result;

            using (var transaction = new TransactionScope(TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }, TransactionScopeAsyncFlowOption.Enabled))
            {
                result = await _appService.OperationNullableCollection(request.Param?.Items.Select(x => x.Items.ToDictionary()).ToList(), context.CancellationToken);
                await _unitOfWork.SaveChangesAsync(context.CancellationToken);
                transaction.Complete();
            }
            await _eventBus.FlushAllAsync(context.CancellationToken);
            return ListOfMapOfStringAndString.Create(result);
        }
    }
}