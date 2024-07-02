using System;
using System.Collections.Generic;
using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.AspNetCore.DistributedCaching.Templates.IDistributedCacheExtensions
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class IDistributedCacheExtensionsTemplate : CSharpTemplateBase<object>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.AspNetCore.DistributedCaching.IDistributedCacheExtensions";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public IDistributedCacheExtensionsTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddUsing("System.Text")
                .AddUsing("System.Text.Json")
                .AddUsing("System.Threading")
                .AddUsing("System.Threading.Tasks")
                .AddUsing("Microsoft.Extensions.Caching.Distributed")
                .AddClass($"DistributedCacheExtensions", @class =>
                {
                    @class.Static();

                    @class.AddMethod("T?", "Get", method =>
                    {
                        method
                            .Static()
                            .AddGenericParameter("T")
                            .AddParameter("IDistributedCache", "cache", p => p.WithThisModifier())
                            .AddParameter("string", "key")
                            ;
                        method.AddStatement("var bytes = cache.Get(key);");
                        method.AddIfStatement("bytes == null", stmt => stmt.AddStatement("return default;"));
                        method.AddStatement("var json = Encoding.UTF8.GetString(bytes);");
                        method.AddStatement("return JsonSerializer.Deserialize<T>(json);");
                    });

                    @class.AddMethod("Task<T?>", "GetAsync", method =>
                    {
                        method
                            .Static()
                            .Async()
                            .AddGenericParameter("T")
                            .AddParameter("IDistributedCache", "cache", p => p.WithThisModifier())
                            .AddParameter("string", "key")
                            .AddParameter("CancellationToken", "cancellationToken", p => p.WithDefaultValue("default"))
                            ;
                        method.AddStatement("var bytes = await cache.GetAsync(key, cancellationToken);");
                        method.AddIfStatement("bytes == null", stmt => stmt.AddStatement("return default;"));
                        method.AddStatement("var json = Encoding.UTF8.GetString(bytes);");
                        method.AddStatement("return JsonSerializer.Deserialize<T>(json);");
                    });

                    @class.AddMethod("void", "Set", method =>
                    {
                        method
                            .Static()
                            .AddGenericParameter("T")
                            .AddParameter("IDistributedCache", "cache", p => p.WithThisModifier())
                            .AddParameter("string", "key")
                            .AddParameter("T", "value")
                            .AddParameter("DistributedCacheEntryOptions?", "options", p => p.WithDefaultValue("default"))
                            ;
                        method.AddStatement("var json = JsonSerializer.Serialize(value);");
                        method.AddStatement("var bytes = Encoding.UTF8.GetBytes(json);");
                        method.AddStatement("cache.Set(key, bytes, options ?? new DistributedCacheEntryOptions());");
                    });
                    @class.AddMethod("Task", "SetAsync", method =>
                    {
                        method
                            .Static()
                            .Async()
                            .AddGenericParameter("T")
                            .AddParameter("IDistributedCache", "cache", p => p.WithThisModifier())
                            .AddParameter("string", "key")
                            .AddParameter("T", "value")
                            .AddParameter("DistributedCacheEntryOptions?", "options", p => p.WithDefaultValue("default"))
                            .AddParameter("CancellationToken", "cancellationToken", p => p.WithDefaultValue("default"))
                            ;
                        method.AddStatement("var json = JsonSerializer.Serialize(value);");
                        method.AddStatement("var bytes = Encoding.UTF8.GetBytes(json);");
                        method.AddStatement("await cache.SetAsync(key, bytes, options ?? new DistributedCacheEntryOptions(), cancellationToken);");
                    });

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