using System;
using AdvancedMappingCrud.RichDomain.Tests.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace AdvancedMappingCrud.RichDomain.Tests.Application.Users.TestDCUser
{
    public class TestDCUserCommand : IRequest<TestAddressDCDto?>, ICommand
    {
        public TestDCUserCommand(Guid id, int index)
        {
            Id = id;
            Index = index;
        }

        public Guid Id { get; set; }
        public int Index { get; set; }
    }
}