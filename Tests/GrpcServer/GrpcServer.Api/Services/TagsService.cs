using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Transactions;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using GrpcServer.Api.Protos.Messages.Tags;
using GrpcServer.Api.Protos.Services;
using GrpcServer.Application.Common.Eventing;
using GrpcServer.Application.Common.Validation;
using GrpcServer.Application.Interfaces;
using GrpcServer.Application.Tags;
using GrpcServer.Domain.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using Microsoft.AspNetCore.Authorization;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.AspNetCore.Grpc.TraditionalService", Version = "1.0")]

namespace GrpcServer.Api.Services
{
    [Authorize]
    public class TagsService : Protos.Services.TagsService.TagsServiceBase
    {
        private readonly ITagsService _appService;
        private readonly IValidationService _validationService;
        private readonly IEventBus _eventBus;
        private readonly IUnitOfWork _unitOfWork;

        public TagsService(ITagsService appService,
            IValidationService validationService,
            IEventBus eventBus,
            IUnitOfWork unitOfWork)
        {
            _appService = appService ?? throw new ArgumentNullException(nameof(appService));
            _validationService = validationService ?? throw new ArgumentNullException(nameof(validationService));
            _eventBus = eventBus ?? throw new ArgumentNullException(nameof(eventBus));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        public override async Task<StringValue> CreateTag(TagsServiceCreateTagRequest request, ServerCallContext context)
        {
            await _validationService.Handle(request.Dto.ToContract(), context.CancellationToken);
            Guid result;

            using (var transaction = new TransactionScope(TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }, TransactionScopeAsyncFlowOption.Enabled))
            {
                result = await _appService.CreateTag(request.Dto.ToContract(), context.CancellationToken);
                await _unitOfWork.SaveChangesAsync(context.CancellationToken);
                transaction.Complete();
            }
            await _eventBus.FlushAllAsync(context.CancellationToken);
            return new StringValue { Value = result.ToString() };
        }

        public override async Task<Empty> UpdateTag(TagsServiceUpdateTagRequest request, ServerCallContext context)
        {
            await _validationService.Handle(request.Dto.ToContract(), context.CancellationToken);

            using (var transaction = new TransactionScope(TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }, TransactionScopeAsyncFlowOption.Enabled))
            {
                await _appService.UpdateTag(Guid.Parse(request.Id), request.Dto.ToContract(), context.CancellationToken);
                await _unitOfWork.SaveChangesAsync(context.CancellationToken);
                transaction.Complete();
            }
            await _eventBus.FlushAllAsync(context.CancellationToken);
            return new Empty();
        }

        [AllowAnonymous]
        public override async Task<Protos.Messages.Tags.TagDto> FindTagById(
            TagsServiceFindTagByIdRequest request,
            ServerCallContext context)
        {
            Application.Tags.TagDto result;

            using (var transaction = new TransactionScope(TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }, TransactionScopeAsyncFlowOption.Enabled))
            {
                result = await _appService.FindTagById(Guid.Parse(request.Id), context.CancellationToken);
                await _unitOfWork.SaveChangesAsync(context.CancellationToken);
                transaction.Complete();
            }
            await _eventBus.FlushAllAsync(context.CancellationToken);
            return Protos.Messages.Tags.TagDto.Create(result);
        }

        [Authorize]
        public override async Task<ListOfTagDto> FindTags(Empty request, ServerCallContext context)
        {
            List<Application.Tags.TagDto> result;

            using (var transaction = new TransactionScope(TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }, TransactionScopeAsyncFlowOption.Enabled))
            {
                result = await _appService.FindTags(context.CancellationToken);
                await _unitOfWork.SaveChangesAsync(context.CancellationToken);
                transaction.Complete();
            }
            await _eventBus.FlushAllAsync(context.CancellationToken);
            return ListOfTagDto.Create(result);
        }

        [Authorize]
        public override async Task<Empty> DeleteTag(TagsServiceDeleteTagRequest request, ServerCallContext context)
        {
            using (var transaction = new TransactionScope(TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }, TransactionScopeAsyncFlowOption.Enabled))
            {
                await _appService.DeleteTag(Guid.Parse(request.Id), context.CancellationToken);
                await _unitOfWork.SaveChangesAsync(context.CancellationToken);
                transaction.Complete();
            }
            await _eventBus.FlushAllAsync(context.CancellationToken);
            return new Empty();
        }
    }
}