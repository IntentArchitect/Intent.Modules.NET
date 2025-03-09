using System;
using Intent.Metadata.Models;
using Intent.Modules.AspNetCore.Grpc.Templates.CommonTypesProtoFile;
using Intent.Modules.AspNetCore.Grpc.Templates.MessageProtoFile;
using Intent.Modules.AspNetCore.Grpc.Templates.PagedResultProtoFile;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;

namespace Intent.Modules.AspNetCore.Grpc.Templates
{
    internal static class ProtoPartialGetTypeNameHelper
    {
        public static string ResolveDtoTypeName<T>(this CSharpTemplateBase<T> template, IHasTypeReference hasTypeReference) =>
            ResolveDtoTypeName(template, hasTypeReference.TypeReference);

        public static string ResolveDtoTypeName<T>(
            this CSharpTemplateBase<T> template,
            ITypeReference typeReference,
            bool? isNullable = null,
            bool? isCollection = null)
        {
            var otherTemplate = GetTemplateInstance<MessageProtoFileTemplate, T>(template, MessageProtoFileTemplate.TemplateId, typeReference.Element.Id);
            otherTemplate.Add(typeReference);

            var typeName = typeReference.GetClosedGenericTypeName();
            if (!string.IsNullOrWhiteSpace(otherTemplate.CSharpNamespace))
            {
                typeName = $"{otherTemplate.CSharpNamespace}.{typeName}";
            }

            return template.UseType(typeName);
        }

        public static string ResolveCommonListOfName<TModel>(this CSharpTemplateBase<TModel> template, ITypeReference typeReference)
        {
            return template.ResolveCommonListOfName(typeReference.GetClosedGenericTypeName());
        }

        public static string ResolveCommonListOfName<TModel>(this CSharpTemplateBase<TModel> template, string messageName)
        {
            var commonTemplate = GetTemplateInstance<CommonTypesProtoFileTemplate, TModel>(template, CommonTypesProtoFileTemplate.TemplateId);

            var typeName = $"ListOf{messageName}";
            if (!string.IsNullOrWhiteSpace(commonTemplate.CSharpNamespace))
            {
                typeName = $"{commonTemplate.CSharpNamespace}.{typeName}";
            }

            return template.UseType(typeName);
        }

        public static string ResolvePagedResultOfName<TModel>(this CSharpTemplateBase<TModel> template, ITypeReference typeReference)
        {
            var messageName = typeReference.GetClosedGenericTypeName();

            var pagedResultTemplate = GetTemplateInstance<PagedResultProtoFileTemplate, TModel>(template, PagedResultProtoFileTemplate.TemplateId);
            if (!string.IsNullOrWhiteSpace(pagedResultTemplate.CSharpNamespace))
            {
                messageName = $"{pagedResultTemplate.CSharpNamespace}.{messageName}";
            }

            return template.UseType(messageName);
        }

        private static TTemplate GetTemplateInstance<TTemplate, TModel>(IIntentTemplate<TModel> template, string templateId, string modelId = null)
            where TTemplate : class
        {
            var otherTemplate = modelId != null
                ? template.ExecutionContext.FindTemplateInstance<IIntentTemplate>(templateId, modelId)
                : template.ExecutionContext.FindTemplateInstance<IIntentTemplate>(templateId);

            return otherTemplate as TTemplate ?? throw new InvalidOperationException();
        }

    }
}
