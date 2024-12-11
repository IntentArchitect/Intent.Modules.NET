using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Hangfire.Api;
using Intent.Modelers.Services.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.DependencyInjection;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.Hangfire.Settings;
using Intent.Modules.Hangfire.Templates.HangfireDashboardAuthFilter;
using Intent.Modules.Hangfire.Templates.HangfireJobs;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;
using static Intent.Hangfire.Api.HangfireJobModelStereotypeExtensions.JobOptions;
using static Intent.Modules.Hangfire.Settings.HangfireSettings;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.Hangfire.Templates.HangfireConfiguration
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class HangfireConfigurationTemplate : CSharpTemplateBase<object>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.Hangfire.HangfireConfiguration";

        private readonly IOutputTarget _outputTarget;

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public HangfireConfigurationTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            _outputTarget = outputTarget;

            AddNugetDependency(NugetPackages.HangfireCore(OutputTarget));
            AddNugetDependency(NugetPackages.HangfireAspNetCore(OutputTarget));


            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddUsing("Hangfire")
                .AddUsing("Microsoft.Extensions.Configuration")
                .AddUsing("Microsoft.Extensions.DependencyInjection")
                .AddClass($"HangfireConfiguration", @class =>
                {
                    @class.Static();
                    @class.AddMethod("IServiceCollection", "ConfigureHangfire", method =>
                    {
                        method.Static();
                        method.AddParameter("IServiceCollection", "services", p => p.WithThisModifier());
                        method.AddParameter("IConfiguration", "configuration");

                        method.AddInvocationStatement("services.AddHangfire", stmt =>
                        {
                            var lambdaBlock = new CSharpLambdaBlock("cfg")
                                .AddStatement($"cfg.SetDataCompatibilityLevel(CompatibilityLevel.Version_180);")
                                .AddStatement("cfg.UseSimpleAssemblyNameTypeSerializer();")
                                .AddStatement("cfg.UseRecommendedSerializerSettings();");

                            if (ExecutionContext.Settings.GetHangfireSettings().Storage().AsEnum() != StorageOptionsEnum.None)
                            {
                                lambdaBlock.AddStatement(AddStorageUseStatement());
                            }

                            stmt.AddArgument(lambdaBlock);

                            if (ExecutionContext.Settings.GetHangfireSettings().ConfigureAsHangfireServer())
                            {
                                method.AddInvocationStatement("services.AddHangfireServer", stmt =>
                                {
                                    var queues = ExecutionContext.MetadataManager.Services(ExecutionContext.GetApplicationConfig().Id).GetHangfireQueueModels();
                                    var queueNames = queues.Select(q => $"\"{q.Name.ToLower()}\"").ToList();
                                    if (!queueNames.Contains("\"default\""))
                                    {
                                        queueNames.Add("\"default\"");
                                    }

                                    var lambdaBlock = new CSharpLambdaBlock("opt")
                                        .AddStatement($"opt.Queues = [{string.Join(",", queueNames)}];");

                                    if (!string.IsNullOrWhiteSpace(ExecutionContext.Settings.GetHangfireSettings().WorkerCount()) &&
                                        int.TryParse(ExecutionContext.Settings.GetHangfireSettings().WorkerCount(), out var result) && result > 0)
                                    {
                                        lambdaBlock.AddStatement($"opt.WorkerCount = {ExecutionContext.Settings.GetHangfireSettings().WorkerCount()};");
                                    }

                                    stmt.AddArgument(lambdaBlock);
                                });
                            }

                            method.AddReturn("services");
                        });

                        if (ExecutionContext.InstalledModules.Any(p => p.ModuleId == "Intent.AspNetCore"))
                        {
                            AddApNetCoreUseHangfire(@class);
                        }
                        else
                        {
                            AddHostUseHangfire(@class);
                        }
                    });
                });
        }

        public override void BeforeTemplateExecution()
        {
            base.BeforeTemplateExecution();

            ExecutionContext.EventDispatcher.Publish(ServiceConfigurationRequest
                .ToRegister("ConfigureHangfire", ServiceConfigurationRequest.ParameterType.Configuration)
                .HasDependency(this));

            if (ExecutionContext.InstalledModules.Any(p => p.ModuleId == "Intent.AspNetCore"))
            {
                ExecutionContext.EventDispatcher.Publish(ApplicationBuilderRegistrationRequest.ToRegister(
                        extensionMethodName: $"UseHangfire", ServiceConfigurationRequest.ParameterType.Configuration)
                    .WithPriority(100));
            }
            else
            {
                ExecutionContext.EventDispatcher.Publish(ApplicationBuilderRegistrationRequest.ToRegister(
                        extensionMethodName: $"UseHangfire")
                    .WithPriority(100));
            }
        }

        private string AddStorageUseStatement() => ExecutionContext.Settings.GetHangfireSettings().Storage().AsEnum() switch
        {
            StorageOptionsEnum.None => "",
            StorageOptionsEnum.InMemory => AddInMemoryStorageUseStatement(),
            StorageOptionsEnum.SQLServer => AddSqlServerStorageUseStatement(),
            _ => ""
        };

        private string AddInMemoryStorageUseStatement()
        {
            AddNugetDependency(NugetPackages.HangfireInMemory(OutputTarget));
            return "cfg.UseInMemoryStorage();";
        }

        private string AddSqlServerStorageUseStatement()
        {
            AddNugetDependency(NugetPackages.HangfireSqlServer(OutputTarget));
            AddNugetDependency(NugetPackages.MicrosoftDataSqlClient(OutputTarget));
            AddUsing("System");

            var x = new CSharpObjectInitializerBlock($"new {UseType("Hangfire.SqlServer.SqlServerStorageOptions")}")
                .AddObjectInitStatement("CommandBatchMaxTimeout", "TimeSpan.FromMinutes(5)")
                .AddObjectInitStatement("SlidingInvisibilityTimeout", "TimeSpan.FromMinutes(5)")
                .AddObjectInitStatement("QueuePollInterval", "TimeSpan.Zero")
                .AddObjectInitStatement("UseRecommendedIsolationLevel", "true")
                .AddObjectInitStatement("DisableGlobalLocks ", "true");

            var statement = new CSharpStatement("cfg")
                .AddInvocation("UseSqlServerStorage", config =>
                {
                    config.AddArgument("configuration.GetConnectionString(\"DefaultConnection\")");
                    config.AddArgument(x);
                }).AddInvocation($"WithJobExpirationTimeout", config =>
                {
                    var retentionHours = 24;

                    if (int.TryParse(ExecutionContext.Settings.GetHangfireSettings().JobRetentionHours(), out var result))
                    {
                        retentionHours = result > 0 ? result : retentionHours;
                    }

                    config.AddArgument($"TimeSpan.FromHours({retentionHours})");
                });

            return statement.ToString();
        }

        private void AddApNetCoreUseHangfire(CSharpClass @class)
        {
            @class.AddMethod("IApplicationBuilder", "UseHangfire", method =>
            {
                AddUsing("Microsoft.AspNetCore.Builder");

                method.Static();
                method.AddParameter("IApplicationBuilder", "app", p => p.WithThisModifier());
                method.AddParameter("IConfiguration", "configuration");

                if (ExecutionContext.Settings.GetHangfireSettings().ShowDashboard())
                {
                    method.AddObjectInitializerBlock("var dashboardOptions = new DashboardOptions", config =>
                    {
                        config.AddInitStatement("Authorization", $"[new {GetTypeName(HangfireDashboardAuthFilterTemplate.TemplateId)}()]");

                        if (!string.IsNullOrWhiteSpace(ExecutionContext.Settings.GetHangfireSettings().DashboardTitle()))
                        {
                            config.AddInitStatement("DashboardTitle", $"\"{ExecutionContext.Settings.GetHangfireSettings().DashboardTitle()}\"");
                        }

                        if (ExecutionContext.Settings.GetHangfireSettings().ReadOnlyDashboard())
                        {
                            config.AddInitStatement("IsReadOnlyFunc ", $"(DashboardContext context) => true");
                        }

                        config.WithSemicolon();
                    });

                    method.AddInvocationStatement($"app.UseHangfireDashboard", useDashConfig =>
                    {
                        useDashConfig.AddArgument($"\"{ExecutionContext.Settings.GetHangfireSettings().DashboardURL()}\"");

                        if (ExecutionContext.Settings.GetHangfireSettings().ShowDashboard())
                        {
                            useDashConfig.AddArgument($"dashboardOptions");
                        }
                    });
                }

                AddHangfireJobs(method, "ApplicationServices");

                method.AddReturn("app");
            });
        }

        private void AddHostUseHangfire(CSharpClass @class)
        {
            @class.AddMethod("IHost", "UseHangfire", method =>
            {
                method.Static();
                method.AddParameter(UseType("Microsoft.Extensions.Hosting.IHost"), "app", p => p.WithThisModifier());

                method.AddObjectInitStatement("var configuration", $"app.Services.GetRequiredService<{UseType("Microsoft.Extensions.Configuration.IConfiguration")}>();");

                AddHangfireJobs(method, "Services");

                method.AddReturn("app");
            });
        }

        private void AddHangfireJobs(CSharpClassMethod method, string serviceProviderName)
        {
            foreach (var job in ExecutionContext.MetadataManager.Services(ExecutionContext.GetApplicationConfig().Id)
                                .GetHangfireJobModels().Where(m => m.GetJobOptions().Enabled()))
            {
                AddRecurringJobInvocation(method, job, serviceProviderName);
            }
        }

        private void AddRecurringJobInvocation(CSharpClassMethod method, HangfireJobModel job, string serviceProviderName)
        {
            var jobHandlerName = GetTypeName(HangfireJobsTemplate.TemplateId, job);
            var addJobInvocationStatement = $"app.{serviceProviderName}.GetService<IRecurringJobManager>().AddOrUpdate<{jobHandlerName}>";

            method.AddInvocationStatement(addJobInvocationStatement, addJobConfig =>
            {
                addJobConfig.AddArgument($"\"{job.Name.ToPascalCase()}\"");
                if (job.HasJobOptions() && !string.IsNullOrWhiteSpace(job.GetJobOptions().Queue()?.Name))
                {
                    addJobConfig.AddArgument($"\"{job.GetJobOptions().Queue()?.Name.ToLower()}\"");
                }
                addJobConfig.AddArgument(new CSharpStatement("j => j.ExecuteAsync()"));
                addJobConfig.AddArgument($"configuration.GetValue<string?>($\"Hangfire:Jobs:{job.Name.ToPascalCase()}:CronSchedule\") ?? \"{job.GetJobOptions().CronSchedule()}\"");
            });
        }

        private void AddDelayedJobInvocation(CSharpClassMethod method, HangfireJobModel job, string serviceProviderName)
        {
            var jobHandlerName = GetTypeName(HangfireJobsTemplate.TemplateId, job);
            var addJobInvocationStatement = $" app.{serviceProviderName}.GetService<IBackgroundJobClient>().Schedule<{jobHandlerName}>";

            method.AddInvocationStatement(addJobInvocationStatement, addJobConfig =>
            {
                if (job.HasJobOptions() && !string.IsNullOrWhiteSpace(job.GetJobOptions().Queue()?.Name))
                {
                    addJobConfig.AddArgument($"\"{job.GetJobOptions().Queue()?.Name.ToLower()}\"");
                }
                addJobConfig.AddArgument(new CSharpStatement("j => j.ExecuteAsync()"));
                addJobConfig.AddArgument($"TimeSpan.From{job.GetJobOptions().DelayTimeFrame().Value}({job.GetJobOptions().DelayValue().Value})");
            });
        }

        private void AddFireForgetJobInvocation(CSharpClassMethod method, HangfireJobModel job, string serviceProviderName)
        {
            var jobHandlerName = GetTypeName(HangfireJobsTemplate.TemplateId, job);
            var addJobInvocationStatement = $" app.{serviceProviderName}.GetService<IBackgroundJobClient>().Enqueue<{jobHandlerName}>";

            method.AddInvocationStatement(addJobInvocationStatement, addJobConfig =>
            {
                if (job.HasJobOptions() && !string.IsNullOrWhiteSpace(job.GetJobOptions().Queue()?.Name))
                {
                    addJobConfig.AddArgument($"\"{job.GetJobOptions().Queue()?.Name.ToLower()}\"");
                }
                addJobConfig.AddArgument(new CSharpStatement("j => j.ExecuteAsync()"));
            });
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