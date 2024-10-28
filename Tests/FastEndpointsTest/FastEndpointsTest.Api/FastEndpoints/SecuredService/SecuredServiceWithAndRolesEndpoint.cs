using System;
using System.Threading;
using System.Threading.Tasks;
using Asp.Versioning;
using FastEndpoints;
using FastEndpoints.AspVersioning;
using FastEndpointsTest.Application.SecuredService.SecuredServiceWithAndRoles;
using Intent.RoslynWeaver.Attributes;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Mode = Intent.RoslynWeaver.Attributes.Mode;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.FastEndpoints.EndpointTemplate", Version = "1.0")]

namespace FastEndpointsTest.Api.FastEndpoints.SecuredService
{
    public class SecuredServiceWithAndRolesEndpoint : EndpointWithoutRequest
    {
        private readonly ISender _mediator;

        public SecuredServiceWithAndRolesEndpoint(ISender mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public override void Configure()
        {
            Put("api/secured-service/secured-service-with-and-roles");
            Description(b =>
            {
                b.WithTags("SecuredService");
                b.Produces(StatusCodes.Status204NoContent);
                b.ProducesProblemDetails(StatusCodes.Status500InternalServerError);
            });
            Roles("Admin", "One");
            Roles("Admin", "Two");
            Options(x => x.WithVersionSet(">>Api Version<<").MapToApiVersion(new ApiVersion(1.0)));
        }

        public override async Task HandleAsync(CancellationToken ct)
        {
            await _mediator.Send(new SecuredServiceWithAndRoles(), ct);
            await SendResultAsync(TypedResults.NoContent());
        }
    }
}