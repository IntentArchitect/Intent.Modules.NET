using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks.Dataflow;
using System.Xml.Schema;
using Intent.Engine;
using Intent.Modelers.Services.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.DependencyInjection;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.CSharp.VisualStudio;
using Intent.Modules.Common.Templates;
using Intent.Modules.Hangfire.Api;
using Intent.Modules.Hangfire.Templates.HangfireDashboardAuthFilter;
using Intent.Modules.Hangfire.Templates.HangfireJobs;
using Intent.Modules.VisualStudio.Projects.Api;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;
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
            AddNugetDependency(NugetPackages.HangfireInMemory(OutputTarget));

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

                        method.AddInvocationStatement("services.AddHangfire", stmt => stmt
                        .AddArgument(new CSharpLambdaBlock("cfg")
                            .AddStatement($"cfg.SetDataCompatibilityLevel(CompatibilityLevel.Version_180);")
                            .AddStatement("cfg.UseSimpleAssemblyNameTypeSerializer();")
                            .AddStatement("cfg.UseRecommendedSerializerSettings();")
                            .AddStatement("cfg.ConfigureHangfireStorage(services, configuration);"))
                        );

                        if (_hangFireConfigurationModel is not null && _hangFireConfigurationModel.HasHangfireOptions() && _hangFireConfigurationModel.GetHangfireOptions().ConfigureAsHangfireServer())
                        {
                            method.AddInvocationStatement("services.AddHangfireServer", stmt =>
                            {
                                if (_hangFireConfigurationModel.GetHangfireOptions().WorkerCount().HasValue)
                                {
                                    stmt.AddArgument(new CSharpLambdaBlock("opt")
                                        .AddStatement($"opt.WorkerCount = {_hangFireConfigurationModel.GetHangfireOptions().WorkerCount().Value};"));
                                }
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

                    @class.AddMethod("IGlobalConfiguration", "ConfigureHangfireStorage", method =>
                    {
                        method.Static();
                        method.AddAttribute(CSharpIntentManagedAttribute.Fully().WithBodyIgnored());

                        method.AddParameter("IGlobalConfiguration", "cfg", p => p.WithThisModifier());
                        method.AddParameter("IServiceCollection", "services");
                        method.AddParameter("IConfiguration", "configuration");
                        method.AddStatement("cfg.UseInMemoryStorage();");

                        method.AddReturn("cfg");
                    });
                }));
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
                        config.AddInitStatement("Authorization", "[new HangfireDashboardAuthFilter()]");
                        
                        var dashAuthTemplate = this.ExecutionContext.FindTemplateInstance<ICSharpFileBuilderTemplate>(HangfireDashboardAuthFilterTemplate.TemplateId);
                        AddUsing(dashAuthTemplate?.Namespace);

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
                var jobTemplate = this.ExecutionContext.FindTemplateInstances<ICSharpFileBuilderTemplate>(HangfireJobsTemplate.TemplateId);
                AddUsing(jobTemplate.FirstOrDefault()?.Namespace);

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

        private static void AddRecurringJobInvocation(CSharpClassMethod method, HangfireJobModel job, string serviceProviderName)
        {
            var jobHandlerName = job.Name.ToPascalCase();
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

        private static void AddDelayedJobInvocation(CSharpClassMethod method, HangfireJobModel job, string serviceProviderName)
        {
            var jobHandlerName = job.Name.ToPascalCase();
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

        private static void AddFireForgetJobInvocation(CSharpClassMethod method, HangfireJobModel job, string serviceProviderName)
        {
            var jobHandlerName = job.Name.ToPascalCase();
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