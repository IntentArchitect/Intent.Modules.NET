using System;
using System.Collections.Generic;
using CleanArchitecture.Comprehensive.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace CleanArchitecture.Comprehensive.Application.Ones.CreateOne
{
    public class CreateOneCommand : IRequest<Guid>, ICommand
    {
        public CreateOneCommand(int oneId, List<CreateOneCommandTwosDto> twos)
        {
            OneId = oneId;
            Twos = twos;
        }

        public int OneId { get; set; }
        public List<CreateOneCommandTwosDto> Twos { get; set; }
    }
}