# Intent.Dapper

This module provides patterns for working with Dapper as a persistence mechanism.

## What is Dapper?

Dapper is a lightweight, high-performance micro-ORM (Object-Relational Mapper) for .NET, designed to simplify data access and manipulation in databases. Developed by Stack Overflow, Dapper allows developers to execute SQL queries and map the results to strongly-typed objects with minimal overhead. It operates by extending IDbConnection and leverages raw SQL, which makes it both fast and flexible. Dapper's primary strength lies in its simplicity and efficiency. 

For more information on Dapper, check out their [official docs](https://www.learndapper.com/).

## Overview

This module generates code to work with Dapper's [`Dapper` NuGet package](https://www.nuget.org/packages/Dapper), in particular:

- Repositories, for persistence.

