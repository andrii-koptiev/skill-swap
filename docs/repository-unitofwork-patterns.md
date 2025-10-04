# Repository & Unit of Work Patterns - SkillSwap API

## 📋 Overview

This document provides comprehensive guidance on the Repository and Unit of Work patterns implemented in the SkillSwap API. These patterns provide a clean abstraction layer over Entity Framework Core, enabling testable, maintainable, and transaction-safe data access.

## 🏗️ Architecture Overview

```
┌─────────────────────────────────────────────────────────────┐
│                    API Layer (Controllers)                  │
│  ┌─────────────────────────────────────────────────────────┐ │
│  │              Unit of Work                               │ │
│  │  ┌──────────────┐  ┌──────────────┐  ┌──────────────┐  │ │
│  │  │ User Repo    │  │ Skill Repo   │  │ Category Repo│  │ │
│  │  └──────────────┘  └──────────────┘  └──────────────┘  │ │
│  │  ┌──────────────────────────────────────────────────────┐ │ │
│  │  │          Generic Repository<T>                       │ │ │
│  │  └──────────────────────────────────────────────────────┘ │ │
│  └─────────────────────────────────────────────────────────┘ │
└─────────────────────────────────────────────────────────────┘
┌─────────────────────────────────────────────────────────────┐
│              Infrastructure Layer                           │
│  ┌─────────────────────────────────────────────────────────┐ │
│  │              Entity Framework Core                       │ │
│  │               SkillSwapDbContext                         │ │
│  └─────────────────────────────────────────────────────────┘ │
└─────────────────────────────────────────────────────────────┘
```

## 🔧 Core Interfaces

### Generic Repository Interface

```csharp
public interface IRepository<T> where T : BaseEntity
{
    // Read Operations
    Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);
    Task<(IEnumerable<T> Items, int TotalCount)> GetPagedAsync(int page, int pageSize, CancellationToken cancellationToken = default);

    // Write Operations
    Task AddAsync(T entity, CancellationToken cancellationToken = default);
    Task AddRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default);
    void Update(T entity);
    void UpdateRange(IEnumerable<T> entities);
    void Delete(T entity);
    void DeleteRange(IEnumerable<T> entities);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);

    // Utility Operations
    Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default);
    Task<int> CountAsync(CancellationToken cancellationToken = default);
    Task<int> CountAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);
}
```

### Unit of Work Interface

```csharp
public interface IUnitOfWork : IDisposable
{
    // Repository Access
    IUserRepository Users { get; }
    ISkillRepository Skills { get; }
    ISkillCategoryRepository SkillCategories { get; }
    IUserSkillRepository UserSkills { get; }
    IUserAvailabilityRepository UserAvailabilities { get; }
    IUserPreferencesRepository UserPreferences { get; }
    IRoleRepository Roles { get; }
    IUserRoleRepository UserRoles { get; }
    IRolePermissionRepository RolePermissions { get; }

    // Generic Repository Access
    IRepository<T> Repository<T>() where T : BaseEntity;

    // Transaction Management
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    Task<IUnitOfWorkTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default);
}
```

## 🎯 Usage Patterns

### Basic CRUD Operations

```csharp
[ApiController]
[Route("api/[controller]")]
public class SkillCategoriesController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;

    public SkillCategoriesController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<SkillCategoryResponse>>> GetAllAsync()
    {
        var categories = await _unitOfWork.SkillCategories.GetAllAsync();
        return Ok(categories.Select(c => new SkillCategoryResponse
        {
            Id = c.Id,
            Name = c.Name,
            Description = c.Description
        }));
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<SkillCategoryResponse>> GetByIdAsync(Guid id)
    {
        var category = await _unitOfWork.SkillCategories.GetByIdAsync(id);
        if (category == null)
            return NotFound();

        return Ok(new SkillCategoryResponse
        {
            Id = category.Id,
            Name = category.Name,
            Description = category.Description
        });
    }

    [HttpPost]
    public async Task<ActionResult<SkillCategoryResponse>> CreateAsync(CreateSkillCategoryRequest request)
    {
        var category = new SkillCategory
        {
            Name = request.Name,
            Description = request.Description
        };

        await _unitOfWork.SkillCategories.AddAsync(category);
        await _unitOfWork.SaveChangesAsync();

        return CreatedAtAction(nameof(GetByIdAsync), new { id = category.Id },
            new SkillCategoryResponse
            {
                Id = category.Id,
                Name = category.Name,
                Description = category.Description
            });
    }
}
```

### Advanced Repository Operations

```csharp
public class UserService
{
    private readonly IUnitOfWork _unitOfWork;

    public UserService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<UserWithSkillsDto> GetUserWithSkillsAsync(Guid userId)
    {
        // Use specific repository method with includes
        var user = await _unitOfWork.Users.GetByIdWithSkillsAsync(userId);
        if (user == null)
            throw new NotFoundException($"User with ID {userId} not found");

        return new UserWithSkillsDto
        {
            Id = user.Id,
            Email = user.Email,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Skills = user.UserSkills.Select(us => new UserSkillDto
            {
                SkillId = us.SkillId,
                SkillName = us.Skill.Name,
                ProficiencyLevel = us.ProficiencyLevel,
                SkillType = us.SkillType
            }).ToList()
        };
    }

    public async Task<PagedResult<UserDto>> SearchUsersAsync(int page, int pageSize, string? searchTerm = null)
    {
        if (string.IsNullOrEmpty(searchTerm))
        {
            var (users, totalCount) = await _unitOfWork.Users.GetPagedAsync(page, pageSize);
            return new PagedResult<UserDto>
            {
                Items = users.Select(MapToDto).ToList(),
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize
            };
        }

        var searchResults = await _unitOfWork.Users.SearchByNameOrEmailAsync(searchTerm, page, pageSize);
        return new PagedResult<UserDto>
        {
            Items = searchResults.Items.Select(MapToDto).ToList(),
            TotalCount = searchResults.TotalCount,
            Page = page,
            PageSize = pageSize
        };
    }
}
```

### Transaction Management

```csharp
public class SkillSwapService
{
    private readonly IUnitOfWork _unitOfWork;

    public SkillSwapService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<SkillSwapSessionDto> CreateSkillSwapSessionAsync(CreateSkillSwapRequest request)
    {
        using var transaction = await _unitOfWork.BeginTransactionAsync();

        try
        {
            // Validate users exist
            var teacher = await _unitOfWork.Users.GetByIdAsync(request.TeacherId);
            var learner = await _unitOfWork.Users.GetByIdAsync(request.LearnerId);

            if (teacher == null || learner == null)
                throw new NotFoundException("One or both users not found");

            // Validate skills
            var skill = await _unitOfWork.Skills.GetByIdAsync(request.SkillId);
            if (skill == null)
                throw new NotFoundException("Skill not found");

            // Create session
            var session = new SkillSwapSession
            {
                TeacherId = request.TeacherId,
                LearnerId = request.LearnerId,
                SkillId = request.SkillId,
                ScheduledDateTime = request.ScheduledDateTime,
                Status = SkillSwapStatus.Scheduled
            };

            await _unitOfWork.SkillSwapSessions.AddAsync(session);

            // Update user statistics (example of coordinated operations)
            teacher.SessionsAsTeacherCount++;
            learner.SessionsAsLearnerCount++;

            _unitOfWork.Users.Update(teacher);
            _unitOfWork.Users.Update(learner);

            await _unitOfWork.SaveChangesAsync();
            await transaction.CommitAsync();

            return MapToDto(session);
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }
}
```

## 📁 Specific Repository Examples

### User Repository

```csharp
public interface IUserRepository : IRepository<User>
{
    Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<User?> GetByIdWithSkillsAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<User?> GetByIdWithRolesAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<(IEnumerable<User> Items, int TotalCount)> SearchByNameOrEmailAsync(string searchTerm, int page, int pageSize, CancellationToken cancellationToken = default);
    Task<IEnumerable<User>> GetUsersWithSkillAsync(Guid skillId, CancellationToken cancellationToken = default);
    Task<bool> EmailExistsAsync(string email, CancellationToken cancellationToken = default);
}

public class UserRepository : Repository<User>, IUserRepository
{
    public UserRepository(SkillSwapDbContext context) : base(context) { }

    public async Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        return await _context.Users
            .FirstOrDefaultAsync(u => u.Email == email, cancellationToken);
    }

    public async Task<User?> GetByIdWithSkillsAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _context.Users
            .Include(u => u.UserSkills)
                .ThenInclude(us => us.Skill)
                    .ThenInclude(s => s.Category)
            .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);
    }

    public async Task<(IEnumerable<User> Items, int TotalCount)> SearchByNameOrEmailAsync(
        string searchTerm, int page, int pageSize, CancellationToken cancellationToken = default)
    {
        var query = _context.Users
            .Where(u => u.FirstName.Contains(searchTerm) ||
                       u.LastName.Contains(searchTerm) ||
                       u.Email.Contains(searchTerm));

        var totalCount = await query.CountAsync(cancellationToken);
        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }

    public async Task<bool> EmailExistsAsync(string email, CancellationToken cancellationToken = default)
    {
        return await _context.Users
            .AnyAsync(u => u.Email == email, cancellationToken);
    }
}
```

## 🔒 Dependency Injection Configuration

```csharp
public static class RepositoryServiceExtensions
{
    public static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        // Register Unit of Work
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        // Register Generic Repository
        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

        // Register Specific Repositories
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<ISkillRepository, SkillRepository>();
        services.AddScoped<ISkillCategoryRepository, SkillCategoryRepository>();
        services.AddScoped<IUserSkillRepository, UserSkillRepository>();
        services.AddScoped<IUserAvailabilityRepository, UserAvailabilityRepository>();
        services.AddScoped<IUserPreferencesRepository, UserPreferencesRepository>();
        services.AddScoped<IRoleRepository, RoleRepository>();
        services.AddScoped<IUserRoleRepository, UserRoleRepository>();
        services.AddScoped<IRolePermissionRepository, RolePermissionRepository>();

        return services;
    }
}

// In Program.cs
builder.Services.AddRepositories();
```

## 🧪 Testing with Repository Pattern

### Unit Testing with Mocks

```csharp
public class UserServiceTests
{
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly Mock<IUserRepository> _mockUserRepository;
    private readonly UserService _userService;

    public UserServiceTests()
    {
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _mockUserRepository = new Mock<IUserRepository>();
        _mockUnitOfWork.Setup(uow => uow.Users).Returns(_mockUserRepository.Object);
        _userService = new UserService(_mockUnitOfWork.Object);
    }

    [Fact]
    public async Task GetUserWithSkillsAsync_UserExists_ReturnsUserWithSkills()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var expectedUser = new User
        {
            Id = userId,
            Email = "test@example.com",
            FirstName = "John",
            LastName = "Doe",
            UserSkills = new List<UserSkill>
            {
                new UserSkill
                {
                    SkillId = Guid.NewGuid(),
                    Skill = new Skill { Name = "C#", Category = new SkillCategory { Name = "Programming" } },
                    ProficiencyLevel = 4,
                    SkillType = SkillType.CanTeach
                }
            }
        };

        _mockUserRepository
            .Setup(repo => repo.GetByIdWithSkillsAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedUser);

        // Act
        var result = await _userService.GetUserWithSkillsAsync(userId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(userId, result.Id);
        Assert.Equal("test@example.com", result.Email);
        Assert.Single(result.Skills);
        Assert.Equal("C#", result.Skills.First().SkillName);
    }

    [Fact]
    public async Task GetUserWithSkillsAsync_UserNotExists_ThrowsNotFoundException()
    {
        // Arrange
        var userId = Guid.NewGuid();
        _mockUserRepository
            .Setup(repo => repo.GetByIdWithSkillsAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() => _userService.GetUserWithSkillsAsync(userId));
    }
}
```

### Integration Testing

```csharp
public class UserRepositoryIntegrationTests : IClassFixture<TestDatabaseFixture>
{
    private readonly TestDatabaseFixture _fixture;

    public UserRepositoryIntegrationTests(TestDatabaseFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task GetByEmailAsync_UserExists_ReturnsUser()
    {
        // Arrange
        using var scope = _fixture.ServiceProvider.CreateScope();
        var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

        var user = new User
        {
            Email = "integration@test.com",
            FirstName = "Integration",
            LastName = "Test",
            PasswordHash = "hashedpassword"
        };

        await unitOfWork.Users.AddAsync(user);
        await unitOfWork.SaveChangesAsync();

        // Act
        var foundUser = await unitOfWork.Users.GetByEmailAsync("integration@test.com");

        // Assert
        Assert.NotNull(foundUser);
        Assert.Equal("integration@test.com", foundUser.Email);
        Assert.Equal("Integration", foundUser.FirstName);
    }
}
```

## 🚀 Performance Considerations

### Efficient Querying

```csharp
// ✅ Good: Use specific methods with explicit includes
var user = await _unitOfWork.Users.GetByIdWithSkillsAsync(userId);

// ❌ Bad: N+1 query problem
var user = await _unitOfWork.Users.GetByIdAsync(userId);
foreach (var userSkill in user.UserSkills) // Each iteration hits database
{
    Console.WriteLine(userSkill.Skill.Name);
}

// ✅ Good: Use pagination for large datasets
var (users, totalCount) = await _unitOfWork.Users.GetPagedAsync(page: 1, pageSize: 20);

// ❌ Bad: Loading all data into memory
var allUsers = await _unitOfWork.Users.GetAllAsync(); // Could be thousands of records
```

### Lazy Loading vs Explicit Loading

```csharp
public class UserRepository : Repository<User>, IUserRepository
{
    // ✅ Explicit loading - predictable performance
    public async Task<User?> GetByIdWithSkillsAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _context.Users
            .Include(u => u.UserSkills)
                .ThenInclude(us => us.Skill)
            .Include(u => u.UserPreferences)
            .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);
    }

    // ✅ Projection for read-only scenarios
    public async Task<IEnumerable<UserSummaryDto>> GetUserSummariesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Users
            .Select(u => new UserSummaryDto
            {
                Id = u.Id,
                FullName = u.FirstName + " " + u.LastName,
                SkillCount = u.UserSkills.Count
            })
            .ToListAsync(cancellationToken);
    }
}
```

## ⚠️ Common Pitfalls & Best Practices

### ❌ Anti-Patterns to Avoid

```csharp
// ❌ Don't dispose Unit of Work in controller - let DI handle it
public class BadController : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult> Get()
    {
        using var unitOfWork = new UnitOfWork(context); // DON'T DO THIS
        return Ok(await unitOfWork.Users.GetAllAsync());
    }
}

// ❌ Don't call SaveChanges multiple times unnecessarily
public async Task BadMethod()
{
    await _unitOfWork.Users.AddAsync(user1);
    await _unitOfWork.SaveChangesAsync(); // Inefficient

    await _unitOfWork.Users.AddAsync(user2);
    await _unitOfWork.SaveChangesAsync(); // Inefficient
}
```

### ✅ Best Practices

```csharp
// ✅ Batch operations when possible
public async Task GoodMethod()
{
    await _unitOfWork.Users.AddRangeAsync(new[] { user1, user2 });
    await _unitOfWork.SaveChangesAsync(); // Single database round trip
}

// ✅ Use transactions for complex operations
public async Task ComplexOperation()
{
    using var transaction = await _unitOfWork.BeginTransactionAsync();
    try
    {
        // Multiple coordinated operations
        await _unitOfWork.Users.AddAsync(user);
        await _unitOfWork.UserSkills.AddRangeAsync(userSkills);
        await _unitOfWork.SaveChangesAsync();

        await transaction.CommitAsync();
    }
    catch
    {
        await transaction.RollbackAsync();
        throw;
    }
}

// ✅ Use CancellationToken for long-running operations
public async Task<User?> GetUserAsync(Guid userId, CancellationToken cancellationToken)
{
    return await _unitOfWork.Users.GetByIdAsync(userId, cancellationToken);
}
```

## 📊 Monitoring & Logging

### Performance Logging

```csharp
public class Repository<T> : IRepository<T> where T : BaseEntity
{
    private readonly SkillSwapDbContext _context;
    private readonly ILogger<Repository<T>> _logger;

    public async Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        using var activity = Activity.StartActivity($"Repository.GetById.{typeof(T).Name}");
        activity?.SetTag("entity.id", id.ToString());

        var stopwatch = Stopwatch.StartNew();

        try
        {
            var entity = await _context.Set<T>().FindAsync(new object[] { id }, cancellationToken);

            stopwatch.Stop();
            _logger.LogDebug("Retrieved {EntityType} with ID {Id} in {ElapsedMs}ms",
                typeof(T).Name, id, stopwatch.ElapsedMilliseconds);

            return entity;
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            _logger.LogError(ex, "Error retrieving {EntityType} with ID {Id} after {ElapsedMs}ms",
                typeof(T).Name, id, stopwatch.ElapsedMilliseconds);
            throw;
        }
    }
}
```

## 🔍 Troubleshooting

### Common Issues

1. **N+1 Query Problems**

   - **Symptom**: Many database queries for related data
   - **Solution**: Use explicit `Include()` statements or projection

2. **Memory Issues with Large Datasets**

   - **Symptom**: High memory usage, slow performance
   - **Solution**: Implement pagination, use `IAsyncEnumerable`, or projections

3. **Transaction Deadlocks**

   - **Symptom**: Database timeouts or deadlock exceptions
   - **Solution**: Keep transactions short, consistent locking order

4. **Stale Data Issues**
   - **Symptom**: Changes not reflected in subsequent queries
   - **Solution**: Ensure `SaveChangesAsync()` is called, check transaction isolation

### Debugging Tips

```csharp
// Enable EF Core logging to see generated SQL
services.AddDbContext<SkillSwapDbContext>(options =>
{
    options.UseNpgsql(connectionString)
           .LogTo(Console.WriteLine, LogLevel.Information)
           .EnableSensitiveDataLogging(); // Only in development!
});
```

## 📚 Additional Resources

- [Entity Framework Core Documentation](https://docs.microsoft.com/en-us/ef/core/)
- [Repository Pattern Guidelines](https://docs.microsoft.com/en-us/dotnet/architecture/microservices/microservice-ddd-cqrs-patterns/infrastructure-persistence-layer-design)
- [Unit of Work Pattern](https://martinfowler.com/eaaCatalog/unitOfWork.html)
- [SkillSwap Architecture Documentation](./tools-and-architecture.md)

---

_This documentation is part of the SkillSwap API project. For questions or improvements, please refer to the project's contribution guidelines._
