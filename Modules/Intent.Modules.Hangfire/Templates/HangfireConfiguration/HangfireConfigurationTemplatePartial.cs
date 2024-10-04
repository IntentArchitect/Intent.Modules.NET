using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Modelers.Services.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.DependencyInjection;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.Hangfire.Api;
using Intent.Modules.Hangfire.Templates.HangfireDashboardAuthFilter;
using Intent.Modules.Hangfire.Templates.HangfireJobs;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;
using static Intent.Modules.Hangfire.Api.HangfireConfigurationModelStereotypeExtensions.HangfireOptions;
using static Intent.Modules.Hangfire.Api.HangfireJobModelStereotypeExtensions.JobOptions;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.Hangfire.Templates.HangfireConfiguration
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class HangfireConfigurationTemplate : CSharpTemplateBase<IList<HangfireConfigurationModel>>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.Modules.Hangfire.HangfireConfiguration";

        private readonly IOutputTarget _outputTarget;

        private HangfireConfigurationModel _hangFireConfigurationModel;

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public HangfireConfigurationTemplate(IOutputTarget outputTarget, IList<HangfireConfigurationModel> model) : base(TemplateId, outputTarget, model)
        {
            // at this stage, this will exactly 0 or 1 models
            _hangFireConfigurationModel = model.FirstOrDefault();

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

                            if (_hangFireConfigurationModel is not null && _hangFireConfigurationModel.HasHangfireOptions()
                            && _hangFireConfigurationModel.GetHangfireOptions().Storage().AsEnum() != StorageOptionsEnum.None)
                            {
                                lambdaBlock.AddStatement(AddStorageUseStatement(_hangFireConfigurationModel));
                            }

                            stmt.AddArgument(lambdaBlock);

                            if (_hangFireConfigurationModel is not null && _hangFireConfigurationModel.HasHangfireOptions() && _hangFireConfigurationModel.GetHangfireOptions().ConfigureAsHangfireServer())
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

                                    if (_hangFireConfigurationModel.GetHangfireOptions().WorkerCount().HasValue)
                                    {
                                        lambdaBlock.AddStatement($"opt.WorkerCount = {_hangFireConfigurationModel.GetHangfireOptions().WorkerCount().Value};");
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

            if (_hangFireConfigurationModel is not null && _hangFireConfigurationModel.GetHangfireOptions().ConfigureAsHangfireServer())
            {
                ExecutionContext.EventDispatcher.Publish(ApplicationBuilderRegistrationRequest.ToRegister(
                            extensionMethodName: $"UseHangfire")
                        .WithPriority(100));
            }
        }

        private string AddStorageUseStatement(HangfireConfigurationModel model) => model.GetHangfireOptions().Storage().AsEnum() switch
        {
            StorageOptionsEnum.None => "",
            StorageOptionsEnum.InMemory => AddInMemoryStorageUseStatement(),
            StorageOptionsEnum.SQLServer => AddSqlServerStorageUseStatement(model),
            _ => ""
        };

        private string AddInMemoryStorageUseStatement()
        {
            AddNugetDependency(NugetPackages.HangfireInMemory(OutputTarget));
            return "cfg.UseInMemoryStorage();";
        }

        private string AddSqlServerStorageUseStatement(HangfireConfigurationModel model)
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
                    config.AddArgument($"TimeSpan.FromHours({model.GetHangfireOptions().JobRetentionHours()})");
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

                if (_hangFireConfigurationModel is not null && _hangFireConfigurationModel.HasHangfireOptions() && _hangFireConfigurationModel.GetHangfireOptions().ShowDashboard())
                {
                    method.AddObjectInitializerBlock("var dashboardOptions = new DashboardOptions", config =>
                    {
                        config.AddInitStatement("Authorization", $"[new {GetTypeName(HangfireDashboardAuthFilterTemplate.TemplateId)}()]");

                        if (!string.IsNullOrWhiteSpace(_hangFireConfigurationModel.GetHangfireOptions().DashboardTitle()))
                        {
                            config.AddInitStatement("DashboardTitle", $"\"{_hangFireConfigurationModel.GetHangfireOptions().DashboardTitle()}\"");
                        }

                        if (_hangFireConfigurationModel.GetHangfireOptions().ReadOnlyDashboard())
                        {
                            config.AddInitStatement("IsReadOnlyFunc ", $"(DashboardContext context) => true");
                        }

                        config.WithSemicolon();
                    });

                    method.AddInvocationStatement($"app.UseHangfireDashboard", useDashConfig =>
                    {
                        useDashConfig.AddArgument($"\"{_hangFireConfigurationModel.GetHangfireOptions().DashboardURL()}\"");

                        if (_hangFireConfigurationModel.HasHangfireOptions() && _hangFireConfigurationModel.GetHangfireOptions().ShowDashboard())
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
                method.AddParameter("IHost", "app", p => p.WithThisModifier());

                AddHangfireJobs(method, "Services");

                method.AddReturn("app");
            });
        }

        private void AddHangfireJobs(CSharpClassMethod method, string serviceProviderName)
        {
            foreach (var job in ExecutionContext.MetadataManager.Services(ExecutionContext.GetApplicationConfig().Id)
                                .GetHangfireJobModels().Where(m => m.GetJobOptions().Enabled()))
            {
                switch (job.GetJobOptions().JobType().AsEnum())
                {
                    case JobTypeOptionsEnum.Recurring:
                        AddRecurringJobInvocation(method, job, serviceProviderName);
                        break;
                    case JobTypeOptionsEnum.Delayed:
                        AddUsing("System");
                        AddDelayedJobInvocation(method, job, serviceProviderName);
                        break;
                    case JobTypeOptionsEnum.FireAndForget:
                        AddFireForgetJobInvocation(method, job, serviceProviderName);
                        break;
                    default:
                        break;
                }

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
                addJobConfig.AddArgument($"\"{job.GetJobOptions().CronSchedule()}\"");
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