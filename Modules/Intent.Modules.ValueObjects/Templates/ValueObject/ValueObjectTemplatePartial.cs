using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Modelers.Domain.ValueObjects.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.ValueObjects.Templates.ValueObject
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    partial class ValueObjectTemplate : CSharpTemplateBase<ValueObjectModel>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.ValueObjects.ValueObject";

        public CSharpFile CSharpFile { get; set; }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public ValueObjectTemplate(IOutputTarget outputTarget, ValueObjectModel model) : base(TemplateId, outputTarget, model)
        {
            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddClass(Model.Name, @class =>
                {
                    @class.WithBaseType(this.GetValueObjectBaseName());
                    
                    if (Model.Attributes.Any())
                    {
                        @class.AddConstructor(ctor =>
                        {
                            ctor.Protected();
                        });
                    }

                    @class.AddConstructor(ctor =>
                    {
                        @class.AddMethod(
                            returnType: $"{UseType("System.Collections.Generic.IEnumerable")}<object>",
                            name: "GetEqualityComponents", method =>
                            {
                                method.Protected()
                                    .Override()
                                    .AddStatement("// Using a yield return statement to return each element one at a time");
                                if (!Model.Attributes.Any())
                                {
                                    method.AddStatement("yield break;");
                                }
                                foreach (var attribute in Model.Attributes)
                                {
                                    ctor.AddParameter(GetTypeName(attribute), attribute.Name.ToCamelCase(), param =>
                                    {
                                        param.IntroduceProperty(prop =>
                                        {
                                            prop.PrivateSetter();
                                            method.AddStatement($"yield return {prop.Name};");
                                        });
                                    });
                                }
                            });
                    });
                });
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        protected override CSharpFileConfig DefineFileConfig()
        {
            return CSharpFile.GetConfig();
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public override string TransformText()
        {
            return CSharpFile.ToString();
        }
    }
}