using System.Text;
using Microsoft.Extensions.Logging;
using SkillSwap.Application.Common;
using SkillSwap.Application.Interfaces;
using SkillSwap.Contracts.Responses;
using SkillSwap.Domain.Entities;

namespace SkillSwap.Application.Features.SkillCategories.Commands.CreateSkillCategory;

/// <summary>
/// Handler for creating a new skill category
/// </summary>
public sealed class CreateSkillCategoryCommandHandler
    : ICommandHandler<CreateSkillCategoryCommand, SkillCategoryResponse>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<CreateSkillCategoryCommandHandler> _logger;

    public CreateSkillCategoryCommandHandler(
        IUnitOfWork unitOfWork,
        ILogger<CreateSkillCategoryCommandHandler> logger
    )
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<SkillCategoryResponse> Handle(
        CreateSkillCategoryCommand request,
        CancellationToken cancellationToken
    )
    {
        _logger.LogInformation("Creating skill category with name: {CategoryName}", request.Name);

        // Check if category with same name already exists
        var existingCategory = await _unitOfWork.SkillCategories.GetByNameAsync(
            request.Name,
            cancellationToken
        );

        if (existingCategory != null)
        {
            throw new InvalidOperationException("A category with this name already exists.");
        }

        // Create the new category
        var category = new SkillCategory(
            name: request.Name,
            description: request.Description,
            slug: GenerateSlug(request.Name),
            color: request.ColorHex,
            icon: request.IconUrl
        );

        // Add to repository and save
        await _unitOfWork.SkillCategories.AddAsync(category, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation(
            "Successfully created skill category {CategoryId} with name: {CategoryName}",
            category.Id,
            category.Name
        );

        // Return response
        return new SkillCategoryResponse
        {
            Id = category.Id,
            Name = category.Name,
            Description = category.Description ?? string.Empty,
            ColorHex = category.Color ?? string.Empty,
            IconUrl = category.Icon,
            CreatedAt = category.CreatedAt,
            UpdatedAt = category.UpdatedAt,
        };
    }

    /// <summary>
    /// Generates a URL-friendly slug from the category name
    /// </summary>
    private static string GenerateSlug(string name)
    {
        var slug = name.Trim().ToLowerInvariant();

        // Replace spaces and special characters with hyphens
        var stringBuilder = new StringBuilder();
        foreach (var c in slug)
        {
            if (char.IsLetterOrDigit(c))
            {
                stringBuilder.Append(c);
            }
            else if (char.IsWhiteSpace(c) || c == '-')
            {
                stringBuilder.Append('-');
            }
        }

        // Remove multiple consecutive hyphens and trim
        slug = stringBuilder.ToString();
        while (slug.Contains("--"))
        {
            slug = slug.Replace("--", "-");
        }

        return slug.Trim('-');
    }
}
