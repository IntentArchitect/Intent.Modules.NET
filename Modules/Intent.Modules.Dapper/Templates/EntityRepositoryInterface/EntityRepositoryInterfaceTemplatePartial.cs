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
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.Dapper.Templates.EntityRepositoryInterface
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class EntityRepositoryInterfaceTemplate : CSharpTemplateBase<ClassModel>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.Dapper.EntityRepositoryInterface";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public EntityRepositoryInterfaceTemplate(IOutputTarget outputTarget, ClassModel model) : base(TemplateId, outputTarget, model)
        {
			CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
				.AddUsing("System")
				.AddUsing("System.Threading")
				.AddUsing("System.Threading.Tasks")
				.AddUsing("System.Collections.Generic")
				.AddInterface($"I{Model.Name.ToPascalCase()}Repository", @interface =>
				{
					FulfillsRole("Repository.Interface.Entity");
					@interface.AddMetadata("model", model);
					@interface.AddMetadata("requires-explicit-update", true);
					@interface.AddAttribute("[IntentManaged(Mode.Merge, Signature = Mode.Fully)]");
					@interface.ExtendsInterface($"{this.GetRepositoryInterfaceName()}<{EntityName}>");
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
		public string EntityName => GetTypeName("Domain.Entity", Model);

		[IntentManaged(Mode.Fully)]
        public CSharpFile CSharpFile { get; }

        [IntentManaged(Mode.Fully)]
        protected override CSharpFileConfig DefineFileConfig()
        {
            return CSharpFile.GetConfig();
        }

        [IntentManaged(Mode.Fully)]
        public override string TransformText()
        {
            return CSharpFile.ToString();
        }
    }
}