using System;
using System.Collections.Generic;
using AdvancedMappingCrud.RichDomain.SeparatedEntityState.Tests.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace AdvancedMappingCrud.RichDomain.SeparatedEntityState.Tests.Application.Companies.CreateCompany
{
    public class CreateCompanyCommand : IRequest<Guid>, ICommand
    {
        public CreateCompanyCommand(string name, List<CreateCompanyContactDetailsVODto> contactDetailsVOS)
        {
            Name = name;
            ContactDetailsVOS = contactDetailsVOS;
        }

        public string Name { get; set; }
        public List<CreateCompanyContactDetailsVODto> ContactDetailsVOS { get; set; }
    }
}