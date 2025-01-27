using System;
using System.Collections.Generic;
using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.Entities.Templates.NotFoundException
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class NotFoundExceptionTemplate : CSharpTemplateBase<object>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.Entities.NotFoundException";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public NotFoundExceptionTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddUsing("System")
                .AddClass($"NotFoundException", @class =>
                {
                    @class.WithBaseType("Exception");
                    @class.AddConstructor();
                    @class.AddConstructor(ctor =>
                    {
                        ctor.AddParameter("string", "message");
                        ctor.CallsBase(b => b.AddArgument("message"));
                    });
                    @class.AddConstructor(ctor =>
                    {
                        ctor.AddParameter("string", "message");
                        ctor.AddParameter("Exception", "innerException");
                        ctor.CallsBase(b => b.AddArgument("message").AddArgument("innerException"));
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