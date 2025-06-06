# Intent.AspNetCore.IdentityService

This module adds ASP.NET Core Identity operations as a service to your application.

## What is ASP.NET Core Identity?

ASP.NET Core Identity is a membership system for ASP.NET Core applications. 

It provides a comprehensive framework for managing users, passwords, roles, claims, tokens, and external logins in a secure and extensible way.

Learn more here [documentation]('https://learn.microsoft.com/en-us/aspnet/core/security/authentication/identity?view=aspnetcore-9.0&tabs=visual-studio')

## Service Designer

Intent.AspNetCore.IdentityService will upon install create a new service called `IdentityService` with exposed `Http Operations` to register, confirm and login users, as well as other operations.

The `IdentityService` will wire up all required aspects required to perform these operations.

If you want to alter or use a different way of performing a specific operation, just remove the `Identity Service Handler` stereotype for that operation, and you are able to implement your own logic.

## What's in This Module?

