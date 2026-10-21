using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.AppStartup;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.CSharp.VisualStudio;
using Intent.Modules.Common.Templates;
using Intent.Modules.VisualStudio.Projects.Api;
using Intent.Modules.VisualStudio.Projects.Templates.CoreWeb.AppSettings;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;
using Intent.Utils;

namespace Intent.Modules.VisualStudio.Projects.Templates.ServiceWorker.ServiceWorkerProgram
{
    public partial class ServiceWorkerProgramTemplate : CSharpTemplateBase<object>, ICSharpFileBuilderTemplate, IProgramTemplate, IAppStartupTemplate
    {
        public const string TemplateId = "Intent.VisualStudio.Projects.ServiceWorker.ServiceWorkerProgram";
        private readonly IAppStartupFile _startupFile;
        private readonly ExistingProgramShape _existingProgramShape;
        private readonly bool _isHandsOff;

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public ServiceWorkerProgramTemplate(IOutputTarget outputTarget, CSharpProjectNETModel model) : base(TemplateId, outputTarget, model)
        {
            var useTopLevelStatements = OutputTarget.GetProject().InternalElement.AsCSharpProjectNETModel()?.GetNETSettings()?.UseTopLevelStatements() == true;

            // Still registered when off, so that other modules resolving the Program template keep working
            // and a previously generated file isn't proposed for deletion
            _isHandsOff = IsHandsOff(model.GetStereotype(CSharpProjectNETModelStereotypeExtensions.NETSettings.DefinitionId));

            // Statement style comes from the setting above. From an existing file only the builder
            // declaration and run call are taken, so that Merge mode doesn't duplicate them.
            _existingProgramShape = ExistingProgramShape.Detect(ReadExistingProgramFile());
            if (_isHandsOff)
            {
                Logging.Log.Info(
                    $"\"Generate Program File\" is off for Worker project \"{model.Name}\", so its Program.cs was not " +
                    "created or changed. Startup code that other modules add to Program.cs (e.g. Serilog, Windows Service hosting) was not applied.");
            }
            else if (_existingProgramShape.Kind == ExistingProgramShapeKind.Unrecognised)
            {
                Logging.Log.Warning(
                    $"The existing Program.cs of Worker project \"{model.Name}\" was left untouched: it doesn't have exactly one " +
                    $"\"var builder = WebApplication.CreateBuilder(args);\" or \"Host.CreateApplicationBuilder(args)\" declaration, " +
                    $"one \"{ExistingProgramShape.BuildStatement}\" and one \"{ExistingProgramShape.RunStatement}\" or " +
                    $"\"{ExistingProgramShape.RunAsyncStatement}\", so it can't be merged into without duplicating statements. " +
                    "Startup code that other modules add to Program.cs (e.g. Serilog, Windows Service hosting) was not applied.");
            }

            CSharpFile = (useTopLevelStatements
                ? new CSharpFile(string.Empty, this.GetFolderPath())
                : new CSharpFile(this.GetNamespace(), this.GetFolderPath()))
                .IntentManagedMerge();

            if (useTopLevelStatements)
            {
                // Top-level statements need a using for what a Main method would see through its
                // enclosing namespace. Checked once every template is registered.
                CSharpFile.OnBuild(file =>
                {
                    var projectNamespace = this.GetNamespace();
                    if (IsNamespaceDeclared(projectNamespace, GetNamespacesGeneratedInThisProject()))
                    {
                        file.AddUsing(projectNamespace);
                    }
                });
            }

            if (_isHandsOff || _existingProgramShape.Kind == ExistingProgramShapeKind.Unrecognised)
            {
                // Leave the existing file untouched
                CSharpFile.WithOverwriteBehaviour(OverwriteBehaviour.OnceOff);
            }

            ProgramFile = new ProgramFile(
                template: this,
                usesMinimalHostingModel: true,
                usesTopLevelStatements: useTopLevelStatements);

            _startupFile = new AppStartupFile(
                template: this,
                usesMinimalHostingModel: true,
                usesTopLevelStatements: useTopLevelStatements);

            AddNugetDependency(NugetPackages.MicrosoftExtensionsHosting(outputTarget));

            if (_existingProgramShape.UsesWebApplicationBuilder)
            {
                CSharpFile.AddUsing("Microsoft.AspNetCore.Builder");
            }

            if (!useTopLevelStatements)
            {
                CSharpFile
                    .AddUsing("Microsoft.Extensions.Hosting")
                    .AddUsing("Microsoft.Extensions.Logging")
                    .AddUsing("Microsoft.Extensions.Logging.EventLog")
                    .AddUsing("Microsoft.Extensions.Logging.Configuration")
                    .AddUsing("Microsoft.Extensions.DependencyInjection")
                    .AddClass("Program", @class =>
                    {
                        AddApplicationLoggingConfig();
                        @class.AddMethod(_existingProgramShape.RunsAsync ? "Task" : "void", "Main", method =>
                        {
                            method.Static();
                            if (_existingProgramShape.RunsAsync)
                            {
                                method.Async();
                            }

                            method.AddParameter("string[]", "args");

                            ApplyStatements(_startupFile!, CSharpFile);
                        });
                    }, priority: int.MinValue);

                if (_existingProgramShape.RunsAsync)
                {
                    CSharpFile.AddUsing("System.Threading.Tasks");
                }
            }
            else
            {
                CSharpFile
                    .AddUsing("Microsoft.Extensions.DependencyInjection")
                    .AddUsing("Microsoft.Extensions.Hosting")
                    .AddTopLevelStatements();

                ApplyStatements(_startupFile!, CSharpFile);
            }
        }

        public IAppStartupFile StartupFile =>
            _startupFile ?? throw new InvalidOperationException(
                $"Based on options chosen in the Visual Studio designer, \"{TemplateId}\" " +
                $"is not responsible for app startup, ensure that you resolve the template with " +
                $"the role \"{IAppStartupTemplate.RoleName}\" to get the correct template.");

        public bool HasStartupFile => _startupFile is not null;

        public override bool CanRunTemplate()
        {
            return ShouldRunFor(_isHandsOff, _existingProgramShape.Kind) && base.CanRunTemplate();
        }

        /// <summary>
        /// Hands-off never creates the file. An existing one is still "generated" (as <c>OnceOff</c>, so
        /// unchanged) so that the Software Factory neither rewrites it nor proposes deleting it.
        /// </summary>
        internal static bool ShouldRunFor(bool isHandsOff, ExistingProgramShapeKind existingProgramShape)
        {
            return !isHandsOff || existingProgramShape != ExistingProgramShapeKind.None;
        }

        /// <summary>
        /// Only an explicit "Generate Program File" of false is hands-off. A project saved before the property
        /// existed has no value for it, and Worker projects always had Program.cs generated, so a missing
        /// value (or a missing stereotype) means on. The generated <c>GenerateProgramFile()</c> accessor
        /// can't be used here, since it reads a missing value as false.
        /// </summary>
        internal static bool IsHandsOff(IStereotype netSettings)
        {
            return netSettings != null && !netSettings.GetProperty("Generate Program File", true);
        }


        /// <summary>
        /// Whether <c>using <paramref name="namespace"/>;</c> would compile: something is declared in that
        /// namespace or one nested under it. A project's code can live under a different root than its
        /// name (dotnet/eShop's OrderProcessor project uses eShop.OrderProcessor.*), and a using for a
        /// namespace that doesn't exist fails with CS0246.
        /// </summary>
        internal static bool IsNamespaceDeclared(string @namespace, IEnumerable<string> declaredNamespaces)
        {
            if (string.IsNullOrWhiteSpace(@namespace))
            {
                return false;
            }

            return declaredNamespaces.Any(declared =>
                declared != null &&
                (declared == @namespace || declared.StartsWith($"{@namespace}.", StringComparison.Ordinal)));
        }

        private IEnumerable<string> GetNamespacesGeneratedInThisProject()
        {
            var projectId = OutputTarget.GetProject()?.Id;

            return ExecutionContext.GetApplicationTemplates()
                .OfType<IntentTemplateBase>()
                .Where(template => !ReferenceEquals(template, this) && template.OutputTarget.GetProject()?.Id == projectId)
                .OfType<IClassProvider>()
                .Select(template => template.Namespace);
        }

        private string ReadExistingProgramFile()
        {
            // TryGetExistingFileContent needs FileMetadata, which isn't configured until after the constructor
            var path = Path.Combine(OutputTarget.Location, this.GetFolderPath(), "Program.cs");
            return ExecutionContext.FileSystem.Exists(path)
                ? ExecutionContext.FileSystem.ReadAllText(path)
                : null;
        }

        private void ApplyStatements(IAppStartupFile startupFile, CSharpFile cSharpFile)
        {
            startupFile.ConfigureServices((hasStatements, _) =>
            {
                hasStatements.AddStatement(_existingProgramShape.BuilderStatement ?? ExistingProgramShape.DefaultBuilderStatement,
                    s => s.AddMetadata("is-builder-statement", true));

                var addServicesComment = new CSharpStatement("// Add services to the container.");
                hasStatements.AddStatement<IHasCSharpStatements, CSharpStatement>(addServicesComment, s => s
                    .AddMetadata("is-add-services-to-container-comment", true)
                    .SeparatedFromPrevious());
            });

            startupFile.ConfigureApp((hasStatements, _) =>
            {
                hasStatements.AddStatement<IHasCSharpStatements, CSharpStatement>(ExistingProgramShape.BuildStatement, s => s
                    // Rather than adding a "// Configure the HTTP request pipeline" line below
                    // this which looks weird when not using ASP.NET Core, we just attach the
                    // metadata to this statement.
                    .AddMetadata("is-configure-request-pipeline-comment", true)
                    .SeparatedFromPrevious());

                hasStatements.AddStatement(_existingProgramShape.RunCallStatement, s => s
                    .SeparatedFromPrevious());
            });
        }

        /// <summary>
        /// Written to this project's own appsettings.json only: an <c>AppSettingRegistrationRequest</c>
        /// would be claimed by every host project in the application.
        /// </summary>
        private void AddApplicationLoggingConfig()
        {
            if (_isHandsOff)
            {
                // They configure the logging this template sets up, which a hands-off Program.cs doesn't get
                return;
            }

            var appSettings = ExecutionContext.FindTemplateInstances<AppSettingsTemplate>(AppSettingsTemplate.TemplateId)
                .FirstOrDefault(x => x.OutputTarget.GetProject()?.Id == OutputTarget.GetProject()?.Id && x.Model.RuntimeEnvironment == null);
            if (appSettings == null)
            {
                return;
            }

            appSettings.AddSetting("Logging:LogLevel:Microsoft.Hosting.Lifetime", "Information");
            appSettings.AddSetting("Logging:EventLog:SourceName", ExecutionContext.GetApplicationConfig().Name);
            appSettings.AddSetting("Logging:EventLog:LogName", "Application");
            appSettings.AddSetting("Logging:EventLog:LogLevel:Microsoft", "Warning");
            appSettings.AddSetting("Logging:EventLog:LogLevel:Microsoft.Hosting.Lifetime", "Information");
        }

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
