using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;

namespace Intent.Modules.Entities.Templates;

public interface ICSharpFileBuilderTemplate : ICSharpTemplate
{
    CSharpFile Output { get; }
}