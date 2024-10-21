# Intent.SharedKernel.Consumer

## Overview

A Shared Kernel is a pattern from Domain-Driven Design (DDD) where a set of common domain models, logic, and business rules are shared between two or more bounded contexts or applications. It represents the portion of the domain that multiple teams or systems collaborate on and use together, ensuring consistency in core functionality while allowing the rest of the domain to remain independent. Teams must communicate closely to coordinate changes in the shared kernel to avoid conflicts or issues in integration. This pattern is often used in situations where certain business rules or models are too critical or central to be duplicated across different systems but still need to be reused.

## What this module does?

This module adapts an implementation of our standard `Clean Architect .NET` application template, to be aware of and incorporate a Shared Kernel application.

To set up a Shared Kernel application read this [document](https://github.com/IntentArchitect/Intent.Modules.NET/blob/development/Modules/Intent.Modules.SharedKernel/README.md).


