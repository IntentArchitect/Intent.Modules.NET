using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Modelers.Domain.ValueObjects.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.Modelers.Domain.Settings;
using Intent.Modules.ValueObjects.Settings;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.ValueObjects.Templates.ValueObject
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class ValueObjectTemplate : CSharpTemplateBase<ValueObjectModel>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.ValueObjects.ValueObject";

        [IntentManaged(Mode.Fully)]
        public CSharpFile CSharpFile { get; }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public ValueObjectTemplate(IOutputTarget outputTarget, ValueObjectModel model) : base(TemplateId, outputTarget, model)
        {
            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath());

            var type = ExecutionContext.Settings.GetValueObjectSettings().ValueObjectType().AsEnum();
            switch (type)
            {
                case ValueObjectSettings.ValueObjectTypeOptionsEnum.Class:
                    DeclareValueObjectClassType();
                    break;
                case ValueObjectSettings.ValueObjectTypeOptionsEnum.Record:
                    DeclareValueObjectRecordType();
                    break;
                default:
                    throw new ArgumentOutOfRangeException($"Invalid Value Object Type: {type}");
            }
        }

        private void DeclareValueObjectClassType()
        {
            CSharpFile.AddClass(Model.Name, @class =>
            {
                @class.WithBaseType(this.GetValueObjectBaseName());
                if (Model.HasSerializationSettings())
                {
                    @class.AddMetadata("serialization", Model.GetSerializationSettings().Type().Value);
                }

                if (Model.Attributes.Any())
                {
                    @class.AddConstructor(ctor => { ctor.Protected(); });
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

        private void DeclareValueObjectRecordType()
        {
            CSharpFile.AddRecord(Model.Name, record =>
            {
                if (Model.HasSerializationSettings())
                {
                    record.AddMetadata("serialization", Model.GetSerializationSettings().Type().Value);
                }

                record.AddPrimaryConstructor(ctor =>
                {
                    foreach (var modelAttribute in Model.Attributes)
                    {
                        ctor.AddParameter(GetTypeName(modelAttribute), modelAttribute.Name);
                    }
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