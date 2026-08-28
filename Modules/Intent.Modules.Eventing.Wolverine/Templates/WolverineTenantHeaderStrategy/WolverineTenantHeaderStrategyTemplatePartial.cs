using System;
using System.Collections.Generic;
using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Configuration;
using Intent.Modules.Common.CSharp.DependencyInjection;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.Eventing.Wolverine.Templates.WolverineTenantHeaderStrategy
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class WolverineTenantHeaderStrategyTemplate : CSharpTemplateBase<object>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.Eventing.Wolverine.WolverineTenantHeaderStrategy";

        /// <summary>
        /// R12.3: the `IConfiguration` key the header name is read from. Shared (by name, since the
        /// two classes live in the consumer's own generated code, not this module's) with
        /// <see cref="Templates.WolverineTenantMiddleware.WolverineTenantMiddlewareTemplate"/> via the
        /// generated <c>ResolveHeaderName</c> static method below, so both sides of the header
        /// contract can never drift out of sync.
        /// </summary>
        private const string HeaderNameConfigurationKey = "Wolverine:TenantHeader";
        private const string DefaultHeaderName = "Tenant-Identifier";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public WolverineTenantHeaderStrategyTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddUsing("Wolverine")
                .AddUsing("Finbuckle.MultiTenant.Abstractions")
                .AddUsing("Microsoft.Extensions.Configuration")
                .AddClass("WolverineTenantHeaderStrategy", @class =>
                {
                    @class.AddField("string", "HeaderNameConfigurationKey", f => f.Constant($@"""{HeaderNameConfigurationKey}"""));

                    @class.AddField("string", "DefaultHeaderName", f => f.Constant($@"""{DefaultHeaderName}"""));

                    @class.AddField("string", "headerName".ToPrivateMemberName(), f => f.PrivateReadOnly());

                    @class.AddConstructor(ctor =>
                    {
                        ctor.AddParameter("IMultiTenantContextAccessor", "multiTenantContextAccessor", param => param.IntroduceReadonlyField());
                        ctor.AddParameter("IConfiguration", "configuration")
                            .AddStatement("_headerName = ResolveHeaderName(configuration);");
                    });

                    @class.AddMethod("string", "ResolveHeaderName", method =>
                    {
                        method.Static();
                        method.AddParameter("IConfiguration", "configuration");
                        method.AddStatement("return configuration.GetValue<string?>(HeaderNameConfigurationKey) ?? DefaultHeaderName;");
                    });

                    @class.AddMethod("DeliveryOptions?", "BuildDeliveryOptions", method =>
                    {
                        // A resolved tenant is optional here, not required: a message may
                        // legitimately be published without one (e.g. a background/system process
                        // with no ambient tenant). Only set the header when a tenant is actually
                        // present - never throw (R12.1/R12.3).
                        method.AddStatement("var tenantIdentifier = _multiTenantContextAccessor.MultiTenantContext?.TenantInfo?.Identifier;");
                        method.AddStatement(
                            """
                            if (tenantIdentifier is null)
                            {
                            return null;
                            }
                            """,
                            s => s.SeparatedFromPrevious());
                        method.AddStatement(
                            """
                            var options = new DeliveryOptions();
                            options.Headers[_headerName] = tenantIdentifier;
                            return options;
                            """,
                            s => s.SeparatedFromPrevious());
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
        /// R12.3: registers the header-name key so it actually appears in appsettings.json - the
        /// generated ResolveHeaderName above already falls back to DefaultHeaderName in code, but
        /// that alone never surfaces the key for a developer to override.
        /// R12.2 (bugfix): also self-registers this class in DI. WolverineMessageBusTemplate injects
        /// it by CONCRETE type (there's no interface), so without this registration
        /// WolverineMessageBus fails to resolve at runtime with "Unable to resolve service for type
        /// WolverineTenantHeaderStrategy" the moment any request reaches it - the middleware/appsetting
        /// wiring alone is not enough. Scoped to match IMultiTenantContextAccessor's own lifetime and
        /// WolverineMessageBus's own registration, avoiding a captive-dependency mismatch.
        /// </summary>
        public override void BeforeTemplateExecution()
        {
            ExecutionContext.EventDispatcher.Publish(new AppSettingRegistrationRequest(HeaderNameConfigurationKey, DefaultHeaderName));

            ExecutionContext.EventDispatcher.Publish(ContainerRegistrationRequest.ToRegister(this)
                .ForConcern("Infrastructure")
                .WithPerServiceCallLifeTime());
        }

        /// <summary>
        /// Gated on Finbuckle multi-tenancy being installed - mirrors
        /// Intent.Eventing.MassTransit's FinbucklePublishingFilterTemplate/FinbuckleMessageHeaderStrategyTemplate,
        /// which use the same check.
        /// </summary>
        public override bool CanRunTemplate()
        {
            return base.CanRunTemplate() &&
                GetTemplate<object>("Intent.Modules.AspNetCore.MultiTenancy.MultiTenancyConfiguration", new TemplateDiscoveryOptions { ThrowIfNotFound = false, TrackDependency = false }) != null;
        }
    }
}
