# SkillSwap API Documentation

## Overview

The SkillSwap API is a RESTful service built with .NET 8 that enables peer-to-peer skill exchange functionality. This document provides comprehensive information for client developers integrating with the API.

## Base URL

- **Development**: `https://localhost:7000/api`
- **Staging**: `https://staging-api.skillswap.com/api`
- **Production**: `https://api.skillswap.com/api`

## Authentication

The API uses JWT (JSON Web Token) based authentication with refresh token support.

### Authentication Flow

1. **Login**: POST `/auth/login` with credentials
2. **Receive Tokens**: Get access token and refresh token
3. **API Calls**: Include access token in Authorization header
4. **Token Refresh**: Use refresh token when access token expires

```http
Authorization: Bearer {access_token}
```

## Error Handling

The SkillSwap API implements a standardized error response system with machine-readable error codes for improved client-side handling and localization support.

### Error Response Format

All API errors return JSON responses in this format:

```json
{
  "code": "VALIDATION_FAILED",
  "message": "One or more validation failures occurred.",
  "errors": {
    "Email": ["Email is required", "Email must be a valid email address"],
    "Password": ["Password must contain at least one uppercase letter"]
  },
  "traceId": "0HN4A8AAAAAAH8:00000001",
  "timestamp": "2025-10-02T10:30:00.123Z"
}
```

### Response Fields

| Field       | Type   | Description                                   | Always Present |
| ----------- | ------ | --------------------------------------------- | -------------- |
| `code`      | string | Machine-readable error identifier             | ✅             |
| `message`   | string | Human-readable error description              | ✅             |
| `errors`    | object | Field-level validation errors (if applicable) | ❌             |
| `traceId`   | string | Request trace identifier for debugging        | ✅             |
| `timestamp` | string | Error occurrence time in ISO 8601 format      | ✅             |

### Standard Error Codes

#### Client Errors (4xx)

| Code                    | HTTP Status      | Description                         | Typical Causes                                                 |
| ----------------------- | ---------------- | ----------------------------------- | -------------------------------------------------------------- |
| `VALIDATION_FAILED`     | 400 Bad Request  | Input validation failed             | Missing required fields, invalid format, constraint violations |
| `DOMAIN_RULE_VIOLATION` | 400 Bad Request  | Business rule violated              | Attempting invalid business operations                         |
| `INVALID_PARAMETERS`    | 400 Bad Request  | Request parameters invalid          | Null/empty required parameters, invalid data types             |
| `INVALID_OPERATION`     | 400 Bad Request  | Operation not allowed               | Action not permitted in current state                          |
| `UNAUTHORIZED_ACCESS`   | 401 Unauthorized | Authentication/authorization failed | Missing/expired token, insufficient permissions                |
| `RESOURCE_NOT_FOUND`    | 404 Not Found    | Requested resource doesn't exist    | Invalid ID, deleted resource                                   |

#### Server Errors (5xx)

| Code                    | HTTP Status               | Description             | Client Action                                    |
| ----------------------- | ------------------------- | ----------------------- | ------------------------------------------------ |
| `INTERNAL_SERVER_ERROR` | 500 Internal Server Error | Unexpected server error | Retry after delay, contact support if persistent |

### Field-Level Validation Errors

When validation fails, the `errors` object contains field-specific error messages:

```json
{
  "code": "VALIDATION_FAILED",
  "message": "One or more validation failures occurred.",
  "errors": {
    "Email": ["Email is required", "Email must be a valid email address"],
    "Password": [
      "Password must be at least 8 characters long",
      "Password must contain at least one uppercase letter",
      "Password must contain at least one special character"
    ],
    "FirstName": ["First name is required"]
  }
}
```

### Client Implementation Examples

#### JavaScript/TypeScript

```typescript
interface ApiError {
  code: string;
  message: string;
  errors?: Record<string, string[]>;
  traceId: string;
  timestamp: string;
}

class ApiClient {
  async handleResponse<T>(response: Response): Promise<T> {
    if (!response.ok) {
      const error: ApiError = await response.json();
      throw new ApiError(error);
    }
    return response.json();
  }
}

class ApiError extends Error {
  constructor(public readonly apiError: ApiError) {
    super(apiError.message);
    this.name = "ApiError";
  }

  get code(): string {
    return this.apiError.code;
  }

  get fieldErrors(): Record<string, string[]> | undefined {
    return this.apiError.errors;
  }

  get traceId(): string {
    return this.apiError.traceId;
  }
}

// Usage example
try {
  await apiClient.createUser(userData);
} catch (error) {
  if (error instanceof ApiError) {
    switch (error.code) {
      case "VALIDATION_FAILED":
        displayValidationErrors(error.fieldErrors);
        break;
      case "UNAUTHORIZED_ACCESS":
        redirectToLogin();
        break;
      case "RESOURCE_NOT_FOUND":
        showNotFoundMessage();
        break;
      default:
        showGenericError(error.message);
    }
  }
}
```

#### React Hook Example

```typescript
import { useState } from "react";

interface FormErrors {
  [key: string]: string[];
}

export function useApiForm<T>() {
  const [errors, setErrors] = useState<FormErrors>({});
  const [isLoading, setIsLoading] = useState(false);

  const submitForm = async (
    apiCall: () => Promise<T>,
    onSuccess?: (result: T) => void
  ) => {
    setIsLoading(true);
    setErrors({});

    try {
      const result = await apiCall();
      onSuccess?.(result);
    } catch (error) {
      if (error instanceof ApiError && error.code === "VALIDATION_FAILED") {
        setErrors(error.fieldErrors || {});
      } else {
        // Handle other error types
        console.error("API Error:", error);
      }
    } finally {
      setIsLoading(false);
    }
  };

  const getFieldError = (fieldName: string): string | undefined => {
    return errors[fieldName]?.[0];
  };

  return {
    errors,
    isLoading,
    submitForm,
    getFieldError,
    hasErrors: Object.keys(errors).length > 0,
  };
}
```

#### C# Client Example

```csharp
public class ApiError
{
    public string Code { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public Dictionary<string, string[]>? Errors { get; set; }
    public string TraceId { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
}

public class ApiException : Exception
{
    public ApiError ApiError { get; }

    public ApiException(ApiError apiError) : base(apiError.Message)
    {
        ApiError = apiError;
    }

    public string Code => ApiError.Code;
    public Dictionary<string, string[]>? FieldErrors => ApiError.Errors;
    public string TraceId => ApiError.TraceId;
}

// Usage
try
{
    await apiClient.CreateUserAsync(userData);
}
catch (ApiException ex) when (ex.Code == "VALIDATION_FAILED")
{
    foreach (var field in ex.FieldErrors ?? new())
    {
        Console.WriteLine($"{field.Key}: {string.Join(", ", field.Value)}");
    }
}
```

### Localization Support

Error codes enable consistent localization across different languages:

```typescript
// Localization configuration
const errorMessages = {
  en: {
    VALIDATION_FAILED: "Please correct the following errors:",
    UNAUTHORIZED_ACCESS: "Please log in to continue",
    RESOURCE_NOT_FOUND: "The requested item was not found",
    DOMAIN_RULE_VIOLATION: "This action is not allowed",
    INVALID_PARAMETERS: "Invalid request data",
    INVALID_OPERATION: "This operation cannot be performed",
    INTERNAL_SERVER_ERROR: "An unexpected error occurred",
  },
  es: {
    VALIDATION_FAILED: "Por favor corrige los siguientes errores:",
    UNAUTHORIZED_ACCESS: "Por favor inicia sesión para continuar",
    RESOURCE_NOT_FOUND: "El elemento solicitado no fue encontrado",
    // ... other translations
  },
};

function getLocalizedError(code: string, locale: string = "en"): string {
  return errorMessages[locale]?.[code] || errorMessages.en[code] || code;
}
```

### Best Practices

1. **Always Check Error Codes**: Use error codes for programmatic decisions, not HTTP status codes alone
2. **Handle Field Errors**: Display field-level validation errors near the relevant form fields
3. **Preserve Trace IDs**: Include trace IDs in support requests for faster debugging
4. **Implement Retry Logic**: For `INTERNAL_SERVER_ERROR`, implement exponential backoff retry
5. **Graceful Degradation**: Have fallback UI states for all error scenarios
6. **User-Friendly Messages**: Use localized, user-friendly messages instead of raw API messages when appropriate

### Rate Limiting

The API implements rate limiting to ensure fair usage:

- **Authenticated Users**: 1000 requests per hour
- **Anonymous Users**: 100 requests per hour
- **Rate Limit Headers**: Check `X-RateLimit-*` headers in responses

When rate limits are exceeded, you'll receive a `429 Too Many Requests` response.

### API Versioning

The API uses URL path versioning:

- Current version: `/api/v1/`
- Version format: Major version only (v1, v2, etc.)
- Backward compatibility maintained within major versions

## Support

- **Documentation**: This document and API reference
- **Support Email**: api-support@skillswap.com
- **GitHub Issues**: For bug reports and feature requests
- **Include Trace ID**: Always include the `traceId` from error responses in support requests
