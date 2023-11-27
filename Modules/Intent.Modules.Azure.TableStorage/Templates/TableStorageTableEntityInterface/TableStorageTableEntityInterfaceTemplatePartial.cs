using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modelers.Domain.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.CSharp.TypeResolvers;
using Intent.Modules.Common.Templates;
using Intent.Modules.Constants;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.Azure.TableStorage.Templates.TableStorageTableEntityInterface
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class TableStorageTableEntityInterfaceTemplate : CSharpTemplateBase<ClassModel>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.Azure.TableStorage.TableStorageTableEntityInterface";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public TableStorageTableEntityInterfaceTemplate(IOutputTarget outputTarget, ClassModel model) : base(TemplateId, outputTarget, model)
        {
            SetDefaultCollectionFormatter(CSharpCollectionFormatter.Create("System.Collections.Generic.IReadOnlyList<{0}>"));
            AddTypeSource(TemplateId);
            AddTypeSource(TemplateRoles.Domain.Enum);
            AddTypeSource(TemplateRoles.Domain.Entity.Primary);

            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddInterface($"I{Model.Name}TableEntity")
                .OnBuild(file =>
                {
                    var @interface = file.Interfaces.First();
                    var entityPropertyIds = EntityStateFileBuilder.CSharpFile.Classes.First().Properties
                        .Select(x => x.TryGetMetadata<IMetadataModel>("model", out var metadataModel) && metadataModel is AttributeModel
                                ? metadataModel.Id
                                : null)
                        .Where(x => x != null)
                        .ToHashSet();

                    var attributes = Model.Attributes
                        .Where(x => entityPropertyIds.Contains(x.Id))
                        .ToList();

                    foreach (var attribute in attributes)
                    {
                        @interface.AddProperty(GetTypeName(attribute.TypeReference), attribute.Name.ToPascalCase(), p => p.WithoutSetter());
                    }

                }, 1000);
        }

        public ICSharpFileBuilderTemplate EntityStateFileBuilder => GetTemplate<ICSharpFileBuilderTemplate>(TemplateRoles.Domain.Entity.Primary, Model);

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