using System;
using System.Collections.Generic;
using Intent.Engine;
using Intent.Modules.Blazor.Settings;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.Blazor.Templates.Templates.Server.ScopedExecutorInterface
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class ScopedExecutorInterfaceTemplate : CSharpTemplateBase<object>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.Blazor.Templates.Server.ScopedExecutorInterfaceTemplate";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public ScopedExecutorInterfaceTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddInterface($"IScopedExecutor", @interface =>
                {
                    @interface.AddMethod("Task", "ExecuteAsync", method =>
                    {
                        method.AddParameter("Func<IServiceProvider, Task>", "action");
                    });
                    @interface.AddMethod("Task<T>", "ExecuteAsync", method =>
                    {
                        method.AddGenericParameter("T");
                        method.AddParameter("Func<IServiceProvider, Task<T>>", "action");
                    });
                    @interface.AddMethod("IAsyncEnumerable<T>", "ExecuteStreamAsync", method =>
                    {
                        method.AddGenericParameter("T");
                        method.AddParameter("Func<IServiceProvider, IAsyncEnumerable<T>>", "action");
                        method.AddParameter("CancellationToken", "cancellationToken", p => p.WithDefaultValue("default"));
                    });
                })
                .AddInterface("ISetCurrentUserContext", @interface =>
                 {
                     @interface.AddMethod("void", "SetContext", method =>
                     {
                         method.AddParameter(UseType("System.Security.Claims.ClaimsPrincipal"), "principal");
                     });
                 })
                ;
        }

        public override bool CanRunTemplate()
        {
            return base.CanRunTemplate() && ExecutionContext.GetSettings().GetBlazor().RenderMode().IsInteractiveServer();
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