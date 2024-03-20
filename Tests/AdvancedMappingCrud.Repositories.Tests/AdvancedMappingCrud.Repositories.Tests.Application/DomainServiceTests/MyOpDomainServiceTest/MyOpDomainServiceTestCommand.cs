using System;
using AdvancedMappingCrud.Repositories.Tests.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Application.DomainServiceTests.MyOpDomainServiceTest
{
    public class MyOpDomainServiceTestCommand : IRequest, ICommand
    {
        public MyOpDomainServiceTestCommand(Guid id)
        {
            Id = id;
        }

        public Guid Id { get; set; }
    }
}