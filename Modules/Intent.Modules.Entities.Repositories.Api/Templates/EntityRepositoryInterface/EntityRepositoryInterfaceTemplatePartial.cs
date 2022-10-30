using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Metadata.RDBMS.Api;
using Intent.Modelers.Domain.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.Common.VisualStudio;
using Intent.Modules.Constants;
using Intent.Modules.Entities.Repositories.Api.Templates.RepositoryInterface;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]
[assembly: DefaultIntentManaged(Mode.Fully)]

namespace Intent.Modules.Entities.Repositories.Api.Templates.EntityRepositoryInterface
{
    [IntentManaged(Mode.Merge, Body = Mode.Merge)]
    public partial class EntityRepositoryInterfaceTemplate : CSharpTemplateBase<ClassModel>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.Entities.Repositories.Api.EntityRepositoryInterface";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public EntityRepositoryInterfaceTemplate(IOutputTarget outputTarget, ClassModel model)
            : base(TemplateId, outputTarget, model)
        {
            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddUsing("System")
                .AddUsing("System.Linq")
                .AddUsing("System.Threading")
                .AddUsing("System.Threading.Tasks")
                .AddUsing("System.Collections.Generic")
                .AddInterface($"I{Model.Name}Repository", @interface =>
                {
                    @interface.AddAttribute("[IntentManaged(Mode.Merge, Signature = Mode.Fully)]");
                    @interface.ExtendsInterface($"{RepositoryInterfaceName}<{EntityInterfaceName}, {EntityStateName}>");

                    if (TryGetTemplate<ICSharpFileBuilderTemplate>(TemplateFulfillingRoles.Domain.Entity.Primary, Model, out var entityTemplate))
                    {
                        entityTemplate.CSharpFile.AfterBuild(file =>
                        {
                            var entityClass = file.Classes.First();
                            if (entityClass.TryGetMetadata<CSharpProperty[]>("primary-keys", out var pks)
                                && pks.Length == 1)
                            {
                                @interface.AddMethod($"Task<{GetTypeName(TemplateFulfillingRoles.Domain.Entity.Interface, Model)}>", "FindByIdAsync", method =>
                                {
                                    method.AddAttribute("[IntentManaged(Mode.Fully)]");
                                    var pk = pks.First();
                                    method.AddParameter(entityTemplate.UseType(pk.Type), pk.Name.ToCamelCase());
                                    method.AddParameter("CancellationToken", "cancellationToken", param => param.WithDefaultValue("default"));
                                });
                                @interface.AddMethod($"Task<List<{GetTypeName(TemplateFulfillingRoles.Domain.Entity.Interface, Model)}>>", "FindByIdsAsync", method =>
                                {
                                    method.AddAttribute("[IntentManaged(Mode.Fully)]");
                                    var pk = pks.First();
                                    method.AddParameter($"{entityTemplate.UseType(pk.Type)}[]", pk.Name.ToCamelCase().Pluralize());
                                    method.AddParameter("CancellationToken", "cancellationToken", param => param.WithDefaultValue("default"));
                                });
                            }
                        }, 4000);
                    }
                });
        }

        public CSharpFile CSharpFile { get; }

        public string RepositoryInterfaceName => GetTypeName(RepositoryInterfaceTemplate.TemplateId);

        public string EntityStateName => GetTypeName("Domain.Entity", Model);

        public string EntityInterfaceName => GetTypeName("Domain.Entity.Interface", Model);

        public string PrimaryKeyType => GetTemplate<ITemplate>("Domain.Entity", Model).GetMetadata().CustomMetadata.TryGetValue("Surrogate Key Type", out var type) ? UseType(type) : UseType("System.Guid");

        public string PrimaryKeyName => Model.Attributes.FirstOrDefault(x => x.HasStereotype("Primary Key"))?.Name.ToPascalCase() ?? "Id";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        protected override CSharpFileConfig DefineFileConfig()
        {
            return new CSharpFileConfig(
                className: $"I{Model.Name}Repository",
                @namespace: $"{this.GetNamespace()}",
                relativeLocation: $"{this.GetFolderPath()}");
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public override string TransformText()
        {
            return CSharpFile.ToString();
        }

    }
}
