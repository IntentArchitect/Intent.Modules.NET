using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Modules.Blazor.Settings;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.DependencyInjection;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.Blazor.Templates.Templates.Server.ScopedExecutor
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class ScopedExecutorTemplate : CSharpTemplateBase<object>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.Blazor.Templates.Server.ScopedExecutorTemplate";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public ScopedExecutorTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddUsing("System")
                .AddUsing("System.Runtime.CompilerServices")
                .AddUsing("Microsoft.Extensions.DependencyInjection")
                .AddClass($"ScopedExecutor", @class =>
                {
                    @class.ImplementsInterface(this.GetScopedExecutorInterfaceTemplateName());
                    @class.AddConstructor(ctor =>
                    {
                        ctor.AddParameter("IServiceScopeFactory", "scopeFactory", param =>
                        {
                            param.IntroduceReadonlyField();
                        });
                        ctor.AddParameter("IServiceProvider", "serviceProvider", param =>
                        {
                            param.IntroduceReadonlyField();
                        });

                        bool doPrincipalPropogation = ExecutionContext.InstalledModules.Any(m => m.ModuleId == "Intent.Blazor.Authentication");

                        @class.AddMethod("Task<IServiceScope>", "CreateScope", method =>
                        {
                            method.Async();

                            if (doPrincipalPropogation)
                            {
                                method.AddStatement("var currentUser = _serviceProvider.GetRequiredService<ICurrentUserService>();");
                                method.AddStatement("var principal = await currentUser.GetPrincipalAsync();");
                                method.AddStatement("var scope = _scopeFactory.CreateScope();");
                                method.AddStatement("var scopedUser = scope.ServiceProvider.GetRequiredService<ICurrentUserService>() as ISetCurrentUserContext;");
                                method.AddIfStatement("scopedUser is not null", ifs => { ifs.AddStatement("scopedUser.SetContext(principal);"); });
                                method.AddStatement("return scope;");

                            }
                            else
                            {
                                method.AddStatement("return _scopeFactory.CreateScope();");
                            }

                        });
                        @class.AddMethod("Task", "ExecuteAsync", method =>
                        {
                            method.Async();
                            method.AddParameter("Func<IServiceProvider, Task>", "action");

                            method.AddStatement("using var scope = await CreateScope();");
                            method.AddStatement("await action(scope.ServiceProvider);");
                        });
                        @class.AddMethod("Task<T>", "ExecuteAsync", method =>
                        {
                            method.Async();
                            method.AddGenericParameter("T");
                            method.AddParameter("Func<IServiceProvider, Task<T>>", "action");

                            method.AddStatement("using var scope = await CreateScope();");
                            method.AddStatement("return await action(scope.ServiceProvider);");
                        });
                        @class.AddMethod("IAsyncEnumerable<T>", "ExecuteStreamAsync", method =>
                        {
                            method.Async();
                            method.AddGenericParameter("T");
                            method.AddParameter("Func<IServiceProvider, IAsyncEnumerable<T>>", "action");
                            method.AddParameter("[EnumeratorCancellation] CancellationToken", "cancellationToken", p => p.WithDefaultValue("default"));

                            method.AddStatement("using var scope = await CreateScope();");
                            method.AddStatement("var stream = action(scope.ServiceProvider);");
                            method.AddStatement(@"await foreach (var item in stream.WithCancellation(cancellationToken).ConfigureAwait(false))
            {
                yield return item;
            }");
                        });
                    });
                })
                ;
        }
        public override bool CanRunTemplate()
        {
            return base.CanRunTemplate() && ExecutionContext.GetSettings().GetBlazor().RenderMode().IsInteractiveServer();
        }

        public override void BeforeTemplateExecution()
        {
            if (!CanRunTemplate()) return;

            ExecutionContext.EventDispatcher.Publish(ContainerRegistrationRequest.ToRegister(this)
                .ForConcern("Infrastructure")
                .WithPerServiceCallLifeTime()
                .ForInterface(this.GetScopedExecutorInterfaceTemplateName()));
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