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

namespace Intent.Modules.Eventing.MassTransit.Templates.FinbuckleConsumingFilter
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class FinbuckleConsumingFilterTemplate : CSharpTemplateBase<object>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.Eventing.MassTransit.FinbuckleConsumingFilter";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public FinbuckleConsumingFilterTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddUsing("System")
                .AddUsing("System.Linq")
                .AddUsing("System.Threading.Tasks")
                .AddUsing("System.Collections.Generic")
                .AddUsing("MassTransit")
                .AddUsing("Finbuckle.MultiTenant")
                .AddUsing("Microsoft.Extensions.DependencyInjection")
                .AddClass($"FinbuckleConsumingFilter", @class =>
                {
                    @class.AddGenericParameter("T", out var t)
                        .ImplementsInterface($"IFilter<ConsumeContext<{t}>>")
                        .AddGenericTypeConstraint(t, c => c
                            .AddType("class"))
                        .AddField("string", "headerName".ToPrivateMemberName(), f => f.PrivateReadOnly())
                        .AddField(this.GetFinbuckleMessageHeaderStrategyName(), "messageHeaderStrategy".ToPrivateMemberName(), f => f.PrivateReadOnly())
                        .AddConstructor(ctor =>
                        {
                            ctor.AddParameter("IServiceProvider", "serviceProvider")
                                .AddParameter("IMultiTenantContextAccessor", "accessor", p => p.IntroduceReadonlyField())
                                .AddParameter("ITenantResolver", "tenantResolver", p => p.IntroduceReadonlyField())
                                .AddParameter(UseType("Microsoft.Extensions.Configuration.IConfiguration"), "configuration")
                                .AddStatement("_headerName = configuration.GetValue<string?>(\"MassTransit:TenantHeader\") ?? \"Tenant-Identifier\";")
                                .AddMethodChainStatement($"_messageHeaderStrategy = ({this.GetFinbuckleMessageHeaderStrategyName()})serviceProvider", chain => chain
                                    .AddChainStatement("GetRequiredService<IEnumerable<IMultiTenantStrategy>>()")
                                    .AddChainStatement($"Single(s => s.GetType() == typeof({this.GetFinbuckleMessageHeaderStrategyName()}))"));
                        })
                        .AddMethod("Task", "Send", method =>
                        {
                            method.Async()
                                .AddParameter($"ConsumeContext<{t}>", "context")
                                .AddParameter($"IPipe<ConsumeContext<{t}>>", "next")
                                .AddIfStatement("context.TryGetHeader<string>(_headerName, out var tenantIdentifier)", stmt =>
                                {
                                    stmt.AddStatement("_messageHeaderStrategy.SetTenantIdentifier(tenantIdentifier);");
                                    stmt.AddStatement("var multiTenantContext = await _tenantResolver.ResolveAsync(context);");
                                    stmt.AddStatement("_accessor.MultiTenantContext = multiTenantContext;");
                                })
                                .AddStatement("await next.Send(context);")
                                ;
                        })
                        .AddMethod("void", "Probe", m => { m.AddParameter("ProbeContext", "context"); });
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

        public override bool CanRunTemplate()
        {
            return base.CanRunTemplate() &&
                   GetTemplate<object>("Intent.Modules.AspNetCore.MultiTenancy.MultiTenancyConfiguration",
                       new TemplateDiscoveryOptions() { ThrowIfNotFound = false, TrackDependency = false }) != null;
        }
    }
}