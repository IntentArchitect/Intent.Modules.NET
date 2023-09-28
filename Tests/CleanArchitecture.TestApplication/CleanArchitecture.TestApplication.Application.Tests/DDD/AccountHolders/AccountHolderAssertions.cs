using System.Collections.Generic;
using System.Linq;
using CleanArchitecture.TestApplication.Application.AccountHolders.UpdateNoteAccountHolder;
using CleanArchitecture.TestApplication.Application.DDD.AccountTransfer;
using CleanArchitecture.TestApplication.Application.DDD.ChangeAccountHolderName;
using CleanArchitecture.TestApplication.Application.DDD.CreateAccountHolder;
using CleanArchitecture.TestApplication.Domain.Entities.DDD;
using FluentAssertions;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CRUD.Tests.Assertions.AssertionClass", Version = "1.0")]

namespace CleanArchitecture.TestApplication.Application.Tests.DDD.AccountHolders
{
    public static class AccountHolderAssertions
    {
    }
}