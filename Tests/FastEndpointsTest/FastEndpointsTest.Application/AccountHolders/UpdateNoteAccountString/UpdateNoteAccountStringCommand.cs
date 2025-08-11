using System;
using FastEndpointsTest.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace FastEndpointsTest.Application.AccountHolders.UpdateNoteAccountString
{
    public class UpdateNoteAccountStringCommand : IRequest<string>, ICommand
    {
        public UpdateNoteAccountStringCommand(Guid accountHolderId, Guid id, string note)
        {
            AccountHolderId = accountHolderId;
            Id = id;
            Note = note;
        }

        public Guid AccountHolderId { get; set; }
        public Guid Id { get; set; }
        public string Note { get; set; }
    }
}