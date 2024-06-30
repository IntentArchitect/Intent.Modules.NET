using System;
using IntegrationTesting.Tests.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace IntegrationTesting.Tests.Application.UniqueConVals.CreateUniqueConVal
{
    public class CreateUniqueConValCommand : IRequest<Guid>, ICommand
    {
        public CreateUniqueConValCommand(string att1, string att2, string attInclude)
        {
            Att1 = att1;
            Att2 = att2;
            AttInclude = attInclude;
        }

        public string Att1 { get; set; }
        public string Att2 { get; set; }
        public string AttInclude { get; set; }
    }
}