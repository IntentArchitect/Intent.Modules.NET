using System.Linq;
using Intent.Engine;
using Intent.Modelers.Domain.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Plugins;
using Intent.Modules.Common.Templates;
using Intent.Modules.Common.TypeResolution;
using Intent.Modules.Common.Types.Api;
using Intent.Modules.Constants;
using Intent.Plugins.FactoryExtensions;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.FactoryExtension", Version = "1.0")]

namespace Intent.Modules.EntityFrameworkCore.FactoryExtensions
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class EnsureParameterlessConstructorsExtension : FactoryExtensionBase
    {
        public override string Id => "Intent.EntityFrameworkCore.EnsureParameterlessConstructorsExtension";

        [IntentManaged(Mode.Ignore)]
        public override int Order => 0;

        protected override void OnAfterTemplateRegistrations(IApplication application)
        {
            foreach (var model in application.MetadataManager.Domain(application).GetClassModels())
            {
                var template = application.FindTemplateInstance<ICSharpFileBuilderTemplate>(TemplateFulfillingRoles.Domain.Entity.Primary, model.Id);
                if (template is null)
                {
                    continue;
                }

                template.CSharpFile.OnBuild(file =>
                {
                    var entityClass = file.Classes.First();
                    if (!entityClass.Constructors.Any() ||
                        entityClass.Constructors.Any(x => !x.Parameters.Any()))
                    {
                        return;
                    }

                    entityClass.AddConstructor(ctor =>
                    {
                        ctor.WithComments(new[]
                        {
                            "/// <summary>",
                            "/// Required by Entity Framework.",
                            "/// </summary>"
                        });
                        ctor.AddAttribute(CSharpIntentManagedAttribute.Fully());
                        ctor.Protected();
                        foreach (var attribute in model.Attributes)
                        {
                            if (!string.IsNullOrWhiteSpace(attribute.Value))
                            {
                                continue;
                            }

                            var typeInfo = template.GetTypeInfo(attribute.TypeReference);
                            if (NeedsNullabilityAssignment(typeInfo))
                            {
                                ctor.AddStatement($"{attribute.Name.ToPascalCase()} = null!;");
                            }
                        }

                        foreach (var associationEnd in model.AssociatedClasses.Where(x => x.IsNavigable))
                        {
                            if (!associationEnd.IsCollection && !associationEnd.IsNullable)
                            {
                                ctor.AddStatement($"{associationEnd.Name.ToPascalCase()} = null!;");
                            }
                        }
                    });
                });
            }
        }

        private static bool NeedsNullabilityAssignment(IResolvedTypeInfo typeInfo)
        {
            return !(typeInfo.IsPrimitive
                     || typeInfo.IsNullable == true
                     || (typeInfo.TypeReference != null && typeInfo.TypeReference.Element.IsEnumModel()));
        }
    }
}