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

namespace Intent.Modules.AzureFunctions.Templates.Isolated.Program
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class ProgramTemplate : CSharpTemplateBase<object>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.AzureFunctions.Isolated.Program";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public ProgramTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddUsing("Microsoft.Azure.Functions.Worker")
                .AddUsing("Microsoft.Extensions.DependencyInjection")
                .AddUsing("Microsoft.Extensions.Hosting")
                .AddUsing("System.Configuration")
                .AddTopLevelStatements(tls =>
                {
                    var hostConfigStatement = new CSharpStatement("new HostBuilder()")
                        .AddInvocation("ConfigureFunctionsWebApplication", i => i.OnNewLine())
                        .AddInvocation("ConfigureServices", cs => cs
                            .OnNewLine()
                            .AddArgument(
                                new CSharpLambdaBlock("(ctx, services)").WithExpressionBody(
                                    new CSharpStatement("var configuration = ctx.Configuration")
                                        .AddInvocation("services.AddApplication", i => i.AddArgument("configuration"))
                                        .AddInvocation("services.ConfigureApplicationSecurity", i => i.AddArgument("configuration"))
                                        .AddInvocation("services.AddInfrastructure", i => i.AddArgument("configuration"))
                                        .AddInvocation("services.AddApplicationInsightsTelemetryWorkerService")
                                        .AddInvocation("services.ConfigureFunctionsApplicationInsights")
                                )
                            )
                        )
                        .AddInvocation("Build");

                    tls.AddStatement(new CSharpAssignmentStatement("var host", hostConfigStatement));
                    tls.AddStatement("host.Run();", s => s.SeparatedFromPrevious());
                });
        }

        public override bool CanRunTemplate()
        {
            return OutputTarget.GetProject().TryGetMaxNetAppVersion(out var maxNetAppVersion) && maxNetAppVersion.Major >= 8;
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