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

namespace Intent.Modules.Eventing.MassTransit.Templates.FinbuckleSendingFilter
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class FinbuckleSendingFilterTemplate : CSharpTemplateBase<object>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.Eventing.MassTransit.FinbuckleSendingFilter";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public FinbuckleSendingFilterTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddUsing("System.Threading.Tasks")
                .AddUsing("MassTransit")
                .AddUsing("Finbuckle.MultiTenant.Abstractions")
                .AddClass($"FinbuckleSendingFilter", @class =>
                {
                    @class.AddGenericParameter("T", out var t)
                        .ImplementsInterface($"IFilter<SendContext<{t}>>")
                        .AddGenericTypeConstraint(t, c => c
                            .AddType("class"))
                        .AddField("string", "headerName".ToPrivateMemberName(), f => f.PrivateReadOnly())
                        .AddConstructor(ctor =>
                        {
                            ctor.AddParameter("IMultiTenantContextAccessor", "multiTenantContextAccessor", p =>
                            {
                                p.IntroduceReadonlyField();
                            })
                                .AddParameter(UseType("Microsoft.Extensions.Configuration.IConfiguration"), "configuration")
                                .AddStatement("_headerName = configuration.GetValue<string?>(\"MassTransit:TenantHeader\") ?? \"Tenant-Identifier\";");
                        })
                        .AddMethod("void", "Probe", m => { m.AddParameter("ProbeContext", "context"); })
                        .AddMethod("Task", "Send", method =>
                        {
                            method.AddParameter($"SendContext<{t}>", "context")
                                .AddParameter($"IPipe<SendContext<{t}>>", "next")
                                // A resolved tenant is optional here, not required: a message may legitimately be
                                // sent without one (e.g. a MassTransit-generated reply/fault correlating to a
                                // previously consumed request via RequestId after the consumer's AsyncLocal-based
                                // Finbuckle tenant context has unwound, or an intentional tenant-less send). Only
                                // set the header when a tenant is actually present - never throw.
                                .AddStatement("var tenantIdentifier = _multiTenantContextAccessor.MultiTenantContext?.TenantInfo?.Identifier;")
                                .AddStatement("""
                                    if (tenantIdentifier is not null)
                                    {
                                        context.Headers.Set(_headerName, tenantIdentifier);
                                    }
                                    """)
                                .AddStatement("return next.Send(context);");
                        });
                });
        }

        public override bool CanRunTemplate()
        {
            return base.CanRunTemplate() &&
                GetTemplate<object>("Intent.Modules.AspNetCore.MultiTenancy.MultiTenancyConfiguration", new TemplateDiscoveryOptions() { ThrowIfNotFound = false, TrackDependency = false }) != null;
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
