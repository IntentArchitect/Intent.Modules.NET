# Intent.VisualStudio.Projects

This [Intent Architect](https://intentarchitect.com/) module adds the `Visual Studio` [Designer](https://docs.intentarchitect.com/articles/application-development/modelling/about-designers/about-designers.html) to an Intent Architect [Application](https://docs.intentarchitect.com/articles/application-development/applications-and-solutions/about-applications/about-applications.html), generates and manages both Visual Studio `.sln` and .NET `.csproj` files.

## The _.NET Settings_ stereotype

### The `Suppress Warnings` property

Adds  a [`<NoWarn />`](https://learn.microsoft.com/dotnet/csharp/language-reference/compiler-options/errors-warnings#nowarn) element to the `.csproj` file with the specified value of semi-colon separated codes of warnings to suppress.

By default this is populated with the value `$(NoWarn)` which will apply the [default suppressed warnings](https://github.com/dotnet/sdk/blob/2eb6c546931b5bcb92cd3128b93932a980553ea1/src/Tasks/Microsoft.NET.Build.Tasks/targets/Microsoft.NET.Sdk.CSharp.props#L16). While this value is set to `$(NoWarn)`, no `<NoWarn />` element will be added to the `.csproj` file.

`;1561` is automatically appended by some Intent Architect application templates to suppress the [Missing XML comment for publicly visible type or member 'Type_or_Member'](https://learn.microsoft.com/dotnet/csharp/language-reference/compiler-messages/cs1591) warning.
