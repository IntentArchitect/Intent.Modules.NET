# Intent.AspNetCore.Identity.AccountController

Generate ASP.NET Core WebAPI controller for account management and JWT authentication.

## What This Module Does

This module generates a complete account management controller that exposes HTTP endpoints for user registration and authentication. It includes:

- **Register Endpoint** - User account registration with email/password
- **Authenticate Endpoint** - User login returning JWT bearer token
- **Email Confirmation** - Email sending service (customizable)
- **JWT Token Generation** - Secure token creation with configurable expiration
- **Token Service** - Reusable token creation abstraction

The controller integrates with ASP.NET Core Identity for user management and JWT for token-based authentication.

## Generated Artifacts

### AccountController
HTTP controller with endpoints:
- `POST /api/account/register` - Create new user account
- `POST /api/account/authenticate` - Login and receive JWT token
- `POST /api/account/refresh-token` - Refresh expired JWT tokens

### TokenService Interface and Implementation
- `ITokenService` - Interface for JWT token generation
- `TokenService` - Implementation creating JWT bearer tokens with:
  - User ID and email claims
  - Expiration configuration
  - Signature validation

### AccountEmailSender Interface and Implementation
- `IAccountEmailSender` - Interface for email notification
- `AccountEmailSender` - Implementation for sending confirmation/reset emails
- Customizable via dependency injection

### Models and DTOs
- Register request model with email/password validation
- Authenticate request model
- Token response model with token and expiration
- User identity models

## Key Design Patterns

### Identity and Authentication Flow
1. User registers with email and password
2. ASP.NET Core Identity hashes and stores password
3. User authenticates with credentials
4. TokenService generates JWT bearer token
5. Client includes token in Authorization header for subsequent requests

### JWT Token Structure
Generated tokens include:
- **Issued Claims**:
  - `sub` (subject) - User ID
  - `email` - User email address
  - `iat` (issued at) - Token creation time
  - `exp` (expiration) - Token expiration time
- **Validation**: HMAC signature verification

### Email Confirmation (Optional)
- Account registration triggers email confirmation email
- Custom `IAccountEmailSender` implementation sends email
- Confirmation link includes verification token
- Email confirmed before account fully activated

### Role-Based Authorization
- Token includes roles/claims from ASP.NET Core Identity
- Controllers use `[Authorize]` and `[Authorize(Roles="Admin")]`
- Custom authorization policies can be defined

## Customization Points

### Token Configuration
Customize via `TokenService` configuration:
- **Expiration Duration** - JWT token lifetime (default: 15 minutes)
- **Refresh Token Lifetime** - Refresh token validity period
- **Secret Key** - HMAC signature secret (from configuration)
- **Issuer/Audience** - JWT claims validation

### Email Sender Implementation
Override `IAccountEmailSender` implementation:
- SendConfirmationEmail - Custom email template
- SendPasswordResetEmail - Password reset email
- Use SendGrid, SMTP, or other providers

### User Identity Entity
Configure via ASP.NET Core Identity options:
- **PasswordPolicy** - Complexity requirements (length, uppercase, digits, symbols)
- **LockoutPolicy** - Account lockout after failed attempts
- **SignInPolicy** - Require email confirmation before signin
- **TokenProvider** - Token generation for email confirmation

### Class Name and Namespace Overrides
- `ClassName` - AccountController class name formula
- `Namespace` - Controller namespace formula

## When To Use

**Use this module when:**
- Building APIs with user authentication and JWT tokens
- You need account registration and login endpoints
- Integrating with ASP.NET Core Identity for user management
- Implementing Role-Based Access Control (RBAC)
- Supporting email-based account confirmation

**Don't use when:**
- Building applications with Windows/NTLM authentication
- Integrating with external OAuth/OIDC providers (consider MSAL module instead)
- API authentication is handled by API Gateway
- Client applications handle identity entirely

## Module Settings

### Identity User Type Configuration
- **IdentityUser Entity** - Entity representing authenticated users
- **Primary Key Type** - GUID (default) or Int

### Database Configuration
- EF Core migrations create AspNetUsers table
- Password hashing via Identity's password hasher
- Claims and roles stored in Identity tables

## Related Modules

- **Intent.AspNetCore.Identity** - ASP.NET Core Identity configuration
- **Intent.Security.JWT** - JWT token generation and validation utilities
- **Intent.Application.Identity** - Current user service and authorization
- **Intent.Security.MSAL** - OAuth/OIDC authentication via Azure AD
