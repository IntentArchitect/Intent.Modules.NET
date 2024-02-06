using System;
using System.Collections.Generic;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.Redis.Om.Repositories.Templates.Templates.RedisOmValueObjectDocumentInterface
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class RedisOmValueObjectDocumentInterfaceTemplate : CSharpTemplateBase<IElement>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.Redis.Om.Repositories.Templates.RedisOmValueObjectDocumentInterface";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public RedisOmValueObjectDocumentInterfaceTemplate(IOutputTarget outputTarget, IElement model = null) : base(TemplateId, outputTarget, model)
        {
            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddInterface($"I{Model.Name}", @interface =>
                {
                    @interface.AddMethod("bool", "ExampleMethod", method =>
                    {
                        method.AddParameter("string", "exampleParam");
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