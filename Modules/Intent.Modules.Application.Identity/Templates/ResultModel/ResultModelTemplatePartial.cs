using System;
using System.Collections.Generic;
using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.CSharp.VisualStudio;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.Application.Identity.Templates.ResultModel
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public partial class ResultModelTemplate : CSharpTemplateBase<object>, ICSharpFileBuilderTemplate
    {
        [IntentManaged(Mode.Fully)]
        public const string TemplateId = "Intent.Application.Identity.ResultModel";

        [IntentManaged(Mode.Ignore, Signature = Mode.Fully)]
        public ResultModelTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            AddUsing("System.Linq");

            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddClass($"Result", @class =>
                {
                    @class.AddConstructor(ctor =>
                    {
                        ctor.Internal();
                        ctor.AddParameter("bool", "succeeded", param =>
                        {
                            param.IntroduceProperty();
                        });
                        ctor.AddParameter(UseType("System.Collections.Generic.IEnumerable<string>"), "errors");
                        ctor.AddFieldAssignmentStatement("Errors", "errors.ToArray()");
                    });

                    @class.AddProperty("string[]", "Errors");

                    @class.AddMethod("Result", "Success", method =>
                    {
                        method.Static();
                        if (outputTarget.GetProject().GetLanguageVersion().Major < 12)
                        {
                            AddUsing("System");
                            method.AddReturn($"new Result(true, Array.Empty<string>())");
                        }
                        else
                        {
                            method.AddReturn("new Result(true, [])");
                        }
                    });

                    @class.AddMethod("Result", "Failure", method =>
                    {
                        method.AddParameter("IEnumerable<string>", "errors");

                        method.Static();
                        method.AddReturn("new Result(false, errors)");
                    });
                });
        }

        [IntentManaged(Mode.Fully)]
        public CSharpFile CSharpFile { get; }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        protected override CSharpFileConfig DefineFileConfig()
        {
            return new CSharpFileConfig(
                className: $"Result",
                @namespace: $"{OutputTarget.GetNamespace()}");
        }

        [IntentManaged(Mode.Fully)]
        public override string TransformText()
        {
            return CSharpFile.ToString();
        }

    }
}