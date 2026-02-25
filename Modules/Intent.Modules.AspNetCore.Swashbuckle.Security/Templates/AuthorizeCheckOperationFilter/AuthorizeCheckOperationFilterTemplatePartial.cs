using System;
using System.Collections.Generic;
using Intent.Engine;
using Intent.Modules.AspNetCore.Swashbuckle.Security.Settings;
using Intent.Modules.AspNetCore.Swashbuckle.Settings;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.Common.VisualStudio;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.AspNetCore.Swashbuckle.Security.Templates.AuthorizeCheckOperationFilter
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class AuthorizeCheckOperationFilterTemplate : CSharpTemplateBase<object>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.AspNetCore.Swashbuckle.Security.AuthorizeCheckOperationFilter";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public AuthorizeCheckOperationFilterTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            var isMicrosoftOpenApi_2_4_1 = OutputTarget.GetMaxNetAppVersion().Major >= 8;
            var openApiNamespace = isMicrosoftOpenApi_2_4_1 ? "Microsoft.OpenApi" : "Microsoft.OpenApi.Models";

            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddUsing("System")
                .AddUsing("System.Collections")
                .AddUsing("System.Collections.Generic")
                .AddUsing("System.Linq")
                .AddUsing("Microsoft.AspNetCore.Authorization")
                .AddUsing(openApiNamespace)
                .AddClass($"AuthorizeCheckOperationFilter", @class =>
                {
                    @class.ImplementsInterface(UseType("Swashbuckle.AspNetCore.SwaggerGen.IOperationFilter"));

                    @class.AddMethod("void", "Apply", method =>
                    {
                        method.AddParameter(UseType($"{openApiNamespace}.OpenApiOperation"), "operation")
                            .AddParameter(UseType("Swashbuckle.AspNetCore.SwaggerGen.OperationFilterContext"), "context");

                        if (isMicrosoftOpenApi_2_4_1)
                        {
                            var authValue = ExecutionContext.Settings.GetSwaggerSettings().Authentication().Value;

                            method.AddIfStatement("!HasAuthorize(context)", block =>
                            {
                                block.AddStatement("return;");
                            });

                            method.AddIfStatement("operation.Security == null", block =>
                            {
                                block.AddStatement("operation.Security = new List<OpenApiSecurityRequirement>();");
                            });

                            method.AddStatement($@"var securityRequirement = new OpenApiSecurityRequirement
            {{
                {{ new OpenApiSecuritySchemeReference(""{authValue}"", context.Document), new List<string>() }}
            }};");
                            method.AddStatement("operation.Security.Add(securityRequirement);");
                        }
                        else
                        {
                            method.AddIfStatement("!HasAuthorize(context)", block =>
                            {
                                block.AddObjectInitStatement("var securityScheme", "new OpenApiSecurityScheme();");
                                block.AddObjectInitStatement("operation.Security", @"new List<OpenApiSecurityRequirement>
                {
                    new OpenApiSecurityRequirement
                    {
                        { securityScheme, Array.Empty<string>() }
                    }
                };");

                                block.AddStatement("return;");
                                block.SeparatedFromNext();
                            });

                            method.AddStatement(@$"operation.Security.Add(new OpenApiSecurityRequirement
            {{
                [new OpenApiSecurityScheme
                {{
                    Reference = new OpenApiReference
                    {{
                        Type = ReferenceType.SecurityScheme,
                        Id = ""{ExecutionContext.Settings.GetSwaggerSettings().Authentication().Value}""
                    }}
                }}] = Array.Empty<string>()
            }});");
                        }

                    });

                    @class.AddMethod("bool", "HasAuthorize", method =>
                    {
                        method.Private();
                        method.Static();
                        method.AddParameter(UseType("Swashbuckle.AspNetCore.SwaggerGen.OperationFilterContext"), "context");

                        method.AddIfStatement("context.MethodInfo.GetCustomAttributes(true).OfType<AuthorizeAttribute>().Any()", block =>
                        {
                            block.AddStatement("return true;");
                            block.SeparatedFromNext();
                        });

                        method.AddStatement(@"return context.MethodInfo.DeclaringType != null
                && context.MethodInfo.DeclaringType.GetCustomAttributes(true).OfType<AuthorizeAttribute>().Any();");
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