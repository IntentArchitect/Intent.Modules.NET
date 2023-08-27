using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Metadata.DocumentDB.Api;
using Intent.Metadata.DocumentDB.Api.Extensions;
using Intent.Metadata.Models;
using Intent.Modelers.Domain.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.Constants;

namespace Intent.Modules.DocumentDB.Shared
{
    internal static class EntityFactoryExtensionHelper
    {
        public static void Execute(IApplication application, Func<ClassModel, bool> dbProviderApplies, bool initializePrimaryKeyOnAggregateRoots)
        {
            var templates = application.FindTemplateInstances<ICSharpFileBuilderTemplate>(TemplateDependency.OnTemplate(TemplateFulfillingRoles.Domain.Entity.Primary));
            foreach (var template in templates)
            {
                var templateModel = ((CSharpTemplateBase<ClassModel>)template).Model;
                if (!dbProviderApplies(templateModel))
                {
                    continue;
                }

                template.CSharpFile.OnBuild((Action<CSharpFile>)(file =>
                {
                    file.AddUsing("System");
                    var @class = file.Classes.First();
                    var model = @class.GetMetadata<ClassModel>("model");

                    var toChangeNavigationProperties = GetNavigableAggregateAssociations(model);
                    foreach (var navigation in toChangeNavigationProperties)
                    {
                        //Remove the "Entity" Properties and backing fields
                        var property = @class.GetAllProperties()
                            .FirstOrDefault(x => x.GetMetadata<IMetadataModel>("model").Id == navigation.Id);
                        @class.Properties.Remove(property);

                        var field = @class.Fields
                            .FirstOrDefault(x => x.GetMetadata<IMetadataModel>("model").Id == navigation.Id);
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

                        if (!model.IsAggregateRoot() || initializePrimaryKeyOnAggregateRoots)
                        {
                            InitializePrimaryKey(template, @class, attribute, existingPk, fieldName);
                        }

                        primaryKeyProperties.Add(existingPk);
                    }

                    if (!@class.TryGetMetadata("primary-keys", out _))
                    {
                        @class.AddMetadata("primary-keys", primaryKeyProperties.ToArray());
                    }
                }));
            }
        }

        private static void InitializePrimaryKey(ICSharpTemplate template, CSharpClass @class, AttributeModel attributePk, CSharpProperty existingPk, string fieldName)
        {
            @class.AddField(template.GetTypeName(attributePk.TypeReference) + "?", fieldName);
            var getExpressionSuffix = attributePk.TypeReference.Element.Name switch
            {
                "string" => $" ??= {template.UseType("System.Guid")}.NewGuid().ToString()",
                "guid" => $" ??= {template.UseType("System.Guid")}.NewGuid()",
                "int" or "long" => $"?? throw new {template.UseType("System.NullReferenceException")}(\"{fieldName} has not been set\")",
                _ => string.Empty
            };

            existingPk.Getter.WithExpressionImplementation($"{fieldName}{getExpressionSuffix}");
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