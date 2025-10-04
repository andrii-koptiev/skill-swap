# CQRS & MediatR Quick Reference

## 🚀 Quick Start Checklist

### Creating a New Command

1. **Create Command Record**

   ```csharp
   public sealed record CreateXCommand(params...) : ICommand<XResponse>;
   ```

2. **Create Command Handler**

   ```csharp
   public sealed class CreateXCommandHandler : ICommandHandler<CreateXCommand, XResponse>
   ```

3. **Create Validator**

   ```csharp
   public sealed class CreateXCommandValidator : AbstractValidator<CreateXCommand>
   ```

4. **Add Controller Action**
   ```csharp
   var result = await _mediator.Send(command, cancellationToken);
   ```

### Creating a New Query

1. **Create Query Record**

   ```csharp
   public sealed record GetXQuery(params...) : IQuery<XResponse>;
   ```

2. **Create Query Handler**

   ```csharp
   public sealed class GetXQueryHandler : IQueryHandler<GetXQuery, XResponse>
   ```

3. **Add Controller Action**
   ```csharp
   var result = await _mediator.Send(query, cancellationToken);
   ```

## 📝 Code Templates

### Command Template

```csharp
// Command
public sealed record Create{Entity}Command(
    string Property1,
    string Property2
) : ICommand<{Entity}Response>;

// Handler
public sealed class Create{Entity}CommandHandler
    : ICommandHandler<Create{Entity}Command, {Entity}Response>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<Create{Entity}CommandHandler> _logger;

    public Create{Entity}CommandHandler(
        IUnitOfWork unitOfWork,
        ILogger<Create{Entity}CommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<{Entity}Response> Handle(
        Create{Entity}Command request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Creating {Entity} with name: {Name}", request.Property1);

        var entity = new {Entity}
        {
            Property1 = request.Property1,
            Property2 = request.Property2
        };

        await _unitOfWork.{Entities}.AddAsync(entity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("{Entity} created successfully with ID: {Id}", entity.Id);

        return new {Entity}Response(
            entity.Id,
            entity.Property1,
            entity.Property2
        );
    }
}

// Validator
public sealed class Create{Entity}CommandValidator : AbstractValidator<Create{Entity}Command>
{
    public Create{Entity}CommandValidator()
    {
        RuleFor(x => x.Property1)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.Property2)
            .NotEmpty()
            .MaximumLength(500);
    }
}
```

### Query Template

```csharp
// Query
public sealed record Get{Entity}ByIdQuery(Guid Id) : IQuery<{Entity}Response>;

// Handler
public sealed class Get{Entity}ByIdQueryHandler
    : IQueryHandler<Get{Entity}ByIdQuery, {Entity}Response>
{
    private readonly IUnitOfWork _unitOfWork;

    public Get{Entity}ByIdQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<{Entity}Response> Handle(
        Get{Entity}ByIdQuery request,
        CancellationToken cancellationToken)
    {
        var entity = await _unitOfWork.{Entities}.GetByIdAsync(request.Id, cancellationToken);

        if (entity == null)
        {
            throw new NotFoundException($"{Entity} with ID {request.Id} was not found");
        }

        return new {Entity}Response(
            entity.Id,
            entity.Property1,
            entity.Property2
        );
    }
}
```

## 🎯 Common Patterns

### Void Commands (No Return Value)

```csharp
public sealed record Delete{Entity}Command(Guid Id) : ICommand;

public sealed class Delete{Entity}CommandHandler : ICommandHandler<Delete{Entity}Command>
{
    public async Task Handle(Delete{Entity}Command request, CancellationToken cancellationToken)
    {
        // Implementation
    }
}
```

### Paginated Queries

```csharp
public sealed record GetPaginated{Entities}Query(
    int PageNumber = 1,
    int PageSize = 10
) : IQuery<PagedResult<{Entity}Response>>;
```

### List Queries

```csharp
public sealed record GetAll{Entities}Query : IQuery<IEnumerable<{Entity}Response>>;
```

## 🛠️ Folder Structure Commands

```bash
# Create new feature structure
mkdir -p Features/{EntityName}/Commands/Create{EntityName}
mkdir -p Features/{EntityName}/Commands/Update{EntityName}
mkdir -p Features/{EntityName}/Commands/Delete{EntityName}
mkdir -p Features/{EntityName}/Queries/Get{EntityName}ById
mkdir -p Features/{EntityName}/Queries/GetAll{EntityName}s
```

## 🧪 Testing Templates

### Handler Unit Test

```csharp
[TestFixture]
public class Create{Entity}CommandHandlerTests
{
    private Mock<IUnitOfWork> _unitOfWorkMock;
    private Mock<ILogger<Create{Entity}CommandHandler>> _loggerMock;
    private Create{Entity}CommandHandler _handler;

    [SetUp]
    public void Setup()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _loggerMock = new Mock<ILogger<Create{Entity}CommandHandler>>();
        _handler = new Create{Entity}CommandHandler(_unitOfWorkMock.Object, _loggerMock.Object);
    }

    [Test]
    public async Task Handle_ValidCommand_Returns{Entity}Response()
    {
        // Arrange
        var command = new Create{Entity}Command("Test Name", "Test Description");

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.That(result.Property1, Is.EqualTo("Test Name"));
        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}
```

## 📋 Common Mistakes to Avoid

❌ **Don't do this:**

```csharp
// Calling repository directly in controller
var entity = await _repository.GetByIdAsync(id);

// Complex logic in command/query classes
public record CreateCommand(string Name) : ICommand
{
    public bool IsValid() => !string.IsNullOrEmpty(Name); // ❌
}

// Not using cancellation tokens
public async Task<Response> Handle(Command request) // ❌

// Returning entities directly
return entity; // ❌
```

✅ **Do this instead:**

```csharp
// Use MediatR in controllers
var result = await _mediator.Send(command, cancellationToken);

// Keep commands simple
public sealed record CreateCommand(string Name) : ICommand<Response>;

// Always use cancellation tokens
public async Task<Response> Handle(Command request, CancellationToken cancellationToken)

// Return DTOs/Response objects
return new EntityResponse(entity.Id, entity.Name);
```

## 🔧 Useful VS Code Snippets

Save these in `.vscode/snippets.json`:

```json
{
  "CQRS Command": {
    "prefix": "cqrs-command",
    "body": [
      "public sealed record ${1:Create}${2:Entity}Command(",
      "    ${3:string Property1}",
      ") : ICommand<${2:Entity}Response>;"
    ]
  },
  "CQRS Query": {
    "prefix": "cqrs-query",
    "body": [
      "public sealed record ${1:Get}${2:Entity}Query(",
      "    ${3:Guid Id}",
      ") : IQuery<${2:Entity}Response>;"
    ]
  },
  "CQRS Handler": {
    "prefix": "cqrs-handler",
    "body": [
      "public sealed class ${1:Command}Handler : I${2:Command}Handler<${1:Command}, ${3:Response}>",
      "{",
      "    private readonly IUnitOfWork _unitOfWork;",
      "    private readonly ILogger<${1:Command}Handler> _logger;",
      "",
      "    public ${1:Command}Handler(",
      "        IUnitOfWork unitOfWork,",
      "        ILogger<${1:Command}Handler> logger)",
      "    {",
      "        _unitOfWork = unitOfWork;",
      "        _logger = logger;",
      "    }",
      "",
      "    public async Task<${3:Response}> Handle(",
      "        ${1:Command} request,",
      "        CancellationToken cancellationToken)",
      "    {",
      "        ${4:// Implementation}",
      "    }",
      "}"
    ]
  }
}
```
