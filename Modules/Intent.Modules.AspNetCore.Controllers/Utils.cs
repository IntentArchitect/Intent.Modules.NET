using Intent.Modules.AspNetCore.Controllers.Templates.Controller;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Metadata.WebApi.Models;

namespace Intent.Modules.AspNetCore.Controllers;

public static class Utils
{
    public static bool ShouldBeJsonResponseWrapped(this ICSharpTemplate template, IControllerOperationModel operationModel)
    {
        var isWrappedReturnType = operationModel.MediaType == HttpMediaType.ApplicationJson;
        var returnsCollection = operationModel.ReturnType?.IsCollection == true;
        var returnsString = operationModel.ReturnType.HasStringType();
        var returnsPrimitive = template.GetTypeInfo(operationModel.ReturnType).IsPrimitive &&
                               !returnsCollection;

        return isWrappedReturnType && (returnsPrimitive || returnsString);
    }
}