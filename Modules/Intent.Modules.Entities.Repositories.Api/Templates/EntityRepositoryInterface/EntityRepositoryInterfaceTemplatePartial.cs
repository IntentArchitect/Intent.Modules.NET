using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Modelers.Domain.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.CSharp.VisualStudio;
using Intent.Modules.Common.Templates;
using Intent.Modules.Constants;
using Intent.Modules.Entities.Repositories.Api.Templates.RepositoryInterface;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

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
                .AddUsing("System.Threading")
                .AddUsing("System.Threading.Tasks")
                .AddUsing("System.Collections.Generic")
                .AddInterface($"I{Model.Name.ToPascalCase()}Repository", @interface =>
                {
                    @interface.AddMetadata("model", model);
                    @interface.AddAttribute("[IntentManaged(Mode.Merge, Signature = Mode.Fully)]");
                    @interface.ExtendsInterface($"{RepositoryInterfaceName}<{EntityInterfaceName}, {EntityStateName}>");
                    foreach (var genericType in Model.GenericTypes)
                    {
                        @interface.AddGenericParameter(genericType);
                    }

                    @interface.RepresentsModel(model);

                    if (TryGetTemplate<ICSharpFileBuilderTemplate>(TemplateRoles.Domain.Entity.Primary, Model, out var entityTemplate))
                    {
                        entityTemplate.CSharpFile.AfterBuild(file =>
                        {
                            var rootEntity = file.Classes.First();
                            while (rootEntity.BaseType != null && !rootEntity.HasMetadata("primary-keys"))
                            {
                                rootEntity = rootEntity.BaseType;
                            }

                            if (rootEntity.TryGetMetadata<CSharpProperty[]>("primary-keys", out var pks))
                            {
                                var genericTypeParameters = model.GenericTypes.Any()
                                    ? $"<{string.Join(", ", model.GenericTypes)}>"
                                    : string.Empty;
                                @interface.AddMethod($"Task<{GetTypeName(TemplateRoles.Domain.Entity.Interface, Model)}{genericTypeParameters}{(OutputTarget.GetProject().NullableEnabled ? "?" : "")}>", "FindByIdAsync", method =>
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
                                    method.AddParameter("CancellationToken", "cancellationToken", param => param.WithDefaultValue("default"));
                                });
                                if (pks.Length == 1)
                                {
                                    @interface.AddMethod($"Task<List<{GetTypeName(TemplateRoles.Domain.Entity.Interface, Model)}{genericTypeParameters}>>", "FindByIdsAsync", method =>
                                    {
                                        method.AddAttribute("[IntentManaged(Mode.Fully)]");
                                        var pk = pks.First();
                                        method.AddParameter($"{entityTemplate.UseType(pk.Type)}[]", pk.Name.ToCamelCase().Pluralize());
                                        method.AddParameter("CancellationToken", "cancellationToken", param => param.WithDefaultValue("default"));
                                    });
                                }
                            }
                        });
                    }
                });
        }

        public CSharpFile CSharpFile { get; }

        public string GenericTypeParameters => Model.GenericTypes.Any()
            ? $"<{string.Join(", ", Model.GenericTypes)}>"
            : string.Empty;

        public string RepositoryInterfaceName => GetTypeName(RepositoryInterfaceTemplate.TemplateId);

        public string EntityStateName => $"{GetTypeName("Domain.Entity", Model)}{GenericTypeParameters}";

        public string EntityInterfaceName => $"{GetTypeName("Domain.Entity.Interface", Model)}{GenericTypeParameters}";

        public string PrimaryKeyType => GetTemplate<ITemplate>("Domain.Entity", Model).GetMetadata().CustomMetadata.TryGetValue("Surrogate Key Type", out var type) ? UseType(type) : UseType("System.Guid");

        public string PrimaryKeyName => Model.Attributes.FirstOrDefault(x => x.HasStereotype("Primary Key"))?.Name.ToPascalCase() ?? "Id";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        protected override CSharpFileConfig DefineFileConfig()
        {
            return new CSharpFileConfig(
                className: $"I{Model.Name.ToPascalCase()}Repository",
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
