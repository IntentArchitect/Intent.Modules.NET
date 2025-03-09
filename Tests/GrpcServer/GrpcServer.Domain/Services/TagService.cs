using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using GrpcServer.Domain.Entities;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.DomainServices.DomainServiceImplementation", Version = "1.0")]

namespace GrpcServer.Domain.Services
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class TagService : ITagService
    {
        [IntentManaged(Mode.Merge, Body = Mode.Ignore)]
        public TagService()
        {
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public async Task<IEnumerable<Tag>> GetOrCreateAsync(
            IEnumerable<string> tagNames,
            CancellationToken cancellationToken = default)
        {
            // TODO: Implement GetOrCreateAsync (TagService) functionality
            throw new NotImplementedException("Implement your domain service logic here...");
        }
    }
}