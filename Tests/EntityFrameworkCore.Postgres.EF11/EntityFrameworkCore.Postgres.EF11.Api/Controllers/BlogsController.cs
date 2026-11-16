using System.ComponentModel.DataAnnotations;
using System.Transactions;
using EntityFrameworkCore.Postgres.EF11.Application;
using EntityFrameworkCore.Postgres.EF11.Application.Interfaces;
using EntityFrameworkCore.Postgres.EF11.Domain.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.AspNetCore.Controllers.Controller", Version = "1.0")]

namespace EntityFrameworkCore.Postgres.EF11.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BlogsController : ControllerBase
    {
        private readonly IBlogsService _appService;
        private readonly IUnitOfWork _unitOfWork;

        public BlogsController(IBlogsService appService, IUnitOfWork unitOfWork)
        {
            _appService = appService ?? throw new ArgumentNullException(nameof(appService));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        /// <summary>
        /// Creates a blog with its first post.
        /// </summary>
        /// <response code="201">Successfully created.</response>
        /// <response code="400">One or more validation errors have occurred.</response>
        [HttpPost]
        [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<Guid>> CreateBlog(
            [FromBody] CreateBlogDto dto,
            CancellationToken cancellationToken = default)
        {
            var result = Guid.Empty;

            using (var transaction = new TransactionScope(TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }, TransactionScopeAsyncFlowOption.Enabled))
            {
                result = await _appService.CreateBlog(dto, cancellationToken);
                await _unitOfWork.SaveChangesAsync(cancellationToken);
                transaction.Complete();
            }
            return Created(string.Empty, result);
        }

        /// <summary>
        /// Returns blogs that have posts published on or after 'since', counting only those posts (filtered Include).
        /// </summary>
        /// <response code="200">Returns the specified List&lt;BlogSummaryDto&gt;.</response>
        /// <response code="400">One or more validation errors have occurred.</response>
        [HttpGet]
        [ProducesResponseType(typeof(List<BlogSummaryDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<List<BlogSummaryDto>>> GetBlogsWithPostsSince(
            [FromQuery][Required] DateOnly since,
            CancellationToken cancellationToken = default)
        {
            var result = default(List<BlogSummaryDto>);
            result = await _appService.GetBlogsWithPostsSince(since, cancellationToken);
            return Ok(result);
        }
    }
}