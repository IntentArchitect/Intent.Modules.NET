using System;
using System.Collections.Generic;
using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.DependencyInjection;
using Intent.Modules.Common.CSharp.Templates;
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
        AddNugetDependency(NugetPackage.MicrosoftAspNetCoreMvcVersioning);
        AddNugetDependency(NugetPackage.MicrosoftAspNetCoreMvcVersioningApiExplorer);

        CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
            .AddUsing("Microsoft.AspNetCore.Mvc.Versioning")
            .AddClass($"ApiVersioningConfiguration", @class =>
            {
                @class.AddMethod("void", "ConfigureApiVersioning", method =>
                {
                    method.Static();
                    method.AddParameter("IServiceCollection", "services", parm => parm.WithThisModifier());
                    method.AddInvocationStatement("AddApiVersioning", stmt => stmt
                        .AddArgument(new CSharpLambdaBlock("options")
                            .AddStatement($@"options.AssumeDefaultVersionWhenUnspecified = false;")
                            .AddStatement($@"options.ReportApiVersions = true;")
                            .AddStatement($@"options.ApiVersionReader = ApiVersionReader.Combine(new UrlSegmentApiVersionReader());")));
                    method.AddInvocationStatement("AddVersionedApiExplorer", stmt => stmt
                        .AddArgument(new CSharpLambdaBlock("options")
                            .AddStatement($@"options.GroupNameFormat = ""'v'VVV"";")
                            .AddStatement($@"options.SubstituteApiVersionInUrl = true;")));
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