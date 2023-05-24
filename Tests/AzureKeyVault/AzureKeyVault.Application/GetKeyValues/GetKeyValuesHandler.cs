using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using MediatR;
using Microsoft.Extensions.Configuration;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryHandler", Version = "1.0")]

namespace AzureKeyVault.Application.GetKeyValues
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class GetKeyValuesHandler : IRequestHandler<GetKeyValues, KeyValuesDTO>
    {
        private readonly IConfiguration _configuration;

        [IntentManaged(Mode.Ignore)]
        public GetKeyValuesHandler(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public async Task<KeyValuesDTO> Handle(GetKeyValues request, CancellationToken cancellationToken)
        {
            return new KeyValuesDTO()
            {
                Keys = new List<KeyValuePairDTO>
                {
                    new KeyValuePairDTO()
                    {
                        Key = "Example-Secret",
                        Value = _configuration["Example-Secret"]
                    },
                    new KeyValuePairDTO()
                    {
                        Key = "Local-Setting",
                        Value = _configuration["Local-Setting"]
                    }
                }
            };
        }
    }
}