using System;
using System.Collections.Generic;
using Intent.Engine;
using Intent.Modules.AspNetCore.Swashbuckle.Templates.AuthorizeCheckOperationFilter;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.AspNetCore.Swashbuckle.Templates.SwashbuckleConfiguration
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public partial class SwashbuckleConfigurationTemplate : CSharpTemplateBase<object>, ICSharpFileBuilderTemplate
    {
        [IntentManaged(Mode.Fully)]
        public const string TemplateId = "Intent.AspNetCore.Swashbuckle.SwashbuckleConfiguration";

        [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
        public SwashbuckleConfigurationTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId,
            outputTarget, model)
        {
            AddNugetDependency(NugetPackages.SwashbuckleAspNetCore);
            AddUsing("Microsoft.AspNetCore.Builder");
            AddUsing("Microsoft.Extensions.Configuration");
            AddUsing("Microsoft.Extensions.DependencyInjection");
            AddUsing("Microsoft.OpenApi.Models");
            AddUsing("Swashbuckle.AspNetCore.SwaggerUI");

            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddClass("SwashbuckleConfiguration", @class =>
                {
                    @class.Static();
                    @class.AddMethod("IServiceCollection", "ConfigureSwagger",
                        method =>
                        {
                            method.Static();
                            method.AddParameter("IServiceCollection", "services", param =>
                            {
                                param.WithThisModifier();
                            });
                            method.AddParameter("IConfiguration", "configuration");
                            
                            method.AddStatement(new CSharpInvocationStatement("services.AddSwaggerGen")
                                .AddArgument(new CSharpLambdaBlock("options")
                                    .AddStatement(new CSharpInvocationStatement("options.SwaggerDoc")
                                        .AddArgument(@"""v1""")
                                        .AddArgument(new CSharpClassInitStatementBlock("new OpenApiInfo")
                                            .AddInitAssignment("Version",@"""v1""")
                                            .AddInitAssignment("Title", $@"""{OutputTarget.ApplicationName()} API""")
                                        )
                                        .WithArgumentsOnNewLines()
                                    )
                                    .AddStatement($@"options.OperationFilter<{GetTypeName(AuthorizeCheckOperationFilterTemplate.TemplateId)}>();")
                                )
                                .WithArgumentsOnNewLines()
                            );
                            method.AddStatement($@"return services;");
                        });
                    
                    @class.AddMethod("void", "UseSwashbuckle", method =>
                    {
                        method.AddStatement(new CSharpInvocationStatement("app.UseSwagger")
                            .AddArgument(new CSharpLambdaBlock("options")
                                .AddStatement(@"options.RouteTemplate = ""swagger/{documentName}/swagger.json"";"))
                            .WithArgumentsOnNewLines());

                        method.AddStatement(new CSharpInvocationStatement("app.UseSwaggerUI")
                            .AddArgument(new CSharpLambdaBlock("options")
                                .AddStatement(@"options.RoutePrefix = ""swagger"";")
                                .AddStatement(@"options.ConfigObject = new ConfigObject()")
                                .AddStatement(new CSharpStatementBlock()
                                    .AddStatement("Urls = new[]")
                                    .AddStatement(new CSharpStatementBlock()
                                        .AddStatement(
                                            $@"new UrlDescriptor() {{ Url = ""/swagger/v1/swagger.json"", Name = ""{OutputTarget.ApplicationName()} API V1"" }}"))
                                    .WithSemicolon())
                                .AddStatement(@"options.EnableDeepLinking();")
                                .AddStatement(@"options.DisplayOperationId();")
                                .AddStatement(@"options.DefaultModelsExpandDepth(-1);")
                                .AddStatement(@"options.DefaultModelsExpandDepth(2);")
                                .AddStatement(@"options.DefaultModelRendering(ModelRendering.Model);")
                                .AddStatement(@"options.DocExpansion(DocExpansion.List);")
                                .AddStatement(@"options.ShowExtensions();")
                                .AddStatement(@"options.EnableFilter(string.Empty);"))
                            .WithArgumentsOnNewLines());
                    });
                });
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        protected override CSharpFileConfig DefineFileConfig()
        {
            return CSharpFile.GetConfig();
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public override string TransformText()
        {
            return CSharpFile.ToString();
        }

        public CSharpFile CSharpFile { get; }
    }
}