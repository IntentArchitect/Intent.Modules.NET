using System;
using CleanArchitecture.TestApplication.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace CleanArchitecture.TestApplication.Application.WithCompositeKeys.UpdateWithCompositeKey
{
    public class UpdateWithCompositeKeyCommand : IRequest, ICommand
    {
        public UpdateWithCompositeKeyCommand(string name, Guid key1Id, Guid key2Id)
        {
            Name = name;
            Key1Id = key1Id;
            Key2Id = key2Id;
        }

        public string Name { get; set; }
        public Guid Key1Id { get; private set; }
        public Guid Key2Id { get; private set; }

        public void SetId(Guid key1Id, Guid key2Id)
        {
            if (Key1Id == default)
            {
                Key1Id = key1Id;
            }

            if (Key2Id == default)
            {
                Key2Id = key2Id;
            }
        }
    }
}