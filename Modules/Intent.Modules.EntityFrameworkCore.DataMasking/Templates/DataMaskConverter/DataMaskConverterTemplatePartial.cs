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
        public const string TemplateId = "Intent.EntityFrameworkCore.DataMasking.DataMaskConverter";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public DataMaskConverterTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddClass($"DataMaskConverter", @class =>
                {
                    @class.WithBaseType(UseType("Microsoft.EntityFrameworkCore.Storage.ValueConversion.ValueConverter<string, string>"));

                    @class.AddField(UseType("System.Func<bool>"), "_isAuthorized", @field => field.PrivateReadOnly());

                    @class.AddConstructor(ctor =>
                    {
                        ctor.WithComments(@"
/// <summary>
/// Creates a new instance of <see cref=""DataMaskConverter""/>.
/// </summary>
/// <param name=""isAuthorized"">A function which returns whether the current user is authorized to see the unmasked version of the data.</param>
/// <param name=""maskData"">A function which is used to mask the data when <paramref name=""isAuthorized""/> returns <see langword=""false""/>.</param>");
                        /// ;
                        ctor.Private();
                        ctor.AddParameter("Func<bool>", "isAuthorized")
                            .AddParameter("Func<string, string>", "maskData");

                        ctor.CallsBase(@base =>
                        {
                            @base.AddArgument("originalValue => originalValue");
                            @base.AddArgument("originalValue => isAuthorized() ? originalValue : maskData(originalValue)");
                        });

                        ctor.AddObjectInitStatement("_isAuthorized", "isAuthorized;");
                    });

                    @class.AddMethod("bool", "IsMasked", method =>
                    {
                        method.WithExpressionBody("!_isAuthorized()");
                    });

                    @class.AddMethod("ValueConverter<string, string>", "FixedLength", method =>
                    {
                        method.Static();

                        method.AddParameter(GetTypeName("Intent.Application.Identity.CurrentUserServiceInterface"), "currentUserService")
                            .AddParameter("char", "maskCharacter")
                            .AddParameter("int", "maskLength")
                            .AddParameter("string[]?", "roles", param => param.WithDefaultValue("null"))
                            .AddParameter("string[]?", "policies", param => param.WithDefaultValue("null"));

                        method.AddStatement("roles ??= [];");
                        method.AddStatement("policies ??= [];");

                        method.AddObjectInitStatement("var fixedLengthMask", "string.Empty.PadLeft(maskLength, maskCharacter);");

                        var invoc = new CSharpInvocationStatement("new DataMaskConverter")
                            .AddArgument("isAuthorized", "() => IsAuthorized(currentUserService, roles, policies)")
                            .AddArgument("maskData", "_ => fixedLengthMask").WithoutSemicolon().WithArgumentsOnNewLines();

                        method.AddReturn(invoc);
                    });

                    @class.AddMethod("ValueConverter<string, string>", "VariableLength", method =>
                    {
                        method.Static();

                        method.AddParameter(GetTypeName("Intent.Application.Identity.CurrentUserServiceInterface"), "currentUserService")
                            .AddParameter("char", "maskCharacter")
                            .AddParameter("string[]?", "roles", param => param.WithDefaultValue("null"))
                            .AddParameter("string[]?", "policies", param => param.WithDefaultValue("null"));

                        method.AddStatement("roles ??= [];");
                        method.AddStatement("policies ??= [];");

                        var invoc = new CSharpInvocationStatement("new DataMaskConverter")
                            .AddArgument("isAuthorized", "() => IsAuthorized(currentUserService, roles, policies)")
                            .AddArgument("maskData", "originalValue => string.Empty.PadLeft(originalValue.Length, maskCharacter)").WithoutSemicolon().WithArgumentsOnNewLines();

                        method.AddReturn(invoc);
                    });

                    @class.AddMethod("ValueConverter<string, string>", "Partial", method =>
                    {
                        method.Static();

                        method.AddParameter(GetTypeName("Intent.Application.Identity.CurrentUserServiceInterface"), "currentUserService")
                           .AddParameter("char", "maskCharacter")
                           .AddParameter("int", "unmaskedPrefixLength")
                           .AddParameter("int", "unmaskedSuffixLength")
                           .AddParameter("string[]?", "roles", param => param.WithDefaultValue("null"))
                           .AddParameter("string[]?", "policies", param => param.WithDefaultValue("null"));

                        method.AddStatement("roles ??= [];");
                        method.AddStatement("policies ??= [];");

                        var lambda = new CSharpLambdaBlock("originalValue")
                            .AddIfStatement("unmaskedPrefixLength + unmaskedSuffixLength >= originalValue.Length", @if =>
                            {
                                @if.AddReturn("string.Empty.PadLeft(originalValue.Length, maskCharacter)");
                            })
                            .AddObjectInitStatement("var prefix", "originalValue[..unmaskedPrefixLength];")
                            .AddObjectInitStatement("var suffix", "originalValue[^unmaskedSuffixLength..];")
                            .AddObjectInitStatement("var maskLength", "originalValue.Substring(unmaskedPrefixLength, originalValue.Length - unmaskedPrefixLength - unmaskedSuffixLength);")
                            .AddReturn("$\"{prefix}{string.Empty.PadLeft(maskLength.Length, maskCharacter)}{suffix}\"");

                        var invoc = new CSharpInvocationStatement("new DataMaskConverter")
                            .AddArgument("isAuthorized", "() => IsAuthorized(currentUserService, roles, policies)")
                            .AddArgument("maskData", lambda).WithoutSemicolon().WithArgumentsOnNewLines();

                        method.AddReturn(invoc);
                    });

                    @class.AddMethod("bool", "IsAuthorized", method =>
                    {
                        method.Static();

                        method.AddParameter(GetTypeName("Intent.Application.Identity.CurrentUserServiceInterface"), "currentUserService")
                            .AddParameter("string[]", "roles")
                            .AddParameter("string[]", "policies");


                        method.AddStatement("// Must be an authenticated user");
                        method.AddIfStatement("currentUserService.UserId is null", @if =>
                        {
                            @if.BeforeSeparator = CSharpCodeSeparatorType.None;
                            @if.AddReturn("false");
                        });

                        method.AddStatement("");
                        method.AddStatement("// Role-based authorization");
                        method.AddIfStatement("roles.Length > 0", @if =>
                        {
                            @if.BeforeSeparator = CSharpCodeSeparatorType.None;
                            @if.AddObjectInitStatement("var authorized", "false;");
                            @if.AddForEachStatement("role", "roles", @for =>
                            {
                                @for.AddObjectInitStatement("var isInRole", "currentUserService.IsInRoleAsync(role).GetAwaiter().GetResult();");
                                @for.AddIfStatement("isInRole", roleIf =>
                                {
                                    roleIf.AddObjectInitStatement("authorized", "true;");
                                    roleIf.AddStatement("break;");
                                });
                            });

                            @if.AddStatement("");
                            @if.AddStatement("// Must be a member of at least one role in roles");
                            @if.AddIfStatement("!authorized", authIf =>
                            {
                                authIf.BeforeSeparator = CSharpCodeSeparatorType.None;
                                authIf.AddReturn("false");
                            });
                        });

                        method.AddStatement("");
                        method.AddStatement("// Policy-based authorization");
                        method.AddIfStatement("policies.Length > 0", @if =>
                        {
                            @if.BeforeSeparator = CSharpCodeSeparatorType.None;
                            @if.AddObjectInitStatement("var authorized", "false;");

                            @if.AddForEachStatement("policy", "policies", @for =>
                            {
                                @for.AddObjectInitStatement("var isAuthorized", "currentUserService.AuthorizeAsync(policy).GetAwaiter().GetResult();");
                                @for.AddIfStatement("isAuthorized", roleIf =>
                                {
                                    roleIf.AddObjectInitStatement("authorized", "true;");
                                    roleIf.AddStatement("break;");
                                });
                            });

                            @if.AddStatement("");
                            @if.AddStatement("// Must be authorized by at least one policy");
                            @if.AddIfStatement("!authorized", authIf =>
                            {
                                authIf.BeforeSeparator = CSharpCodeSeparatorType.None;
                                authIf.AddReturn("false");
                            });
                        });

                        method.AddReturn("true");
                    });

                    //@class.AddConstructor(ctor =>
                    //{
                    //    ctor.AddParameter(GetTypeName("Intent.Application.Identity.CurrentUserServiceInterface"), "currentUserService")
                    //        .AddParameter("MaskDataType", "maskType")
                    //        .AddParameter("string", "maskCharacter", param => param.WithDefaultValue("\"*\""))
                    //        .AddParameter("int", "maskLength", param => param.WithDefaultValue("0"))
                    //        .AddParameter("int", "unmaskedPrefixLength", param => param.WithDefaultValue("0"))
                    //        .AddParameter("int", "unmaskedSuffixLength", param => param.WithDefaultValue("0"))
                    //        .AddParameter("string[]?", "roles", param => param.WithDefaultValue("default"))
                    //        .AddParameter("string[]?", "policies", param => param.WithDefaultValue("default"));

                    //    ctor.CallsBase(@base =>
                    //    {
                    //        @base.AddArgument("v => v");
                    //        @base.AddArgument("v => MaskValue(currentUserService, maskType, v, maskCharacter, maskLength, unmaskedPrefixLength, unmaskedSuffixLength, roles, policies)");
                    //    });
                    //});

                    //    @class.AddMethod("string", "MaskValue", method =>
                    //    {
                    //        method.Private().Static();

                    //        method.AddParameter(GetTypeName("Intent.Application.Identity.CurrentUserServiceInterface"), "currentUserService")
                    //            .AddParameter("MaskDataType", "maskType")
                    //            .AddParameter("string", "value")
                    //            .AddParameter("string", "maskCharacter")
                    //            .AddParameter("int", "maskLength")
                    //            .AddParameter("int", "unmaskedPrefixLength")
                    //            .AddParameter("int", "unmaskedSuffixLength")
                    //            .AddParameter("string[]?", "roles", param => param.WithDefaultValue("default"))
                    //            .AddParameter("string[]?", "policies", param => param.WithDefaultValue("default"));

                    //        method.AddIfStatement("UserAuthorized(currentUserService, roles, policies)", @if =>
                    //        {
                    //            @if.AddReturn("value");
                    //        });

                    //        method.AddIfStatement("string.IsNullOrWhiteSpace(value)", @if =>
                    //        {
                    //            @if.AddReturn("value");
                    //        });

                    //        method.AddIfStatement("maskType == MaskDataType.SetLength", @if =>
                    //        {
                    //            @if.AddReturn($"string.Concat({UseType("System.Linq.Enumerable")}.Repeat(maskCharacter, maskLength))");
                    //        });

                    //        method.AddIfStatement("maskType == MaskDataType.VariableLength", @if =>
                    //        {
                    //            @if.AddReturn($"string.Concat({UseType("System.Linq.Enumerable")}.Repeat(maskCharacter, value.Length))");
                    //        });

                    //        method.AddIfStatement("unmaskedPrefixLength + unmaskedSuffixLength >= value.Length", @if =>
                    //        {
                    //            @if.AddReturn("string.Concat(Enumerable.Repeat(maskCharacter, value.Length))");
                    //        });

                    //        method.AddObjectInitStatement("var prefix", "value[..unmaskedPrefixLength];");
                    //        method.AddObjectInitStatement("var suffix", "value[^unmaskedSuffixLength..];");
                    //        method.AddObjectInitStatement("var toMask", "value.Substring(unmaskedPrefixLength, value.Length - unmaskedPrefixLength - unmaskedSuffixLength);");
                    //        method.AddObjectInitStatement("var maskedPortion", "string.Concat(Enumerable.Repeat(maskCharacter, toMask.Length));");

                    //        method.AddReturn("$\"{prefix}{maskedPortion}{suffix}\"");
                    //    });

                    //    @class.AddMethod("bool", "UserAuthorized", method =>
                    //    {
                    //        method.AddParameter("ICurrentUserService", "currentUserService")
                    //            .AddParameter("string[]?", "roles", param => param.WithDefaultValue("default"))
                    //            .AddParameter("string[]?", "policies", param => param.WithDefaultValue("default"));

                    //        method.Static();

                    //        method.AddIfStatement("(roles is null && policies is null) || (roles?.Length == 0 && policies?.Length == 0)", @if =>
                    //        {
                    //            @if.AddReturn("false");
                    //        });

                    //        method.AddIfStatement("roles != null && roles.Length > 0", @if =>
                    //        {
                    //            @if.AddForEachStatement("role", "roles", @for =>
                    //            {
                    //                @for.AddObjectInitStatement("var isInRole", "currentUserService.IsInRoleAsync(role).GetAwaiter().GetResult();");
                    //                @for.AddIfStatement("isInRole", roleIf =>
                    //                {
                    //                    roleIf.AddReturn("true");
                    //                });
                    //            });
                    //        });

                    //        method.AddIfStatement("policies != null && policies.Length > 0", @if =>
                    //        {
                    //            @if.AddForEachStatement("policy", "policies", @for =>
                    //            {
                    //                @for.AddObjectInitStatement("var isAuthorized", "currentUserService.AuthorizeAsync(policy).GetAwaiter().GetResult();");
                    //                @for.AddIfStatement("isAuthorized", roleIf =>
                    //                {
                    //                    roleIf.AddReturn("true");
                    //                });
                    //            });
                    //        });

                    //        method.AddReturn("false");
                    //    });

                    //}).AddEnum("MaskDataType", @enum =>
                    //{
                    //    @enum.AddLiteral("SetLength")
                    //        .AddLiteral("VariableLength")
                    //        .AddLiteral("PartialMask");
                    //});
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