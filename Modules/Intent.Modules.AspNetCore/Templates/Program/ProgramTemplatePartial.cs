using System;
using System.Collections.Generic;
using System.IO;
using Intent.Engine;
using Intent.Modules.AspNetCore.Settings;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.AppStartup;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.CSharp.VisualStudio;
using Intent.Modules.Common.Templates;
using Intent.Modules.Constants;
using Intent.Modules.VisualStudio.Projects.Api;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;
using Intent.Utils;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.AspNetCore.Templates.Program
{
    [IntentManaged(Mode.Merge, Signature = Mode.Merge)]
    public partial class ProgramTemplate : CSharpTemplateBase<object>, ICSharpFileBuilderTemplate, IAppStartupTemplate, IProgramTemplate
    {
        private readonly IAppStartupFile? _startupFile;

        [IntentManaged(Mode.Fully)]
        public const string TemplateId = "Intent.AspNetCore.Program";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public ProgramTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            var useTopLevelStatements = OutputTarget.GetProject().InternalElement.AsCSharpProjectNETModel()?.GetNETSettings()?.UseTopLevelStatements() == true;

            // Merge mode so that registrations and other statements added by hand are kept
            CSharpFile = (useTopLevelStatements
                    ? new CSharpFile(string.Empty, this.GetFolderPath()).AddUsing(this.GetNamespace())
                    : new CSharpFile(this.GetNamespace(), this.GetFolderPath()))
                .IntentManagedMerge();
            ProgramFile = new ProgramFile(this);

            if (UseMinimalHostingModel)
            {
                _startupFile = new AppStartupFile(this);
            }

            // Keep the developer's run call (e.g. "await app.RunAsync();") rather than re-adding the default one
            var existingRunCall = ExistingProgramRunCall.Detect(ReadExistingProgramFile(), UseMinimalHostingModel);
            var runStatement = existingRunCall.GetStatementOrDefault(UseMinimalHostingModel);
            if (existingRunCall.Kind == ExistingProgramRunCallKind.Unrecognised)
            {
                var receiver = UseMinimalHostingModel
                    ? ExistingProgramRunCall.MinimalHostingReceiver
                    : ExistingProgramRunCall.GenericHostingReceiver;
                Logging.Log.Warning(
                    $"The existing Program.cs of project \"{OutputTarget.GetProject().Name}\" doesn't have exactly one recognised host run call, " +
                    $"so \"{runStatement}\" is being added. Recognised run calls are \"{receiver}.Run(...);\", \"await {receiver}.RunAsync(...);\" " +
                    $"(optionally with \".ConfigureAwait(...)\"), \"{receiver}.RunAsync(...).GetAwaiter().GetResult();\" and \"{receiver}.RunAsync(...).Wait();\".");
            }

            var mainReturnType = existingRunCall.UsesAsyncMain ? "Task" : "void";

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
                                @class.AddMethod(mainReturnType, "Main", method =>
                                {
                                    method.Static();
                                    if (existingRunCall.UsesAsyncMain)
                                    {
                                        method.Async();
                                    }

                                    method.AddParameter("string[]", "args");
                                    AddGenericModelMainStatements(method, runStatement);
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
                                AddGenericModelMainStatements(tls, runStatement);
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
                                @class.AddMethod(mainReturnType, "Main", method =>
                                {
                                    method.Static();
                                    if (existingRunCall.UsesAsyncMain)
                                    {
                                        method.Async();
                                    }

                                    method.AddParameter("string[]", "args");

                                    ApplyMinimalHostingModelStatements(_startupFile!, CSharpFile, runStatement);
                                });
                            }, priority: int.MinValue);
                        break;
                    }
                // Minimal hosting model with top-level statements
                case (true, true):
                    {
                        CSharpFile
                            .AddUsing("Microsoft.AspNetCore.Builder")
                            .AddUsing("Microsoft.Extensions.DependencyInjection")
                            .AddUsing("Microsoft.Extensions.Hosting")
                            .AddTopLevelStatements();

                        ApplyMinimalHostingModelStatements(_startupFile!, CSharpFile, runStatement);

                        break;
                    }
            }

            if (existingRunCall.UsesAsyncMain && !useTopLevelStatements)
            {
                CSharpFile.AddUsing("System.Threading.Tasks");
            }
        }

        public IAppStartupFile StartupFile =>
            _startupFile ?? throw new InvalidOperationException(
                $"Based on options chosen in the Visual Studio designer, \"{TemplateId}\" " +
                $"is not responsible for app startup, ensure that you resolve the template with " +
                $"the role \"{IAppStartupTemplate.RoleName}\" to get the correct template.");

        public bool HasStartupFile => _startupFile is not null;

        public override void BeforeTemplateExecution()
        {
            if (!(ExecutionContext.Settings.GetASPNETCoreSettings()?.EnableHTTPSRedirect() ?? true))
            {
                ExecutionContext.EventDispatcher.Publish(LaunchProfileHttpPortRequired.EventId, new Dictionary<string, string>());
            }
        }

        private static void ApplyMinimalHostingModelStatements(IAppStartupFile startupFile, CSharpFile cSharpFile, string runStatement)
        {
            startupFile.ConfigureServices((hasStatements, _) =>
            {
                var builderCreationStatement = new CSharpStatement("var builder = WebApplication.CreateBuilder(args);");
                var addServicesComment = new CSharpStatement("// Add services to the container.");

                hasStatements.AddStatement(builderCreationStatement, s => s.AddMetadata("is-builder-statement", true));

                hasStatements.AddStatement(addServicesComment, s => s
                    .AddMetadata("is-add-services-to-container-comment", true)
                    .SeparatedFromPrevious());

                cSharpFile.AfterBuild(_ =>
                {
                    var statements = hasStatements.Statements;

                    statements.Remove(addServicesComment);
                    builderCreationStatement.InsertBelow(addServicesComment);

                    var index = statements.IndexOf(addServicesComment);
                    if (statements.Count > index + 1)
                    {
                        statements[index + 1].BeforeSeparator = CSharpCodeSeparatorType.NewLine;
                    }
                }, 100_000);
            });

            startupFile.ConfigureApp((hasStatements, _) =>
            {
                hasStatements.AddStatement("var app = builder.Build();", s => s
                    .SeparatedFromPrevious());

                hasStatements.AddStatement("// Configure the HTTP request pipeline.", s => s
                    .AddMetadata("is-configure-request-pipeline-comment", true)
                    .SeparatedFromPrevious());

                hasStatements.AddStatement(runStatement, s => s
                    .AddMetadata("host-run", true)
                    .SeparatedFromPrevious());
            });
        }

        private static void AddGenericModelMainStatements(IHasCSharpStatements hasStatements, string runStatement)
        {
            hasStatements.AddStatement(runStatement,
                stmt => stmt.AddMetadata("host-run", true));
        }

        private string? ReadExistingProgramFile()
        {
            // TryGetExistingFileContent needs FileMetadata, which isn't configured until after the constructor.
            // System.IO rather than ExecutionContext.FileSystem, which needs a newer SDK than this module targets.
            var path = Path.Combine(OutputTarget.Location, this.GetFolderPath(), "Program.cs");
            return File.Exists(path)
                ? File.ReadAllText(path)
                : null;
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
