using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Metadata.DocumentDB.Api;
using Intent.Metadata.Models;
using Intent.Modelers.Domain.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.Constants;

namespace Intent.Modules.DocumentDB.Shared
{
    internal interface IPrimaryKeyInitStrategy
    {
        bool ShouldInsertPkInitializationCode(ClassModel targetClass);

        string GetGetterInitExpression(ICSharpTemplate template, ClassModel targetClass, string fieldName, string fieldTypeName);
    }

    internal static class EntityFactoryExtensionHelper
    {
        public static void Execute(
            IApplication application,
            Func<ClassModel, bool> dbProviderApplies,
            IPrimaryKeyInitStrategy primaryKeyInitStrategy,
            bool makeNonPersistentPropertiesVirtual)
        {
            
            
            // Implementation
            {
                var templates = application.FindTemplateInstances<ICSharpFileBuilderTemplate>(TemplateDependency.OnTemplate(TemplateRoles.Domain.Entity.Primary));
                foreach (var template in templates)
                {
                    var templateModel = ((CSharpTemplateBase<ClassModel>)template).Model;
                    if (!dbProviderApplies(templateModel))
                    {
                        continue;
                    }

                    template.CSharpFile.OnBuild(file =>
                    {
                        file.AddUsing("System");
                        var @class = file.Classes.First();
                        var model = @class.GetMetadata<ClassModel>("model");

                        var toChangeNavigationProperties = GetNavigableAggregateAssociations(model);
                        foreach (var navigation in toChangeNavigationProperties)
                        {
                            // Remove the "Entity" Properties and backing fields, there can be
                            // multiple if there are explicit interface implementations
                            var properties = @class.GetAllProperties()
                                .Where(x => x.TryGetMetadata<IMetadataModel>("model", out var metadataModel) &&
                                            metadataModel.Id == navigation.Id)
                                .ToArray();
                            foreach (var property in properties)
                            {
                                @class.Properties.Remove(property);
                            }

                            var field = @class.Fields
                                .FirstOrDefault(x => x.TryGetMetadata<IMetadataModel>("model", out var metadataModel) &&
                                                     metadataModel.Id == navigation.Id);
                            if (field != null)
                            {
                                @class.Fields.Remove(field);
                            }
                        }

                        var pks = model.Attributes.Where(x => x.HasPrimaryKey()).ToArray();
                        if (!pks.Any())
                        {
                            return;
                        }

                        var primaryKeyProperties = new List<CSharpProperty>();
                        foreach (var attribute in pks)
                        {
                            var existingPk = @class
                                .GetAllProperties()
                                .First(x => x.Name.Equals(attribute.Name, StringComparison.InvariantCultureIgnoreCase));
                            var fieldName = $"_{attribute.Name.ToCamelCase()}";

                            if (primaryKeyInitStrategy.ShouldInsertPkInitializationCode(model))
                            {
                                InitializePrimaryKey(primaryKeyInitStrategy, template, @class, model, attribute, existingPk, fieldName);
                            }

                            primaryKeyProperties.Add(existingPk);
                        }

                        if (!@class.TryGetMetadata("primary-keys", out _))
                        {
                            @class.AddMetadata("primary-keys", primaryKeyProperties.ToArray());
                        }

                        if (makeNonPersistentPropertiesVirtual)
                        {
                            MakeNonPersistentPropertiesVirtual(@class);
                        }
                    });
                }
            }

            // Interface
            {
                var templates = application.FindTemplateInstances<ICSharpFileBuilderTemplate>(TemplateDependency.OnTemplate(TemplateRoles.Domain.Entity.Interface));
                foreach (var template in templates)
                {
                    var templateModel = ((CSharpTemplateBase<ClassModel>)template).Model;
                    if (!dbProviderApplies(templateModel))
                    {
                        continue;
                    }

                    template.CSharpFile.OnBuild(file =>
                    {
                        var @interface = file.Interfaces.FirstOrDefault();
                        if (@interface == null)
                        {
                            return;
                        }

                        var model = @interface.GetMetadata<ClassModel>("model");

                        var toChangeNavigationProperties = GetNavigableAggregateAssociations(model);
                        foreach (var navigation in toChangeNavigationProperties)
                        {
                            //Remove the "Entity" Properties and backing fields
                            var property = @interface.Properties
                                .FirstOrDefault(x => x.TryGetMetadata<IMetadataModel>("model", out var metadataModel) &&
                                                     metadataModel.Id == navigation.Id);
                            @interface.Properties.Remove(property);

                            var field = @interface.Fields
                                .FirstOrDefault(x => x.TryGetMetadata<IMetadataModel>("model", out var metadataModel) &&
                                                     metadataModel.Id == navigation.Id);
                            if (field != null)
                            {
                                @interface.Fields.Remove(field);
                            }
                        }

                        var pks = model.Attributes.Where(x => x.HasPrimaryKey()).ToArray();
                        if (!pks.Any())
                        {
                            return;
                        }

                        var primaryKeyProperties = new List<CSharpProperty>();
                        foreach (var attribute in pks)
                        {
                            var existingPk = @interface.Properties
                                .First(x => x.Name.Equals(attribute.Name, StringComparison.InvariantCultureIgnoreCase));

                            primaryKeyProperties.Add(existingPk);
                        }

                        if (!@interface.TryGetMetadata("primary-keys", out _))
                        {
                            @interface.AddMetadata("primary-keys", primaryKeyProperties.ToArray());
                        }
                    });
                }
            }
        }

        private static void MakeNonPersistentPropertiesVirtual(CSharpClass @class)
        {
            foreach (var property in @class.Properties.Where(p => p.HasMetadata("non-persistent")))
            {
                property.Virtual();
            }
        }

        private static void InitializePrimaryKey(
            IPrimaryKeyInitStrategy initStrategy,
            ICSharpTemplate template,
            CSharpClass @class,
            ClassModel targetClass,
            AttributeModel attributePk,
            CSharpProperty existingPk,
            string fieldName)
        {
            var templateBase = (IntentTemplateBase)template;
            @class.AddField(templateBase.UseType(templateBase.GetTypeInfo(attributePk.TypeReference).WithIsNullable(true)), fieldName);

            var getExpressionSuffix = initStrategy.GetGetterInitExpression(template, targetClass, fieldName, attributePk.TypeReference.Element.Name);

            existingPk.Getter.WithExpressionImplementation(getExpressionSuffix);
            existingPk.Setter.WithExpressionImplementation($"{fieldName} = value");
        }

        private static bool HasNavigationProperty(ClassModel model, AssociationEndModel association)
        {
            return (association.Association.SourceEnd.Element.Id == model.Id && association.Association.TargetEnd.IsNavigable) ||
                   (association.Association.TargetEnd.Element.Id == model.Id && association.Association.SourceEnd.IsNavigable);
        }

        private static IEnumerable<AssociationEndModel> GetNavigableAggregateAssociations(ClassModel model)
        {
            return model
                .AssociationEnds()
                .Where(x => HasNavigationProperty(model, x) &&
                            (x.Association.SourceEnd.IsCollection || x.Association.SourceEnd.IsNullable))
                .ToArray();
        }
    }
}