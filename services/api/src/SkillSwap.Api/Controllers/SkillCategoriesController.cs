using MediatR;
using Microsoft.AspNetCore.Mvc;
using SkillSwap.Api.Extensions;
using SkillSwap.Application.Features.SkillCategories.Commands.CreateSkillCategory;
using SkillSwap.Application.Features.SkillCategories.Queries.GetAllSkillCategories;
using SkillSwap.Application.Features.SkillCategories.Queries.GetSkillCategoryById;
using SkillSwap.Contracts.Requests;
using SkillSwap.Contracts.Responses;

namespace SkillSwap.Api.Controllers;

[ApiController]
[Route("api/skill-categories")]
[Produces("application/json")]
public class SkillCategoriesController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<SkillCategoriesController> _logger;

    public SkillCategoriesController(IMediator mediator, ILogger<SkillCategoriesController> logger)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    [HttpPost]
    [ProducesResponseType(typeof(SkillCategoryResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<SkillCategoryResponse>> CreateSkillCategory(
        [FromBody] CreateSkillCategoryRequest request
    )
    {
        _logger.LogInformation(
            "Creating skill category with name: {CategoryName}",
            LoggingExtensions.SanitizeRequestNameForLog(request.Name)
        );

        try
        {
            var command = new CreateSkillCategoryCommand
            {
                Name = request.Name,
                Description = request.Description,
                ColorHex = request.ColorHex,
                IconUrl = request.IconUrl,
            };

            var response = await _mediator.Send(command);

            _logger.LogInformation(
                "Successfully created skill category {CategoryId} with name: {CategoryName}",
                response.Id,
                response.Name
            );

            return CreatedAtAction(nameof(GetSkillCategory), new { id = response.Id }, response);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(
                ex,
                "Business rule violation while creating skill category: {CategoryName}",
                LoggingExtensions.SanitizeRequestNameForLog(request.Name)
            );
            return BadRequest(new { message = ex.Message });
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(
                ex,
                "Invalid input while creating skill category: {CategoryName}",
                LoggingExtensions.SanitizeRequestNameForLog(request.Name)
            );
            return BadRequest(new { message = ex.Message });
        }
        catch (TimeoutException ex)
        {
            _logger.LogError(
                ex,
                "Timeout occurred while creating skill category with name: {CategoryName}",
                LoggingExtensions.SanitizeRequestNameForLog(request.Name)
            );
            return StatusCode(
                504,
                new { message = "The request timed out while creating the skill category." }
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Error creating skill category with name: {CategoryName}",
                LoggingExtensions.SanitizeRequestNameForLog(request.Name)
            );
            return StatusCode(
                500,
                new { message = "An unexpected error occurred while creating the skill category." }
            );
        }
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(SkillCategoryResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<SkillCategoryResponse>> GetSkillCategory(Guid id)
    {
        _logger.LogInformation("Getting skill category with ID: {CategoryId}", id);

        try
        {
            var query = new GetSkillCategoryByIdQuery { Id = id };
            var response = await _mediator.Send(query);

            if (response == null)
            {
                return NotFound(new { message = $"Skill category with ID {id} not found." });
            }

            return Ok(response);
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning(
                ex,
                "Skill category with ID: {CategoryId} not found (exception).",
                id
            );
            return NotFound(new { message = $"Skill category with ID {id} not found." });
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(
                ex,
                "Invalid argument when retrieving skill category with ID: {CategoryId}.",
                id
            );
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving skill category with ID: {CategoryId}", id);
            return StatusCode(
                500,
                new
                {
                    message = "An unexpected error occurred while retrieving the skill category.",
                }
            );
        }
    }

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<SkillCategoryResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<SkillCategoryResponse>>> GetSkillCategories()
    {
        _logger.LogInformation("Getting all skill categories");

        var query = new GetAllSkillCategoriesQuery();
        var responses = await _mediator.Send(query);

        return Ok(responses);
    }
}
