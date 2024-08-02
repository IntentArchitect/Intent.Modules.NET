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

namespace Intent.Modules.AspNetCore.IntegrationTesting.Templates.ContainerCosmosClientProvider
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class ContainerCosmosClientProviderTemplate : CSharpTemplateBase<object>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.AspNetCore.IntegrationTesting.ContainerCosmosClientProvider";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public ContainerCosmosClientProviderTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddUsing("Microsoft.Azure.Cosmos")
                .AddUsing("Microsoft.Azure.CosmosRepository.Providers")
                .AddClass($"ContainerCosmosClientProvider", @class =>
                {
                    AddNugetDependency(NugetPackages.IEvangelistAzureCosmosRepository(outputTarget));
                    @class
                        .ImplementsInterface("ICosmosClientProvider")
                        .ImplementsInterface("IDisposable");
                    @class.AddConstructor(ctor =>
                    {
                        ctor.AddParameter("CosmosClient", "cosmosClient", param =>
                        {
                            param.IntroduceReadonlyField();
                        });
                    });

                    @class.AddProperty("CosmosClient", "CosmosClient", p =>
                    {
                        p.Getter.WithExpressionImplementation("_cosmosClient");
                        p.ReadOnly();
                    });
                    @class.AddMethod("Task<T>", "UseClientAsync", method =>
                    {
                        method
                            .AddGenericParameter("T")
                            .AddParameter("Func<CosmosClient, Task<T>>", "consume")
                            .WithExpressionBody("consume.Invoke(_cosmosClient)")
                            ;
                    });
                    @class.AddMethod("void", "Dispose", method =>
                    {
                        method.AddIfStatement("_cosmosClient != null", stmt =>
                        {
                            stmt.AddStatement("_cosmosClient.Dispose();");
                        });
                    });
                });
        }

        public override bool CanRunTemplate()
        {
            return base.CanRunTemplate() && ContainerHelper.RequireCosmosContainer(this);
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