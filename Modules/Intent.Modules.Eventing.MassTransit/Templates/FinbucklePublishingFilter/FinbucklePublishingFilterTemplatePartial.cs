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

namespace Intent.Modules.Eventing.MassTransit.Templates.FinbucklePublishingFilter
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class FinbucklePublishingFilterTemplate : CSharpTemplateBase<object>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.Eventing.MassTransit.FinbucklePublishingFilter";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public FinbucklePublishingFilterTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddUsing("System.Threading.Tasks")
                .AddUsing("MassTransit")
                .AddUsing("Finbuckle.MultiTenant")
                .AddClass($"FinbucklePublishingFilter", @class =>
                {
                    @class.AddGenericParameter("T", out var t)
                    .ImplementsInterface($"IFilter<PublishContext<{t}>>")
                    .AddGenericTypeConstraint(t, c => c
                        .AddType("class"))
                    .AddField("string", "headerName".ToPrivateMemberName(), f => f.PrivateReadOnly())
                    .AddConstructor(ctor =>
                    {
                        ctor.AddParameter("ITenantInfo", "tenant", p =>
                        {
                            p.IntroduceReadonlyField();
                        })
                        .AddParameter(UseType("Microsoft.Extensions.Configuration.IConfiguration"), "configuration")
                        .AddStatement("_headerName = configuration.GetValue<string?>(\"MassTransit:TenantHeader\") ?? \"Tenant-Identifier\";");
                    })
                    .AddMethod("void", "Probe", m => { m.AddParameter("ProbeContext", "context"); })
                    .AddMethod("Task", "Send", method =>
                    {
                        method.AddParameter($"PublishContext<{t}>", "context")
                            .AddParameter($"IPipe<PublishContext<{t}>>", "next")
                            .AddStatement("context.Headers.Set(_headerName, _tenant.Identifier);")
                            .AddStatement("return next.Send(context);");
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

        public override bool CanRunTemplate()
        {
            return base.CanRunTemplate() &&
                   GetTemplate<object>("Intent.Modules.AspNetCore.MultiTenancy.MultiTenancyConfiguration", new TemplateDiscoveryOptions() { ThrowIfNotFound = false, TrackDependency = false }) != null;
        }
    }
}