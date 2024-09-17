using System;
using AdvancedMappingCrud.RichDomain.SeparatedEntityState.Tests.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace AdvancedMappingCrud.RichDomain.SeparatedEntityState.Tests.Application.Users.TestVOUser
{
    public class TestVOUserCommand : IRequest<TestContactDetailsVODto>, ICommand
    {
        public TestVOUserCommand(Guid id)
        {
            Id = id;
        }

        public Guid Id { get; set; }
    }
}