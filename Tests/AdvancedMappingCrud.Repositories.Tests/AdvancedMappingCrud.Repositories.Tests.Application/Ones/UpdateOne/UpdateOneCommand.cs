using System;
using AdvancedMappingCrud.Repositories.Tests.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Application.Ones.UpdateOne
{
    public class UpdateOneCommand : IRequest, ICommand
    {
        public UpdateOneCommand(Guid id, string oneName, UpdateOneTwoDto two)
        {
            Id = id;
            OneName = oneName;
            Two = two;
        }

        public Guid Id { get; set; }
        public string OneName { get; set; }
        public UpdateOneTwoDto Two { get; set; }
    }
}