using System;
using System.Collections.Generic;
using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.DependencyInjection;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.Dapr.AspNetCore.StateManagement.Templates.DaprStateStoreUnitOfWorkInterface;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.Dapr.AspNetCore.StateManagement.Templates.DaprStateStoreUnitOfWork
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class DaprStateStoreUnitOfWorkTemplate : CSharpTemplateBase<object>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.Dapr.AspNetCore.StateManagement.DaprStateStoreUnitOfWork";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public DaprStateStoreUnitOfWorkTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddUsing("System")
                .AddUsing("System.Collections.Concurrent")
                .AddUsing("System.Threading")
                .AddUsing("System.Threading.Tasks")
                .AddClass($"DaprStateStoreUnitOfWork", @class =>
                {
                    @class
                        .ImplementsInterface(this.GetDaprStateStoreUnitOfWorkInterfaceName())
                        .AddField("ConcurrentQueue<Func<CancellationToken, Task>>", "_actions", field => field
                            .PrivateReadOnly()
                            .WithAssignment("new()")
                        )
                        .AddMethod("void", "Enqueue", method => method
                            .AddParameter("Func<CancellationToken, Task>", "action")
                            .AddStatement("_actions.Enqueue(action);")
                        )
                        .AddMethod("Task", "SaveChangesAsync", method => method
                            .Async()
                            .AddParameter("CancellationToken", "cancellationToken", parameter => parameter.WithDefaultValue("default"))
                            .AddStatementBlock("while (_actions.TryDequeue(out var action))", block => block
                                .AddStatement("await action(cancellationToken);")
                            )
                        )
                        ;
                });
        }

        public override void BeforeTemplateExecution()
        {
            base.BeforeTemplateExecution();

            var interfaceTemplate = Project.FindTemplateInstance<IClassProvider>(DaprStateStoreUnitOfWorkInterfaceTemplate.TemplateId);

            ExecutionContext.EventDispatcher.Publish(ContainerRegistrationRequest.ToRegister(this)
                .ForConcern("Infrastructure")
                .WithPerServiceCallLifeTime());

            ExecutionContext.EventDispatcher.Publish(ContainerRegistrationRequest.ToRegister(this)
                .ForConcern("Infrastructure")
                .WithPerServiceCallLifeTime()
                .WithResolveFromContainer()
                .ForInterface(interfaceTemplate));
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