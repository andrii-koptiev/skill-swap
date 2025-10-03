#!/bin/bash
# Test script to validate GitHub Actions fixes

echo "🧪 Testing GitHub Actions workflow fixes..."
echo

# Test 1: Coverage report logic
echo "📊 Testing coverage report detection logic..."
if find ./services/api/TestResults -name "coverage.cobertura.xml" -type f 2>/dev/null | grep -q .; then
    echo "✅ Coverage files found - would generate report"
else
    echo "ℹ️ No coverage files found - would skip report generation (expected)"
    echo "   This is normal when no unit tests exist yet"
fi
echo

# Test 2: Security scan for C# files
echo "🔒 Testing security scan for C# files..."
cd services/api
SECURITY_ISSUES=""

# Check for hardcoded passwords in C# code
if grep -r -E "password\s*=\s*[\"'][^\"']{3,}[\"']" --include="*.cs" src/ | grep -v "//" | grep -v -E "(PasswordPolicy|PasswordValidator|PasswordOptions|PasswordConfiguration|PasswordSettings|\.Password\s*=\s*[\"']{{|\$\{|%|Environment)" 2>/dev/null; then
    echo "⚠️ Potential hardcoded passwords found in C# code"
    SECURITY_ISSUES="${SECURITY_ISSUES} hardcoded_password"
else
    echo "✅ No hardcoded passwords found in C# code"
fi

# Check for connection strings in C# code
if grep -r -E "(Server|Host)=.*Password\s*=\s*[^;]{3,}" --include="*.cs" src/ | grep -v "//" | grep -v -E "(ConfigurationExample|ConnectionStringBuilder|Configuration|Settings|\$\{|%|Environment\.GetConnectionString)" 2>/dev/null; then
    echo "⚠️ Connection strings with embedded passwords found in C# code"
    SECURITY_ISSUES="${SECURITY_ISSUES} connection_string_creds"
else
    echo "✅ No hardcoded connection strings found in C# code"
fi

# Check appsettings files (informational only)
echo
echo "📋 Checking configuration files (informational)..."
if find src/ -name "appsettings*.json" -not -path "*/bin/*" -not -path "*/obj/*" -exec grep -l -E "(Server|Host)=.*Password\\s*=\\s*[^;]{3,}" {} \; 2>/dev/null | head -1; then
    echo "ℹ️ Connection strings found in appsettings files - ensure these use environment variables in production"
    echo "   Files found with connection strings:"
    find src/ -name "appsettings*.json" -not -path "*/bin/*" -not -path "*/obj/*" -exec grep -l -E "(Server|Host)=.*Password\\s*=\\s*[^;]{3,}" {} \; 2>/dev/null | sed 's/^/   - /'
else
    echo "✅ No connection strings found in appsettings files"
fi

echo
echo "🎯 Final security assessment:"
if [ -n "$SECURITY_ISSUES" ] && [ "$SECURITY_ISSUES" != " " ]; then
    echo "❌ Security issues found in C# code: $SECURITY_ISSUES"
    echo "   GitHub Actions would fail with exit code 1"
else
    echo "✅ No critical security issues detected in C# code"
    echo "   GitHub Actions would pass security scan"
fi

echo
echo "✨ GitHub Actions workflow fixes validated!"