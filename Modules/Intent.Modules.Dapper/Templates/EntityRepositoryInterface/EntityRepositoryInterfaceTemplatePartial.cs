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

                    @interface.AddMethod($"Task<{EntityName}?>", "FindByIdAsync", method =>
                    {
                        var pks = model.GetPks();

                        if (pks.Count == 1)
                        {
                            var pk = pks[0];
                            method.AddParameter(GetTypeName(pk.TypeReference), pk.Name.ToCamelCase());
                        }
                        else
                        {
                            method.AddParameter($"({string.Join(", ", pks.Select(pk => $"{GetTypeName(pk)} {pk.Name.ToPascalCase()}"))})", "id");
                        }
                        method
                            .AddParameter("CancellationToken", "cancellationToken", x => x.WithDefaultValue("default"));
                    });
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