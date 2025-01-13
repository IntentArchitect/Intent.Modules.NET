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

namespace Intent.Modules.EntityFrameworkCore.DataMasking.Templates.DataMaskConverter
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class DataMaskConverterTemplate : CSharpTemplateBase<object>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.Modules.EntityFrameworkCore.DataMasking.DataMaskConverter";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public DataMaskConverterTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddClass($"DataMaskConverter", @class =>
                {
                    @class.WithBaseType(UseType("Microsoft.EntityFrameworkCore.Storage.ValueConversion.ValueConverter<string, string>"));

                    @class.AddConstructor(ctor =>
                    {
                        ctor.AddParameter(GetTypeName("Intent.Application.Identity.CurrentUserServiceInterface"), "currentUserService")
                            .AddParameter("MaskDataType", "maskType")
                            .AddParameter("string", "maskCharacter", param => param.WithDefaultValue("\"*\""))
                            .AddParameter("int", "maskLength", param => param.WithDefaultValue("0"))
                            .AddParameter("int", "unmaskedPrefixLength", param => param.WithDefaultValue("0"))
                            .AddParameter("int", "unmaskedSuffixLength", param => param.WithDefaultValue("0"));

                        ctor.CallsBase(@base =>
                        {
                            @base.AddArgument("v => v");
                            @base.AddArgument("v => MaskValue(currentUserService, maskType, v, maskCharacter, maskLength, unmaskedPrefixLength, unmaskedSuffixLength)");
                        });
                    });

                    @class.AddMethod("string", "MaskValue", method =>
                    {
                        method.Private().Static();

                        method.AddParameter(GetTypeName("Intent.Application.Identity.CurrentUserServiceInterface"), "currentUserService")
                            .AddParameter("MaskDataType", "maskType")
                            .AddParameter("string", "value")
                            .AddParameter("string", "maskCharacter")
                            .AddParameter("int", "maskLength")
                            .AddParameter("int", "unmaskedPrefixLength")
                            .AddParameter("int", "unmaskedSuffixLength");

                        method.AddIfStatement("string.IsNullOrWhiteSpace(value)", @if =>
                        {
                            @if.AddReturn("value");
                        });

                        method.AddIfStatement("maskType == MaskDataType.SetLength", @if =>
                        {
                            @if.AddReturn("string.Concat(Enumerable.Repeat(maskCharacter, maskLength))");
                        });

                        method.AddIfStatement("maskType == MaskDataType.VariableLength", @if =>
                        {
                            @if.AddReturn("string.Concat(Enumerable.Repeat(maskCharacter, value.Length))");
                        });

                        method.AddIfStatement("unmaskedPrefixLength + unmaskedSuffixLength >= value.Length", @if =>
                        {
                            @if.AddReturn("string.Concat(Enumerable.Repeat(maskCharacter, value.Length))");
                        });

                        method.AddObjectInitStatement("var prefix", "value[..unmaskedPrefixLength];");
                        method.AddObjectInitStatement("var suffix", "value[^unmaskedSuffixLength..];");
                        method.AddObjectInitStatement("var toMask", "value.Substring(unmaskedPrefixLength, value.Length - unmaskedPrefixLength - unmaskedSuffixLength);");
                        method.AddObjectInitStatement("var maskedPortion", "string.Concat(Enumerable.Repeat(maskCharacter, toMask.Length));");

                        method.AddReturn("$\"{prefix}{maskedPortion}{suffix}\"");
                    });
                }).AddEnum("MaskDataType", @enum =>
                {
                    @enum.AddLiteral("SetLength")
                        .AddLiteral("VariableLength")
                        .AddLiteral("PartialMask");
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