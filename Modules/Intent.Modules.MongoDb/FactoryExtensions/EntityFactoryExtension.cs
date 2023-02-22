using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Metadata.RDBMS.Api;
using Intent.Modelers.Domain.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Plugins;
using Intent.Modules.Common.Templates;
using Intent.Modules.Common.TypeResolution;
using Intent.Modules.Constants;
using Intent.Modules.Metadata.RDBMS.Settings;
using Intent.MongoDb.Api;
using Intent.Plugins.FactoryExtensions;
using Intent.RoslynWeaver.Attributes;
using Intent.Utils;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.FactoryExtension", Version = "1.0")]

namespace Intent.Modules.MongoDb.FactoryExtensions
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class EntityFactoryExtension : FactoryExtensionBase
    {
        public override string Id => "Intent.MongoDb.EntityFactoryExtension";

        [IntentManaged(Mode.Ignore)]
        public override int Order => 0;

        protected override void OnAfterTemplateRegistrations(IApplication application)
        {
            var templates = application.FindTemplateInstances<ICSharpFileBuilderTemplate>(TemplateDependency.OnTemplate(TemplateFulfillingRoles.Domain.Entity.Primary));
            foreach (var template in templates)
            {
                var templateModel = ((CSharpTemplateBase<ClassModel>)template).Model;
                if (!templateModel.InternalElement.Package.IsMongoDomainPackageModel())
                {
                    continue;
                }

                LogAggregateRelationshipsAsError(templateModel);

                template.CSharpFile.OnBuild(file =>
                {
                    var @class = file.Classes.First();
                    var model = @class.GetMetadata<ClassModel>("model");

                    var pks = model.GetExplicitPrimaryKey();
                    var primaryKeyProperties = new List<CSharpProperty>();
                    if (pks.Any())
                    {
                        foreach (var attribute in pks)
                        {
                            var existingPk = @class.GetAllProperties().FirstOrDefault(x => x.Name.Equals(attribute.Name, StringComparison.InvariantCultureIgnoreCase));
                            if (existingPk == null)
                            {
                                var typeName = attribute.Type != null
                                    ? template.GetTypeName(attribute.Type.Element.AsTypeReference(isNullable: attribute.Type.IsNullable, isCollection: attribute.Type.IsCollection))
                                    : GetDefaultSurrogateKeyType(template.ExecutionContext) + (attribute.Type.IsNullable ? "?" : string.Empty);

                                @class.AddField(template.UseType(typeName) + "?", attribute.Name.ToPrivateMemberName());
                                @class.InsertProperty(0, template.UseType(typeName), attribute.Name, property =>
                                {
                                    property.Getter.WithExpressionImplementation($"{attribute.Name.ToPrivateMemberName()} ??= {GetNewSurrogateInstance(template.ExecutionContext)}");
                                    property.Setter.WithExpressionImplementation($"{attribute.Name.ToPrivateMemberName()} = value");
                                    primaryKeyProperties.Add(property);
                                });
                            }
                            else
                            {
                                @class.AddField(template.UseType(existingPk.Type) + "?", existingPk.Name.ToPrivateMemberName());
                                existingPk.Getter.WithExpressionImplementation($"{attribute.Name.ToPrivateMemberName()} ??= {GetNewSurrogateInstance(template.ExecutionContext)}");
                                existingPk.Setter.WithExpressionImplementation($"{attribute.Name.ToPrivateMemberName()} = value");
                                
                                primaryKeyProperties.Add(existingPk);
                            }
                        }
                    }
                    else
                    {
                        @class.InsertProperty(0, template.UseType(GetDefaultSurrogateKeyType(template.ExecutionContext)), "Id", property =>
                        {
                            primaryKeyProperties.Add(property);
                        });
                        
                        @class.AddField(template.UseType(GetDefaultSurrogateKeyType(template.ExecutionContext)) + "?", "_id");
                        @class.InsertProperty(0, GetDefaultSurrogateKeyType(template.ExecutionContext), "Id", property =>
                        {
                            property.Getter.WithExpressionImplementation($"_id ??= {GetNewSurrogateInstance(template.ExecutionContext)}");
                            property.Setter.WithExpressionImplementation($"_id = value");
                            primaryKeyProperties.Add(property);
                        });
                    }

                    if (!@class.TryGetMetadata("primary-keys", out var keys))
                    {
                        @class.AddMetadata("primary-keys", primaryKeyProperties.ToArray());
                    }
                });
            }
        }

        private string GetNewSurrogateInstance(ISoftwareFactoryExecutionContext executionContext)
        {
            var settingType = executionContext.Settings.GetDatabaseSettings()?.KeyType().Value ?? "guid";
            switch (settingType)
            {
                case "guid":
                    return "Guid.NewGuid()";
                case "int":
                    return "new Random().Next()";
                case "long":
                    return "new Random().NextInt64()";
                default:
                    return "default";
            }
        }

        private void LogAggregateRelationshipsAsError(ClassModel model)
        {
            var notSupportedAssociations = model
                .AssociationEnds()
                .Where(x => x.Association.SourceEnd.Element.Id == model.Id &&
                            (x.Association.SourceEnd.IsCollection || x.Association.SourceEnd.IsNullable))
                .ToArray();
            foreach (var association in notSupportedAssociations)
            {
                var sourceClass = model;
                var targetClass = association.Class;
                Logging.Log.Failure($"Association not supported between {sourceClass.Name} and {targetClass.Name}. Ensure the source end is set to not be Is Nullable and not Is Collection.");
            }
        }

        static string GetDefaultSurrogateKeyType(ISoftwareFactoryExecutionContext executionContext)
        {
            var settingType = executionContext.Settings.GetDatabaseSettings()?.KeyType().Value ?? "guid";
            switch (settingType)
            {
                case "guid":
                    return "System.Guid";
                case "int":
                    return "int";
                case "long":
                    return "long";
                default:
                    return settingType;
            }
        }
    }
}