using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Modules.AspNetCore.DistributedCaching.Settings;
using Intent.Modules.AspNetCore.DistributedCaching.Templates.DistributedCacheWithUnitOfWorkInterface;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Configuration;
using Intent.Modules.Common.CSharp.DependencyInjection;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.Constants;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.AspNetCore.DistributedCaching.Templates.DistributedCacheWithUnitOfWork
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class DistributedCacheWithUnitOfWorkTemplate : CSharpTemplateBase<object>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.AspNetCore.DistributedCaching.DistributedCacheWithUnitOfWork";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public DistributedCacheWithUnitOfWorkTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddUsing("System")
                .AddUsing("System.Collections.Concurrent")
                .AddUsing("System.Collections.Generic")
                .AddUsing("System.Threading")
                .AddUsing("System.Threading.Tasks")
                .AddUsing("Microsoft.Extensions.Caching.Distributed")
                .AddClass($"DistributedCacheWithUnitOfWork", @class =>
                {
                    @class.ImplementsInterface(this.GetDistributedCacheWithUnitOfWorkInterfaceName());
                    @class.AddField("AsyncLocal<ScopedData?>", "_scopedData", f => f.PrivateReadOnly().WithAssignment(new CSharpStatement("new AsyncLocal<ScopedData?>()")));

                    @class.AddConstructor(ctor =>
                    {
                        ctor.AddParameter("IDistributedCache", "distributedCache", p => p.IntroduceReadonlyField());
                    });

                    @class.AddMethod("IDisposable", "EnableUnitOfWork", method =>
                    {
                        method.AddStatement("_scopedData.Value ??= new ScopedData(() => _scopedData.Value = null);");
                        method.AddStatement("return _scopedData.Value;", s => s.SeparatedFromPrevious());
                    });

                    @class.AddMethod("byte[]?", "Get", method =>
                    {
                        method.AddParameter("string", "key");

                        method.AddIfStatement("_scopedData.Value == null", @if =>
                        {
                            @if.AddStatement("return _distributedCache.Get(key);");
                        });

                        method.AddIfStatement("!_scopedData.Value.Cache.TryGetValue(key, out var value)", @if =>
                        {
                            @if.AddStatement("return _distributedCache.Get(key);");
                        });

                        method.AddStatement("return value;");
                    });

                    @class.AddMethod("byte[]?", "GetAsync", method =>
                    {
                        method.Async();

                        method.AddParameter("string", "key");
                        method.AddOptionalCancellationTokenParameter("token");

                        method.AddIfStatement("_scopedData.Value == null", @if =>
                        {
                            @if.AddStatement("return await _distributedCache.GetAsync(key, token);");
                        });

                        method.AddIfStatement("!_scopedData.Value.Cache.TryGetValue(key, out var value)", @if =>
                        {
                            @if.AddStatement("return await _distributedCache.GetAsync(key, token);");
                        });

                        method.AddStatement("return value;");
                    });

                    @class.AddMethod("void", "Set", method =>
                    {
                        method.AddParameter("string", "key");
                        method.AddParameter("byte[]", "value");
                        method.AddParameter("DistributedCacheEntryOptions", "options");

                        method.AddIfStatement("_scopedData.Value == null", @if =>
                        {
                            @if.AddStatement("_distributedCache.Set(key, value, options);");
                            @if.AddStatement("return;");
                        });

                        method.AddStatement("_scopedData.Value.ActionQueue.Enqueue(cancellationToken => _distributedCache.SetAsync(key, value, cancellationToken));");
                        method.AddStatement("_scopedData.Value.Cache.AddOrUpdate(key, value, (_, _) => value);");
                    });

                    @class.AddMethod("void", "SetAsync", method =>
                    {
                        method.Async();

                        method.AddParameter("string", "key");
                        method.AddParameter("byte[]", "value");
                        method.AddParameter("DistributedCacheEntryOptions", "options");
                        method.AddOptionalCancellationTokenParameter("token");

                        method.AddIfStatement("_scopedData.Value == null", @if =>
                        {
                            @if.AddStatement("await _distributedCache.SetAsync(key, value, options, token);");
                            @if.AddStatement("return;");
                        });

                        method.AddStatement("_scopedData.Value.ActionQueue.Enqueue(cancellationToken => _distributedCache.SetAsync(key, value, cancellationToken));");
                        method.AddStatement("_scopedData.Value.Cache.AddOrUpdate(key, value, (_, _) => value);");
                    });

                    @class.AddMethod("void", "Refresh", method =>
                    {
                        method.AddParameter("string", "key");

                        method.AddIfStatement("_scopedData.Value == null", @if =>
                        {
                            @if.AddStatement("_distributedCache.Refresh(key);");
                            @if.AddStatement("return;");
                        });

                        method.AddStatement("_scopedData.Value.ActionQueue.Enqueue(cancellationToken => _distributedCache.RefreshAsync(key, cancellationToken));");
                    });

                    @class.AddMethod("void", "RefreshAsync", method =>
                    {
                        method.Async();

                        method.AddParameter("string", "key");
                        method.AddOptionalCancellationTokenParameter("token");

                        method.AddIfStatement("_scopedData.Value == null", @if =>
                        {
                            @if.AddStatement("await _distributedCache.RefreshAsync(key, token);");
                            @if.AddStatement("return;");
                        });

                        method.AddStatement("_scopedData.Value.ActionQueue.Enqueue(cancellationToken => _distributedCache.RefreshAsync(key, cancellationToken));");
                    });

                    @class.AddMethod("void", "Remove", method =>
                    {
                        method.AddParameter("string", "key");

                        method.AddIfStatement("_scopedData.Value == null", @if =>
                        {
                            @if.AddStatement("_distributedCache.Remove(key);");
                            @if.AddStatement("return;");
                        });

                        method.AddStatement("_scopedData.Value.ActionQueue.Enqueue(cancellationToken => _distributedCache.RemoveAsync(key, cancellationToken));");
                        method.AddStatement("_scopedData.Value.Cache.AddOrUpdate(key, (byte[]?)null, (_, _) => null);");
                    });

                    @class.AddMethod("void", "RemoveAsync", method =>
                    {
                        method.Async();

                        method.AddParameter("string", "key");
                        method.AddOptionalCancellationTokenParameter("token");

                        method.AddIfStatement("_scopedData.Value == null", @if =>
                        {
                            @if.AddStatement("await _distributedCache.RemoveAsync(key, token);");
                            @if.AddStatement("return;");
                        });

                        method.AddStatement("_scopedData.Value.ActionQueue.Enqueue(cancellationToken => _distributedCache.RemoveAsync(key, cancellationToken));");
                        method.AddStatement("_scopedData.Value.Cache.AddOrUpdate(key, (byte[]?)null, (_, _) => null);");
                    });

                    @class.AddMethod("void", "SaveChangesAsync", method =>
                    {
                        method.Async();
                        method.AddOptionalCancellationTokenParameter();

                        method.AddWhileStatement(@"!cancellationToken.IsCancellationRequested &&
                   _scopedData.Value != null &&
                   _scopedData.Value.ActionQueue.TryDequeue(out var action)", @while =>
                        {
                            @while.AddStatement("await action(cancellationToken);");
                        });
                    });

                    @class.AddNestedClass("ScopedData", nestedClass =>
                    {
                        nestedClass.Private().ImplementsInterface("IDisposable");

                        nestedClass.AddConstructor(ctor =>
                        {
                            ctor.AddParameter("Action", "disposeAction", p => p.IntroduceReadonlyField());
                        });

                        nestedClass.AddProperty("Queue<Func<CancellationToken, Task>>", "ActionQueue", p =>
                        {
                            p.WithoutSetter();
                            p.WithInitialValue("new Queue<Func<CancellationToken, Task>>()");
                        });

                        nestedClass.AddProperty("ConcurrentDictionary<string, byte[]?>", "Cache", p =>
                        {
                            p.WithoutSetter();
                            p.WithInitialValue("new ConcurrentDictionary<string, byte[]?>()");
                        });

                        nestedClass.AddMethod("void", "Dispose", m => m.WithExpressionBody("_disposeAction()"));
                    });
                });
        }


        public override void AfterTemplateRegistration()
        {
            base.AfterTemplateRegistration();


            var interfaceTemplate = ExecutionContext.FindTemplateInstance<IClassProvider>(DistributedCacheWithUnitOfWorkInterfaceTemplate.TemplateId);

            ExecutionContext.EventDispatcher.Publish(ContainerRegistrationRequest
                .ToRegister(this)
                .WithSingletonLifeTime()
                .ForConcern("Infrastructure")
                .ForInterface(interfaceTemplate)
                .HasDependency(interfaceTemplate));

            var template = ExecutionContext.FindTemplateInstance<ICSharpFileBuilderTemplate>(TemplateRoles.Infrastructure.DependencyInjection);
            if (template == null)
            {
                return;
            }

            if (ExecutionContext.Settings.GetDistributedCachingSettings().Provider().AsEnum() == DistributedCachingSettings.ProviderOptionsEnum.StackExchangeRedis)
            {
                ExecutionContext.EventDispatcher.Publish(new InfrastructureRegisteredEvent(Infrastructure.Redis.Name)
                    .WithProperty(Infrastructure.Redis.Property.ConnectionStringSettingPath, "ConnectionStrings:RedisCache"));
            }

            template.CSharpFile.OnBuild(file =>
            {
                var method = file.Classes.First().FindMethod("AddInfrastructure");

                switch (ExecutionContext.Settings.GetDistributedCachingSettings().Provider().AsEnum())
                {
                    case DistributedCachingSettings.ProviderOptionsEnum.Memory:
                        method.AddStatement("services.AddDistributedMemoryCache();");
                        break;
                    case DistributedCachingSettings.ProviderOptionsEnum.StackExchangeRedis:
                        method.AddInvocationStatement("services.AddStackExchangeRedisCache", invocation =>
                        {
                            template.AddNugetDependency(NugetPackages.MicrosoftExtensionsCachingStackExchangeRedis(template.OutputTarget));

                            invocation.AddArgument(new CSharpLambdaBlock("options"), argument =>
                            {
                                ((CSharpLambdaBlock)argument).AddStatement("options.Configuration = configuration.GetConnectionString(\"RedisCache\");");
                            });

                            this.ApplyAppSetting("ConnectionStrings:RedisCache", "localhost:6379");
                        });
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }, 10);
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