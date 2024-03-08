using System;
using System.Linq;
using System.Threading;
using Intent.Engine;
using Intent.Modules.AspNetCore.Logging.Serilog.Settings;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.AppStartup;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.DependencyInjection;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.CSharp.VisualStudio;
using Intent.Modules.Common.Plugins;
using Intent.Modules.VisualStudio.Projects.Api;
using Intent.Plugins.FactoryExtensions;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.FactoryExtension", Version = "1.0")]

namespace Intent.Modules.AspNetCore.Logging.Serilog.FactoryExtensions
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class SerilogStartupConfigurationExtension : FactoryExtensionBase
    {
        public override string Id => "Intent.Modules.AspNetCore.Logging.Serilog.SerilogStartupConfigurationExtension";

        [IntentManaged(Mode.Ignore)] public override int Order => 0;

        protected override void OnAfterTemplateRegistrations(IApplication application)
        {
            ConfigureStartup(application);
            ConfigureProgram(application);
        }

        private static void ConfigureStartup(IApplication application)
        {
            var template = application.FindTemplateInstance<IAppStartupTemplate>(IAppStartupTemplate.RoleName);
            template.AddNugetDependency(NugetPackages.SerilogAspNetCore(template.OutputTarget));
            template.AddUsing("Serilog");

            if (application.Settings.GetSerilogSettings().Sinks().Any(x => x.IsGraylog()))
            {
                template.AddNugetDependency(NugetPackages.SerilogSinksGraylog);
                template.AddNugetDependency(NugetPackages.SerilogEnrichersSpan);
            }

            application.EventDispatcher.Publish(
                ApplicationBuilderRegistrationRequest.ToRegister(
                        extensionMethodName: "UseSerilogRequestLogging")
                    .WithPriority(-100));
        }

        private static void ConfigureProgram(IApplication application)
        {
            var template = application.FindTemplateInstance<ICSharpFileBuilderTemplate>("App.Program");

            var usesMinimalHostingModel = template.OutputTarget.GetProject()?.InternalElement?.AsCSharpProjectNETModel()?.GetNETSettings()?.UseMinimalHostingModel() == true;
            if (usesMinimalHostingModel)
            {
                var usesTopLevelStatements = template.OutputTarget.GetProject()?.InternalElement?.AsCSharpProjectNETModel()?.GetNETSettings()?.UseTopLevelStatements() == true;
                template.CSharpFile.AfterBuild(file => MinimalHostingSerilogSetup(file, usesTopLevelStatements, template));
                return;
            }

            template.CSharpFile.OnBuild(file => ClassicProgramSerilogSetup(file, template), 10);
        }

        private static void ClassicProgramSerilogSetup(CSharpFile file, ICSharpFileBuilderTemplate template)
        {
            file.AddUsing("Serilog");
            file.AddUsing("Serilog.Events");

            var main = (IHasCSharpStatements)file.TopLevelStatements ?? file.Classes.First().FindMethod("Main");
            var hostBuilder = (IHasCSharpStatements)file.TopLevelStatements?.FindMethod("CreateHostBuilder") ?? file.Classes.First().FindMethod("CreateHostBuilder");

            var hostRunStmt = main.FindStatement(stmt => stmt.HasMetadata("host-run"));
            hostRunStmt.Remove();

            AddBootstrapLoggerStatement(main);

            main.AddTryBlock(block =>
                block.AddStatement(@"Log.Information(""Starting web host"");")
                    .AddStatement(hostRunStmt));
            
            main.AddCatchBlock(template.UseType("System.Exception"), "ex",
                block => block.AddStatement(@"Log.Fatal(ex, ""Host terminated unexpectedly"");"));
            main.AddFinallyBlock(block => block.AddStatement("Log.CloseAndFlush();"));

            var hostBuilderChain = (CSharpMethodChainStatement)hostBuilder.Statements.First();
            hostBuilderChain.Statements.Last().InsertAbove(new CSharpInvocationStatement("UseSerilog")
                .WithoutSemicolon()
                .AddArgument(GetSerilogConfigurationLambda()));
        }

        private static void MinimalHostingSerilogSetup(CSharpFile file, bool usesTopLevelStatements, ICSharpFileBuilderTemplate template)
        {
            file.AddUsing("Serilog");
            file.AddUsing("Serilog.Events");

            var targetBlock = usesTopLevelStatements
                ? (IHasCSharpStatements)file.TopLevelStatements
                : file.Classes.First().Methods.First(x => x.Name == "Main");

            var existingStatements = targetBlock.Statements.ToList();
            targetBlock.Statements.Clear();

            AddBootstrapLoggerStatement(targetBlock);

            targetBlock.AddTryBlock(tryBlock =>
            {
                tryBlock.AddStatements(existingStatements);
                existingStatements.FirstOrDefault()?.SeparatedFromPrevious();

                var hostBuilderStatement = tryBlock.FindStatement(x => x.HasMetadata("is-builder-statement"));

                // Add a line above the next statement:
                var nextStatementIndex = tryBlock.Statements.IndexOf(hostBuilderStatement) + 1;
                if (tryBlock.Statements.Count > nextStatementIndex)
                {
                    tryBlock.Statements[nextStatementIndex].SeparatedFromPrevious();
                }

                hostBuilderStatement.InsertBelow(new CSharpInvocationStatement("builder.Host.UseSerilog")
                    .AddArgument(GetSerilogConfigurationLambda())
                    .SeparatedFromPrevious());
            });

            targetBlock.AddCatchBlock(@catch => @catch
                .WithExceptionType(template.UseType("System.Exception"))
                .WithParameterName("ex")
                .AddStatement("Log.Fatal(ex, \"Application terminated unexpectedly\");"));

            targetBlock.AddFinallyBlock(@finally => @finally
                .AddStatement("Log.CloseAndFlush();"));
        }

        private static CSharpStatement GetSerilogConfigurationLambda()
        {
            return new CSharpLambdaBlock("(context, services, configuration)")
                .WithExpressionBody(new CSharpMethodChainStatement("configuration")
                    .AddChainStatement("ReadFrom.Configuration(context.Configuration)")
                    .AddChainStatement("ReadFrom.Services(services)"));
        }

        private static void AddBootstrapLoggerStatement(IHasCSharpStatements targetBlock)
        {
            targetBlock.AddMethodChainStatement("Log.Logger = new LoggerConfiguration()",
                stmt => stmt
                    .AddChainStatement("MinimumLevel.Override(\"Microsoft\", LogEventLevel.Information)")
                    .AddChainStatement("Enrich.FromLogContext()")
                    .AddChainStatement("WriteTo.Console()")
                    .AddChainStatement("CreateBootstrapLogger()"));
        }
    }
}