using System;
using System.Collections.Generic;
using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.Eventing.Wolverine.Templates.WolverineTenantHeaderStrategy;
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
                .AddUsing("Finbuckle.MultiTenant")
                .AddUsing("Finbuckle.MultiTenant.Abstractions")
                .AddUsing("Microsoft.Extensions.Configuration")
                .AddClass("WolverineTenantMiddleware", @class =>
                {
                    @class.Static();

                    // R12.2/R12.4: Wolverine discovers Before/FinallyAsync middleware by naming convention on
                    // a plain static class (Wolverine.Middleware), not through an interface - matching the
                    // shape cited in
                    // intent/.specs/wolverine-eventing-module/golden-sample/probes/DurableAndTransportProbe/Probe.cs
                    // (TenancyMiddlewareProbe). A message with no tenant header (R12.4) leaves the prior
                    // context untouched and proceeds - it is never rejected.
                    // Resolved here (inside the AddClass callback), not before CSharpFile is built: the
                    // SDK derives this template's own output filename from its first added class, so
                    // resolving a foreign type before any class exists throws a NullReferenceException
                    // on a consumer application that has never generated this file before.
                    var headerStrategyTypeName = GetTypeName(WolverineTenantHeaderStrategyTemplate.TemplateId);

                    @class.AddMethod("IMultiTenantContext?", "Before", method =>
                    {
                        method.Static();
                        method.AddParameter("Envelope", "envelope");
                        method.AddParameter("ITenantResolver", "tenantResolver");
                        method.AddParameter("IMultiTenantContextAccessor", "contextAccessor");
                        method.AddParameter("IMultiTenantContextSetter", "contextSetter");
                        method.AddParameter("IConfiguration", "configuration");

                        method.AddStatement("var previous = contextAccessor.MultiTenantContext;");
                        method.AddStatement($"var headerName = {headerStrategyTypeName}.ResolveHeaderName(configuration);", s => s.SeparatedFromPrevious());
                        method.AddStatement(
                            """
                            if (!envelope.Headers.TryGetValue(headerName, out var tenantId) || string.IsNullOrEmpty(tenantId))
                            {
                                return previous;
                            }
                            """,
                            s => s.SeparatedFromPrevious());
                        method.AddStatement(
                            """
                            contextSetter.MultiTenantContext = tenantResolver.ResolveAsync(envelope).GetAwaiter().GetResult();
                            return previous;
                            """,
                            s => s.SeparatedFromPrevious());
                    });

                    @class.AddMethod("Task", "FinallyAsync", method =>
                    {
                        method.Static();
                        method.AddParameter("IMultiTenantContext?", "previous");
                        method.AddParameter("IMultiTenantContextSetter", "contextSetter");

                        method.AddStatement("contextSetter.MultiTenantContext = previous!;");
                        method.AddStatement("return Task.CompletedTask;", s => s.SeparatedFromPrevious());
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
        /// <see cref="Templates.WolverineTenantHeaderStrategy.WolverineTenantHeaderStrategyTemplate"/>.
        /// </summary>
        public override bool CanRunTemplate()
        {
            return base.CanRunTemplate() &&
                GetTemplate<object>("Intent.Modules.AspNetCore.MultiTenancy.MultiTenancyConfiguration", new TemplateDiscoveryOptions { ThrowIfNotFound = false, TrackDependency = false }) != null;
        }
    }
}