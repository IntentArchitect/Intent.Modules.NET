using System.Collections.Generic;
using Intent.Metadata.Models;
using Intent.Modelers.Services.Api;
using Intent.Modules.AspNetCore.Grpc.Templates.CommonTypesPartial;
using Intent.Modules.AspNetCore.Grpc.Templates.CqrsService;
using Intent.Modules.AspNetCore.Grpc.Templates.GrpcConfiguration;
using Intent.Modules.AspNetCore.Grpc.Templates.GrpcExceptionInterceptor;
using Intent.Modules.AspNetCore.Grpc.Templates.MessagePartial;
using Intent.Modules.AspNetCore.Grpc.Templates.PagedResultPartial;
using Intent.Modules.AspNetCore.Grpc.Templates.TraditionalService;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.TemplateExtensions", Version = "1.0")]

namespace Intent.Modules.AspNetCore.Grpc.Templates
{
    public static class TemplateExtensions
    {
        public static string GetCommonTypesPartialName(this IIntentTemplate template)
        {
            return template.GetTypeName(CommonTypesPartialTemplate.TemplateId);
        }

        public static string GetCqrsServiceName<T>(this IIntentTemplate<T> template) where T : IElement
        {
            return template.GetTypeName(CqrsServiceTemplate.TemplateId, template.Model);
        }

        public static string GetCqrsServiceName(this IIntentTemplate template, IElement model)
        {
            return template.GetTypeName(CqrsServiceTemplate.TemplateId, model);
        }

        public static string GetGrpcConfigurationName(this IIntentTemplate template)
        {
            return template.GetTypeName(GrpcConfigurationTemplate.TemplateId);
        }

        public static string GetGrpcExceptionInterceptorName(this IIntentTemplate template)
        {
            return template.GetTypeName(GrpcExceptionInterceptorTemplate.TemplateId);
        }

        public static string GetMessagePartialName<T>(this IIntentTemplate<T> template) where T : IElement
        {
            return template.GetTypeName(MessagePartialTemplate.TemplateId, template.Model);
        }

        public static string GetMessagePartialName(this IIntentTemplate template, IElement model)
        {
            return template.GetTypeName(MessagePartialTemplate.TemplateId, model);
        }

        public static string GetPagedResultPartialName(this IIntentTemplate template)
        {
            return template.GetTypeName(PagedResultPartialTemplate.TemplateId);
        }

        public static string GetTraditionalServiceName<T>(this IIntentTemplate<T> template) where T : ServiceModel
        {
            return template.GetTypeName(TraditionalServiceTemplate.TemplateId, template.Model);
        }

        public static string GetTraditionalServiceName(this IIntentTemplate template, ServiceModel model)
        {
            return template.GetTypeName(TraditionalServiceTemplate.TemplateId, model);
        }

    }
}