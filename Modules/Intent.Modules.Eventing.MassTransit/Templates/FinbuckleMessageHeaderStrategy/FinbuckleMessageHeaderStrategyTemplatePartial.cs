using System;
using System.Collections.Generic;
using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.CSharp.VisualStudio;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.Eventing.MassTransit.Templates.FinbuckleMessageHeaderStrategy
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class FinbuckleMessageHeaderStrategyTemplate : CSharpTemplateBase<object>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.Eventing.MassTransit.FinbuckleMessageHeaderStrategy";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public FinbuckleMessageHeaderStrategyTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddUsing("System.Threading.Tasks")
                .AddUsing("Finbuckle.MultiTenant")
                .AddClass($"FinbuckleMessageHeaderStrategy", @class =>
                {
                    @class.ImplementsInterface("IMultiTenantStrategy")
                        .AddField($"string{NullableSymbol}", "tenantIdentifier".ToPrivateMemberName(), f => f.Private())
                        .AddMethod($"Task<string{NullableSymbol}>", "GetIdentifierAsync", method =>
                        {
                            method.AddParameter("object", "context")
                                .AddStatement("return Task.FromResult(_tenantIdentifier);");
                        })
                        .AddMethod("void", "SetTenantIdentifier", method =>
                        {
                            method.AddParameter("string", "tenantIdentifier")
                                .AddStatement("_tenantIdentifier = tenantIdentifier;");
                        })
                        ;
                });
        }

        private string NullableSymbol => OutputTarget.GetProject().NullableEnabled ? "?" : string.Empty;

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