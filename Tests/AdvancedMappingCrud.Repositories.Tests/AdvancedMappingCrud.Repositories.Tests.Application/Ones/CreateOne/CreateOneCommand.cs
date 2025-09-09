using System;
using AdvancedMappingCrud.Repositories.Tests.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Application.Ones.CreateOne
{
    public class CreateOneCommand : IRequest<Guid>, ICommand
    {
        public CreateOneCommand(string oneName, CreateOneTwoDto two)
        {
            OneName = oneName;
            Two = two;
        }

        public string OneName { get; set; }
        public CreateOneTwoDto Two { get; set; }
    }
}