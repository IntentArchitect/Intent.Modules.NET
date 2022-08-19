using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Modelers.Domain.ValueObjects.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.ValueObjects.Templates.ValueObject
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    partial class ValueObjectTemplate : CSharpTemplateBase<ValueObjectModel>
    {
        public const string TemplateId = "Intent.ValueObjects.ValueObject";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public ValueObjectTemplate(IOutputTarget outputTarget, ValueObjectModel model) : base(TemplateId, outputTarget, model)
        {
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        protected override CSharpFileConfig DefineFileConfig()
        {
            return new CSharpFileConfig(
                className: $"{Model.Name}",
                @namespace: $"{this.GetNamespace()}",
                relativeLocation: $"{this.GetFolderPath()}");
        }

        private string GetClass()
        {
            // NOTE: This is an experimental approach to class templating
            var @class = new CSharpClass(Model.Name)
                .WithBaseType(this.GetValueObjectBaseName());

            var ctor = @class.AddConstructor();

            var getEqualityComponentsMethod = @class.AddMethod($"{UseType("System.Collections.Generic.IEnumerable")}<object>", "GetEqualityComponents")
                .Protected()
                .Override()
                .AddStatement("// Using a yield return statement to return each element one at a time");

            if (!Model.Attributes.Any())
            {
                getEqualityComponentsMethod.AddStatement("yield break;");
            }
            foreach (var attribute in Model.Attributes)
            {
                var prop = ctor.AddParameter(GetTypeName(attribute), attribute.Name.ToCamelCase())
                    .IntroduceProperty()
                        .PrivateSetter();
                getEqualityComponentsMethod.AddStatement($"yield return {prop.Name};");
            }
            return @class.ToString("    ");
        }
    }
}