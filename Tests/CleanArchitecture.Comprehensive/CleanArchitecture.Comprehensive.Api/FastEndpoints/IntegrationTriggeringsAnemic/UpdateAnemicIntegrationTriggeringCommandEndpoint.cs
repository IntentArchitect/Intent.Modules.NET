using System;
using System.Net.Mime;
using System.Threading;
using System.Threading.Tasks;
using CleanArchitecture.Comprehensive.Application.IntegrationTriggeringsAnemic.UpdateAnemicIntegrationTriggering;
using FastEndpoints;
using Intent.RoslynWeaver.Attributes;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Mode = Intent.RoslynWeaver.Attributes.Mode;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.FastEndpoints.EndpointTemplate", Version = "1.0")]

namespace CleanArchitecture.Comprehensive.Api.FastEndpoints.IntegrationTriggeringsAnemic
{
    public class UpdateAnemicIntegrationTriggeringCommandEndpoint : Endpoint<UpdateAnemicIntegrationTriggeringCommand>
    {
        private readonly ISender _mediator;

        public UpdateAnemicIntegrationTriggeringCommandEndpoint(ISender mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public override void Configure()
        {
            Put("api/integration-triggerings-anemic/{id}");
            Description(b =>
            {
                b.WithTags("IntegrationTriggeringsAnemic");
                b.Accepts<UpdateAnemicIntegrationTriggeringCommand>(MediaTypeNames.Application.Json);
                b.Produces(StatusCodes.Status204NoContent);
                b.ProducesProblemDetails();
                b.ProducesProblemDetails(StatusCodes.Status500InternalServerError);
            });
        }

        public override async Task HandleAsync(UpdateAnemicIntegrationTriggeringCommand req, CancellationToken ct)
        {
            await _mediator.Send(req, ct);
            await SendResultAsync(TypedResults.NoContent());
        }
    }
}