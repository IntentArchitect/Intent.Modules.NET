using System.Collections.Generic;
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

namespace Intent.Modules.Entities.Templates.DataContract
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class DataContractTemplate : CSharpTemplateBase<DataContractModel>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.Entities.DataContract";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public DataContractTemplate(IOutputTarget outputTarget, DataContractModel model) : base(TemplateId, outputTarget, model)
        {
            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddRecord($"{Model.Name}", record =>
                {
                    if (Model.BaseType != null)
                    {
                        record.WithBaseType(Model.BaseType.Element.Name);
                    }

                    record.AddConstructor(ctor =>
                    {
                        foreach (var attribute in Model.Attributes)
                        {
                            ctor.AddParameter(GetTypeName(attribute), attribute.Name.ToCamelCase(), param =>
                            {
                                param.IntroduceProperty(prop => prop.Init());
                            });
                        }
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