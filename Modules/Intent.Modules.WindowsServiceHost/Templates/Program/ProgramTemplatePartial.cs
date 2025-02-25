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

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.WindowsServiceHost.Templates.Program
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class ProgramTemplate : CSharpTemplateBase<object>, ICSharpFileBuilderTemplate, IProgramTemplate, IAppStartupTemplate
    {
        private readonly IAppStartupFile _startupFile;

        public const string TemplateId = "Intent.WindowsServiceHost.Program";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public ProgramTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            var useTopLevelStatements = OutputTarget.GetProject().InternalElement.AsCSharpProjectNETModel()?.GetNETSettings()?.UseTopLevelStatements() == true;

            CSharpFile = useTopLevelStatements
                ? new CSharpFile(string.Empty, this.GetFolderPath()).AddUsing(this.GetNamespace())
                : new CSharpFile(this.GetNamespace(), this.GetFolderPath());
            ProgramFile = new ProgramFile(this);

            _startupFile = new AppStartupFile(this);
            AddNugetDependency(NugetPackages.MicrosoftExtensionsHosting(outputTarget));
            AddNugetDependency(NugetPackages.MicrosoftExtensionsHostingWindowsServices(outputTarget));
            AddNugetDependency(NugetPackages.MicrosoftExtensionsDependencyInjection(outputTarget));
            AddNugetDependency(NugetPackages.MicrosoftExtensionsConfigurationAbstractions(outputTarget));
            AddNugetDependency(NugetPackages.MicrosoftExtensionsConfigurationBinder(outputTarget));
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

                            ApplyMinimalHostingModelStatements(_startupFile!, CSharpFile);
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

                ApplyMinimalHostingModelStatements(_startupFile!, CSharpFile);
            }
        }

        public IAppStartupFile StartupFile =>
            _startupFile ?? throw new InvalidOperationException(
                $"Based on options chosen in the Visual Studio designer, \"{TemplateId}\" " +
                $"is not responsible for app startup, ensure that you resolve the template with " +
                $"the role \"{IAppStartupTemplate.RoleName}\" to get the correct template.");

        public bool HasStartupFile => _startupFile is not null;
        
        private void ApplyMinimalHostingModelStatements(IAppStartupFile startupFile, CSharpFile cSharpFile)
        {
            startupFile.ConfigureServices((hasStatements, _) =>
            {
                hasStatements.AddStatement("HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);",
                    s => s.AddMetadata("is-builder-statement", true));
                hasStatements.AddStatement($@"builder.Services.AddWindowsService(options =>
            {{
                options.ServiceName = ""{ExecutionContext.GetApplicationConfig().Name}"";
            }});");
                hasStatements.AddStatement("LoggerProviderOptions.RegisterProviderOptions<EventLogSettings, EventLogLoggerProvider>(builder.Services);");

                var addHostedServiceStatement = new CSharpStatement("builder.Services.AddHostedService<WindowsBackgroundService>();");
                hasStatements.AddStatement(addHostedServiceStatement);

                var addServicesComment = new CSharpStatement("// Add services to the container.");
                hasStatements.AddStatement(addServicesComment, s => s
                    .AddMetadata("is-add-services-to-container-comment", true)
                    .SeparatedFromPrevious());

                cSharpFile.AfterBuild(_ =>
                {
                    var statements = hasStatements.Statements;

                    statements.Remove(addServicesComment);
                    addHostedServiceStatement.InsertBelow(addServicesComment);

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