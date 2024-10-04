using Intent.Modules.Common.CSharp.Builder;

namespace Intent.Modules.Blazor.Templates.Templates.Client.Program;

/// <summary>
/// Abstracts working with methods for <c>Program.cs</c> file as depending on whether the use
/// <see href="https://learn.microsoft.com/dotnet/csharp/fundamentals/program-structure/top-level-statements">top-level statements</see>
/// option has been selected, the ultimate types used will be a <see cref="ICSharpMethod{TCSharpMethod}"/> where TCSharpMethod is
/// <see cref="ICSharpClassMethodDeclaration"/> or <see cref="ICSharpLocalFunction"/>.
/// </summary>
public interface IBlazorProgramFileMethod : ICSharpMethod<IBlazorProgramFileMethod>;