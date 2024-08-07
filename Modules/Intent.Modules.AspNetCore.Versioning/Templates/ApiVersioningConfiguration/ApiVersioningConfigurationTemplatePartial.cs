using System;
using System.Collections.Generic;
using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.DependencyInjection;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.CSharp.VisualStudio;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.AspNetCore.Versioning.Templates.ApiVersioningConfiguration;

[IntentManaged(Mode.Fully, Body = Mode.Merge)]
public partial class ApiVersioningConfigurationTemplate : CSharpTemplateBase<object>, ICSharpFileBuilderTemplate
{
    public const string TemplateId = "Intent.AspNetCore.Versioning.ApiVersioningConfiguration";

    [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
    public ApiVersioningConfigurationTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
    {
        AddNugetDependency(NugetPackages.AspVersioningMvc(outputTarget));
        AddNugetDependency(NugetPackages.AspVersioningMvcApiExplorer(outputTarget));

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
                    method.AddInvocationStatement("services.AddApiVersioning", stmt => stmt
                        .AddArgument(new CSharpLambdaBlock("options")
                            .AddStatement($@"options.AssumeDefaultVersionWhenUnspecified = true;")
                            .AddStatement($@"options.ReportApiVersions = true;")
                            .AddStatement($@"options.ApiVersionReader = ApiVersionReader.Combine(new UrlSegmentApiVersionReader());"))
                        .WithoutSemicolon());
                    method.AddStatement(".AddMvc()");
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
        ExecutionContext.EventDispatcher.Publish(new RemoveNugetPackageEvent("Microsoft.AspNetCore.Mvc.Versioning", OutputTarget));
        ExecutionContext.EventDispatcher.Publish(new RemoveNugetPackageEvent("Microsoft.AspNetCore.Mvc.Versioning.ApiExplorer", OutputTarget));

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