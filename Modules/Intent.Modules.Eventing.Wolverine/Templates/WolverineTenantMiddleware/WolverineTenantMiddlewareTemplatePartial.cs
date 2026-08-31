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

namespace Intent.Modules.Eventing.Wolverine.Templates.WolverineTenantMiddleware
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class WolverineTenantMiddlewareTemplate : CSharpTemplateBase<object>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.Eventing.Wolverine.WolverineTenantMiddleware";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public WolverineTenantMiddlewareTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddUsing("System.Threading.Tasks")
                .AddUsing("Wolverine")
                .AddUsing("Finbuckle.MultiTenant.Abstractions")
                .AddClass("WolverineTenantMiddleware", @class =>
                {
                    @class.Static();

                    // R12.2/R12.4: Wolverine discovers BeforeAsync middleware by naming convention on
                    // a plain static class (Wolverine.Middleware), not through an interface - matching
                    // the shape cited in
                    // intent/.specs/wolverine-eventing-module/golden-sample/probes/DurableAndTransportProbe/Probe.cs
                    // (TenancyMiddlewareProbe). A message with no tenant id (R12.4) leaves the ambient
                    // context untouched and proceeds - it is never rejected.
                    // No FinallyAsync: restoring the pre-message context has no consumer requirement,
                    // and the plumbing to carry it (an IMultiTenantContext? return value threaded
                    // through to a Finally step) is cost without a caller - dropped rather than kept
                    // "just in case".
                    @class.AddMethod("Task", "BeforeAsync", method =>
                    {
                        method.Static().Async();
                        method.AddParameter("Envelope", "envelope");
                        method.AddParameter("ITenantResolver", "tenantResolver");
                        method.AddParameter("IMultiTenantContextSetter", "contextSetter");

                        method.AddStatement("if (string.IsNullOrEmpty(envelope.TenantId)) return;");
                        method.AddStatement("contextSetter.MultiTenantContext = await tenantResolver.ResolveAsync(envelope);", s => s.SeparatedFromPrevious());
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

        /// <summary>
        /// Gated on Finbuckle multi-tenancy being installed, same as
        /// <see cref="Templates.WolverineTenantStrategy.WolverineTenantStrategyTemplate"/>.
        /// </summary>
        public override bool CanRunTemplate()
        {
            return base.CanRunTemplate() &&
                GetTemplate<object>("Intent.Modules.AspNetCore.MultiTenancy.MultiTenancyConfiguration", new TemplateDiscoveryOptions { ThrowIfNotFound = false, TrackDependency = false }) != null;
        }
    }
}
