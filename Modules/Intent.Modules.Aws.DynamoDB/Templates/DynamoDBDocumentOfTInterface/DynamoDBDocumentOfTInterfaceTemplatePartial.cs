using Intent.Engine;
using Intent.Modules.Aws.DynamoDB.Settings;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Modelers.Domain.Settings;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.Aws.DynamoDB.Templates.DynamoDBDocumentOfTInterface
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class DynamoDBDocumentOfTInterfaceTemplate : CSharpTemplateBase<object>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.Aws.DynamoDB.DynamoDBDocumentOfTInterface";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public DynamoDBDocumentOfTInterfaceTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            var createEntityInterfaces = ExecutionContext.Settings.GetDomainSettings().CreateEntityInterfaces();
            var useOptimisticConcurrency = ExecutionContext.Settings.GetDynamoDBSettings().UseOptimisticConcurrency();

            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddUsing("System")
                .AddInterface("IDynamoDBDocument", @interface =>
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
                            .AddType($"{this.GetDynamoDBDocumentOfTInterfaceName()}<{tDomain}{tDomainStateConstraint}, {tDocument}>"));

                    @interface.AddMethod("object", "GetKey");

                    if (useOptimisticConcurrency)
                    {
                        @interface.AddMethod("int?", "GetVersion");
                        @interface.AddMethod(tDocument, "PopulateFromEntity", c => c
                            .AddParameter(tDomain, "entity")
                            .AddParameter("Func<object, int?>", "getVersion"));
                    }
                    else
                    {
                        @interface.AddMethod(tDocument, "PopulateFromEntity", c => c
                        .AddParameter(tDomain, "entity"));
                    }

                    @interface.AddMethod(tDomainState, "ToEntity", c => c
                        .AddParameter($"{tDomainState}?", "entity", p => p.WithDefaultValue("null")));
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