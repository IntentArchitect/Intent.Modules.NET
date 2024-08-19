using System;
using System.Collections.Generic;
using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.AppStartup;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.CSharp.VisualStudio;
using Intent.Modules.Common.Templates;
using Intent.Modules.VisualStudio.Projects.Api;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.AspNetCore.Templates.Program
{
    [IntentManaged(Mode.Merge, Signature = Mode.Merge)]
    public partial class ProgramTemplate : CSharpTemplateBase<object>, ICSharpFileBuilderTemplate, IAppStartupTemplate, IProgramTemplate
    {
        private readonly IAppStartupFile _startupFile;

        [IntentManaged(Mode.Fully)]
        public const string TemplateId = "Intent.AspNetCore.Program";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public ProgramTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            var useTopLevelStatements = OutputTarget.GetProject().InternalElement.AsCSharpProjectNETModel()?.GetNETSettings()?.UseTopLevelStatements() == true;

            CSharpFile = useTopLevelStatements
                ? new CSharpFile(string.Empty, this.GetFolderPath()).AddUsing(this.GetNamespace())
                : new CSharpFile(this.GetNamespace(), this.GetFolderPath());
            ProgramFile = new ProgramFile(this);

            if (UseMinimalHostingModel)
            {
                _startupFile = new AppStartupFile(this);
            }

            switch (UseMinimalHostingModel, useTopLevelStatements)
            {
                // Generic hosting model with Program class and Main method
                case (false, false):
                    {
                        CSharpFile
                            .AddUsing("Microsoft.AspNetCore.Hosting")
                            .AddUsing("Microsoft.Extensions.Hosting")
                            .AddClass("Program", @class =>
                            {
                                @class.AddMethod("void", "Main", method =>
                                {
                                    method.Static();
                                    method.AddParameter("string[]", "args");
                                    AddGenericModelMainStatements(method);
                                });
                                @class.AddMethod("IHostBuilder", "CreateHostBuilder", method =>
                                {
                                    method.Static();
                                    method.AddParameter("string[]", "args");
                                    method.WithExpressionBody(GetGenericModelCreateHostStatement());
                                });
                            }, priority: int.MinValue);
                        break;
                    }
                // Generic hosting model with top-level statements
                case (false, true):
                    {
                        CSharpFile
                            .AddUsing("Microsoft.AspNetCore.Hosting")
                            .AddUsing("Microsoft.Extensions.Hosting")
                            .AddTopLevelStatements(tls =>
                            {
                                AddGenericModelMainStatements(tls);
                                tls.AddLocalMethod("IHostBuilder", "CreateHostBuilder", method =>
                                {
                                    method.Static();
                                    method.AddParameter("string[]", "args");
                                    method.WithExpressionBody(GetGenericModelCreateHostStatement());
                                });
                            }, priority: int.MinValue);
                        break;
                    }
                // Minimal hosting model with Program class and Main method
                case (true, false):
                    {
                        CSharpFile
                            .AddUsing("Microsoft.AspNetCore.Builder")
                            .AddUsing("Microsoft.Extensions.DependencyInjection")
                            .AddUsing("Microsoft.Extensions.Hosting")
                            .AddClass("Program", @class =>
                            {
                                @class.AddMethod("void", "Main", method =>
                                {
                                    method.Static();
                                    method.AddParameter("string[]", "args");
                                    AddMinimalModelStatements(method);
                                });
                            }, priority: int.MinValue);
                        break;
                    }
                // Minimal hosting model with top-level statements
                default:
                    {
                        CSharpFile
                            .AddUsing("Microsoft.AspNetCore.Builder")
                            .AddUsing("Microsoft.Extensions.DependencyInjection")
                            .AddUsing("Microsoft.Extensions.Hosting")
                            .AddTopLevelStatements(AddMinimalModelStatements, priority: int.MinValue);
                        break;
                    }
            }
        }

        public IAppStartupFile StartupFile =>
            _startupFile ?? throw new InvalidOperationException(
                $"Based on options chosen in the Visual Studio designer, \"{TemplateId}\" " +
                $"is not responsible for app startup, ensure that you resolve the template with " +
                $"the role \"{IAppStartupTemplate.RoleName}\" to get the correct template.");

        private static void AddMinimalModelStatements(IHasCSharpStatements hasStatements)
        {
            hasStatements.AddStatement("var builder = WebApplication.CreateBuilder(args);", s => s
                .AddMetadata("is-builder-statement", true));

            hasStatements.AddStatement("// Add services to the container.", s => s
                .AddMetadata("is-add-services-to-container-comment", true)
                .SeparatedFromPrevious());

            hasStatements.AddStatement("var app = builder.Build();", s => s
                .SeparatedFromPrevious());

            hasStatements.AddStatement("// Configure the HTTP request pipeline.", s => s
                .AddMetadata("is-configure-request-pipeline-comment", true)
                .SeparatedFromPrevious());

            hasStatements.AddStatement("app.Run();", s => s
                .SeparatedFromPrevious());
        }

        private static void AddGenericModelMainStatements(IHasCSharpStatements hasStatements)
        {
            hasStatements.AddStatement("CreateHostBuilder(args).Build().Run();",
                stmt => stmt.AddMetadata("host-run", true));
        }

        private CSharpStatement GetGenericModelCreateHostStatement()
        {
            return new CSharpMethodChainStatement("Host.CreateDefaultBuilder(args)")
                .AddChainStatement(new CSharpInvocationStatement("ConfigureWebHostDefaults")
                    .AddArgument(new CSharpLambdaBlock("webBuilder")
                        .AddStatement($"webBuilder.UseStartup<{this.GetStartupName()}>();"))
                    .WithoutSemicolon()
                );
        }

        private bool UseMinimalHostingModel => OutputTarget.GetProject().InternalElement.AsCSharpProjectNETModel()?.GetNETSettings()?.UseMinimalHostingModel() == true;

        public IProgramFile ProgramFile { get; }

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
