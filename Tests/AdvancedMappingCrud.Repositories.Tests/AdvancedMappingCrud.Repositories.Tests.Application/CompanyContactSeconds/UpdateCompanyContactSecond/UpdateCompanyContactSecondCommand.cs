using System;
using AdvancedMappingCrud.Repositories.Tests.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Application.CompanyContactSeconds.UpdateCompanyContactSecond
{
    public class UpdateCompanyContactSecondCommand : IRequest, ICommand
    {
        public UpdateCompanyContactSecondCommand(Guid contactSecondId, Guid id, string contactName, string name)
        {
            ContactSecondId = contactSecondId;
            Id = id;
            ContactName = contactName;
            Name = name;
        }

        public Guid ContactSecondId { get; set; }
        public Guid Id { get; set; }
        public string ContactName { get; set; }
        public string Name { get; set; }
    }
}