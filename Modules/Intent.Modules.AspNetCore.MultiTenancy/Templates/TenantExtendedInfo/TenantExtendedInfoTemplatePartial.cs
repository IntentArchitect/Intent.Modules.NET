using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Multitenancy;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;
using static Intent.Modules.Constants.TemplateRoles.Repository;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.AspNetCore.MultiTenancy.Templates.TenantExtendedInfo
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class TenantExtendedInfoTemplate : CSharpTemplateBase<object>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.Modules.AspNetCore.MultiTenancy.TenantExtendedInfo";
        private readonly List<MultitenantConnectionStringRegistrationRequest> _connectionRequests = [];

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public TenantExtendedInfoTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            AddNugetDependency(NugetPackages.FinbuckleMultiTenant(outputTarget));

            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddUsing("Finbuckle.MultiTenant")
                .AddClass("TenantExtendedInfo", @class =>
                {
                    @class.WithBaseType("TenantInfo");
                    if (_connectionRequests.Any())
                    {
                        @class.ImplementsInterface(this.GetTenantConnectionsInterfaceTemplateName());
                    }
                    foreach (var request in _connectionRequests)
                    {
                        @class.AddProperty("string?", request.Name.ToCSharpIdentifier());
                    }
                });
            ExecutionContext.EventDispatcher.Subscribe<MultitenantConnectionStringRegistrationRequest>(Handle);
        }

        private void Handle(MultitenantConnectionStringRegistrationRequest @event)
        {
            _connectionRequests.Add(@event);
        }

        public override bool CanRunTemplate()
        {
            return base.CanRunTemplate() && _connectionRequests.Any();
        }

        [IntentManaged(Mode.Fully)]
        public CSharpFile CSharpFile { get; }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        protected override CSharpFileConfig DefineFileConfig()
        {
            return new CSharpFileConfig(
                className: $"TenantExtendedInfo",
                @namespace: $"{this.GetNamespace()}",
                relativeLocation: $"{this.GetFolderPath()}");
        }

        [IntentManaged(Mode.Fully)]
        public override string TransformText()
        {
            return CSharpFile.ToString();
        }
    }
}