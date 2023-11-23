using Intent.Metadata.Models;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.Common.TypeResolution;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intent.Modules.AspNetCore.Controllers.Templates.Controller
{
    public interface IControllerTemplate<IControllerModel> : ICSharpFileBuilderTemplate, IIntentTemplate<IControllerModel>, IIntentTemplate
    {
        string GetTypeName(IHasTypeReference hasTypeReference);
        ClassTypeSource AddTypeSource(string templateId, string collectionFormat);
        CSharpStatement GetReturnStatement(IControllerOperationModel operationModel);
    }
}
