using Intent.Engine;
using Intent.Modules.Aws.DynamoDB.Settings;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.Aws.DynamoDB.Templates.EnumAsStringConverter
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class EnumAsStringConverterTemplate : CSharpTemplateBase<object>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.Aws.DynamoDB.EnumAsStringConverter";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public EnumAsStringConverterTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddUsing("System")
                .AddUsing("Amazon.DynamoDBv2.DataModel")
                .AddUsing("Amazon.DynamoDBv2.DocumentModel")
                .AddClass("EnumAsStringConverter", @class =>
                {
                    @class.Internal();
                    @class.ImplementsInterface("IPropertyConverter");
                    @class.AddGenericParameter("TEnum", out var tEnum);
                    @class.AddGenericTypeConstraint(tEnum, c => c.AddType("struct"));

                    @class.AddMethod("DynamoDBEntry", "ToEntry", method =>
                    {
                        method.AddParameter("object", "value");
                        method.AddStatement("return new Primitive(value.ToString());");
                    });

                    @class.AddMethod("object", "FromEntry", method =>
                    {
                        method.AddParameter("DynamoDBEntry", "entry");
                        method.AddStatement("return Enum.Parse<TEnum>(entry.AsString());");
                    });
                });
        }

        public override bool CanRunTemplate()
        {
            return ExecutionContext.Settings.GetDynamoDBSettings().StoreEnumsAsStrings();
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