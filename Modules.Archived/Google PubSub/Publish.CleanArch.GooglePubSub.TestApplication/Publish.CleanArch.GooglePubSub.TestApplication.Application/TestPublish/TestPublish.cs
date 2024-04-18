using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;
using MediatR;
using Publish.CleanArch.GooglePubSub.TestApplication.Application.Common.Interfaces;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace Publish.CleanArch.GooglePubSub.TestApplication.Application.TestPublish
{
    public class TestPublish : IRequest, ICommand
    {
        public TestPublish(string message)
        {
            Message = message;
        }
        public string Message { get; set; }

    }
}