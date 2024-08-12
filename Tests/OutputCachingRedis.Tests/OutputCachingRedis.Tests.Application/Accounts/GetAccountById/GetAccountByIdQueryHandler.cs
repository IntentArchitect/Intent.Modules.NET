using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;
using MediatR;
using OutputCachingRedis.Tests.Domain.Common.Exceptions;
using OutputCachingRedis.Tests.Domain.Repositories;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryHandler", Version = "1.0")]

namespace OutputCachingRedis.Tests.Application.Accounts.GetAccountById
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class GetAccountByIdQueryHandler : IRequestHandler<GetAccountByIdQuery, AccountDto>
    {
        private readonly IAccountRepository _accountRepository;
        private readonly IMapper _mapper;

        [IntentManaged(Mode.Merge)]
        public GetAccountByIdQueryHandler(IAccountRepository accountRepository, IMapper mapper)
        {
            _accountRepository = accountRepository;
            _mapper = mapper;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<AccountDto> Handle(GetAccountByIdQuery request, CancellationToken cancellationToken)
        {
            var account = await _accountRepository.FindByIdAsync(request.Id, cancellationToken);
            if (account is null)
            {
                throw new NotFoundException($"Could not find Account '{request.Id}'");
            }
            return account.MapToAccountDto(_mapper);
        }
    }
}