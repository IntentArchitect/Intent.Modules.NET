using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CleanArchitecture.Comprehensive.Domain.Entities;
using CleanArchitecture.Comprehensive.Domain.Repositories;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace CleanArchitecture.Comprehensive.Application.Ones.CreateOne
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class CreateOneCommandHandler : IRequestHandler<CreateOneCommand, Guid>
    {
        private readonly IOneRepository _oneRepository;

        [IntentManaged(Mode.Merge)]
        public CreateOneCommandHandler(IOneRepository oneRepository)
        {
            _oneRepository = oneRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<Guid> Handle(CreateOneCommand request, CancellationToken cancellationToken)
        {
            var one = new One
            {
                OneId = request.OneId,
                Seconds = request.Twos
                    .Select(s => new Second
                    {
                        OneId = request.OneId,
                        Twoid = s.TwoId,
                        Threes = s.Threes
                            .Select(t => new Three
                            {
                                OneId = request.OneId,
                                TwoId = s.TwoId,
                                ThreeId = t.ThreeId,
                                Finals = t.Finals
                                    .Select(f => new Final
                                    {
                                        OneId = request.OneId,
                                        TwoId = s.TwoId,
                                        ThreeId = t.ThreeId,
                                        FourId = f.FourId
                                    })
                                    .ToList()
                            })
                            .ToList()
                    })
                    .ToList()
            };

            _oneRepository.Add(one);
            await _oneRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
            return one.Id;
        }
    }
}