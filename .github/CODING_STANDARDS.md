# SkillSwap Coding Standards & Best Practices

## 📖 Overview

This document defines the coding standards, conventions, and best practices for the SkillSwap project. These guidelines ensure consistency, maintainability, and quality across the codebase while supporting effective GitHub Copilot code reviews.

---

## 🏗️ Architecture Standards

### Clean Architecture Principles

#### Layer Responsibilities

**Domain Layer** (`SkillSwap.Domain`)

```csharp
// ✅ GOOD: Rich domain entity with business logic
public class User : BaseEntity
{
    private readonly List<UserSkill> _skills = new();

    public IReadOnlyCollection<UserSkill> Skills => _skills.AsReadOnly();

    public void AddSkill(Skill skill, SkillType skillType, ProficiencyLevel proficiency)
    {
        // Business rule validation
        if (_skills.Any(s => s.SkillId == skill.Id && s.SkillType == skillType))
            throw new DomainException("User already has this skill with the same type");

        var userSkill = new UserSkill(Id, skill.Id, skillType, proficiency);
        _skills.Add(userSkill);
    }
}

// ❌ BAD: Anemic domain model
public class User : BaseEntity
{
    public List<UserSkill> Skills { get; set; } = new();
    // No business logic - just data container
}
```

**Application Layer** (`SkillSwap.Application`)

```csharp
// ✅ GOOD: Application service orchestrating domain logic
public class UserSkillService : IUserSkillService
{
    private readonly IUserRepository _userRepository;
    private readonly ISkillRepository _skillRepository;
    private readonly IUnitOfWork _unitOfWork;

    public async Task<Result> AddUserSkillAsync(Guid userId, AddUserSkillRequest request)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null) return Result.NotFound("User not found");

        var skill = await _skillRepository.GetByIdAsync(request.SkillId);
        if (skill == null) return Result.NotFound("Skill not found");

        // Domain logic in domain entity
        user.AddSkill(skill, request.SkillType, request.ProficiencyLevel);

        await _unitOfWork.SaveChangesAsync();
        return Result.Success();
    }
}
```

#### Dependency Rules

- **Domain**: No dependencies on other layers
- **Application**: Can depend on Domain only
- **Infrastructure**: Can depend on Domain and Application
- **API**: Can depend on all layers for DI registration only

---

## 🎯 Naming Conventions

### C# Naming Standards

#### Classes and Interfaces

```csharp
// ✅ GOOD
public class UserSkillService { }
public interface IUserRepository { }
public class SkillSwapDbContext { }

// ❌ BAD
public class userSkillService { }
public interface UserRepository { }  // Missing 'I' prefix
public class skillSwapContext { }
```

#### Methods and Properties

```csharp
// ✅ GOOD
public async Task<User> GetUserByIdAsync(Guid userId) { }
public string FirstName { get; set; }
public bool IsActive { get; private set; }

// ❌ BAD
public async Task<User> getUserById(Guid user_id) { }
public string firstName { get; set; }
public bool isActive { get; private set; }
```

#### Fields and Parameters

```csharp
// ✅ GOOD
private readonly IUserRepository _userRepository;
private readonly ILogger<UserService> _logger;

public async Task ProcessUser(Guid userId, CancellationToken cancellationToken)
{
    var existingUser = await _userRepository.GetByIdAsync(userId);
}

// ❌ BAD
private readonly IUserRepository userRepository;  // Missing underscore
private readonly ILogger<UserService> Logger;    // Should be camelCase

public async Task ProcessUser(Guid UserId, CancellationToken CancellationToken)
{
    var ExistingUser = await userRepository.GetByIdAsync(UserId);
}
```

#### Constants and Enums

```csharp
// ✅ GOOD
public const int MaxSkillsPerUser = 50;
public const string DefaultTimeZone = "UTC";

public enum SkillType
{
    CanTeach,
    WantToLearn,
    Interested
}

// ❌ BAD
public const int MAX_SKILLS_PER_USER = 50;  // Use PascalCase, not UPPER_CASE
public const string default_timezone = "UTC";

public enum skillType  // Should be PascalCase
{
    can_teach,         // Should be PascalCase
    want_to_learn,
    interested
}
```

---

## 🔄 Async/Await Best Practices

### Proper Async Patterns

#### Method Signatures

```csharp
// ✅ GOOD: Async methods with proper naming and return types
public async Task<User> GetUserAsync(Guid id)
public async Task<List<Skill>> GetUserSkillsAsync(Guid userId)
public async Task<Result<User>> CreateUserAsync(CreateUserRequest request)
public async Task UpdateUserAsync(User user)

// ❌ BAD: Missing Async suffix, wrong return types
public async Task<User> GetUser(Guid id)           // Missing "Async"
public Task<List<Skill>> GetUserSkills(Guid userId)  // Missing async keyword
public async void UpdateUser(User user)              // Should return Task
```

#### ConfigureAwait Usage

```csharp
// ✅ GOOD: Use ConfigureAwait(false) in library code
public async Task<User> GetUserAsync(Guid id)
{
    var user = await _repository.GetByIdAsync(id).ConfigureAwait(false);
    return user;
}

// ✅ GOOD: Don't use ConfigureAwait in API controllers (top-level)
[HttpGet("{id}")]
public async Task<ActionResult<UserDto>> GetUser(Guid id)
{
    var user = await _userService.GetUserAsync(id);  // No ConfigureAwait needed
    return Ok(user);
}
```

#### Async Enumerable

```csharp
// ✅ GOOD: Use IAsyncEnumerable for streaming data
public async IAsyncEnumerable<User> GetUsersAsync(
    [EnumeratorCancellation] CancellationToken cancellationToken = default)
{
    await foreach (var user in _repository.GetUsersStreamAsync(cancellationToken))
    {
        yield return user;
    }
}
```

---

## 🗄️ Entity Framework Best Practices

### Entity Configuration

```csharp
// ✅ GOOD: Use Fluent API for entity configuration
public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("users");

        builder.HasKey(u => u.Id);

        builder.Property(u => u.Email)
            .IsRequired()
            .HasMaxLength(255)
            .HasConversion(
                email => email.Value,
                value => Email.Create(value).Value);

        builder.HasIndex(u => u.Email)
            .IsUnique()
            .HasDatabaseName("IX_users_email");

        builder.HasMany(u => u.Skills)
            .WithOne()
            .HasForeignKey(us => us.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

// ❌ BAD: Using data annotations instead of Fluent API
public class User : BaseEntity
{
    [Required]
    [MaxLength(255)]
    public string Email { get; set; }  // Should use value object
}
```

### Query Patterns

```csharp
// ✅ GOOD: Efficient queries with proper includes and filtering
public async Task<User> GetUserWithSkillsAsync(Guid userId)
{
    return await _context.Users
        .Include(u => u.Skills)
            .ThenInclude(us => us.Skill)
                .ThenInclude(s => s.Category)
        .FirstOrDefaultAsync(u => u.Id == userId);
}

// ✅ GOOD: Projection for read-only scenarios
public async Task<UserSummaryDto> GetUserSummaryAsync(Guid userId)
{
    return await _context.Users
        .Where(u => u.Id == userId)
        .Select(u => new UserSummaryDto
        {
            Id = u.Id,
            FullName = u.FirstName + " " + u.LastName,
            SkillCount = u.Skills.Count
        })
        .FirstOrDefaultAsync();
}

// ❌ BAD: N+1 query problem
public async Task<List<UserDto>> GetAllUsersWithSkills()
{
    var users = await _context.Users.ToListAsync();
    foreach (var user in users)
    {
        // This creates N+1 queries!
        user.Skills = await _context.UserSkills
            .Where(us => us.UserId == user.Id)
            .ToListAsync();
    }
    return users.Select(u => new UserDto(u)).ToList();
}
```

### Repository Pattern

```csharp
// ✅ GOOD: Repository with proper abstractions
public interface IUserRepository : IRepository<User>
{
    Task<User> GetWithSkillsAsync(Guid id);
    Task<List<User>> GetBySkillAsync(Guid skillId);
    Task<bool> ExistsByEmailAsync(string email);
}

public class UserRepository : Repository<User>, IUserRepository
{
    public UserRepository(SkillSwapDbContext context) : base(context) { }

    public async Task<User> GetWithSkillsAsync(Guid id)
    {
        return await _context.Users
            .Include(u => u.Skills)
            .FirstOrDefaultAsync(u => u.Id == id);
    }
}
```

---

## 🛡️ Security Standards

### Input Validation

```csharp
// ✅ GOOD: Proper input validation
public class CreateUserRequest
{
    [Required]
    [EmailAddress]
    [MaxLength(255)]
    public string Email { get; set; }

    [Required]
    [StringLength(50, MinimumLength = 2)]
    public string FirstName { get; set; }

    [Required]
    [StringLength(50, MinimumLength = 2)]
    public string LastName { get; set; }
}

// ✅ GOOD: Additional business validation
public async Task<Result<User>> CreateUserAsync(CreateUserRequest request)
{
    // Validate email uniqueness
    if (await _userRepository.ExistsByEmailAsync(request.Email))
        return Result.Error("Email already exists");

    // Create value object with validation
    var email = Email.Create(request.Email);
    if (email.IsFailure)
        return Result.Error(email.Error);

    // Domain object creation
    var user = User.Create(email.Value, request.FirstName, request.LastName);
    // ... rest of creation logic
}
```

### Authentication & Authorization

```csharp
// ✅ GOOD: Proper authorization attributes
[Authorize]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    [HttpGet("{id}")]
    [Authorize(Policy = "CanViewUserProfile")]
    public async Task<ActionResult<UserDto>> GetUser(Guid id)
    {
        // Implementation
    }

    [HttpPost]
    [Authorize(Roles = "Admin,UserManager")]
    public async Task<ActionResult<UserDto>> CreateUser(CreateUserRequest request)
    {
        // Implementation
    }
}
```

### Sensitive Data Handling

```csharp
// ✅ GOOD: Proper password handling
public class User : BaseEntity
{
    public string PasswordHash { get; private set; }

    public void SetPassword(string plainPassword)
    {
        if (string.IsNullOrWhiteSpace(plainPassword))
            throw new ArgumentException("Password cannot be empty");

        // Use proper hashing (BCrypt, Argon2, etc.)
        PasswordHash = BCrypt.Net.BCrypt.HashPassword(plainPassword);
    }

    public bool VerifyPassword(string plainPassword)
    {
        return BCrypt.Net.BCrypt.Verify(plainPassword, PasswordHash);
    }
}

// ❌ BAD: Storing plain text passwords
public class User : BaseEntity
{
    public string Password { get; set; }  // Never store plain text!
}
```

---

## 🔬 Testing Standards

### Unit Testing

```csharp
// ✅ GOOD: Well-structured unit test
[Test]
public async Task CreateUserAsync_WithValidData_ShouldCreateUser()
{
    // Arrange
    var email = Email.Create("test@example.com").Value;
    var createRequest = new CreateUserRequest
    {
        Email = email.Value,
        FirstName = "John",
        LastName = "Doe"
    };

    _userRepository.Setup(r => r.ExistsByEmailAsync(email.Value))
        .ReturnsAsync(false);

    // Act
    var result = await _userService.CreateUserAsync(createRequest);

    // Assert
    result.IsSuccess.Should().BeTrue();
    result.Value.Email.Should().Be(email);
    _userRepository.Verify(r => r.AddAsync(It.IsAny<User>()), Times.Once);
}

// ✅ GOOD: Testing business rules
[Test]
public void User_AddSkill_WithDuplicateSkill_ShouldThrowException()
{
    // Arrange
    var user = User.Create(Email.Create("test@example.com").Value, "John", "Doe").Value;
    var skill = new Skill("JavaScript", "Programming language", Guid.NewGuid());

    user.AddSkill(skill, SkillType.CanTeach, ProficiencyLevel.Intermediate);

    // Act & Assert
    var exception = Assert.Throws<DomainException>(
        () => user.AddSkill(skill, SkillType.CanTeach, ProficiencyLevel.Advanced));

    exception.Message.Should().Contain("already has this skill");
}
```

### Integration Testing

```csharp
// ✅ GOOD: Integration test for API endpoint
[Test]
public async Task GetUser_WithValidId_ShouldReturnUser()
{
    // Arrange
    var user = await CreateTestUserAsync();

    // Act
    var response = await _client.GetAsync($"/api/users/{user.Id}");

    // Assert
    response.StatusCode.Should().Be(HttpStatusCode.OK);
    var userDto = await response.Content.ReadFromJsonAsync<UserDto>();
    userDto.Email.Should().Be(user.Email.Value);
}
```

---

## 📝 Documentation Standards

### XML Documentation

```csharp
/// <summary>
/// Creates a new user with the specified email and personal information.
/// </summary>
/// <param name="request">The user creation request containing email and personal details.</param>
/// <param name="cancellationToken">Token to cancel the operation.</param>
/// <returns>
/// A <see cref="Result{User}"/> containing the created user if successful,
/// or an error result if the operation fails.
/// </returns>
/// <exception cref="ArgumentNullException">Thrown when <paramref name="request"/> is null.</exception>
/// <exception cref="ValidationException">Thrown when the request contains invalid data.</exception>
public async Task<Result<User>> CreateUserAsync(
    CreateUserRequest request,
    CancellationToken cancellationToken = default)
{
    // Implementation
}
```

### Code Comments

```csharp
// ✅ GOOD: Comments explaining business logic
public class SkillMatchingService
{
    public async Task<List<User>> FindMatchingUsersAsync(Guid userId, Guid skillId)
    {
        // Business rule: Find users who can teach the skill that this user wants to learn,
        // and who want to learn a skill that this user can teach (mutual exchange)
        var user = await _userRepository.GetWithSkillsAsync(userId);
        var teachableSkills = user.Skills
            .Where(s => s.SkillType == SkillType.CanTeach)
            .Select(s => s.SkillId);

        return await _userRepository.FindUsersForSkillExchangeAsync(
            wantedSkillId: skillId,
            teachableSkillIds: teachableSkills);
    }
}

// ❌ BAD: Obvious comments
public async Task<User> GetUserAsync(Guid id)
{
    // Get user by id
    return await _userRepository.GetByIdAsync(id);
}
```

---

## 🚨 Error Handling Standards

### Exception Handling

```csharp
// ✅ GOOD: Proper exception handling with specific types
public class UserService : IUserService
{
    public async Task<Result<User>> GetUserAsync(Guid id)
    {
        try
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null)
                return Result.NotFound($"User with ID {id} not found");

            return Result.Success(user);
        }
        catch (DbException ex)
        {
            _logger.LogError(ex, "Database error while retrieving user {UserId}", id);
            return Result.Error("An error occurred while retrieving the user");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error while retrieving user {UserId}", id);
            return Result.Error("An unexpected error occurred");
        }
    }
}

// ✅ GOOD: Domain exceptions for business rule violations
public class DomainException : Exception
{
    public DomainException(string message) : base(message) { }
    public DomainException(string message, Exception innerException) : base(message, innerException) { }
}
```

### Result Pattern

```csharp
// ✅ GOOD: Using Result pattern instead of exceptions for business logic
public class Result<T>
{
    public bool IsSuccess { get; private set; }
    public T Value { get; private set; }
    public string Error { get; private set; }

    public static Result<T> Success(T value) => new() { IsSuccess = true, Value = value };
    public static Result<T> Failure(string error) => new() { IsSuccess = false, Error = error };
}
```

---

## 📊 Performance Guidelines

### Memory Management

```csharp
// ✅ GOOD: Proper disposal and memory management
public class FileService : IFileService, IDisposable
{
    private readonly HttpClient _httpClient;
    private bool _disposed = false;

    public async Task<byte[]> DownloadFileAsync(string url)
    {
        using var response = await _httpClient.GetAsync(url);
        return await response.Content.ReadAsByteArrayAsync();
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed && disposing)
        {
            _httpClient?.Dispose();
            _disposed = true;
        }
    }
}
```

### Efficient Collections

```csharp
// ✅ GOOD: Use appropriate collection types
public class SkillCacheService
{
    private readonly ConcurrentDictionary<Guid, Skill> _skillCache = new();
    private readonly HashSet<Guid> _activeSkillIds = new();

    public void CacheSkill(Skill skill)
    {
        _skillCache.TryAdd(skill.Id, skill);
        if (skill.IsActive)
            _activeSkillIds.Add(skill.Id);
    }
}

// ❌ BAD: Inefficient operations
public List<User> FilterActiveUsers(List<User> allUsers)
{
    var activeUsers = new List<User>();
    foreach (var user in allUsers)  // Use LINQ Where instead
    {
        if (user.IsActive)
            activeUsers.Add(user);
    }
    return activeUsers;
}
```

---

## 🎯 Code Review Checklist

### For GitHub Copilot Reviews

When reviewing code, ensure:

1. **Architecture Compliance**

   - [ ] Code is in the correct layer
   - [ ] Dependencies flow inward
   - [ ] Domain logic is in domain entities

2. **Security**

   - [ ] Input validation present
   - [ ] Authentication/authorization implemented
   - [ ] No hardcoded secrets

3. **Performance**

   - [ ] Async/await used properly
   - [ ] Database queries optimized
   - [ ] Proper memory management

4. **Maintainability**

   - [ ] Clear naming conventions
   - [ ] Adequate documentation
   - [ ] SOLID principles followed

5. **Testing**
   - [ ] Unit tests for business logic
   - [ ] Integration tests for APIs
   - [ ] Edge cases covered

---

## 📚 Additional Resources

- [Microsoft C# Coding Conventions](https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/inside-a-program/coding-conventions)
- [Clean Architecture by Robert Martin](https://blog.cleancoder.com/uncle-bob/2012/08/13/the-clean-architecture.html)
- [Entity Framework Core Best Practices](https://docs.microsoft.com/en-us/ef/core/miscellaneous/configuring-dbcontext)
- [ASP.NET Core Security Best Practices](https://docs.microsoft.com/en-us/aspnet/core/security/)

---

**Note**: These standards are continuously evolving. Update this document when new patterns or practices are adopted in the project.
