using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Metadata.WebApi.Api;
using Intent.Modelers.Services.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.DependencyInjection;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.FastEndpoints.Templates.ApiVersioningConfiguration
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class ApiVersioningConfigurationTemplate : CSharpTemplateBase<object>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.FastEndpoints.ApiVersioningConfiguration";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public ApiVersioningConfigurationTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            AddNugetDependency(NugetPackages.FastEndpointsAspVersioning(OutputTarget));

            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddUsing("Asp.Versioning")
                .AddUsing("Asp.Versioning.ApiExplorer")
                .AddUsing("Microsoft.Extensions.DependencyInjection")
                .AddClass($"ApiVersioningConfiguration", @class =>
                {
                    @class.Static();
                    @class.AddMethod("IServiceCollection", "ConfigureApiVersioning", method =>
                    {
                        method.Static();
                        method.AddParameter("IServiceCollection", "services", param => param.WithThisModifier());

                        var version = new CSharpStatement("v");
                        var apiVersionModel = ExecutionContext.MetadataManager.Services(ExecutionContext.GetApplicationConfig().Id).GetApiVersionModels().FirstOrDefault();
                        if (apiVersionModel is not null)
                        {
                            AddUsing("FastEndpoints.AspVersioning");
                            
                            foreach (var versionModel in apiVersionModel.Versions)
                            {
                                version = version.AddInvocation("HasApiVersion",
                                    inv => inv.AddArgument($"new ApiVersion({versionModel.Name.Replace("V", "")})")
                                        .OnNewLine()
                                        .WithoutSemicolon());
                            }

                            method.AddStatement("VersionSets", stmt => stmt.AddInvocation("CreateApi", inv => inv
                                .AddArgument(@""">>Api Version<<""")
                                .AddArgument(new CSharpLambdaBlock("v").WithExpressionBody(version))));
                        }

                        method.AddInvocationStatement("services.AddVersioning", stmt => stmt
                            .AddArgument(new CSharpLambdaBlock("options")
                                .AddStatement($@"options.AssumeDefaultVersionWhenUnspecified = true;")
                                .AddStatement($@"options.ReportApiVersions = true;")
                                .AddStatement($@"options.ApiVersionReader = ApiVersionReader.Combine(new UrlSegmentApiVersionReader());")));

                        method.AddInvocationStatement("services.AddApiVersioning", stmt => stmt
                            .WithoutSemicolon()
                            .SeparatedFromPrevious());
                        method.AddInvocationStatement(".AddApiExplorer", stmt => stmt
                            .AddArgument(new CSharpLambdaBlock("options")
                                .AddStatement($@"options.GroupNameFormat = ""'v'VVV"";")
                                .AddStatement($@"options.SubstituteApiVersionInUrl = true;")));
                        method.AddStatement("return services;");
                    });
                });
        }

        public override void BeforeTemplateExecution()
        {
            ExecutionContext.EventDispatcher.Publish(ServiceConfigurationRequest
                .ToRegister("ConfigureApiVersioning")
                .HasDependency(this));
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