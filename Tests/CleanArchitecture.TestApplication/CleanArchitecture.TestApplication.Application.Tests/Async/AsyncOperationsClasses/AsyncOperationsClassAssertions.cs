using System.Collections.Generic;
using System.Linq;
using CleanArchitecture.TestApplication.Application.AsyncOperationsClasses.Explicit;
using CleanArchitecture.TestApplication.Application.AsyncOperationsClasses.ExplicitWithReturn;
using CleanArchitecture.TestApplication.Application.AsyncOperationsClasses.ImplicitAsync;
using CleanArchitecture.TestApplication.Application.AsyncOperationsClasses.ImplicitAsyncWithReturn;
using CleanArchitecture.TestApplication.Domain.Entities.Async;
using FluentAssertions;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CRUD.Tests.Assertions.AssertionClass", Version = "1.0")]

namespace CleanArchitecture.TestApplication.Application.Tests.Async.AsyncOperationsClasses
{
    public static class AsyncOperationsClassAssertions
    {
    }
}