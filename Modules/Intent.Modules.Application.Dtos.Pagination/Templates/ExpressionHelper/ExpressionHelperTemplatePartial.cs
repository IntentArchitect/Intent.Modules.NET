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

namespace Intent.Modules.Application.Dtos.Pagination.Templates.ExpressionHelper
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class ExpressionHelperTemplate : CSharpTemplateBase<object>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.Application.Dtos.Pagination.ExpressionHelper";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public ExpressionHelperTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddClass($"ExpressionHelper", @class =>
                {
                    AddUsing("System.Linq.Expressions");
                    @class.Static();

                    @class.AddMethod($"Expression<{UseType("System.Func<T, bool>")}>", "Combine", method =>
                    {
                        method.Static();
                        method.AddGenericParameter("T");

                        method.AddParameter("Expression<Func<T, bool>>", "first", prm =>
                        {
                            prm.WithThisModifier();
                        });
                        method.AddParameter("Expression<Func<T, bool>>", "second");

                        method.AddObjectInitStatement("var param", "Expression.Parameter(typeof(T));");
                        method.AddObjectInitStatement("var body", "Expression.AndAlso(Expression.Invoke(first, param), Expression.Invoke(second, param));");

                        method.AddReturn("Expression.Lambda<Func<T, bool>>(body, param)");
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