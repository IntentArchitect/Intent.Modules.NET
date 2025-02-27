using System;
using System.Collections.Generic;
using System.Net.Mime;
using System.Threading;
using System.Threading.Tasks;
using AdvancedMappingCrud.Repositories.Tests.Api.Controllers.ResponseTypes;
using AdvancedMappingCrud.Repositories.Tests.Application.Farmers;
using AdvancedMappingCrud.Repositories.Tests.Application.Farmers.AddPlotFarmer;
using AdvancedMappingCrud.Repositories.Tests.Application.Farmers.ChangeNameFarmer;
using AdvancedMappingCrud.Repositories.Tests.Application.Farmers.ChangeNameMachines;
using AdvancedMappingCrud.Repositories.Tests.Application.Farmers.CreateFarmer;
using AdvancedMappingCrud.Repositories.Tests.Application.Farmers.CreateMachines;
using AdvancedMappingCrud.Repositories.Tests.Application.Farmers.DeleteFarmer;
using AdvancedMappingCrud.Repositories.Tests.Application.Farmers.DeleteMachines;
using AdvancedMappingCrud.Repositories.Tests.Application.Farmers.GetFarmerById;
using AdvancedMappingCrud.Repositories.Tests.Application.Farmers.GetFarmers;
using AdvancedMappingCrud.Repositories.Tests.Application.Farmers.GetMachines;
using AdvancedMappingCrud.Repositories.Tests.Application.Farmers.GetMachinesById;
using AdvancedMappingCrud.Repositories.Tests.Application.Farmers.UpdateFarmer;
using AdvancedMappingCrud.Repositories.Tests.Application.Farmers.UpdateMachines;
using Intent.RoslynWeaver.Attributes;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.AspNetCore.Controllers.Controller", Version = "1.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Api.Controllers
{
    [ApiController]
    public class FarmersController : ControllerBase
    {
        private readonly ISender _mediator;

        public FarmersController(ISender mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        /// <summary>
        /// </summary>
        /// <response code="201">Successfully created.</response>
        /// <response code="400">One or more validation errors have occurred.</response>
        /// <response code="404">One or more entities could not be found with the provided parameters.</response>
        [HttpPost("api/farmers/{id}/add-plot")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> AddPlotFarmer(
            [FromRoute] Guid id,
            [FromBody] AddPlotFarmerCommand command,
            CancellationToken cancellationToken = default)
        {
            if (command.Id == Guid.Empty)
            {
                command.Id = id;
            }

            if (id != command.Id)
            {
                return BadRequest();
            }

            await _mediator.Send(command, cancellationToken);
            return Created(string.Empty, null);
        }

        /// <summary>
        /// </summary>
        /// <response code="204">Successfully updated.</response>
        /// <response code="400">One or more validation errors have occurred.</response>
        /// <response code="404">One or more entities could not be found with the provided parameters.</response>
        [HttpPut("api/farmers/{id}/change-name")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> ChangeNameFarmer(
            [FromRoute] Guid id,
            [FromBody] ChangeNameFarmerCommand command,
            CancellationToken cancellationToken = default)
        {
            if (command.Id == Guid.Empty)
            {
                command.Id = id;
            }

            if (id != command.Id)
            {
                return BadRequest();
            }

            await _mediator.Send(command, cancellationToken);
            return NoContent();
        }

        /// <summary>
        /// </summary>
        /// <response code="204">Successfully updated.</response>
        /// <response code="400">One or more validation errors have occurred.</response>
        /// <response code="404">One or more entities could not be found with the provided parameters.</response>
        [HttpPut("api/farmers/{farmerId}/machines/{id}/change-name")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> ChangeNameMachines(
            [FromRoute] Guid farmerId,
            [FromRoute] Guid id,
            [FromBody] ChangeNameMachinesCommand command,
            CancellationToken cancellationToken = default)
        {
            if (command.FarmerId == Guid.Empty)
            {
                command.FarmerId = farmerId;
            }

            if (command.Id == Guid.Empty)
            {
                command.Id = id;
            }

            if (farmerId != command.FarmerId)
            {
                return BadRequest();
            }

            if (id != command.Id)
            {
                return BadRequest();
            }

            await _mediator.Send(command, cancellationToken);
            return NoContent();
        }

        /// <summary>
        /// </summary>
        /// <response code="201">Successfully created.</response>
        /// <response code="400">One or more validation errors have occurred.</response>
        [HttpPost("api/farmers")]
        [Produces(MediaTypeNames.Application.Json)]
        [ProducesResponseType(typeof(JsonResponse<Guid>), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<JsonResponse<Guid>>> CreateFarmer(
            [FromBody] CreateFarmerCommand command,
            CancellationToken cancellationToken = default)
        {
            var result = await _mediator.Send(command, cancellationToken);
            return CreatedAtAction(nameof(GetFarmerById), new { id = result }, new JsonResponse<Guid>(result));
        }

        /// <summary>
        /// </summary>
        /// <response code="201">Successfully created.</response>
        /// <response code="400">One or more validation errors have occurred.</response>
        /// <response code="404">One or more entities could not be found with the provided parameters.</response>
        [HttpPost("api/farmers/{farmerId}/machines")]
        [Produces(MediaTypeNames.Application.Json)]
        [ProducesResponseType(typeof(JsonResponse<Guid>), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<JsonResponse<Guid>>> CreateMachines(
            [FromRoute] Guid farmerId,
            [FromBody] CreateMachinesCommand command,
            CancellationToken cancellationToken = default)
        {
            if (command.FarmerId == Guid.Empty)
            {
                command.FarmerId = farmerId;
            }

            if (farmerId != command.FarmerId)
            {
                return BadRequest();
            }

            var result = await _mediator.Send(command, cancellationToken);
            return CreatedAtAction(nameof(GetMachinesById), new { farmerId = farmerId, id = result }, new JsonResponse<Guid>(result));
        }

        /// <summary>
        /// </summary>
        /// <response code="200">Successfully deleted.</response>
        /// <response code="400">One or more validation errors have occurred.</response>
        /// <response code="404">One or more entities could not be found with the provided parameters.</response>
        [HttpDelete("api/farmers/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> DeleteFarmer([FromRoute] Guid id, CancellationToken cancellationToken = default)
        {
            await _mediator.Send(new DeleteFarmerCommand(id: id), cancellationToken);
            return Ok();
        }

        /// <summary>
        /// </summary>
        /// <response code="200">Successfully deleted.</response>
        /// <response code="400">One or more validation errors have occurred.</response>
        /// <response code="404">One or more entities could not be found with the provided parameters.</response>
        [HttpDelete("api/farmers/{farmerId}/machines/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> DeleteMachines(
            [FromRoute] Guid farmerId,
            [FromRoute] Guid id,
            CancellationToken cancellationToken = default)
        {
            await _mediator.Send(new DeleteMachinesCommand(farmerId: farmerId, id: id), cancellationToken);
            return Ok();
        }

        /// <summary>
        /// </summary>
        /// <response code="204">Successfully updated.</response>
        /// <response code="400">One or more validation errors have occurred.</response>
        /// <response code="404">One or more entities could not be found with the provided parameters.</response>
        [HttpPut("api/farmers/{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> UpdateFarmer(
            [FromRoute] Guid id,
            [FromBody] UpdateFarmerCommand command,
            CancellationToken cancellationToken = default)
        {
            if (command.Id == Guid.Empty)
            {
                command.Id = id;
            }

            if (id != command.Id)
            {
                return BadRequest();
            }

            await _mediator.Send(command, cancellationToken);
            return NoContent();
        }

        /// <summary>
        /// </summary>
        /// <response code="204">Successfully updated.</response>
        /// <response code="400">One or more validation errors have occurred.</response>
        /// <response code="404">One or more entities could not be found with the provided parameters.</response>
        [HttpPut("api/farmers/{farmerId}/machines/{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> UpdateMachines(
            [FromRoute] Guid farmerId,
            [FromRoute] Guid id,
            [FromBody] UpdateMachinesCommand command,
            CancellationToken cancellationToken = default)
        {
            if (command.FarmerId == Guid.Empty)
            {
                command.FarmerId = farmerId;
            }

            if (command.Id == Guid.Empty)
            {
                command.Id = id;
            }

            if (farmerId != command.FarmerId)
            {
                return BadRequest();
            }

            if (id != command.Id)
            {
                return BadRequest();
            }

            await _mediator.Send(command, cancellationToken);
            return NoContent();
        }

        /// <summary>
        /// </summary>
        /// <response code="200">Returns the specified FarmerDto.</response>
        /// <response code="400">One or more validation errors have occurred.</response>
        /// <response code="404">No FarmerDto could be found with the provided parameters.</response>
        [HttpGet("api/farmers/{id}")]
        [ProducesResponseType(typeof(FarmerDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<FarmerDto>> GetFarmerById(
            [FromRoute] Guid id,
            CancellationToken cancellationToken = default)
        {
            var result = await _mediator.Send(new GetFarmerByIdQuery(id: id), cancellationToken);
            return result == null ? NotFound() : Ok(result);
        }

        /// <summary>
        /// </summary>
        /// <response code="200">Returns the specified List&lt;FarmerDto&gt;.</response>
        [HttpGet("api/farmers")]
        [ProducesResponseType(typeof(List<FarmerDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<List<FarmerDto>>> GetFarmers(CancellationToken cancellationToken = default)
        {
            var result = await _mediator.Send(new GetFarmersQuery(), cancellationToken);
            return Ok(result);
        }

        /// <summary>
        /// </summary>
        /// <response code="200">Returns the specified MachinesDto.</response>
        /// <response code="400">One or more validation errors have occurred.</response>
        /// <response code="404">No MachinesDto could be found with the provided parameters.</response>
        [HttpGet("api/farmers/{farmerId}/machines/{id}")]
        [ProducesResponseType(typeof(MachinesDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<MachinesDto>> GetMachinesById(
            [FromRoute] Guid farmerId,
            [FromRoute] Guid id,
            CancellationToken cancellationToken = default)
        {
            var result = await _mediator.Send(new GetMachinesByIdQuery(farmerId: farmerId, id: id), cancellationToken);
            return result == null ? NotFound() : Ok(result);
        }

        /// <summary>
        /// </summary>
        /// <response code="200">Returns the specified List&lt;MachinesDto&gt;.</response>
        /// <response code="400">One or more validation errors have occurred.</response>
        /// <response code="404">No List&lt;MachinesDto&gt; could be found with the provided parameters.</response>
        [HttpGet("api/farmers/{farmerId}/machines")]
        [ProducesResponseType(typeof(List<MachinesDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<List<MachinesDto>>> GetMachines(
            [FromRoute] Guid farmerId,
            CancellationToken cancellationToken = default)
        {
            var result = await _mediator.Send(new GetMachinesQuery(farmerId: farmerId), cancellationToken);
            return result == null ? NotFound() : Ok(result);
        }
    }
}