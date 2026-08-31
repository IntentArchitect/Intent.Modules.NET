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

namespace Intent.Modules.Eventing.Wolverine.Templates.WolverineTenantStrategy
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class WolverineTenantStrategyTemplate : CSharpTemplateBase<object>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.Eventing.Wolverine.WolverineTenantStrategy";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public WolverineTenantStrategyTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddUsing("System.Threading.Tasks")
                .AddUsing("Wolverine")
                .AddUsing("Finbuckle.MultiTenant.Abstractions")
                .AddClass("WolverineTenantStrategy", @class =>
                {
                    // R12.2: a simple storage mechanism - reads the identifier straight off the
                    // envelope Wolverine itself is already carrying it on (Envelope.TenantId), so
                    // unlike MassTransit's FinbuckleMessageHeaderStrategy this needs no mutable
                    // per-message state primed by a consuming filter. Returns null for any context
                    // that is not an Envelope (e.g. HttpContext), leaving the HTTP header strategy
                    // to answer instead - Finbuckle walks every registered strategy and uses
                    // whichever one returns non-null.
                    @class.ImplementsInterface("IMultiTenantStrategy");

                    @class.AddMethod("Task<string?>", "GetIdentifierAsync", method =>
                    {
                        method.AddParameter("object", "context");
                        method.AddStatement("return Task.FromResult((context as Envelope)?.TenantId);");
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
        /// <see cref="Templates.WolverineTenantMiddleware.WolverineTenantMiddlewareTemplate"/>.
        /// </summary>
        public override bool CanRunTemplate()
        {
            return base.CanRunTemplate() &&
                GetTemplate<object>("Intent.Modules.AspNetCore.MultiTenancy.MultiTenancyConfiguration", new TemplateDiscoveryOptions { ThrowIfNotFound = false, TrackDependency = false }) != null;
        }
    }
}