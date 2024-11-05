using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Configuration;
using Intent.Modules.Common.CSharp.DependencyInjection;
using Intent.Modules.Common.CSharp.Multitenancy;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.Constants;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;
using JetBrains.Annotations;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.AspNetCore.MultiTenancy.Templates.TenantConnectionsInterface
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class TenantConnectionsInterfaceTemplate : CSharpTemplateBase<object>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.Modules.AspNetCore.MultiTenancy.TenantConnectionsInterfaceTemplate";
        private readonly List<MultitenantConnectionStringRegistrationRequest> _connectionRequests = [];

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public TenantConnectionsInterfaceTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddInterface($"ITenantConnections", @interface =>
                {
                    @interface.AddProperty("string?", "Id");
                    foreach (var request in _connectionRequests)
                    {
                        @interface.AddProperty("string?", request.Name.ToCSharpIdentifier());
                    }
                    if (_connectionRequests.Any())
                    {
                        DoDIRegisttration();
                    }
                });
            ExecutionContext.EventDispatcher.Subscribe<MultitenantConnectionStringRegistrationRequest>(Handle);
        }

        private void DoDIRegisttration()
        {
            var template = ExecutionContext.FindTemplateInstance<ICSharpFileBuilderTemplate>(TemplateRoles.Infrastructure.DependencyInjection);
            if (template == null)
            {
                return;
            }
            template.CSharpFile.OnBuild(file =>
            {
                var method = file.Classes.FirstOrDefault()?.FindMethod("AddInfrastructure");
                if (method == null)
                {
                    return;
                }
                method.AddStatement(@"services.AddScoped<ITenantConnections>(
                provider => provider.GetService<ITenantInfo>() as TenantExtendedInfo ?? 
                throw new Finbuckle.MultiTenant.MultiTenantException(""Failed to resolve tenant info""));");
            });
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