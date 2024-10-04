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

namespace Intent.Modules.FastEndpoints.Templates.JsonResponse
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class JsonResponseTemplate : CSharpTemplateBase<object>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.FastEndpoints.JsonResponseTemplate";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public JsonResponseTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddClass($"JsonResponse", @class =>
                {
                    @class.WithComments(
                        """
                        /// <summary>
                        /// Implicit wrapping of types that serialize to non-complex values.
                        /// </summary>
                        /// <typeparam name="T">Types such as string, Guid, int, long, etc.</typeparam>
                        """);
                    @class.AddGenericParameter("T", out var T);

                    @class.AddConstructor(ctor => ctor
                        .AddParameter(T, "value")
                        .AddStatement("Value = value;"));

                    @class.AddProperty(T, "Value");
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