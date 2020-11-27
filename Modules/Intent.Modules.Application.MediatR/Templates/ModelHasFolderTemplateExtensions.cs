using System.Collections.Generic;
using System.Linq;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.Common.Types.Api;

namespace Intent.Modules.Application.MediatR.Templates
{
    public static class ModelHasFolderTemplateExtensions
    {
        public static IList<string> GetParentFolderParts<TModel>(this IntentTemplateBase<TModel> template)
            where TModel : IHasFolder
        {
            return template.Model.GetFolderPath().Select(x => x.Name).ToList();
        }

        public static string GetFolderPath<TModel>(this IntentTemplateBase<TModel> template)
            where TModel : IHasFolder
        {
            return string.Join("/", template.GetParentFolderParts());
        }

        public static string GetNamespace<TModel>(this CSharpTemplateBase<TModel> template)
            where TModel : IHasFolder
        {
            if (template.GetParentFolderParts().Any())
            {
                return $"{template.OutputTarget.GetNamespace()}.{string.Join(".", template.GetParentFolderParts())}";
            }
            else
            {
                return template.OutputTarget.GetNamespace();
            }
        }
    }
}