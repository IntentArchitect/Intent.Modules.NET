using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Modules.AspNetCore.DistributedCaching.Templates;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.Constants;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.EntityFrameworkCore.SecondLevelCaching.Templates.DistributedCacheServiceProvider
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class DistributedCacheServiceProviderTemplate : CSharpTemplateBase<object>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.EntityFrameworkCore.SecondLevelCaching.DistributedCacheServiceProvider";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public DistributedCacheServiceProviderTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            AddNugetDependency(NugetPackages.EFCoreSecondLevelCacheInterceptor(OutputTarget));
            AddNugetDependency(NugetPackages.MessagePack(OutputTarget));

            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddUsing("System")
                .AddUsing("System.Collections.Generic")
                .AddUsing("EFCoreSecondLevelCacheInterceptor")
                .AddUsing("MessagePack")
                .AddUsing("Microsoft.Extensions.Caching.Distributed")
                .AddUsing("Microsoft.Extensions.Logging")
                .AddClass($"DistributedCacheServiceProvider", @class =>
                {
                    // This implementation is based off
                    // https://github.com/VahidN/EFCoreSecondLevelCacheInterceptor/blob/5979c6038b255c1b661c8c26a6c48b7d850e92fc/src/EFCoreSecondLevelCacheInterceptor/EFEasyCachingCoreProvider.cs
                    // where it's been changed to use IDistributedCache.

                    @class.ImplementsInterface("IEFCacheServiceProvider");
                    @class.AddField("IDistributedCache", "_distributedCache", f => f.PrivateReadOnly());

                    @class.AddConstructor(ctor =>
                    {
                        ctor.AddStatement("_distributedCache = distributedCache;");
                        ctor.AddParameter(this.GetDistributedCacheWithUnitOfWorkInterfaceName(), "distributedCache");
                        ctor.AddParameter("IEFDebugLogger", "efDebugLogger", p => p.IntroduceReadonlyField());
                        ctor.AddParameter($"ILogger<{@class.Name}>", "logger", p => p.IntroduceReadonlyField());
                    });

                    @class.AddMethod("void", "InsertValue", method =>
                    {
                        method.AddParameter("EFCacheKey", "cacheKey");
                        method.AddParameter("EFCachedData?", "value");
                        method.AddParameter("EFCachePolicy", "cachePolicy");

                        method.AddIfStatement("cacheKey is null", @if =>
                        {
                            @if.AddStatement("throw new ArgumentNullException(nameof(cacheKey));");
                        });

                        method.AddIfStatement("cachePolicy is null", @if =>
                        {
                            @if.AddStatement("throw new ArgumentNullException(nameof(cachePolicy));");
                        });

                        method.AddStatement("value ??= new EFCachedData { IsNull = true };");

                        method.AddStatement("var keyHash = cacheKey.KeyHash;", s => s.SeparatedFromPrevious());

                        method.AddForEachStatement("rootCacheKey", "cacheKey.CacheDependencies", @for =>
                        {
                            @for.AddIfStatement("string.IsNullOrWhiteSpace(rootCacheKey)", @if =>
                            {
                                @if.AddStatement("continue;");
                            });

                            @for.AddStatement("var items = GetCacheItem<HashSet<string>>(rootCacheKey);");
                            @for.AddIfStatement("items is null", @if =>
                            {
                                @if.SeparatedFromPrevious(false);
                                @if.AddStatement("items = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { keyHash };");
                                @if.AddStatement("SetCacheItem(rootCacheKey, items, cachePolicy);");
                            });
                            @for.AddElseStatement(@else =>
                            {
                                @else.AddStatement("items.Add(keyHash);");
                                @else.AddStatement("SetCacheItem(rootCacheKey, items, cachePolicy);");
                            });
                        });

                        method.AddStatement("SetCacheItem(keyHash, value, cachePolicy);");
                    });

                    @class.AddMethod("void", "ClearAllCachedEntries", method =>
                    {
                        method.AddStatement("// NOP, unsupported");
                    });

                    @class.AddMethod("EFCachedData?", "GetValue", method =>
                    {
                        method.AddParameter("EFCacheKey", "cacheKey");
                        method.AddParameter("EFCachePolicy", "cachePolicy");

                        method.AddIfStatement("cacheKey is null", @if =>
                        {
                            @if.AddStatement("throw new ArgumentNullException(nameof(cacheKey));");
                        });

                        method.AddStatement("return GetCacheItem<EFCachedData>(cacheKey.KeyHash);");
                    });

                    @class.AddMethod("void", "InvalidateCacheDependencies", method =>
                    {
                        method.AddParameter("EFCacheKey", "cacheKey");

                        method.AddIfStatement("cacheKey is null", @if =>
                        {
                            @if.AddStatement("throw new ArgumentNullException(nameof(cacheKey));");
                        });

                        method.AddForEachStatement("rootCacheKey", "cacheKey.CacheDependencies", @for =>
                        {
                            @for.AddIfStatement("string.IsNullOrWhiteSpace(rootCacheKey)", @if =>
                            {
                                @if.AddStatement("continue;");
                            });

                            @for.AddStatement("var cachedValue = GetCacheItem<EFCachedData>(cacheKey.KeyHash);");
                            @for.AddStatement("var dependencyKeys = GetCacheItem<HashSet<string>>(rootCacheKey);");
                            @for.AddIfStatement("AreRootCacheKeysExpired(cachedValue, dependencyKeys)", @if =>
                            {
                                @if.SeparatedFromPrevious(false);
                                @if.AddIfStatement("_efDebugLogger.IsLoggerEnabled", ifLogging =>
                                {
                                    ifLogging.AddStatement("_logger.LogDebug(CacheableEventId.QueryResultInvalidated, \"Invalidated all of the cache entries due to early expiration of a root cache key[{RootCacheKey}].\", rootCacheKey);");
                                });

                                @if.AddStatement("ClearAllCachedEntries();");
                                @if.AddStatement("return;");
                            });

                            @for.AddStatement("ClearDependencyValues(dependencyKeys);");
                            @for.AddStatement("_distributedCache.Remove(rootCacheKey);");
                        });
                    });

                    @class.AddMethod("void", "ClearDependencyValues", method =>
                    {
                        method.Private();
                        method.AddParameter("HashSet<string>?", "dependencyKeys");

                        method.AddIfStatement("dependencyKeys is null", @if =>
                        {
                            @if.AddStatement("return;");
                        });

                        method.AddForEachStatement("dependencyKey", "dependencyKeys", @for =>
                        {
                            @for.AddStatement("_distributedCache.Remove(dependencyKey);");
                        });
                    });

                    @class.AddMethod("bool", "AreRootCacheKeysExpired", method =>
                    {
                        method.Private().Static();
                        method.AddParameter("EFCachedData?", "cachedValue");
                        method.AddParameter("HashSet<string>?", "dependencyKeys");
                        method.WithExpressionBody("cachedValue is not null && dependencyKeys is null");
                    });

                    @class.AddMethod("T?", "GetCacheItem", method =>
                    {
                        method.Private();
                        method.AddGenericParameter("T");
                        method.AddGenericTypeConstraint("T", c => c.AddType("class"));

                        method.AddParameter("string", "key");

                        method.AddStatement("var serialized = _distributedCache.Get(key);");
                        method.AddIfStatement("serialized == null", @if =>
                        {
                            @if.AddStatement("return null;");
                        });

                        method.AddStatement("return (T)MessagePackSerializer.Typeless.Deserialize(serialized)!;");
                    });

                    @class.AddMethod("void", "SetCacheItem", method =>
                    {
                        method.Private();
                        method.AddGenericParameter("T");
                        method.AddParameter("string", "key");
                        method.AddParameter("T", "value");
                        method.AddParameter("EFCachePolicy", "cachePolicy");

                        method.AddStatement("""
                                            var cacheEntryOptions = cachePolicy.CacheExpirationMode switch
                                            {
                                                CacheExpirationMode.Absolute => new DistributedCacheEntryOptions
                                                {
                                                    AbsoluteExpirationRelativeToNow = cachePolicy.CacheTimeout
                                                },
                                                CacheExpirationMode.Sliding => new DistributedCacheEntryOptions
                                                {
                                                    SlidingExpiration = cachePolicy.CacheTimeout
                                                },
                                                _ => throw new ArgumentOutOfRangeException()
                                            };
                                            """);

                        method.AddStatement("var serialized = MessagePackSerializer.Typeless.Serialize(value);");
                        method.AddStatement("_distributedCache.Set(key, serialized, cacheEntryOptions);");
                    });
                });
        }

        public override void AfterTemplateRegistration()
        {
            base.AfterTemplateRegistration();

            var template = ExecutionContext.FindTemplateInstance<ICSharpFileBuilderTemplate>(TemplateRoles.Infrastructure.DependencyInjection);
            if (template == null)
            {
                return;
            }

            template.CSharpFile.OnBuild(file =>
            {
                template.AddUsing("EFCoreSecondLevelCacheInterceptor");

                var method = file.Classes.First().FindMethod("AddInfrastructure");

                var addDbContext = method.FindStatement(x => x.Text.StartsWith("services.AddDbContext")) as IHasCSharpStatements;
                addDbContext?.AddStatement("options.AddInterceptors(sp.GetRequiredService<SecondLevelCacheInterceptor>());");

                method.AddInvocationStatement("services.AddEFSecondLevelCache", invocation =>
                {
                    invocation.AddArgument(new CSharpLambdaBlock("options"), argument =>
                    {
                        ((CSharpLambdaBlock)argument).AddStatement($"options.UseCustomCacheProvider<{template.GetDistributedCacheServiceProviderName()}>();");
                    });
                });
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