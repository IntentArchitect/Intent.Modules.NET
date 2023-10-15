using System.Collections.Generic;
using Intent.Modules.Azure.TableStorage.Templates.TableStorageRepository;
using Intent.Modules.Azure.TableStorage.Templates.TableStorageRepositoryBase;
using Intent.Modules.Azure.TableStorage.Templates.TableStorageRepositoryInterface;
using Intent.Modules.Azure.TableStorage.Templates.TableStorageTableEntity;
using Intent.Modules.Azure.TableStorage.Templates.TableStorageTableIEntityInterface;
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
        public static string GetTableStorageRepositoryName<T>(this IIntentTemplate<T> template) where T : Intent.Modelers.Domain.Api.ClassModel
        {
            return template.GetTypeName(TableStorageRepositoryTemplate.TemplateId, template.Model);
        }

        public static string GetTableStorageRepositoryName(this IIntentTemplate template, Intent.Modelers.Domain.Api.ClassModel model)
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

        public static string GetTableStorageTableEntityName<T>(this IIntentTemplate<T> template) where T : Intent.Modelers.Domain.Api.ClassModel
        {
            return template.GetTypeName(TableStorageTableEntityTemplate.TemplateId, template.Model);
        }

        public static string GetTableStorageTableEntityName(this IIntentTemplate template, Intent.Modelers.Domain.Api.ClassModel model)
        {
            return template.GetTypeName(TableStorageTableEntityTemplate.TemplateId, model);
        }

        public static string GetTableStorageTableIEntityInterfaceName(this IIntentTemplate template)
        {
            return template.GetTypeName(TableStorageTableIEntityInterfaceTemplate.TemplateId);
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