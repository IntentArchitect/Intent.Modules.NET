using CleanArchitecture.TestApplication.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace CleanArchitecture.TestApplication.Application.NamedQueryStrings.NamedQueryStrings
{
    public class NamedQueryStringsCommand : IRequest, ICommand
    {
        public NamedQueryStringsCommand(string par1)
        {
            Par1 = par1;
        }

        public string Par1 { get; set; }
    }
}