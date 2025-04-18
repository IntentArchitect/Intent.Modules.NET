using System;
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
            InstallParameterlessCtorInEntities(application);
            InstallParameterlessCtorInDataContracts(application);
        }

        private static void InstallParameterlessCtorInEntities(IApplication application)
        {
            var classModels = application.MetadataManager.Domain(application).GetClassModels()
                .Where(x => x.InternalElement.Package.AsDomainPackageModel()?.HasRelationalDatabase() == true)
                .ToArray();

            foreach (var model in classModels)
            {
                var entityTemplate = application.FindTemplateInstance<ICSharpFileBuilderTemplate>(TemplateRoles.Domain.Entity.EntityImplementation, model.Id);

                entityTemplate?.CSharpFile.OnBuild(file =>
                {
                    var stateTemplate = application.FindTemplateInstance<ICSharpFileBuilderTemplate>(TemplateRoles.Domain.Entity.State, model.Id);

                    var entityImplCtors = file.Classes.First().Constructors;
                    var stateCtors = stateTemplate?.CSharpFile.Classes.First().Constructors ?? ArraySegment<CSharpConstructor>.Empty;
                    var collectiveCtors = entityImplCtors.Concat(stateCtors).ToArray();

                    if (collectiveCtors.Length == 0 || collectiveCtors.Any(x => !x.Parameters.Any()))
                    {
                        return;
                    }

                    if (stateTemplate is null)
                    {
                        var entityClass = file.Classes.First();
                        IntroduceClassConstructor(entityClass, model, entityTemplate);
                    }
                    else
                    {
                        var stateClass = stateTemplate.CSharpFile.Classes.First();
                        IntroduceClassConstructor(stateClass, model, stateTemplate);
                    }
                });
            }
        }

        private static void InstallParameterlessCtorInDataContracts(IApplication application)
        {
            var dataContractModels = application.MetadataManager.Domain(application).GetDataContractModels()
                .Where(x => x.InternalElement.Package.AsDomainPackageModel()?.HasRelationalDatabase() == true)
                .ToArray();
            foreach (var model in dataContractModels)
            {
                var dataContractTemplate = application.FindTemplateInstance<ICSharpFileBuilderTemplate>(TemplateRoles.Domain.DataContract, model.Id);
                dataContractTemplate?.CSharpFile.OnBuild(file =>
                {
                    var record = file.Records.First();
                    if (record.Constructors.Count == 0 || record.Constructors.Any(p => p.Parameters.Count == 0))
                    {
                        return;
                    }

                    IntroduceRecordConstructor(record, model, dataContractTemplate);
                });
            }
        }

        private static void IntroduceClassConstructor(CSharpClass entityClass, ClassModel model, ICSharpFileBuilderTemplate primaryTemplate)
        {
            entityClass.AddConstructor(ctor =>
            {
                ctor.WithComments([
                    "/// <summary>",
                    "/// Required by Entity Framework.",
                    "/// </summary>"
                ]);
                ctor.AddAttribute(CSharpIntentManagedAttribute.Fully());
                ctor.Protected();
                foreach (var attribute in model.Attributes)
                {
                    if (!string.IsNullOrWhiteSpace(attribute.Value))
                    {
                        continue;
                    }

                    var typeInfo = primaryTemplate.GetTypeInfo(attribute.TypeReference);
                    if (NeedsNullabilityAssignment(typeInfo))
                    {
                        ctor.AddStatement($"{attribute.Name} = null!;");
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
        }

        private static void IntroduceRecordConstructor(CSharpRecord record, DataContractModel model, ICSharpFileBuilderTemplate dataContractTemplate)
        {
            record.AddConstructor(ctor =>
            {
                ctor.WithComments([
                    "/// <summary>",
                    "/// Required by Entity Framework.",
                    "/// </summary>"
                ]);
                ctor.AddAttribute(CSharpIntentManagedAttribute.Fully());
                ctor.Protected();
                foreach (var attribute in model.Attributes)
                {
                    if (!string.IsNullOrWhiteSpace(attribute.Value))
                    {
                        continue;
                    }

                    var typeInfo = dataContractTemplate.GetTypeInfo(attribute.TypeReference);
                    if (NeedsNullabilityAssignment(typeInfo))
                    {
                        ctor.AddStatement($"{attribute.Name} = null!;");
                    }
                }
            });
        }

        private static bool NeedsNullabilityAssignment(IResolvedTypeInfo typeInfo)
        {
            return !(typeInfo.IsPrimitive
                     || typeInfo.IsNullable == true
                     || (typeInfo.TypeReference != null && typeInfo.TypeReference.Element.IsEnumModel()));
        }
    }
}