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

namespace Intent.Modules.CosmosDB.Templates.CosmosDBDocumentInterface
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class CosmosDBDocumentInterfaceTemplate : CSharpTemplateBase<object>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.CosmosDB.CosmosDBDocumentInterface";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public CosmosDBDocumentInterfaceTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddUsing("System")
                .AddInterface($"ICosmosDBDocument", @interface =>
                {
                    @interface
                        .Internal()
                        .AddGenericParameter("TDocument", out var tDocument, g => g.Covariant())
                        .AddGenericParameter("TDomain", out var tDomain, g => g.Contravariant())
                        .AddGenericTypeConstraint(tDocument, c => c
                            .AddType($"{this.GetCosmosDBDocumentInterfaceName()}<{tDocument}, {tDomain}>"));

                    @interface.ImplementsInterfaces(UseType("ICosmosDBDocument"));

                    @interface.AddMethod(tDocument, "PopulateFromEntity", c => c
                        .AddParameter(tDomain, "entity")
                    );
                })
                .AddInterface($"ICosmosDBDocument", @interface =>
                {
                    @interface.Internal();
                    @interface.ImplementsInterfaces(UseType("Microsoft.Azure.CosmosRepository.IItem"));

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