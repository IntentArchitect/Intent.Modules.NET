using FastEndpoints;
using FastEndpointsTest.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace FastEndpointsTest.Application.Headers.HeaderField
{
    public class HeaderFieldCommand : IRequest, ICommand
    {
        public HeaderFieldCommand(string header)
        {
            Header = header;
        }

        [FromHeader("X-HEADER-FIELD")]
        public string Header { get; set; }
    }
}