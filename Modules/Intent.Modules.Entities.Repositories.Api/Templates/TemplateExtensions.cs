using System;
using System.Collections.Generic;
using Intent.Modelers.Domain.Api;
using Intent.Modelers.Domain.Repositories.Api;
using Intent.Modules.Common.Templates;
using Intent.Modules.Entities.Repositories.Api.Templates.CursorPagedListInterface;
using Intent.Modules.Entities.Repositories.Api.Templates.CustomRepository;
using Intent.Modules.Entities.Repositories.Api.Templates.CustomRepositoryInterface;
using Intent.Modules.Entities.Repositories.Api.Templates.EntityRepositoryInterface;
using Intent.Modules.Entities.Repositories.Api.Templates.PagedListInterface;
using Intent.Modules.Entities.Repositories.Api.Templates.RepositoryInterface;
using Intent.Modules.Entities.Repositories.Api.Templates.UnitOfWorkInterface;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.TemplateExtensions", Version = "1.0")]

namespace Intent.Modules.Entities.Repositories.Api.Templates
{
    public static class TemplateExtensions
    {
        public static string GetCursorPagedListInterfaceName(this IIntentTemplate template)
        {
            return template.GetTypeName(CursorPagedListInterfaceTemplate.TemplateId);
        }
        public static string GetCustomRepositoryName<T>(this IIntentTemplate<T> template) where T : RepositoryModel
        {
            return template.GetTypeName(CustomRepositoryTemplate.TemplateId, template.Model);
        }

        public static string GetCustomRepositoryName(this IIntentTemplate template, RepositoryModel model)
        {
            return template.GetTypeName(CustomRepositoryTemplate.TemplateId, model);
        }
        public static string GetCustomRepositoryInterfaceName<T>(this IIntentTemplate<T> template) where T : RepositoryModel
        {
            return template.GetTypeName(CustomRepositoryInterfaceTemplate.TemplateId, template.Model);
        }

        public static string GetCustomRepositoryInterfaceName(this IIntentTemplate template, RepositoryModel model)
        {
            return template.GetTypeName(CustomRepositoryInterfaceTemplate.TemplateId, model);
        }
        public static string GetEntityRepositoryInterfaceName<T>(this IIntentTemplate<T> template) where T : ClassModel
        {
            return template.GetTypeName(EntityRepositoryInterfaceTemplate.TemplateId, template.Model);
        }

        public static string GetEntityRepositoryInterfaceName(this IIntentTemplate template, ClassModel model)
        {
            return template.GetTypeName(EntityRepositoryInterfaceTemplate.TemplateId, model);
        }

        public static string GetPagedListInterfaceName(this IIntentTemplate template)
        {
            return template.GetTypeName(PagedListInterfaceTemplate.TemplateId);
        }

        public static string GetEntityRepositoryInterfaceName<T>(this IntentTemplateBase<T> template) where T : Intent.Modelers.Domain.Api.ClassModel
        {
            return template.GetTypeName(EntityRepositoryInterfaceTemplate.TemplateId, template.Model);
        }

        public static string GetEntityRepositoryInterfaceName(this IntentTemplateBase template, Intent.Modelers.Domain.Api.ClassModel model)
        {
            return template.GetTypeName(EntityRepositoryInterfaceTemplate.TemplateId, model);
        }

        [Obsolete]
        [IntentIgnore]
        public static string GetPagedResultInterfaceName<T>(this IntentTemplateBase<T> template)
        {
            return template.GetTypeName(PagedListInterfaceTemplate.TemplateId);
        }

        [Obsolete]
        [IntentIgnore]
        public static string GetRepositoryInterfaceName<T>(this IntentTemplateBase<T> template)
        {
            return template.GetTypeName(RepositoryInterfaceTemplate.TemplateId);
        }

        [Obsolete]
        [IntentIgnore]
        public static string GetUnitOfWorkInterfaceName<T>(this IntentTemplateBase<T> template)
        {
            return template.GetTypeName(UnitOfWorkInterfaceTemplate.TemplateId);
        }

        public static string GetRepositoryInterfaceName<T>(this IIntentTemplate template)
        {
            return template.GetTypeName(RepositoryInterfaceTemplate.TemplateId);
        }

        public static string GetUnitOfWorkInterfaceName<T>(this IIntentTemplate template)
        {
            return template.GetTypeName(UnitOfWorkInterfaceTemplate.TemplateId);
        }
    }
}