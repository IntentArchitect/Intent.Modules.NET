using System;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Application.Exceptions.NotFoundException", Version = "1.0")]
[assembly: DefaultIntentManaged(Mode.Fully)]

namespace CleanArchitecture.TestApplication.Application.Common.Exceptions;

public class NotFoundException : Exception
{
    public NotFoundException(string message) : base(message)
    {
    }
}