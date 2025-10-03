---
name: Code Review Improvement
about: Suggest code quality improvements identified during review
title: "[CODE-REVIEW] Brief description of improvement"
labels: ["code-review", "enhancement", "needs-discussion"]
assignees: ""
---

## 🔍 Code Review Finding

### Area of Improvement

<!-- Select the primary area this improvement relates to -->

- [ ] 🏗️ **Architecture**: Clean architecture, layer separation, dependency management
- [ ] 🛡️ **Security**: Authentication, authorization, input validation, data protection
- [ ] ⚡ **Performance**: Database queries, async operations, memory usage
- [ ] 🎯 **Domain Logic**: Business rules, entity design, domain modeling
- [ ] 🧪 **Testing**: Unit tests, integration tests, test coverage
- [ ] 📝 **Documentation**: Code comments, API documentation, README updates
- [ ] 🎨 **Code Quality**: Naming conventions, code structure, SOLID principles
- [ ] 🔧 **Technical Debt**: Refactoring opportunities, obsolete code, dependencies

### Current Implementation

<!-- Describe the current code or pattern that needs improvement -->

**File(s)**:
**Line(s)**:

```csharp
// Current implementation
public class ExampleClass
{
    // Code that needs improvement
}
```

### Proposed Improvement

<!-- Describe the recommended approach or pattern -->

```csharp
// Improved implementation
public class ExampleClass
{
    // Better approach following SkillSwap standards
}
```

### Rationale

<!-- Explain why this improvement is beneficial -->

**Benefits**:

- [ ] Improves maintainability
- [ ] Enhances performance
- [ ] Increases security
- [ ] Better follows clean architecture
- [ ] Reduces technical debt
- [ ] Improves testability

**Standards Reference**:

<!-- Reference relevant sections from coding standards -->

- See [Coding Standards](.github/CODING_STANDARDS.md) - Section:
- Follows clean architecture principle:
- Addresses security concern:

---

## 🎯 Implementation Details

### Affected Components

<!-- List all files, classes, or modules that would be affected -->

- [ ] Domain entities
- [ ] Application services
- [ ] Infrastructure repositories
- [ ] API controllers
- [ ] Database migrations
- [ ] Unit tests
- [ ] Integration tests

### Breaking Changes

- [ ] **No breaking changes** - Internal refactoring only
- [ ] **API changes** - May affect client applications
- [ ] **Database changes** - Requires migration
- [ ] **Configuration changes** - Requires deployment updates

### Dependencies

<!-- List any dependencies or prerequisites for this improvement -->

- [ ] Requires other issues to be completed first: #
- [ ] Depends on external library updates
- [ ] Needs team discussion/approval
- [ ] Requires database schema changes

---

## 📋 Implementation Plan

### Phase 1: Preparation

- [ ] Review current implementation thoroughly
- [ ] Identify all affected areas
- [ ] Plan backward compatibility strategy
- [ ] Update documentation

### Phase 2: Implementation

- [ ] Implement core changes
- [ ] Update unit tests
- [ ] Update integration tests
- [ ] Add/update documentation

### Phase 3: Validation

- [ ] Code review by team
- [ ] Test all affected functionality
- [ ] Performance testing (if applicable)
- [ ] Security review (if applicable)

---

## 🧪 Testing Strategy

### Test Coverage

- [ ] **Unit Tests**: Business logic properly tested
- [ ] **Integration Tests**: API endpoints and data access tested
- [ ] **Performance Tests**: Performance impact measured
- [ ] **Security Tests**: Security implications validated

### Test Cases

<!-- Describe specific test scenarios that should be covered -->

```csharp
[Test]
public void ExampleTest_WithScenario_ShouldExpectedBehavior()
{
    // Test implementation
}
```

---

## 📊 Success Criteria

### Quality Metrics

- [ ] Code coverage maintained/improved
- [ ] No performance regression
- [ ] All existing tests pass
- [ ] New functionality properly tested
- [ ] Documentation updated

### Architecture Compliance

- [ ] Follows clean architecture principles
- [ ] Proper layer separation maintained
- [ ] Domain logic in appropriate layer
- [ ] Dependencies flow inward

### Code Quality

- [ ] Follows SkillSwap coding standards
- [ ] Proper naming conventions
- [ ] Adequate error handling
- [ ] SOLID principles applied

---

## 🔗 Related Issues

### Dependencies

<!-- Link to related issues or pull requests -->

- Blocks: #
- Blocked by: #
- Related to: #

### Documentation References

- [ ] [Repository Instructions](.github/copilot-instructions.md)
- [ ] [Coding Standards](.github/CODING_STANDARDS.md)
- [ ] [Architecture Documentation](docs/tools-and-architecture.md)
- [ ] [Database Schema](docs/database-schema.md)

---

## 💬 Discussion Points

### Questions for Team

<!-- Raise any questions or concerns that need team input -->

1.
2.
3.

### Alternative Approaches

<!-- Describe any alternative solutions considered -->

1. **Option A**:

   - Pros:
   - Cons:

2. **Option B**:
   - Pros:
   - Cons:

### Risk Assessment

- **Low Risk**: Internal refactoring with good test coverage
- **Medium Risk**: Changes to public APIs or database schema
- **High Risk**: Major architectural changes or security-sensitive areas

---

## 📝 Additional Context

### Background

<!-- Provide any additional context about how this improvement was identified -->

### References

<!-- Link to any relevant resources, documentation, or examples -->

-
-
-

---

## ✅ Definition of Done

- [ ] Code implements the proposed improvement
- [ ] All affected tests pass
- [ ] New tests added where appropriate
- [ ] Code review completed and approved
- [ ] Documentation updated
- [ ] Performance impact assessed
- [ ] Security implications reviewed
- [ ] Clean architecture compliance verified

---

**Reviewer Note**: This improvement was identified through automated analysis and/or manual code review. Please refer to the [Repository Instructions](.github/copilot-instructions.md) for detailed review criteria and project context.
