using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AdvancedMappingCrud.Cosmos.Tests.Domain.Entities;
using AdvancedMappingCrud.Cosmos.Tests.Domain.Repositories;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace AdvancedMappingCrud.Cosmos.Tests.Application.Parents.CreateParent
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class CreateParentCommandHandler : IRequestHandler<CreateParentCommand, string>
    {
        private readonly IParentRepository _parentRepository;

        [IntentManaged(Mode.Merge)]
        public CreateParentCommandHandler(IParentRepository parentRepository)
        {
            _parentRepository = parentRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<string> Handle(CreateParentCommand request, CancellationToken cancellationToken)
        {
            var parent = new Parent
            {
                Name = request.Name,
                Children = request.Children?
                    .Select(c => new Child
                    {
                        Name = c.Name,
                        Age = c.Age
                    })
                    .ToList(),
                ParentDetails = request.ParentDetails is not null
                    ? new ParentDetails
                    {
                        DetailsLine1 = request.ParentDetails.DetailsLine1,
                        DetailsLine2 = request.ParentDetails.DetailsLine2,
                        ParentSubDetails = request.ParentDetails?.ParentSubDetails is not null
                            ? new ParentSubDetails
                            {
                                SubDetailsLine1 = request.ParentDetails.ParentSubDetails.SubDetailsLine1,
                                SubDetailsLine2 = request.ParentDetails.ParentSubDetails.SubDetailsLine2
                            }
                            : null,
                        ParentDetailsTags = request.ParentDetails.ParentDetailsTags?
                            .Select(pdt => new ParentDetailsTags
                            {
                                TagName = pdt.TagName,
                                TagValue = pdt.TagValue
                            })
                            .ToList()
                    }
                    : null
            };

            _parentRepository.Add(parent);
            await _parentRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
            return parent.Id;
        }
    }
}