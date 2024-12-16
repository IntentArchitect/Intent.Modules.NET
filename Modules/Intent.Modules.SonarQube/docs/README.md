# Intent.SonarQube

This module installs SonarQube IDE as part of the solution, providing real-time static code analysis and feedback.

## What is SonarQube IDE?

SonarQube IDE integrates seamlessly into your development workflow, providing real-time code analysis and feedback as you write code. It can detect complex bugs, vulnerabilities, and code smells, offering detailed reports on software maintainability, reliability, and security.

This module installs the [SonarQube ide](https://www.sonarsource.com/products/sonarlint/features/visual-studio/) NuGet package, which provides a number of `analyzers` which handle the static code analysis.

More information on SonarQube can be found on [GitHub](https://github.com/SonarSource/sonar-dotnet) and the [SonrQube ide product page](https://github.com/SonarSource/sonar-dotnet)

## Configuration

No configuration is required - the analyzers will be installed on all projects in the solution by default, and will start analyzing the code.
