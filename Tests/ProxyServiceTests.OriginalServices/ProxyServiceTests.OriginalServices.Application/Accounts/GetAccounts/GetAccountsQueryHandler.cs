using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;
using MediatR;
using ProxyServiceTests.OriginalServices.Domain.Repositories;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryHandler", Version = "1.0")]

namespace ProxyServiceTests.OriginalServices.Application.Accounts.GetAccounts
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class GetAccountsQueryHandler : IRequestHandler<GetAccountsQuery, List<AccountDto>>
    {
        private readonly IAccountRepository _accountRepository;
        private readonly IMapper _mapper;

        [IntentManaged(Mode.Merge)]
        public GetAccountsQueryHandler(IAccountRepository accountRepository, IMapper mapper)
        {
            _accountRepository = accountRepository;
            _mapper = mapper;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<List<AccountDto>> Handle(GetAccountsQuery request, CancellationToken cancellationToken)
        {
            var accounts = await _accountRepository.FindAllAsync(cancellationToken);
            return accounts.MapToAccountDtoList(_mapper);
        }
    }
}