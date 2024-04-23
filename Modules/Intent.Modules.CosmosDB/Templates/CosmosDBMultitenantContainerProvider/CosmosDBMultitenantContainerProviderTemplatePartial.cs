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

namespace Intent.Modules.CosmosDB.Templates.CosmosDBMultitenantContainerProvider
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class CosmosDBMultitenantContainerProviderTemplate : CSharpTemplateBase<object>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.CosmosDB.CosmosDBMultitenantContainerProvider";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public CosmosDBMultitenantContainerProviderTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddUsing("System")
                .AddUsing("System.Threading.Tasks")
                .AddUsing("System.Collections.Concurrent")
                .AddUsing("Microsoft.Azure.Cosmos")
                .AddUsing("Microsoft.Azure.CosmosRepository")
                .AddUsing("Microsoft.Azure.CosmosRepository.Services")
                .AddUsing("Microsoft.Azure.CosmosRepository.Providers")
                .AddClass($"CosmosDBMultitenantContainerProvider", @class =>
                {
                    @class
                        .AddGenericParameter("TItem")
                        .ImplementsInterface("ICosmosContainerProvider<TItem>")
                        .AddGenericTypeConstraint("TItem", c => c.AddType("IItem"));
                    @class.AddField("ConcurrentDictionary<string, Container>", "_containers", f => f.PrivateReadOnly().WithAssignment("new ConcurrentDictionary<string, Container>()"));
                    @class.AddField(this.GetCosmosDBMultiTenantClientProviderName(), "_clientProvider", f => f.PrivateReadOnly());
                    @class.AddConstructor(ctor =>
                    {
                        ctor.AddParameter("ICosmosContainerService", "containerService", param =>
                        {
                            param.IntroduceReadonlyField();
                        });
                        ctor.AddParameter("ICosmosClientProvider", "clientProvider");
                        ctor.AddStatement($"_clientProvider = ({this.GetCosmosDBMultiTenantClientProviderName()})clientProvider;");
                    });
                    @class.AddMethod("Task<Container>", "GetContainerAsync", method =>
                    {
                        method.Async();
                        method.AddStatements(@"if (_clientProvider.Tenant == null || _clientProvider.Tenant.Id == null)
			{
				throw new Exception(""Tenant Info missing for Connection lookup"");
			}

			var tenantInfo = _clientProvider.Tenant;
			if (!_containers.TryGetValue(tenantInfo.Id, out var container))
			{
				container = await _containerService.GetContainerAsync<TItem>(); 
				_containers.AddOrUpdate(tenantInfo.Id, container, (k,c) => c);
			}
			return container;".ConvertToStatements());
                    });

                });
        }

        public override bool CanRunTemplate()
        {
            return base.CanRunTemplate() && DocumentTemplateHelpers.IsSeparateDatabaseMultiTenancy(ExecutionContext.Settings);
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