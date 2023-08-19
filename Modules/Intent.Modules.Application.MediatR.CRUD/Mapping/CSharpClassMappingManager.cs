using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Templates;

namespace Intent.Modules.Application.MediatR.CRUD.Mapping;

public class CSharpClassMappingManager : MappingManagerBase
{
    private readonly ICSharpFileBuilderTemplate _template;

    public CSharpClassMappingManager(ICSharpFileBuilderTemplate template) : base()
    {
        _template = template;
    }
}