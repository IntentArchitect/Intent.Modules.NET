using System;
using System.Net.Mime;
using System.Threading;
using System.Threading.Tasks;
using Asp.Versioning;
using FastEndpoints;
using FastEndpoints.AspVersioning;
using FastEndpointsTest.Application.AccountHolders.UpdateNoteAccountNoResult;
using Intent.RoslynWeaver.Attributes;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Mode = Intent.RoslynWeaver.Attributes.Mode;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.FastEndpoints.EndpointTemplate", Version = "1.0")]

namespace FastEndpointsTest.Api.FastEndpoints.AccountHolders
{
    public class UpdateNoteAccountNoResultCommandEndpoint : Endpoint<UpdateNoteAccountNoResultCommand>
    {
        private readonly ISender _mediator;

        public UpdateNoteAccountNoResultCommandEndpoint(ISender mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public override void Configure()
        {
            Put("api/account-holders/account/{id}/note/no-result");
            Description(b =>
            {
                b.WithTags("AccountHolders");
                b.Accepts<UpdateNoteAccountNoResultCommand>(MediaTypeNames.Application.Json);
                b.Produces(StatusCodes.Status207MultiStatus, contentType: MediaTypeNames.Application.Json);
                b.ProducesProblemDetails();
                b.ProducesProblemDetails(StatusCodes.Status500InternalServerError);
            });
            AllowAnonymous();
            Options(x => x.WithVersionSet(">>Api Version<<").MapToApiVersion(new ApiVersion(1.0)));
        }

        public override async Task HandleAsync(UpdateNoteAccountNoResultCommand req, CancellationToken ct)
        {
            await _mediator.Send(req, ct);
            await Send.ResultAsync(TypedResults.StatusCode(207));
        }
    }
}