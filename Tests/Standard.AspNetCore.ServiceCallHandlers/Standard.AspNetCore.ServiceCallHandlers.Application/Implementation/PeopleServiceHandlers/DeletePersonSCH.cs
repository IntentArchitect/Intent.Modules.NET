using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Standard.AspNetCore.ServiceCallHandlers.Application.Common.Interfaces;
using Standard.AspNetCore.ServiceCallHandlers.Domain.Common.Exceptions;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.ServiceCallHandlers.ServiceCallHandlerImplementation", Version = "1.0")]

namespace Standard.AspNetCore.ServiceCallHandlers.Application.Implementation.PeopleServiceHandlers
{
    [IntentManaged(Mode.Merge)]
    public class DeletePersonSCH
    {
        private readonly IApplicationDbContext _dbContext;

        [IntentManaged(Mode.Merge)]
        public DeletePersonSCH(IApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task Handle(Guid id, CancellationToken cancellationToken = default)
        {
            var person = await _dbContext.People.SingleOrDefaultAsync(x => x.Id == id, cancellationToken);
            if (person is null)
            {
                throw new NotFoundException($"Could not find Person '{id}'");
            }

            _dbContext.People.Remove(person);
        }
    }
}