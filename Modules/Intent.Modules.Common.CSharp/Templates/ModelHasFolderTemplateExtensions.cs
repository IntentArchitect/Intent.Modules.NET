using System.Collections.Generic;
using System.Linq;
using Intent.Modules.Common.Templates;
using Intent.Modules.Common.Types.Api;

namespace Intent.Modules.Common.CSharp.Templates
{
    public static class ModelHasFolderTemplateExtensions
    {
        public static string GetFolderPath<TModel>(this IntentTemplateBase<TModel> template)
            where TModel : IHasFolder
        {
            return string.Join("/", template.Model.GetParentFolderNames());
        }

        public static string GetNamespace<TModel>(this CSharpTemplateBase<TModel> template)
            where TModel : IHasFolder
        {
            if (template.Model.GetParentFolderNames().Any())
            {
                return $"{template.OutputTarget.GetNamespace()}.{string.Join(".", template.Model.GetParentFolderNames())}";
            }
            else
            {
                return template.OutputTarget.GetNamespace();
            }
        }
    }
}