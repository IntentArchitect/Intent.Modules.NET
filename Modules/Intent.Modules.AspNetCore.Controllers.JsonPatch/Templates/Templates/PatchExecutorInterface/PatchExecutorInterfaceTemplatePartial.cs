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

namespace Intent.Modules.AspNetCore.Controllers.JsonPatch.Templates.Templates.PatchExecutorInterface
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class PatchExecutorInterfaceTemplate : CSharpTemplateBase<object>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.AspNetCore.Controllers.JsonPatch.Templates.PatchExecutorInterface";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public PatchExecutorInterfaceTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddUsing("System.Threading")
                .AddUsing("System.Threading.Tasks")
                .AddInterface($"IPatchExecutor", @interface =>
                {
                    @interface.WithComments(
                        """
                        /// <summary>
                        /// Defines abstraction for applying and (if applicable) validating JSON Merge Patch operations.
                        /// </summary>
                        /// <typeparam name="T">The type of object to apply the patch to.</typeparam>
                        """);
                    @interface.AddGenericParameter("T", out var T);

                    @interface.AddMethod("Task", "ApplyToAsync", method =>
                    {
                        method.WithComments("""
                                            /// <summary>
                                            /// Applies the patch to the target object and (if applicable) validates the result asynchronously.
                                            /// </summary>
                                            /// <param name="target">The object to apply the patch to.</param>
                                            /// <param name="cancellationToken">A token to cancel the operation.</param>
                                            """);
                        method.AddParameter(T, "target");
                        method.AddParameter("CancellationToken", "cancellationToken", param => param.WithDefaultValue("default"));
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