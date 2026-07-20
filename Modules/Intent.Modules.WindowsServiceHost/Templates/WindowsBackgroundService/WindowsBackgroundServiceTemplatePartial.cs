using System;
using System.Collections.Generic;
using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.AppStartup;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;
using Microsoft.VisualBasic;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.WindowsServiceHost.Templates.WindowsBackgroundService
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class WindowsBackgroundServiceTemplate : CSharpTemplateBase<object>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.WindowsServiceHost.WindowsBackgroundService";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public WindowsBackgroundServiceTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddUsing("System")
                .AddUsing("System.Threading")
                .AddUsing("System.Threading.Tasks")
                .AddUsing("Microsoft.Extensions.Hosting")
                .AddUsing("Microsoft.Extensions.Logging")
                .AddUsing("Microsoft.Extensions.DependencyInjection")
                .AddClass($"WindowsBackgroundService", @class =>
                {
                    @class.WithBaseType("BackgroundService");
                    @class.AddConstructor(ctor =>
                    {
                        ctor.AddParameter("IServiceProvider", "globalServiceProvider", param =>
                        {
                            param.IntroduceReadonlyField();
                        });
                        ctor.AddParameter("ILogger<WindowsBackgroundService>", "logger", param =>
                        {
                            param.IntroduceReadonlyField();
                        });
                    });
                    @class.AddMethod("Task", "ExecuteAsync", method =>
                    {
                        method
                            .Protected()
                            .Override()
                            .Async()
                            .AddParameter("CancellationToken", "stoppingToken");
                        ;

                        method.AddStatements(@"try
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using var scopedServiceProvder =  _globalServiceProvider.CreateAsyncScope();
                //Add Custom Dispatching here using the scopedServiceProvder

                await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
            }
        }
        catch (OperationCanceledException)
        {
            // When the stopping token is canceled, for example, a call made from services.msc,
            // we shouldn't exit with a non-zero exit code. In other words, this is expected...
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ""{Message}"", ex.Message);

            // Terminates this process and returns an exit code to the operating system.
            // This is required to avoid the 'BackgroundServiceExceptionBehavior', which
            // performs one of two scenarios:
            // 1. When set to ""Ignore"": will do nothing at all, errors cause zombie services.
            // 2. When set to ""StopHost"": will cleanly stop the host, and log errors.
            //
            // In order for the Windows Service Management system to leverage configured
            // recovery options, we need to terminate the process with a non-zero exit code.
            Environment.Exit(1);
        }
".ConvertToStatements());
                    });
                });
        }

        public override void AfterTemplateRegistration()
        {
            var appStartup = ExecutionContext.FindTemplateInstance<IAppStartupTemplate>(IAppStartupTemplate.RoleName, OutputTarget);

            appStartup.AddNugetDependency(NugetPackages.MicrosoftExtensionsHostingWindowsServices(appStartup.OutputTarget));
            appStartup.AddNugetDependency(NugetPackages.MicrosoftExtensionsDependencyInjection(appStartup.OutputTarget));
            appStartup.AddNugetDependency(NugetPackages.MicrosoftExtensionsConfigurationAbstractions(appStartup.OutputTarget));
            appStartup.AddNugetDependency(NugetPackages.MicrosoftExtensionsConfigurationBinder(appStartup.OutputTarget));

            appStartup.AddUsing("System");
            appStartup.AddUsing("Microsoft.Extensions.Logging");
            appStartup.AddUsing("Microsoft.Extensions.Logging.EventLog");
            appStartup.AddUsing("Microsoft.Extensions.Logging.Configuration");
            appStartup.AddUsing("Microsoft.Extensions.DependencyInjection");

            appStartup.CSharpFile.OnBuild(_ =>
            {
                appStartup.StartupFile.ConfigureServices((hasStatements, _) =>
                {
                    hasStatements.AddStatement($@"builder.Services.AddWindowsService(options =>
                {{
                    options.ServiceName = ""{ExecutionContext.GetApplicationConfig().Name}"";
                }});");

                    hasStatements.AddIfStatement("OperatingSystem.IsWindows()", ifs =>
                    {
                        ifs.AddStatement("LoggerProviderOptions.RegisterProviderOptions<EventLogSettings, EventLogLoggerProvider>(builder.Services);");
                    });

                    var addHostedServiceStatement = new CSharpStatement("builder.Services.AddHostedService<WindowsBackgroundService>();");
                    hasStatements.AddStatement<IHasCSharpStatements, CSharpStatement>(addHostedServiceStatement);

                    var addServicesComment = new CSharpStatement("// Add services to the container.");
                    hasStatements.AddStatement<IHasCSharpStatements, CSharpStatement>(addServicesComment, s => s
                        .AddMetadata("is-add-services-to-container-comment", true)
                        .SeparatedFromPrevious());

                    appStartup.CSharpFile.AfterBuild(_ =>
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
            });

            base.AfterTemplateRegistration();
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