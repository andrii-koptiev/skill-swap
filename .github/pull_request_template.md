---
name: Pull Request
about: Submit changes for review with comprehensive AI code analysis
title: "[feat/fix/docs]: Brief description"
labels: ["needs-review"]
---

## 📋 Pull Request Overview

### Summary

<!-- Brief description of what this PR accomplishes -->

### Type of Change

- [ ] 🆕 **Feature**: New functionality
- [ ] 🐛 **Bug Fix**: Fixes an issue
- [ ] 🔨 **Refactor**: Code restructuring without functional changes
- [ ] 📚 **Documentation**: Documentation updates
- [ ] 🎨 **Style**: Code formatting/style changes
- [ ] ⚡ **Performance**: Performance improvements
- [ ] 🧪 **Tests**: Adding or updating tests
- [ ] 🔧 **Chore**: Maintenance tasks

### Related Issues

<!-- Link to related issues: Closes #123, Relates to #456 -->

---

## 🏗️ Architecture & Design

### Clean Architecture Compliance

- [ ] Changes are in the correct architectural layer
- [ ] Domain logic is in Domain entities, not controllers/infrastructure
- [ ] Application layer contains business logic and validation
- [ ] API layer only contains controllers, DTOs, and middleware
- [ ] Infrastructure layer properly implements interfaces

### Domain-Driven Design

- [ ] Entities are rich domain models with business logic
- [ ] Value objects are used for domain concepts
- [ ] Business invariants are enforced in domain entities
- [ ] Domain events are used for cross-aggregate communication (if applicable)

---

## 🔍 Code Quality Checklist

### Security Review

- [ ] **Authentication**: All protected endpoints require proper auth
- [ ] **Authorization**: Role-based access control is implemented
- [ ] **Input Validation**: All user inputs are validated and sanitized
- [ ] **SQL Injection**: Using parameterized queries/EF Core properly
- [ ] **Secrets**: No hardcoded credentials or sensitive data
- [ ] **Data Protection**: Sensitive data is properly encrypted/hashed

### Performance & Database

- [ ] **Async Operations**: All I/O operations are async with proper await
- [ ] **Database Queries**: Optimized queries, no N+1 problems
- [ ] **Pagination**: Large datasets use pagination
- [ ] **Indexing**: New queries have appropriate database indexes
- [ ] **Connection Management**: Proper DbContext usage and disposal

### Entity Framework

- [ ] **Migrations**: Proper EF migrations for schema changes
- [ ] **Fluent API**: Entity configurations use Fluent API, not attributes
- [ ] **Query Efficiency**: LINQ queries are optimized
- [ ] **Seed Data**: Updated seed data for new entities (if needed)

---

## 🧪 Testing & Validation

### Testing Coverage

- [ ] **Unit Tests**: Business logic has unit tests with 80%+ coverage
- [ ] **Integration Tests**: API endpoints have integration tests (if applicable)
- [ ] **Repository Tests**: Data access logic is tested
- [ ] **Edge Cases**: Error conditions and edge cases are tested

### Manual Testing

- [ ] **API Testing**: Endpoints tested manually or with Postman/Swagger
- [ ] **Database**: Database changes tested with real data
- [ ] **Error Scenarios**: Error handling tested with invalid inputs
- [ ] **Cross-browser**: Frontend changes tested across browsers (if applicable)

---

## 📝 Documentation & Standards

### Code Documentation

- [ ] **XML Comments**: Public APIs have XML documentation
- [ ] **README Updates**: Documentation updated if API changes
- [ ] **Swagger**: API documentation is accurate and complete
- [ ] **Code Comments**: Complex business logic is well-commented

### Naming & Conventions

- [ ] **C# Conventions**: Follows C# naming conventions (PascalCase, camelCase)
- [ ] **Async Naming**: Async methods end with "Async"
- [ ] **Interface Naming**: Interfaces prefixed with "I"
- [ ] **File Organization**: One class per file, proper namespaces

---

## 🔄 Database Changes

### Schema Changes

- [ ] **Migration Script**: EF migration created and tested
- [ ] **Backward Compatibility**: Changes don't break existing data
- [ ] **Foreign Keys**: Proper relationships and constraints
- [ ] **Indexes**: Performance indexes for new queries
- [ ] **Rollback Plan**: Migration can be safely reverted

### Data Changes

- [ ] **Seed Data**: Development and production seed data updated
- [ ] **Data Migration**: Existing data is properly migrated
- [ ] **Validation**: Data integrity constraints are enforced

---

## 🚨 Breaking Changes

### API Compatibility

- [ ] **No Breaking Changes**: Existing API contracts unchanged
- [ ] **Versioning Strategy**: New API version if breaking changes required
- [ ] **Client Impact**: Documented impact on existing clients
- [ ] **Migration Guide**: Instructions for adapting to changes

---

## 📊 Business Logic Validation

### Skill Exchange Rules

- [ ] **Skill Proficiency**: Users can only teach skills marked as "Can Teach"
- [ ] **Session Limits**: Concurrent session limits enforced
- [ ] **Availability**: Session scheduling respects user availability
- [ ] **Mutual Exchange**: Bidirectional skill swaps properly handled
- [ ] **Data Integrity**: Business rules enforced at domain level

### User Management

- [ ] **Role Validation**: User roles and permissions properly validated
- [ ] **Profile Consistency**: User profiles maintain data consistency
- [ ] **Authentication Flow**: Auth flow follows security best practices

---

## 🎯 Reviewer Focus Areas

### For GitHub Copilot Review

Please pay special attention to:

1. **Architecture Compliance**: Verify clean architecture boundaries
2. **Security Vulnerabilities**: Check for common security issues
3. **Performance Bottlenecks**: Identify potential performance problems
4. **Business Logic**: Ensure domain rules are properly implemented
5. **Error Handling**: Comprehensive error handling and logging
6. **Code Maintainability**: Long-term maintainability and readability

### Manual Review Priorities

1. **Security**: Authentication, authorization, input validation
2. **Performance**: Database queries, async operations
3. **Architecture**: Layer separation, dependency direction
4. **Business Logic**: Domain rules and constraints
5. **Testing**: Coverage and quality of tests

---

## 📸 Screenshots/Demo

<!-- Include screenshots for UI changes or describe API behavior changes -->

---

## 🔗 Deployment Notes

<!-- Any special deployment considerations, environment variable changes, etc. -->

---

## ✅ Pre-merge Checklist

### Developer Checklist

- [ ] Code compiles without warnings
- [ ] All tests pass locally
- [ ] Database migrations applied successfully
- [ ] No merge conflicts
- [ ] Self-reviewed the code changes
- [ ] Updated relevant documentation

### Reviewer Checklist

- [ ] Architecture review completed
- [ ] Security review completed
- [ ] Performance considerations reviewed
- [ ] Business logic validation completed
- [ ] Testing coverage acceptable
- [ ] Documentation is adequate

---

## 💬 Additional Notes

<!-- Any additional context, concerns, or discussion points for reviewers -->

---

**Note for Reviewers**: This PR follows the SkillSwap coding standards and clean architecture principles. Please refer to `.github/copilot-instructions.md` for detailed review guidelines and project context.
