using System;
using System.Linq;
using Intent.Engine;
using Intent.Metadata.DocumentDB.Api;
using Intent.Metadata.DocumentDB.Api.Extensions;
using Intent.Metadata.Models;
using Intent.Modelers.Domain.Api;
using Intent.Modules.Azure.TableStorage.Templates;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Plugins;
using Intent.Modules.Common.Templates;
using Intent.Modules.Constants;
using Intent.Plugins.FactoryExtensions;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.FactoryExtension", Version = "1.0")]

namespace Intent.Modules.Azure.TableStorage.FactoryExtensions
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class EntityFactoryExtension : FactoryExtensionBase
    {
        public override string Id => "Intent.Azure.TableStorage.EntityFactoryExtension";

        [IntentManaged(Mode.Ignore)]
        public override int Order => 0;
        protected override void OnAfterTemplateRegistrations(IApplication application)
        {
            RegisterServices(application);

        }

        private static void RegisterServices(IApplication application)
        {
            var template = application.FindTemplateInstance<ICSharpFileBuilderTemplate>(TemplateRoles.Infrastructure.DependencyInjection);
            if (template is null)
            {
                return;
            }

            template.CSharpFile.OnBuild(file =>
            {
                file.AddUsing("Microsoft.Extensions.DependencyInjection");
                var method = file.Classes.First().FindMethod("AddInfrastructure");
                method.AddStatement($"services.AddScoped<{template.UseType("Azure.Data.Tables.TableServiceClient")}>(provider => new TableServiceClient(configuration[\"TableStorageConnectionString\"]));");
            });

            // this entire block is to update domain entities with default values
            // if the "Auto-Generated" PK property has been set
            var classModels = application.MetadataManager.Domain(application).GetClassModels().Where(TableStorageProvider.FilterDbProvider);
            foreach (var classModel in classModels)
            {
                // if any of the class models have a PK which is auto generated
                if (classModel.Attributes.Any(a => a.HasPrimaryKey() && a.GetPrimaryKey().DataSource().AsEnum() != AttributeModelStereotypeExtensions.PrimaryKey.DataSourceOptionsEnum.UserSupplied))
                {
                    // find the domain entity for the class model
                    var entityTemplate = application.FindTemplateInstance<ICSharpFileBuilderTemplate>(TemplateRoles.Domain.Entity.Primary, classModel);
                    entityTemplate.CSharpFile.AfterBuild(file =>
                    {
                        if (file.Template.TryGetModel<ClassModel>(out var model))
                        {
                            var @class = file.Classes.FirstOrDefault();
                            if (@class is not null)
                            {
                                // only if the attribute is a PK, is not "user supplied" and doesn't already have a PK
                                foreach (var attribute in model.Attributes.Where(a => a.HasPrimaryKey()
                                    && a.GetPrimaryKey().DataSource().AsEnum() != AttributeModelStereotypeExtensions.PrimaryKey.DataSourceOptionsEnum.UserSupplied
                                    && string.IsNullOrWhiteSpace(a.Value)))
                                {
                                    // get the c# property which matches the attribute on the model
                                    var properties = @class.Properties.Where(a => a.HasMetadata("model")
                                        && a.TryGetMetadata<AttributeModel>("model", out var metaAttribute) && metaAttribute.Id == attribute.Id);

                                    if (properties != null && properties.Any())
                                    {
                                        file.AddUsing("System");
                                    }

                                    // should really only be one
                                    foreach (var attr in properties)
                                    {
                                        attr.WithInitialValue("Guid.NewGuid().ToString()");
                                    }

                                    var constructor = @class.Constructors.FirstOrDefault(c => c.Parameters.Count == 0);
                                    if (constructor is not null)
                                    {
                                        var removeStatement = constructor.Statements.FirstOrDefault(s => s.Text == $"{attribute.Name.ToPascalCase()} = null!;");
                                        if (removeStatement is not null)
                                        {
                                            constructor.Statements.Remove(removeStatement);
                                        }
                                    }
                                }
                            }
                        }
                    });
                }
            }
        }
    }
}