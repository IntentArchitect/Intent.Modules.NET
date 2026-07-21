using System;
using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.AppStartup;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.CSharp.VisualStudio;
using Intent.Modules.Common.Templates;
using Intent.Modules.VisualStudio.Projects.Api;
using Intent.RoslynWeaver.Attributes;

namespace Intent.Modules.VisualStudio.Projects.Templates.ServiceWorker.ServiceWorkerProgram
{
    public partial class ServiceWorkerProgramTemplate : CSharpTemplateBase<object>, ICSharpFileBuilderTemplate, IProgramTemplate, IAppStartupTemplate
    {
        public const string TemplateId = "Intent.VisualStudio.Projects.ServiceWorker.ServiceWorkerProgram";
        private readonly IAppStartupFile _startupFile;

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public ServiceWorkerProgramTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            var useTopLevelStatements = OutputTarget.GetProject().InternalElement.AsCSharpProjectNETModel()?.GetNETSettings()?.UseTopLevelStatements() == true;

            CSharpFile = useTopLevelStatements
                ? new CSharpFile(string.Empty, this.GetFolderPath()).AddUsing(this.GetNamespace())
                : new CSharpFile(this.GetNamespace(), this.GetFolderPath());
            ProgramFile = new ProgramFile(
                template: this,
                usesMinimalHostingModel: true,
                usesTopLevelStatements: useTopLevelStatements);

            _startupFile = new AppStartupFile(
                template: this,
                usesMinimalHostingModel: true,
                usesTopLevelStatements: useTopLevelStatements);

            AddNugetDependency(NugetPackages.MicrosoftExtensionsHosting(outputTarget));

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
                        @class.AddMethod("void", "Main", method =>
                        {
                            method.Static();
                            method.AddParameter("string[]", "args");

                            ApplyStatements(_startupFile!, CSharpFile);
                        });
                    }, priority: int.MinValue);
            }
            else
            {
                CSharpFile
                    .AddUsing("Microsoft.AspNetCore.Builder")
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

        private void ApplyStatements(IAppStartupFile startupFile, CSharpFile cSharpFile)
        {
            startupFile.ConfigureServices((hasStatements, _) =>
            {
                hasStatements.AddStatement("HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);",
                    s => s.AddMetadata("is-builder-statement", true));

                var addServicesComment = new CSharpStatement("// Add services to the container.");
                hasStatements.AddStatement<IHasCSharpStatements, CSharpStatement>(addServicesComment, s => s
                    .AddMetadata("is-add-services-to-container-comment", true)
                    .SeparatedFromPrevious());
            });

            startupFile.ConfigureApp((hasStatements, _) =>
            {
                hasStatements.AddStatement<IHasCSharpStatements, CSharpStatement>("var app = builder.Build();", s => s
                    // Rather than adding a "// Configure the HTTP request pipeline" line below
                    // this which looks weird when not using ASP.NET Core, we just attach the
                    // metadata to this statement.
                    .AddMetadata("is-configure-request-pipeline-comment", true)
                    .SeparatedFromPrevious());

                hasStatements.AddStatement("app.Run();", s => s
                    .SeparatedFromPrevious());
            });
        }

        private void AddApplicationLoggingConfig()
        {
            this.ApplyAppSetting("Logging:LogLevel:Microsoft.Hosting.Lifetime", "Information");
            this.ApplyAppSetting("Logging:EventLog:SourceName", ExecutionContext.GetApplicationConfig().Name);
            this.ApplyAppSetting("Logging:EventLog:LogName", "Application");
            this.ApplyAppSetting("Logging:EventLog:LogLevel:Microsoft", "Warning");
            this.ApplyAppSetting("Logging:EventLog:LogLevel:Microsoft.Hosting.Lifetime", "Information");
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