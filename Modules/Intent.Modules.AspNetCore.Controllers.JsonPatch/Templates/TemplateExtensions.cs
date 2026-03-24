using System.Collections.Generic;
using Intent.Modules.AspNetCore.Controllers.JsonPatch.Templates.Templates.JsonMergePatchExecutor;
using Intent.Modules.AspNetCore.Controllers.JsonPatch.Templates.Templates.JsonMergePatchOperationFilter;
using Intent.Modules.AspNetCore.Controllers.JsonPatch.Templates.Templates.JsonMergePatchOperationTransformer;
using Intent.Modules.AspNetCore.Controllers.JsonPatch.Templates.Templates.PatchExecutorInterface;
using Intent.Modules.AspNetCore.Controllers.JsonPatch.Templates.Templates.PatchIgnoreMetadataProvider;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.TemplateExtensions", Version = "1.0")]

namespace Intent.Modules.AspNetCore.Controllers.JsonPatch.Templates
{
    public static class TemplateExtensions
    {
        public static string GetJsonMergePatchExecutorName(this IIntentTemplate template)
        {
            return template.GetTypeName(JsonMergePatchExecutorTemplate.TemplateId);
        }

        public static string GetJsonMergePatchOperationFilterName(this IIntentTemplate template)
        {
            return template.GetTypeName(JsonMergePatchOperationFilterTemplate.TemplateId);
        }

        public static string GetJsonMergePatchOperationTransformerName(this IIntentTemplate template)
        {
            return template.GetTypeName(JsonMergePatchOperationTransformerTemplate.TemplateId);
        }

        public static string GetPatchExecutorInterfaceName(this IIntentTemplate template)
        {
            return template.GetTypeName(PatchExecutorInterfaceTemplate.TemplateId);
        }

        public static string GetPatchIgnoreMetadataProviderName(this IIntentTemplate template)
        {
            return template.GetTypeName(PatchIgnoreMetadataProviderTemplate.TemplateId);
        }

    }
}