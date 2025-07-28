using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.Aws.DynamoDB.Templates.DynamoDBUnitOfWorkInterface
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class DynamoDBUnitOfWorkInterfaceTemplate : CSharpTemplateBase<object>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.Aws.DynamoDB.DynamoDBUnitOfWorkInterface";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public DynamoDBUnitOfWorkInterfaceTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddUsing("System.Threading")
                .AddUsing("System.Threading.Tasks")
                .AddInterface($"IDynamoDBUnitOfWork", @interface =>
                {
                    @interface.AddMethod("Task", "SaveChangesAsync", method => method
                        .AddParameter("CancellationToken", "cancellationToken", parameter => parameter.WithDefaultValue("default"))
                    );
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