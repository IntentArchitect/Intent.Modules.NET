using System;
using AdvancedMappingCrud.Repositories.Tests.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Application.CompanyContactSeconds.CreateCompanyContactSecond
{
    public class CreateCompanyContactSecondCommand : IRequest<Guid>, ICommand
    {
        public CreateCompanyContactSecondCommand(Guid contactSecondId)
        {
            ContactSecondId = contactSecondId;
        }

        public Guid ContactSecondId { get; set; }
    }
}