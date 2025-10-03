/**
 * @name SkillSwap Security Patterns
 * @description Custom security analysis for SkillSwap application
 * @kind problem
 * @problem.severity warning
 * @security-severity 6.0
 * @precision medium
 * @id csharp/skillswap-security
 * @tags security
 *       external/cwe/cwe-200
 *       external/cwe/cwe-209
 */

import csharp

// Check for potential password exposure in logs
from MethodCall call, StringLiteral arg
where
  call.getTarget().getName().matches("%Log%") and
  arg = call.getAnArgument() and
  (
    arg.getValue().toLowerCase().matches("%password%") or
    arg.getValue().toLowerCase().matches("%secret%") or
    arg.getValue().toLowerCase().matches("%token%") or
    arg.getValue().toLowerCase().matches("%api%key%")
  )
select call, "Potential sensitive data exposure in logging: " + arg.getValue()

// Check for hardcoded connection strings
from FieldDeclaration field, StringLiteral value
where
  field.getAVariable().getInitializer() = value and
  (
    value.getValue().matches("%Server=%") or
    value.getValue().matches("%Host=%") or
    value.getValue().matches("%Password=%")
  ) and
  not value.getValue().matches("%{%") // Exclude configuration placeholders
select field, "Hardcoded connection string detected: consider using configuration"

// Check for direct SQL command usage (should use EF Core)
from MethodCall call
where
  call.getTarget().getDeclaringType().getName().matches("%SqlCommand%") or
  call.getTarget().getName() = "ExecuteSqlRaw"
select call, "Direct SQL usage detected: ensure parameterized queries are used"

// Check for missing authorization attributes on controllers
from Method method, Class controller
where
  controller.getName().matches("%Controller%") and
  method.getDeclaringType() = controller and
  method.isPublic() and
  (method.getName().matches("Get%") or method.getName().matches("Post%") or 
   method.getName().matches("Put%") or method.getName().matches("Delete%")) and
  not exists(Attribute attr | 
    attr.getTarget() = method and 
    (attr.getType().getName() = "Authorize" or attr.getType().getName() = "AllowAnonymous")
  )
select method, "Public controller action missing authorization attribute"