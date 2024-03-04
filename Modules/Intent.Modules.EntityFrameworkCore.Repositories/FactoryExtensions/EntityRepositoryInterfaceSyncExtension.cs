using System.Linq;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.CSharp.VisualStudio;
using Intent.Modules.Common.Plugins;
using Intent.Modules.Common.Templates;
using Intent.Modules.Constants;
using Intent.Modules.EntityFrameworkCore.Repositories.Settings;
using Intent.Modules.Metadata.RDBMS.Settings;
using Intent.Plugins.FactoryExtensions;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.FactoryExtension", Version = "1.0")]

namespace Intent.Modules.EntityFrameworkCore.Repositories.FactoryExtensions
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class EntityRepositoryInterfaceSyncExtension : FactoryExtensionBase
    {
        public override string Id => "Intent.EntityFrameworkCore.Repositories.EntityRepositoryInterfaceSyncExtension";

        [IntentManaged(Mode.Ignore)]
        public override int Order => 0;

        protected override void OnAfterTemplateRegistrations(IApplication application)
        {
            var templates = application
                .FindTemplateInstances<ICSharpFileBuilderTemplate>(TemplateDependency.OnTemplate("Intent.Entities.Repositories.Api.EntityRepositoryInterface"))
                .ToArray();
            if (!templates.Any() || !templates.First().ExecutionContext.Settings.GetDatabaseSettings().AddSynchronousMethodsToRepositories())
            {
                return;
            }

            foreach (var template in templates)
            {
                template.CSharpFile.OnBuild(_ =>
                {
                    var @interface = template.CSharpFile.Interfaces.First();
                    var model = @interface.GetMetadata<IMetadataModel>("model");
                    if (!template.TryGetTemplate<ICSharpFileBuilderTemplate>(TemplateRoles.Domain.Entity.Primary, model, out var entityTemplate))
                    {
                        return;
                    }

                    entityTemplate.CSharpFile.AfterBuild(file =>
                    {
                        var rootEntity = file.Classes.First();
                        while (rootEntity.BaseType != null && !rootEntity.HasMetadata("primary-keys"))
                        {
                            rootEntity = rootEntity.BaseType;
                        }

                        if (!rootEntity.TryGetMetadata<CSharpProperty[]>("primary-keys", out var pks))
                        {
                            return;
                        }

                        @interface.AddMethod($"{template.GetTypeName(TemplateRoles.Domain.Entity.Interface, model)}{(template.OutputTarget.GetProject().NullableEnabled ? "?" : "")}", "FindById", method =>
                        {
                            method.AddAttribute("[IntentManaged(Mode.Fully)]");
                            if (pks.Length == 1)
                            {
                                var pk = pks.First();
                                method.AddParameter(entityTemplate.UseType(pk.Type), pk.Name.ToCamelCase());
                            }
                            else
                            {
                                method.AddParameter($"({string.Join(", ", pks.Select(pk => $"{entityTemplate.UseType(pk.Type)} {pk.Name.ToPascalCase()}"))})", "id");
                            }
                        });

                        if (pks.Length == 1)
                        {
                            @interface.AddMethod($"List<{template.GetTypeName(TemplateRoles.Domain.Entity.Interface, model)}>", "FindByIds", method =>
                            {
                                method.AddAttribute("[IntentManaged(Mode.Fully)]");
                                var pk = pks.First();
                                method.AddParameter($"{entityTemplate.UseType(pk.Type)}[]", pk.Name.ToCamelCase().Pluralize());
                            });
                        }
                    });
                });
            }
        }
    }
}