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

namespace Intent.Modules.Redis.Om.Repositories.Templates.Templates.RedisOmDocumentTypeExtensionMethods
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class RedisOmDocumentTypeExtensionMethodsTemplate : CSharpTemplateBase<object>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.Redis.Om.Repositories.Templates.RedisOmDocumentTypeExtensionMethods";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public RedisOmDocumentTypeExtensionMethodsTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddUsing("System")
                .AddUsing("System.Linq")
                .AddClass($"TypeExtensionMethods", @class =>
                {
                    @class.Internal().Static();
                    @class.AddMethod("string", "GetNameForDocument", method =>
                    {
                        method.Static();
                        method.AddParameter("Type", "type", p => p.WithThisModifier());

                        method.AddIfStatement("type.IsArray", @if =>
                        {
                            @if.AddStatement("return GetNameForDocument(type.GetElementType()!) + \"[]\";");
                        });

                        method.AddIfStatement("type.IsGenericType", @if =>
                        {
                            @if.AddStatement("return $\"{type.Name[..type.Name.LastIndexOf(\"`\", StringComparison.InvariantCulture)]}<{string.Join(\", \", type.GetGenericArguments().Select(GetNameForDocument))}>\";");
                        });

                        method.AddStatement("return type.Name;", s => s.SeparatedFromPrevious());
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