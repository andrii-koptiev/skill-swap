# CQRS Pattern & MediatR Implementation Guide

## 🎯 Overview

This document provides comprehensive guidance for implementing and maintaining the **Command Query Responsibility Segregation (CQRS)** pattern using **MediatR** in the SkillSwap API.

## 🏗️ Architecture Overview

### CQRS Fundamentals

**CQRS** separates read and write operations into distinct models:

- **Commands**: Handle write operations (Create, Update, Delete)
- **Queries**: Handle read operations (Get, List, Search)
- **Handlers**: Process commands and queries independently

### Benefits

1. **Performance Optimization**: Separate models for read/write operations
2. **Scalability**: Independent scaling of read and write sides
3. **Maintainability**: Clear separation of concerns
4. **Testability**: Isolated business logic in handlers
5. **Flexibility**: Different models optimized for their specific use cases

## 🔧 Implementation Components

### Core Interfaces

```csharp
// Commands
public interface ICommand : IRequest { }
public interface ICommand<out TResponse> : IRequest<TResponse> { }

// Queries
public interface IQuery<out TResponse> : IRequest<TResponse> { }

// Handlers
public interface ICommandHandler<in TCommand> : IRequestHandler<TCommand>
    where TCommand : ICommand { }

public interface ICommandHandler<in TCommand, TResponse> : IRequestHandler<TCommand, TResponse>
    where TCommand : ICommand<TResponse> { }

public interface IQueryHandler<in TQuery, TResponse> : IRequestHandler<TQuery, TResponse>
    where TQuery : IQuery<TResponse> { }
```

### MediatR Configuration

```csharp
// Program.cs
builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssembly(typeof(AssemblyReference).Assembly);
    cfg.AddBehavior(typeof(ValidationBehavior<,>), typeof(ValidationBehavior<,>));
});
```

## 📝 Implementation Patterns

### 1. Command Implementation

**Command Definition:**

```csharp
public sealed record CreateSkillCategoryCommand(
    string Name,
    string Description,
    string? IconName = null
) : ICommand<SkillCategoryResponse>;
```

**Command Handler:**

```csharp
public sealed class CreateSkillCategoryCommandHandler
    : ICommandHandler<CreateSkillCategoryCommand, SkillCategoryResponse>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<CreateSkillCategoryCommandHandler> _logger;

    public async Task<SkillCategoryResponse> Handle(
        CreateSkillCategoryCommand request,
        CancellationToken cancellationToken)
    {
        // 1. Validation (handled by ValidationBehavior)
        // 2. Business logic
        // 3. Data persistence
        // 4. Response mapping
    }
}
```

**Command Validator:**

```csharp
public sealed class CreateSkillCategoryCommandValidator
    : AbstractValidator<CreateSkillCategoryCommand>
{
    public CreateSkillCategoryCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.Description)
            .NotEmpty()
            .MaximumLength(500);
    }
}
```

### 2. Query Implementation

**Query Definition:**

```csharp
public sealed record GetAllSkillCategoriesQuery : IQuery<IEnumerable<SkillCategoryResponse>>;
```

**Query Handler:**

```csharp
public sealed class GetAllSkillCategoriesQueryHandler
    : IQueryHandler<GetAllSkillCategoriesQuery, IEnumerable<SkillCategoryResponse>>
{
    private readonly IUnitOfWork _unitOfWork;

    public async Task<IEnumerable<SkillCategoryResponse>> Handle(
        GetAllSkillCategoriesQuery request,
        CancellationToken cancellationToken)
    {
        // 1. Data retrieval
        // 2. Response mapping
        // 3. Return results
    }
}
```

### 3. Controller Integration

```csharp
[ApiController]
[Route("api/skill-categories")]
public class SkillCategoriesController : ControllerBase
{
    private readonly IMediator _mediator;

    public SkillCategoriesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<ActionResult<SkillCategoryResponse>> CreateSkillCategory(
        CreateSkillCategoryRequest request,
        CancellationToken cancellationToken)
    {
        var command = new CreateSkillCategoryCommand(
            request.Name,
            request.Description,
            request.IconName
        );

        var result = await _mediator.Send(command, cancellationToken);
        return CreatedAtAction(nameof(GetSkillCategory), new { id = result.Id }, result);
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<SkillCategoryResponse>>> GetAllSkillCategories(
        CancellationToken cancellationToken)
    {
        var query = new GetAllSkillCategoriesQuery();
        var result = await _mediator.Send(query, cancellationToken);
        return Ok(result);
    }
}
```

## 🛡️ Cross-Cutting Concerns

### Validation Behavior

```csharp
public sealed class ValidationBehavior<TRequest, TResponse>
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        if (_validators.Any())
        {
            var context = new ValidationContext<TRequest>(request);
            var validationResults = await Task.WhenAll(
                _validators.Select(v => v.ValidateAsync(context, cancellationToken)));

            var failures = validationResults
                .Where(r => r.Errors.Any())
                .SelectMany(r => r.Errors)
                .ToArray();

            if (failures.Any())
                throw new ValidationException(failures);
        }

        return await next();
    }
}
```

## 📁 Project Structure

```
SkillSwap.Application/
├── Common/
│   ├── ICommand.cs
│   ├── IQuery.cs
│   ├── ICommandHandler.cs
│   ├── IQueryHandler.cs
│   └── Behaviors/
│       └── ValidationBehavior.cs
└── Features/
    └── SkillCategories/
        ├── Commands/
        │   ├── CreateSkillCategory/
        │   │   ├── CreateSkillCategoryCommand.cs
        │   │   ├── CreateSkillCategoryCommandHandler.cs
        │   │   └── CreateSkillCategoryCommandValidator.cs
        │   ├── UpdateSkillCategory/
        │   └── DeleteSkillCategory/
        └── Queries/
            ├── GetAllSkillCategories/
            │   ├── GetAllSkillCategoriesQuery.cs
            │   └── GetAllSkillCategoriesQueryHandler.cs
            └── GetSkillCategoryById/
```

## 📋 Best Practices

### 1. Command Design

✅ **DO:**

- Use immutable records for commands and queries
- Include all required data in the command
- Keep commands focused on single operations
- Use meaningful names that describe the intent

❌ **DON'T:**

- Include complex logic in command/query classes
- Share commands between different operations
- Expose internal implementation details

### 2. Handler Design

✅ **DO:**

- Keep handlers focused on single responsibility
- Use dependency injection for required services
- Implement proper error handling
- Use cancellation tokens appropriately
- Log important operations

❌ **DON'T:**

- Put business logic in controllers
- Call handlers directly (always use MediatR)
- Ignore cancellation tokens
- Swallow exceptions without logging

### 3. Error Handling

✅ **DO:**

- Use domain exceptions for business rule violations
- Let global exception middleware handle error responses
- Throw exceptions directly in handlers for error conditions
- Use specific exception types (DomainException, ValidationException)
- Focus handlers on happy path scenarios

❌ **DON'T:**

- Use Result pattern with CQRS (conflicts with MediatR pipeline)
- Handle exceptions within handlers (let middleware handle it)
- Return null for not found scenarios (throw NotFoundException)
- Mix exception and Result patterns

**Exception-Based Error Handling Example:**

```csharp
public async Task<SkillCategoryResponse> Handle(
    GetSkillCategoryByIdQuery request,
    CancellationToken cancellationToken)
{
    var category = await _unitOfWork.SkillCategories.GetByIdAsync(request.Id);

    // ✅ Throw exception - middleware will convert to proper HTTP response
    if (category == null)
        throw new NotFoundException($"Skill category with ID {request.Id} not found");

    return new SkillCategoryResponse(category.Id, category.Name, category.Description);
}
```

### 4. Validation

✅ **DO:**

- Validate all input at the command level
- Use FluentValidation for complex rules
- Provide clear, actionable error messages
- Validate business rules in handlers

❌ **DON'T:**

- Skip validation for "trusted" inputs
- Duplicate validation logic
- Use exceptions for business rule validation

## 🔍 Testing Strategies

### Unit Testing Handlers

```csharp
[Test]
public async Task Handle_ValidCommand_CreatesSkillCategory()
{
    // Arrange
    var unitOfWork = new Mock<IUnitOfWork>();
    var logger = new Mock<ILogger<CreateSkillCategoryCommandHandler>>();
    var handler = new CreateSkillCategoryCommandHandler(unitOfWork.Object, logger.Object);

    var command = new CreateSkillCategoryCommand("Programming", "Software Development");

    // Act
    var result = await handler.Handle(command, CancellationToken.None);

    // Assert
    Assert.That(result.Name, Is.EqualTo("Programming"));
    unitOfWork.Verify(x => x.SkillCategories.AddAsync(It.IsAny<SkillCategory>(), It.IsAny<CancellationToken>()), Times.Once);
}
```

### Integration Testing

```csharp
[Test]
public async Task CreateSkillCategory_ValidRequest_ReturnsCreatedResponse()
{
    // Arrange
    var request = new CreateSkillCategoryRequest("Programming", "Software Development");

    // Act
    var response = await _client.PostAsJsonAsync("/api/skill-categories", request);

    // Assert
    response.StatusCode.Should().Be(HttpStatusCode.Created);
    var result = await response.Content.ReadFromJsonAsync<SkillCategoryResponse>();
    result.Name.Should().Be("Programming");
}
```

## 🚀 Performance Considerations

### Query Optimization

1. **Use Projections**: Select only required fields
2. **Implement Paging**: For large result sets
3. **Add Caching**: For frequently accessed data
4. **Use ReadOnly Contexts**: For query-only operations

### Command Optimization

1. **Batch Operations**: Group related commands
2. **Async Processing**: For non-critical operations
3. **Transaction Scoping**: Minimize transaction scope
4. **Validation Caching**: Cache expensive validation rules

## 📚 Additional Resources

- [MediatR Documentation](https://github.com/jbogard/MediatR)
- [CQRS Journey](<https://docs.microsoft.com/en-us/previous-versions/msp-n-p/jj554200(v=pandp.10)>)
- [Clean Architecture with ASP.NET Core](https://docs.microsoft.com/en-us/dotnet/architecture/modern-web-apps-azure/common-web-application-architectures)
- [FluentValidation Documentation](https://docs.fluentvalidation.net/)

## 🎯 Next Steps

1. **Error Handling**: Implement global error handling for commands/queries
2. **Caching**: Add caching behavior for frequently accessed queries
3. **Audit Logging**: Track command execution for auditing
4. **Performance Monitoring**: Add metrics for handler execution times
5. **Authentication**: Integrate with authentication/authorization behaviors
