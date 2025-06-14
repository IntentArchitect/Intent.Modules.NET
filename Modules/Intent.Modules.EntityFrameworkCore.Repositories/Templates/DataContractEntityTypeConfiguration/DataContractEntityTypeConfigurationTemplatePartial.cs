using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Modelers.Domain.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.EntityFrameworkCore.Repositories.Templates.DataContractEntityTypeConfiguration
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class DataContractEntityTypeConfigurationTemplate : CSharpTemplateBase<DataContractModel>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.EntityFrameworkCore.Repositories.DataContractEntityTypeConfiguration";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public DataContractEntityTypeConfigurationTemplate(IOutputTarget outputTarget, DataContractModel model) : base(TemplateId, outputTarget, model)
        {
            AddTypeSource("Domain.Entity");
            AddTypeSource("Domain.ValueObject");
            AddTypeSource("Intent.Entities.DomainEnum");

            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddUsing("Microsoft.EntityFrameworkCore")
                .AddUsing("Microsoft.EntityFrameworkCore.Metadata.Builders")
                .AddClass($"{Model.Name}Configuration", @class =>
                {
                    IIntentTemplate entityTemplate = GetTemplate<IIntentTemplate>("Intent.Entities.DataContract", Model);

                    @class.ImplementsInterface($"IEntityTypeConfiguration<{GetTypeName(entityTemplate)}>")
                        .AddMethod("void", "Configure", method =>
                        {
                            method.AddMetadata("model", Model.InternalElement);
                            method.AddParameter($"EntityTypeBuilder<{GetTypeName(entityTemplate)}>", "builder");
                        });
                });
        }

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