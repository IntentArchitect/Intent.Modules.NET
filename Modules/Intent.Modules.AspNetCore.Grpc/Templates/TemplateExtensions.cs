using System.Collections.Generic;
using Intent.Modules.AspNetCore.Grpc.Templates.MessagePartials;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.TemplateExtensions", Version = "1.0")]

namespace Intent.Modules.AspNetCore.Grpc.Templates
{
    public static class TemplateExtensions
    {
        public static string GetMessagePartialsName<T>(this IIntentTemplate<T> template) where T : IGrpcMessage
        {
            return template.GetTypeName(MessagePartialsTemplate.TemplateId, template.Model);
        }

        public static string GetMessagePartialsName(this IIntentTemplate template, IGrpcMessage model)
        {
            return template.GetTypeName(MessagePartialsTemplate.TemplateId, model);
        }

    }
}