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
using Intent.Plugins.FactoryExtensions;
using Intent.RoslynWeaver.Attributes;

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

                                @class.InsertProperty(0, template.UseType(typeName), attribute.Name, property =>
                                {
                                    primaryKeyProperties.Add(property);
                                });
                            }
                            else
                            {
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
                    }

                    if (!@class.TryGetMetadata("primary-keys", out var keys))
                    {
                        @class.AddMetadata("primary-keys", primaryKeyProperties.ToArray());
                    }
                });
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