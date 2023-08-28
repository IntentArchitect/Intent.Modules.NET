using System.Collections.Generic;
using System.Linq;
using CleanArchitecture.TestApplication.Application.OperationsClasses.Sync;
using CleanArchitecture.TestApplication.Application.OperationsClasses.SyncWithReturn;
using CleanArchitecture.TestApplication.Domain.Entities.Operations;
using FluentAssertions;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CRUD.Tests.Assertions.AssertionClass", Version = "1.0")]

namespace CleanArchitecture.TestApplication.Application.Tests.Operations.OperationsClasses
{
    public static class OperationsClassAssertions
    {
    }
}