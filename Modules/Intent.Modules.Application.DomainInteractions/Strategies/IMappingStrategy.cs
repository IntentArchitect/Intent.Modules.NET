using Intent.Metadata.Models;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Interactions;
using Intent.Modules.Common.CSharp.Templates;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Intent.Modules.Application.DomainInteractions.Strategies
{
    public interface IMappingStrategy
    {
        bool HasProjectTo();
        bool IsMatch(ICSharpClassMethodDeclaration method);

        void ImplementMappingStatement(ICSharpClassMethodDeclaration method, List<CSharpStatement> statements,
            EntityDetails entity, ICSharpTemplate template, ITypeReference returnType, string? returnDto);

        void ImplementPagedMappingStatement(ICSharpClassMethodDeclaration method, List<CSharpStatement> statements, EntityDetails entity, ICSharpTemplate template,
            ITypeReference returnType, string? returnDto, string? mappingMethod);
    }
}
