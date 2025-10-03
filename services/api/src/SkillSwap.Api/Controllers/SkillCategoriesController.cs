using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using SkillSwap.Contracts.Requests;

namespace SkillSwap.Api.Controllers;

/// <summary>
/// Controller for managing skill categories.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class SkillCategoriesController : ControllerBase
{
    private readonly ILogger<SkillCategoriesController> _logger;
    private readonly IValidator<CreateSkillCategoryRequest> _validator;

    public SkillCategoriesController(
        ILogger<SkillCategoriesController> logger,
        IValidator<CreateSkillCategoryRequest> validator
    )
    {
        _logger = logger;
        _validator = validator;
    }

    /// <summary>
    /// Creates a new skill category.
    /// </summary>
    /// <param name="request">The skill category creation request.</param>
    /// <returns>The created skill category.</returns>
    [HttpPost]
    [ProducesResponseType(typeof(CreateSkillCategoryRequest), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<CreateSkillCategoryRequest>> CreateSkillCategory(
        [FromBody] CreateSkillCategoryRequest request
    )
    {
        _logger.LogInformation("Creating skill category with name: {CategoryName}", request.Name);

        // Validate the request
        var validationResult = await _validator.ValidateAsync(request);
        if (!validationResult.IsValid)
        {
            foreach (var error in validationResult.Errors)
            {
                ModelState.AddModelError(error.PropertyName, error.ErrorMessage);
            }
            return BadRequest(ModelState);
        }

        // TODO: Implement actual creation logic
        _logger.LogInformation(
            "Skill category '{CategoryName}' created successfully",
            request.Name
        );

        return NoContent();
    }

    /// <summary>
    /// Gets a skill category by ID.
    /// </summary>
    /// <param name="id">The skill category ID.</param>
    /// <returns>The skill category.</returns>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(CreateSkillCategoryRequest), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CreateSkillCategoryRequest>> GetSkillCategory(Guid id)
    {
        _logger.LogInformation("Getting skill category with ID: {CategoryId}", id);

        // TODO: Implement actual retrieval logic
        await Task.Delay(1); // Simulate async operation

        return NotFound(
            new ProblemDetails
            {
                Status = StatusCodes.Status404NotFound,
                Title = "Skill Category Not Found",
                Detail = $"Skill category with ID {id} not found",
                Instance = HttpContext?.Request?.Path,
            }
        );
    }

    /// <summary>
    /// Gets all skill categories.
    /// </summary>
    /// <returns>List of skill categories.</returns>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<CreateSkillCategoryRequest>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<CreateSkillCategoryRequest>>> GetSkillCategories()
    {
        _logger.LogInformation("Getting all skill categories");

        // TODO: Implement actual retrieval logic
        await Task.Delay(1); // Simulate async operation

        var sampleCategories = new[]
        {
            new CreateSkillCategoryRequest
            {
                Name = "Technology",
                Description = "Programming, software development, and technical skills",
                ColorHex = "#0078D4",
                IconUrl = null,
            },
            new CreateSkillCategoryRequest
            {
                Name = "Creative",
                Description = "Art, design, music, and creative skills",
                ColorHex = "#E74C3C",
                IconUrl = null,
            },
        };

        return Ok(sampleCategories);
    }
}
