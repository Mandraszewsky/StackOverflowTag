using MediatR;
using Microsoft.AspNetCore.Mvc;
using StackOverflow.Application.Tags.Commands;
using StackOverflow.Application.Tags.Dtos;
using StackOverflow.Application.Tags.Queries;

namespace StackOverflow.WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TagsController(IMediator mediator) : ControllerBase
{
    /// <summary>
    /// Gets paginated tags with optional sorting.
    /// </summary>
    /// <param name="pageNumber">Page number (default: 1)</param>
    /// <param name="pageSize">Page size (default: 25)</param>
    /// <param name="sortBy">Sort by field: "name", "percentage", or "count" (default: count)</param>
    /// <param name="sortDirection">Sort direction: "asc" or "desc" (default: asc)</param>
    [HttpGet]
    [ProducesResponseType(typeof(PagedResultDto<TagResponseDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetTags(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 25,
        [FromQuery] string sortBy = "",
        [FromQuery] string sortDirection = "asc")
    {
        var query = new GetTagsQuery
        {
            PageNumber = pageNumber,
            PageSize = pageSize,
            SortBy = sortBy,
            SortDirection = sortDirection
        };

        var result = await mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// Forces a re-fetch of tags from the StackOverflow API.
    /// </summary>
    [HttpPost("refresh")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> RefreshTags()
    {
        await mediator.Send(new RefreshTagsCommand());
        return NoContent();
    }
}
