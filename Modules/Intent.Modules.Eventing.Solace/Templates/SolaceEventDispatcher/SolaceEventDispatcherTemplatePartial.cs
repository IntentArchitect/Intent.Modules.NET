using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.Eventing.Contracts.Templates;
using Intent.Modules.UnitOfWork.Persistence.Shared;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.Eventing.Solace.Templates.SolaceEventDispatcher
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class SolaceEventDispatcherTemplate : CSharpTemplateBase<object>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.Eventing.Solace.SolaceEventDispatcher";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public SolaceEventDispatcherTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {

            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddUsing("System")
                .AddUsing("System.Threading")
                .AddUsing("System.Threading.Tasks")
                .AddUsing("System.Transactions")
                .AddClass($"SolaceEventDispatcher", @class =>
                {
                    @class.AddGenericParameter("T", out var t);
                    @class.AddGenericTypeConstraint(t, c => c.AddType("class"));
                    @class.ImplementsInterface($"{this.GetSolaceEventDispatcherInterfaceName()}<{t}>");

                    @class.AddConstructor(ctor =>
                    {
                        ctor.AddParameter(this.GetEventBusInterfaceName(), "eventBus", p => p.IntroduceReadonlyField());
                        ctor.AddParameter($"{this.GetIntegrationEventHandlerInterfaceName()}<{t}>", "handler", p => p.IntroduceReadonlyField());
                    });

                    @class.AddMethod("void", "Dispatch", method =>
                    {
                        method.Async();
                        method.AddParameter(t, "message");
                        method.AddOptionalCancellationTokenParameter();

                        method.ApplyUnitOfWorkImplementations(
                            template: this,
                            constructor: @class.Constructors.First(),
                            invocationStatement: "await _handler.HandleAsync(message, cancellationToken);",
                            allowTransactionScope: true,
                            cancellationTokenExpression: "cancellationToken");

                        method.AddStatement("await _eventBus.FlushAllAsync(cancellationToken);", s => s.SeparatedFromPrevious());
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