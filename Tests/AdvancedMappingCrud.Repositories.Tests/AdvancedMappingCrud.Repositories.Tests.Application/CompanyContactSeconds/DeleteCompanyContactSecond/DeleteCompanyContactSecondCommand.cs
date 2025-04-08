using System;
using AdvancedMappingCrud.Repositories.Tests.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Application.CompanyContactSeconds.DeleteCompanyContactSecond
{
    public class DeleteCompanyContactSecondCommand : IRequest, ICommand
    {
        public DeleteCompanyContactSecondCommand(Guid id)
        {
            Id = id;
        }

        public Guid Id { get; set; }
    }
}