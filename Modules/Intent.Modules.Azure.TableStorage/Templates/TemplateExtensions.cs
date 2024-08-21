using System.Collections.Generic;
using Intent.Modelers.Domain.Api;
using Intent.Modules.Azure.TableStorage.Templates.TableStorageRepository;
using Intent.Modules.Azure.TableStorage.Templates.TableStorageRepositoryBase;
using Intent.Modules.Azure.TableStorage.Templates.TableStorageRepositoryInterface;
using Intent.Modules.Azure.TableStorage.Templates.TableStorageTableAdapterInterface;
using Intent.Modules.Azure.TableStorage.Templates.TableStorageTableEntity;
using Intent.Modules.Azure.TableStorage.Templates.TableStorageTableEntityInterface;
using Intent.Modules.Azure.TableStorage.Templates.TableStorageUnitOfWork;
using Intent.Modules.Azure.TableStorage.Templates.TableStorageUnitOfWorkInterface;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.TemplateExtensions", Version = "1.0")]

namespace Intent.Modules.Azure.TableStorage.Templates
{
    public static class TemplateExtensions
    {
        public static string GetTableStorageRepositoryName<T>(this IIntentTemplate<T> template) where T : ClassModel
        {
            return template.GetTypeName(TableStorageRepositoryTemplate.TemplateId, template.Model);
        }

        public static string GetTableStorageRepositoryName(this IIntentTemplate template, ClassModel model)
        {
            return template.GetTypeName(TableStorageRepositoryTemplate.TemplateId, model);
        }

        public static string GetTableStorageRepositoryBaseName(this IIntentTemplate template)
        {
            return template.GetTypeName(TableStorageRepositoryBaseTemplate.TemplateId);
        }

        public static string GetTableStorageRepositoryInterfaceName(this IIntentTemplate template)
        {
            return template.GetTypeName(TableStorageRepositoryInterfaceTemplate.TemplateId);
        }

        public static string GetTableStorageTableAdapterInterfaceName(this IIntentTemplate template)
        {
            return template.GetTypeName(TableStorageTableAdapterInterfaceTemplate.TemplateId);
        }

        public static string GetTableStorageTableEntityName<T>(this IIntentTemplate<T> template) where T : ClassModel
        {
            return template.GetTypeName(TableStorageTableEntityTemplate.TemplateId, template.Model);
        }

        public static string GetTableStorageTableEntityName(this IIntentTemplate template, ClassModel model)
        {
            return template.GetTypeName(TableStorageTableEntityTemplate.TemplateId, model);
        }

        public static string GetTableStorageTableEntityInterfaceName<T>(this IIntentTemplate<T> template) where T : ClassModel
        {
            return template.GetTypeName(TableStorageTableEntityInterfaceTemplate.TemplateId, template.Model);
        }

        public static string GetTableStorageTableEntityInterfaceName(this IIntentTemplate template, ClassModel model)
        {
            return template.GetTypeName(TableStorageTableEntityInterfaceTemplate.TemplateId, model);
        }

        public static string GetTableStorageUnitOfWorkName(this IIntentTemplate template)
        {
            return template.GetTypeName(TableStorageUnitOfWorkTemplate.TemplateId);
        }

        public static string GetTableStorageUnitOfWorkInterfaceName(this IIntentTemplate template)
        {
            return template.GetTypeName(TableStorageUnitOfWorkInterfaceTemplate.TemplateId);
        }

    }
}