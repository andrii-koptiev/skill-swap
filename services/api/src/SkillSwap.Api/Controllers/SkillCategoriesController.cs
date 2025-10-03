using System.Text;
using Microsoft.AspNetCore.Mvc;
using SkillSwap.Api.Extensions;
using SkillSwap.Application.Interfaces;
using SkillSwap.Contracts.Requests;
using SkillSwap.Contracts.Responses;
using SkillSwap.Domain.Entities;

namespace SkillSwap.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class SkillCategoriesController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<SkillCategoriesController> _logger;

    public SkillCategoriesController(
        IUnitOfWork unitOfWork,
        ILogger<SkillCategoriesController> logger
    )
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
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
            var existingCategory = await _unitOfWork.SkillCategories.GetByNameAsync(request.Name);

            if (existingCategory != null)
            {
                return BadRequest(new { message = "A category with this name already exists." });
            }

            var category = new SkillCategory(
                name: request.Name,
                description: request.Description,
                slug: GenerateSlug(request.Name),
                color: request.ColorHex,
                icon: request.IconUrl
            );

            await _unitOfWork.SkillCategories.AddAsync(category);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation(
                "Successfully created skill category {CategoryId} with name: {CategoryName}",
                category.Id,
                category.Name
            );

            var response = new SkillCategoryResponse
            {
                Id = category.Id,
                Name = category.Name,
                Description = category.Description ?? string.Empty,
                ColorHex = category.Color ?? string.Empty,
                IconUrl = category.Icon,
                CreatedAt = DateTimeOffset.UtcNow,
                UpdatedAt = DateTimeOffset.UtcNow,
            };

            return CreatedAtAction(nameof(GetSkillCategory), new { id = category.Id }, response);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Error creating skill category with name: {CategoryName}",
                request.Name
            );
            return StatusCode(
                500,
                new { message = "An error occurred while creating the skill category." }
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
            var category = await _unitOfWork.SkillCategories.GetByIdAsync(id);

            if (category == null)
            {
                return NotFound(new { message = $"Skill category with ID {id} not found." });
            }

            var response = new SkillCategoryResponse
            {
                Id = category.Id,
                Name = category.Name,
                Description = category.Description,
                ColorHex = category.Color ?? string.Empty,
                IconUrl = category.Icon,
                CreatedAt = DateTimeOffset.UtcNow,
                UpdatedAt = DateTimeOffset.UtcNow,
            };

            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving skill category with ID: {CategoryId}", id);
            return StatusCode(
                500,
                new { message = "An error occurred while retrieving the skill category." }
            );
        }
    }

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<SkillCategoryResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<SkillCategoryResponse>>> GetSkillCategories()
    {
        _logger.LogInformation("Getting all skill categories");

        try
        {
            var categories = await _unitOfWork.SkillCategories.GetActiveAsync();

            var responses = categories.Select(category => new SkillCategoryResponse
            {
                Id = category.Id,
                Name = category.Name,
                Description = category.Description ?? string.Empty,
                ColorHex = category.Color,
                IconUrl = category.Icon,
                CreatedAt = DateTimeOffset.UtcNow,
                UpdatedAt = DateTimeOffset.UtcNow,
            });

            return Ok(responses);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving skill categories");
            return StatusCode(
                500,
                new { message = "An error occurred while retrieving skill categories." }
            );
        }
    }

    private static string GenerateSlug(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            return string.Empty;

        return name.ToLowerInvariant()
            .Replace(" ", "-")
            .Replace("&", "and")
            .Replace("'", "")
            .Replace("\"", "")
            // Remove any non-alphanumeric characters except hyphens
            .Where(c => char.IsLetterOrDigit(c) || c == '-')
            .Aggregate(new StringBuilder(), (sb, c) => sb.Append(c))
            .ToString()
            .Trim('-')
            // Remove consecutive hyphens
            .Replace("--", "-");
    }
}
