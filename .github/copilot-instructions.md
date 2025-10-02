# SkillSwap Repository Instructions for GitHub Copilot

## 🎯 Project Overview

**SkillSwap** is a peer-to-peer skill exchange platform built with .NET 8 and PostgreSQL. Users can offer skills they have and request skills they want to learn, facilitating mutual learning through structured skill swap sessions.

### Core Domain
- **Skill Management**: Categories, skills, user skill profiles with proficiency levels
- **User Management**: Authentication, profiles, preferences, availability
- **Skill Matching**: Finding compatible skill exchange partners
- **Session Management**: Scheduling, conducting, and tracking skill swap sessions
- **Role-Based Access**: User roles and permissions system

## 🏗️ Architecture & Technology Stack

### Technology Stack
- **Backend**: .NET 8 Web API
- **Database**: PostgreSQL with Entity Framework Core 9.0.9
- **Architecture**: Clean Architecture (Domain, Application, Infrastructure, API layers)
- **Patterns**: Repository Pattern, Unit of Work, CQRS (planned)

### Project Structure
```
services/api/src/
├── SkillSwap.Api/           # Web API layer, controllers, middleware
├── SkillSwap.Application/   # Business logic, application services
├── SkillSwap.Contracts/     # DTOs, API contracts
├── SkillSwap.Domain/        # Domain entities, value objects, enums
└── SkillSwap.Infrastructure/ # Data access, external services
```

### Key Domain Entities
- **User**: Core user entity with skills, preferences, and availability
- **Skill**: Skills organized by categories with descriptions
- **SkillSwapRequest**: Requests to exchange skills between users
- **SkillSwapSession**: Actual skill exchange sessions
- **Role/Permission**: RBAC system for authorization

## 🎯 Code Review Focus Areas

### 1. Domain-Driven Design Compliance
- Ensure entities are rich domain models, not anemic data containers
- Validate proper use of value objects for domain concepts (Email, etc.)
- Check for domain logic leakage into infrastructure or API layers
- Verify entities follow invariants and business rules

### 2. Clean Architecture Principles
- **API Layer**: Only controllers, middleware, DTOs, dependency registration
- **Application Layer**: Business logic, application services, validation
- **Domain Layer**: No dependencies on outer layers, pure business logic
- **Infrastructure Layer**: Data access, external services, implementations

### 3. Entity Framework Best Practices
- Proper entity configurations using Fluent API
- Efficient query patterns, avoid N+1 problems
- Proper use of async/await for database operations
- Migration scripts are reversible and safe

### 4. Security & Data Protection
- All endpoints require proper authentication/authorization
- Sensitive data (passwords, personal info) is properly protected
- SQL injection prevention through parameterized queries
- Input validation and sanitization

### 5. Performance Considerations
- Database queries are optimized with proper indexing
- Use of pagination for large data sets
- Efficient JSON serialization
- Proper caching strategies where applicable

## 📋 Code Standards & Conventions

### Naming Conventions
- **Classes**: PascalCase (UserService, SkillRepository)
- **Methods**: PascalCase (GetUserSkillsAsync, CreateSkillAsync)
- **Properties**: PascalCase (UserId, SkillName)
- **Fields**: camelCase with underscore prefix (_dbContext, _logger)
- **Parameters**: camelCase (userId, skillId)
- **Constants**: PascalCase (DefaultPageSize, MaxSkillsPerUser)

### File Organization
- One class per file, file name matches class name
- Interfaces prefixed with 'I' (IUserRepository, ISkillService)
- Group related files in appropriate folders
- Use meaningful namespace hierarchies

### Async/Await Guidelines
- All I/O operations must be async
- Use `ConfigureAwait(false)` in library code
- Suffix async methods with "Async"
- Avoid async void except for event handlers

## 🔍 Code Review Checklist

### Required Reviews
- [ ] **Architecture Compliance**: Code is in the correct layer
- [ ] **Domain Logic**: Business rules are in domain entities
- [ ] **Error Handling**: Proper exception handling and logging
- [ ] **Security**: Authentication, authorization, input validation
- [ ] **Performance**: Efficient database queries and operations
- [ ] **Testing**: Unit tests for business logic
- [ ] **Documentation**: XML comments for public APIs

### Database Changes
- [ ] **Migrations**: Proper EF migrations for schema changes
- [ ] **Seed Data**: Updated seed data if needed
- [ ] **Indexes**: Performance indexes for new queries
- [ ] **Constraints**: Proper foreign keys and constraints

### API Changes
- [ ] **Versioning**: Backward compatibility considerations
- [ ] **Documentation**: Swagger/OpenAPI documentation
- [ ] **Error Responses**: Consistent error response format
- [ ] **Status Codes**: Appropriate HTTP status codes

## 🚨 Critical Review Points

### Security Red Flags
- Direct SQL queries without parameters
- Missing authentication on protected endpoints
- Sensitive data in logs or responses
- Missing input validation
- Hardcoded secrets or credentials

### Performance Red Flags
- N+1 query patterns in Entity Framework
- Missing async/await on I/O operations
- Large data sets without pagination
- Missing database indexes for frequently queried columns
- Inefficient LINQ queries

### Architecture Red Flags
- Domain logic in controllers or infrastructure
- Circular dependencies between layers
- Infrastructure references in domain layer
- Business logic in DTOs or data models

## 🎯 Skill Exchange Domain Rules

### Business Rules to Enforce
1. **Skill Proficiency**: Users can only teach skills they have marked as "Can Teach"
2. **Mutual Exchange**: Skill swaps should be bidirectional when possible
3. **Session Limits**: Users have limits on concurrent active sessions
4. **Availability Matching**: Sessions must respect user availability windows
5. **Rating System**: Both parties must rate sessions for platform health

### Data Integrity Rules
1. **User Skills**: Users cannot have duplicate skills with same proficiency
2. **Session Scheduling**: No overlapping sessions for the same user
3. **Swap Requests**: Cannot request swaps with yourself
4. **Skill Categories**: Skills must belong to valid, active categories

## 📝 Commit Message Standards
- Use conventional commit format: `type(scope): description`
- Types: feat, fix, docs, style, refactor, test, chore
- Include issue numbers when applicable
- Keep first line under 50 characters

## 🔧 Development Guidelines

### Before Submitting PR
1. Run `dotnet build` to ensure compilation
2. Run `dotnet test` for all unit tests
3. Update database with `dotnet ef database update`
4. Test API endpoints manually or with integration tests
5. Update documentation if API changes

### Code Quality Standards
- Maintain minimum 80% code coverage for business logic
- Follow SOLID principles
- Use dependency injection consistently
- Implement proper logging with structured data
- Handle edge cases and error conditions

## 🎯 Review Priorities

### High Priority (Must Fix)
- Security vulnerabilities
- Architecture violations
- Breaking changes to public APIs
- Performance issues with database queries
- Missing error handling

### Medium Priority (Should Fix)
- Code style inconsistencies
- Missing unit tests
- Incomplete documentation
- Minor performance improvements
- Refactoring opportunities

### Low Priority (Consider)
- Code formatting
- Variable naming improvements
- Additional comments
- Optional optimizations

## 📚 Additional Context

### Key Documentation Files
- `/docs/database-schema.md` - Complete database schema and ERD
- `/docs/tools-and-architecture.md` - Architecture patterns and guidelines
- `/init-scripts/01-init.sql` - Database initialization script

### External Dependencies
- Entity Framework Core for ORM
- Npgsql for PostgreSQL connectivity
- Swagger for API documentation
- Docker for local development environment

---

**Note**: This project is in active development. Focus reviews on core functionality, security, and maintainable architecture. Suggest improvements that align with the clean architecture principles and domain-driven design patterns already established.