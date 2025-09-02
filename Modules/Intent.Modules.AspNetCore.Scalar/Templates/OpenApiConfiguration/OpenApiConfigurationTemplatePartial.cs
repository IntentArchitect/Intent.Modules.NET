using System;
using System.Collections.Generic;
using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.CSharp.VisualStudio;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.AspNetCore.Scalar.Templates.OpenApiConfiguration
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class OpenApiConfigurationTemplate : CSharpTemplateBase<object>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.AspNetCore.Scalar.OpenApiConfiguration";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public OpenApiConfigurationTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            var versionCheck = OutputTarget.GetProject().GetMaxNetAppVersion();
            if (versionCheck.Major < 9)
            {
                throw new Exception("Microsoft.AspNetCore.OpenApi is only supported on .NET 9 or greater, please update your application.");
            }
            AddNugetDependency(NugetPackages.ScalarAspNetCore(OutputTarget));
            AddNugetDependency(NugetPackages.MicrosoftAspNetCoreOpenApi(OutputTarget));

            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddUsing("Microsoft.OpenApi.Models")
                .AddClass($"OpenApiConfiguration", @class =>
                {
                    @class.Static();

                    @class.AddMethod("IServiceCollection", "ConfigureOpenApi", configureOpenApi =>
                    {
                        configureOpenApi.Static();
                        configureOpenApi.AddParameter("IServiceCollection", "services", c => c.WithThisModifier());

                        
                        configureOpenApi.AddStatement(new CSharpInvocationStatement("services.AddOpenApi")
                            .AddArgument(new CSharpLambdaBlock("options"), argument =>
                            {
                                var lambdaBlock = (CSharpLambdaBlock)argument;

                                lambdaBlock.AddStatement(new CSharpInvocationStatement("options.AddDocumentTransformer")
                                        .AddArgument(new CSharpLambdaBlock("(document, context, cancellationToken)"), configure =>
                                        {
                                            configure.AddStatement("document.Components ??= new();");

                                            configure.AddStatement(new CSharpObjectInitializerBlock("document.Components.SecuritySchemes[\"Bearer\"] = new()")
                                                .AddInitStatement("Type", "Microsoft.OpenApi.Models.SecuritySchemeType.Http")
                                                .AddInitStatement("Scheme", "\"bearer\"")
                                                .AddInitStatement("BearerFormat", "\"JWT\"")
                                                .WithSemicolon());

                                            configure.AddStatement(new CSharpObjectInitializerBlock("var bearerSchemeReference = new OpenApiSecurityScheme")
                                                .AddInitStatement("Reference", new CSharpObjectInitializerBlock("new OpenApiReference")
                                                    .AddInitStatement("Id", "\"Bearer\"")
                                                    .AddInitStatement("Type", "ReferenceType.SecurityScheme")
                                                )
                                                .WithSemicolon());

                                            configure.AddStatement(new CSharpObjectInitializerBlock("var securityStatement = new Microsoft.OpenApi.Models.OpenApiSecurityRequirement")
                                                .AddInitStatement("[bearerSchemeReference]", "new List<string>()").WithSemicolon());

                                            configure.AddStatement("document.SecurityRequirements.Add(securityStatement);");

                                            configure.AddStatement("return Task.CompletedTask;");
                                        })
                                    );
                            })
                            .WithArgumentsOnNewLines()
                        );
                        configureOpenApi.AddReturn("services");
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