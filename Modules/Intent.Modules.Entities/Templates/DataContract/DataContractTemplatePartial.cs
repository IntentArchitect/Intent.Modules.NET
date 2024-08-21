using System;
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
                    record.RepresentsModel(Model);

                    if (Model.BaseDataContract is not null)
                    {
                        record.WithBaseType(GetTypeName(TemplateId, Model.BaseDataContract));
                    }

                    record.AddConstructor(ctor =>
                    {
                        if (Model.BaseDataContract is not null)
                        {
                            foreach (var baseAttribute in Model.BaseDataContract.Attributes)
                            {
                                ctor.AddParameter(GetTypeName(baseAttribute), baseAttribute.Name.ToCamelCase(), param =>
                                {
                                    param.RepresentsModel(baseAttribute);
                                    ctor.CallsBase(x => x.AddArgument(param.Name));
                                });
                            }
                        }

                        foreach (var attribute in Model.Attributes)
                        {
                            ctor.AddParameter(GetTypeName(attribute), attribute.Name.ToCamelCase(), param =>
                            {
                                param.RepresentsModel(attribute);
                                param.IntroduceProperty(prop => prop.Init().RepresentsModel(attribute));
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