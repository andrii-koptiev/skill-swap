using Microsoft.AspNetCore.Mvc;
using SkillSwap.Api.Extensions;
using SkillSwap.Contracts.Requests;
using SkillSwap.Contracts.Responses;

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

    public SkillCategoriesController(ILogger<SkillCategoriesController> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Creates a new skill category.
    /// </summary>
    /// <param name="request">The skill category creation request.</param>
    /// <returns>The created skill category.</returns>
    [HttpPost]
    [ProducesResponseType(typeof(SkillCategoryResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public Task<ActionResult<SkillCategoryResponse>> CreateSkillCategory(
        [FromBody] CreateSkillCategoryRequest request
    )
    {
        _logger.LogInformation(
            "Creating skill category with name: {CategoryName}",
            LoggingExtensions.SanitizeRequestNameForLog(request.Name)
        );

        // TODO: Implement actual creation logic with repository/service
        var now = DateTimeOffset.UtcNow;
        var categoryId = Guid.NewGuid();

        var createdCategory = new SkillCategoryResponse
        {
            Id = categoryId,
            Name = request.Name,
            Description = request.Description,
            ColorHex = request.ColorHex,
            IconUrl = request.IconUrl,
            CreatedAt = now,
            UpdatedAt = now,
        };

        _logger.LogInformation(
            "Skill category '{CategoryName}' created successfully with ID: {CategoryId}",
            LoggingExtensions.SanitizeRequestNameForLog(request.Name),
            categoryId
        );

        return Task.FromResult<ActionResult<SkillCategoryResponse>>(
            CreatedAtAction(nameof(GetSkillCategory), new { id = categoryId }, createdCategory)
        );
    }

    /// <summary>
    /// Gets a skill category by ID.
    /// </summary>
    /// <param name="id">The skill category ID.</param>
    /// <returns>The skill category.</returns>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(SkillCategoryResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<SkillCategoryResponse>> GetSkillCategory(Guid id)
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
    [ProducesResponseType(typeof(IEnumerable<SkillCategoryResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<SkillCategoryResponse>>> GetSkillCategories()
    {
        _logger.LogInformation("Getting all skill categories");

        // TODO: Implement actual retrieval logic
        await Task.Delay(1); // Simulate async operation

        var now = DateTimeOffset.UtcNow;
        var sampleCategories = new[]
        {
            new SkillCategoryResponse
            {
                Id = Guid.NewGuid(),
                Name = "Technology",
                Description = "Programming, software development, and technical skills",
                ColorHex = "#0078D4",
                IconUrl = null,
                CreatedAt = now.AddDays(-30),
                UpdatedAt = now.AddDays(-30),
            },
            new SkillCategoryResponse
            {
                Id = Guid.NewGuid(),
                Name = "Creative",
                Description = "Art, design, music, and creative skills",
                ColorHex = "#E74C3C",
                IconUrl = null,
                CreatedAt = now.AddDays(-25),
                UpdatedAt = now.AddDays(-25),
            },
        };

        return Ok(sampleCategories);
    }
}
