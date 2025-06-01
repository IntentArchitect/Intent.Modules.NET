using System.Linq;
using Intent.Engine;
using Intent.Modelers.Domain.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Plugins;
using Intent.Modules.Common.Templates;
using Intent.Modules.Constants;
using Intent.Modules.EntityFrameworkCore.DiffAudit.Templates.DiffAuditInterface;
using Intent.Plugins.FactoryExtensions;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.FactoryExtension", Version = "1.0")]

namespace Intent.Modules.EntityFrameworkCore.DiffAudit.FactoryExtensions
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class DiffAuditExtension : FactoryExtensionBase
    {
        public override string Id => "Intent.EntityFrameworkCore.DiffAudit.DiffAuditExtension";

        [IntentManaged(Mode.Ignore)]
        public override int Order => 0;

        protected override void OnAfterTemplateRegistrations(IApplication application)
        {
            var entityStateTemplates = application.FindTemplateInstances<ICSharpFileBuilderTemplate>(TemplateDependency.OnTemplate(TemplateRoles.Domain.Entity.Primary));
            foreach (var template in entityStateTemplates)
            {
                if (!template.TryGetModel<ClassModel>(out var templateModel) ||
                    !templateModel.HasStereotype("Diff Audit"))
                {
                    continue;
                }
                template.CSharpFile.OnBuild(file =>
                {
                    var @class = file.Classes.FirstOrDefault();

                    if (@class.TryGetMetadata<ClassModel>("model", out var model))
                    {
                        @class.ImplementsInterface(template.GetTypeName(DiffAuditInterfaceTemplate.TemplateId));
                    }
                });
            }
        }
    }
}