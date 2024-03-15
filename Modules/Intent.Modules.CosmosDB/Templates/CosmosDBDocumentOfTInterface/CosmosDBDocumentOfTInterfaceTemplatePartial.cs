using System;
using System.Collections.Generic;
using System.Threading;
using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.CosmosDB.Settings;
using Intent.Modules.Modelers.Domain.Settings;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.CosmosDB.Templates.CosmosDBDocumentOfTInterface
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class CosmosDBDocumentOfTInterfaceTemplate : CSharpTemplateBase<object>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.CosmosDB.CosmosDBDocumentOfTInterface";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public CosmosDBDocumentOfTInterfaceTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            var createEntityInterfaces = ExecutionContext.Settings.GetDomainSettings().CreateEntityInterfaces();
            var useOptimisticConcurrency = ExecutionContext.Settings.GetCosmosDb().UseOptimisticConcurrency();

            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddUsing("System")
                .AddInterface($"ICosmosDBDocument", @interface =>
                {
                    @interface
                        .Internal()
                        .AddGenericParameter("TDomain", out var tDomain, genericParameter =>
                        {
                            if (createEntityInterfaces)
                            {
                                genericParameter.Contravariant();
                            }
                        });

                    var tDomainState = tDomain;
                    if (createEntityInterfaces)
                    {
                        @interface
                            .AddGenericParameter("TDomainState", out tDomainState);
                    }

                    @interface
                        .AddGenericParameter("TDocument", out var tDocument, g => g.Covariant())
                        .AddGenericTypeConstraint(tDomain, c => c
                            .AddType("class"));
                    if (createEntityInterfaces)
                    {
                        @interface
                            .AddGenericTypeConstraint(tDomainState, c => c
                                .AddType("class")
                                .AddType(tDomain));
                    }

                    var tDomainStateConstraint = createEntityInterfaces
                        ? $", {tDomainState}"
                        : string.Empty;
                    @interface
                        .AddGenericTypeConstraint(tDocument, c => c
                            .AddType($"{this.GetCosmosDBDocumentOfTInterfaceName()}<{tDomain}{tDomainStateConstraint}, {tDocument}>"));

                    @interface.ImplementsInterfaces(UseType("ICosmosDBDocument"));

                    if (useOptimisticConcurrency)
                    {
                        @interface.AddMethod(tDocument, "PopulateFromEntity", c => c
                            .AddParameter(tDomain, "entity")
                            .AddParameter("Func<string, string?>", "getEtag"));
                    }
                    else
                    {
                        @interface.AddMethod(tDocument, "PopulateFromEntity", c => c
                        .AddParameter(tDomain, "entity"));
                    }

                    @interface.AddMethod(tDomainState, "ToEntity", c => c
                        .AddParameter($"{tDomainState}?", "entity", p => p.WithDefaultValue("null")));
                })
                .AddInterface($"ICosmosDBDocument", @interface =>
                {
                    var implementsInterface = UseType("Microsoft.Azure.CosmosRepository.IItem");
                    if (useOptimisticConcurrency)
                    {
                        implementsInterface = UseType("Microsoft.Azure.CosmosRepository.IItemWithEtag");
                    }

                    @interface.Internal();
                    @interface.ImplementsInterfaces(implementsInterface);

                    @interface.AddProperty("string", "PartitionKey", method => method
                        .ExplicitlyImplements(UseType("Microsoft.Azure.CosmosRepository.IItem"))
                        .WithoutSetter()
                        .Getter.WithExpressionImplementation("PartitionKey!")
                    );

                    @interface.AddProperty("string?", "PartitionKey", property =>
                    {
                        property.New();
                        property.Getter.WithExpressionImplementation("Id");
                        property.Setter.WithExpressionImplementation("Id = value ?? throw new ArgumentNullException(nameof(value))");
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
    }
}