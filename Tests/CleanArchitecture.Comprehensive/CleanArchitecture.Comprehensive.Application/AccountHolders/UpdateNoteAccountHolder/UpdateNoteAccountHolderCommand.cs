using System;
using CleanArchitecture.Comprehensive.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace CleanArchitecture.Comprehensive.Application.AccountHolders.UpdateNoteAccountHolder
{
    public class UpdateNoteAccountHolderCommand : IRequest<string>, ICommand
    {
        public UpdateNoteAccountHolderCommand(Guid accountHolderId, Guid id, string note)
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