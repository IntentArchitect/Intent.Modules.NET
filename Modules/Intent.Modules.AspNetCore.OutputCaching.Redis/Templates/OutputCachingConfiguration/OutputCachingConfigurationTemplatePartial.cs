using System;
using System.Collections.Generic;
using System.Linq;
using Intent.AspNetCore.OutputCaching.Redis.Api;
using Intent.Engine;
using Intent.Exceptions;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Configuration;
using Intent.Modules.Common.CSharp.DependencyInjection;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.Constants;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;
using static Intent.AspNetCore.OutputCaching.Redis.Api.CachingPolicyModelStereotypeExtensions;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.AspNetCore.OutputCaching.Redis.Templates.OutputCachingConfiguration
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class OutputCachingConfigurationTemplate : CSharpTemplateBase<object>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.AspNetCore.OutputCaching.Redis.OutputCachingConfiguration";

        [IntentManaged(Mode.Ignore)]
        public OutputCachingConfigurationTemplate(IOutputTarget outputTarget, IList<CachingPolicyModel> models = null) : base(TemplateId, outputTarget, models)
        {
            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddUsing("System")
                .AddUsing("Microsoft.Extensions.Configuration")
                .AddUsing("Microsoft.Extensions.DependencyInjection")
                .AddClass($"OutputCachingConfiguration", @class =>
                {
                    AddNugetDependency(NugetPackages.StackExchangeRedis(OutputTarget));
                    @class.Static();
                    @class.AddMethod("IServiceCollection", "ConfigureOutputCaching", method =>
                    {
                        method
                            .Static()
                            .AddParameter("IServiceCollection", "services", p => p.WithThisModifier())
                            .AddParameter("IConfiguration", "configuration")
                            ;
                        method.AddStatement("var redisConfig = configuration.GetSection(\"OutputCaching\").Get<OutputCachingConfig>();");
                        method.AddIfStatement("redisConfig == null", stmt => { stmt.AddStatement("throw new Exception(\"Missing 'OutputCaching' configuration in appsettings.json \");"); });
                        method.AddInvocationStatement("services.AddStackExchangeRedisOutputCache", stmt => stmt
                            .AddArgument(new CSharpLambdaBlock("options")
                                .AddStatement($@"options.Configuration = redisConfig.Configuration;")
                                .AddStatement($@"options.InstanceName = redisConfig.InstanceName;")));
                        method.AddInvocationStatement("services.AddOutputCache", stmt =>
                        {
                            stmt.AddArgument(new CSharpLambdaBlock("options"), x =>
                            {
                                var lambda = (CSharpLambdaBlock)x;
                                lambda.AddStatement($@"options.AddBasePolicy(b => b.NoCache());");
                                foreach (var cachingPolicy in models)
                                {
                                    var builderFluent = new CSharpMethodChainStatement("builder").WithoutSemicolon();
                                    var config = cachingPolicy.GetCachingConfig();
                                    if (config == null)
                                    {
                                        throw new ElementException(cachingPolicy.InternalElement, "Missing 'Caching Config' stereotype.");
                                    }
                                    if (config.NoCaching())
                                    {
                                        builderFluent.AddChainStatement($"NoCache()");
                                    }
                                    else
                                    {
                                        if (config.Duration() is not null)
                                        {
                                            builderFluent.AddChainStatement($"Expire(TimeSpan.FromSeconds(configuration.GetValue<int?>(\"{GetTimeoutConfigSetting(cachingPolicy)}\") ?? {config.Duration()}))");
                                        }
                                        if (!string.IsNullOrEmpty(config.Tags()))
                                        {
                                            string tags = string.Join(", ", config.Tags().Split(",").Select(x => $"\"{x.Trim()}\""));
                                            builderFluent.AddChainStatement($"Tag({tags})");
                                        }
                                        if (!string.IsNullOrEmpty(config.VaryByRouteValueNames()))
                                        {
                                            string routeValueNames = string.Join(", ", config.VaryByRouteValueNames().Split(",").Select(x => $"\"{x.Trim()}\""));
                                            builderFluent.AddChainStatement($"SetVaryByRouteValue({routeValueNames})");
                                        }
                                        if (!string.IsNullOrEmpty(config.VaryByQueryKeys()))
                                        {
                                            string queryKeys = string.Join(", ", config.VaryByQueryKeys().Split(",").Select(x => $"\"{x.Trim()}\""));
                                            builderFluent.AddChainStatement($"SetVaryByQuery({queryKeys})");
                                        }
                                        if (!string.IsNullOrEmpty(config.VaryByHeaderNames()))
                                        {
                                            string headerNames = string.Join(", ", config.VaryByHeaderNames().Split(",").Select(x => $"\"{x.Trim()}\""));
                                            builderFluent.AddChainStatement($"SetVaryByHeader({headerNames})");
                                        }
                                        if (NothingConfigured(config))
                                        {
                                            builderFluent.AddChainStatement($"Expire(TimeSpan.FromSeconds(configuration.GetValue<int?>(\"{GetTimeoutConfigSetting(cachingPolicy)}\") ?? options.DefaultExpirationTimeSpan.Seconds))");
                                        }
                                    }
                                    lambda.AddInvocationStatement("options.AddPolicy", policy =>
                                    {
                                        policy.AddArgument($"\"{cachingPolicy.Name}\"");
                                        policy.AddArgument(new CSharpLambdaBlock("builder").WithExpressionBody(builderFluent));
                                    });
                                }

                            });
                        });

                        method.AddStatement("return services;");
                    });
                    @class.AddNestedClass($"OutputCachingConfig", child =>
                     {
                         child.Private();
                         child.AddProperty("string?", "Configuration");
                         child.AddProperty("string?", "InstanceName");
                     });
                });
        }

        internal bool NothingConfigured(CachingConfig config)
        {
            return config.Duration() is null
                && string.IsNullOrEmpty(config.Tags())
                && string.IsNullOrEmpty(config.VaryByRouteValueNames())
                && string.IsNullOrEmpty(config.VaryByQueryKeys())
                && string.IsNullOrEmpty(config.VaryByHeaderNames());
        }

        internal string GetTimeoutConfigSetting(CachingPolicyModel cachingPolicy)
        {
            return $"OutputCaching:Policies:{cachingPolicy.Name.ToCSharpIdentifier()}:Duration";
        }


        public override void AfterTemplateRegistration()
        {
            base.AfterTemplateRegistration();
            ExecutionContext.EventDispatcher.Publish(new InfrastructureRegisteredEvent(Infrastructure.Redis.Name)
                .WithProperty(Infrastructure.Redis.Property.ConnectionStringSettingPath, "OutputCaching:Configuration"));
        }

        public override void BeforeTemplateExecution()
        {
            if (!CanRunTemplate()) { return; }

            ExecutionContext.EventDispatcher.Publish(ServiceConfigurationRequest
                .ToRegister("ConfigureOutputCaching", ServiceConfigurationRequest.ParameterType.Configuration)
                .HasDependency(this));

            //This need to be called after UseCors(-2) and UseRouting()
            //https://learn.microsoft.com/en-us/aspnet/core/performance/caching/output?view=aspnetcore-8.0#add-the-middleware-to-the-app
            ExecutionContext.EventDispatcher.Publish(ApplicationBuilderRegistrationRequest
                .ToRegister("UseOutputCache")
                .WithPriority(-1));

            ExecutionContext.EventDispatcher.Publish(new AppSettingRegistrationRequest("OutputCaching", new
            {
                Configuration = "localhost:6379",
                InstanceName = ExecutionContext.GetApplicationConfig().Name.ToCSharpIdentifier() // "options.InstanceName, it’s going to be your application name" - https://nishanc.medium.com/redis-as-a-distributed-cache-on-net-6-0-949ef5b795ee
            }));

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