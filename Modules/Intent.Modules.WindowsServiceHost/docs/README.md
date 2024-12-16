# Intent.WindowsServiceHost

This module provides patterns for hosting a Windows Service.

## What are Windows Services?

A Windows Service is a background application or process in the Microsoft Windows operating system that runs independently of user interaction. Unlike regular applications that require a user to be logged in and interact with a graphical user interface, a Windows Service is designed to run in the background, often performing system-related tasks, automated processes, or server functionalities without direct user intervention. Services are typically started when the operating system boots and continue to run until the system is shut down. They provide a way for developers to create long-running, robust applications that can operate efficiently without the need for constant user presence or input. Services are managed through the Windows Service Control Manager (SCM), allowing users to configure their startup type, dependencies, and other settings.

For more information on Windows Services, check out their [official docs](https://learn.microsoft.com/en-us/dotnet/core/extensions/windows-service).

## What's in this module?

This module creates the required infrastructure for a Windows Service:-

* Program File with Host Creation and wiring
* `app.settings` configuration.
* Windows Background Service
 


