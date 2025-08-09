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

namespace Intent.Modules.Blazor.Templates.Templates.Server.ScopedMediator
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class ScopedMediatorTemplate : CSharpTemplateBase<object>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.Blazor.Templates.Server.ScopedMediatorTemplate";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public ScopedMediatorTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddUsing("MediatR")
                .AddUsing("Microsoft.Extensions.DependencyInjection")
                .AddClass($"ScopedMediator", @class =>
                {
                    @class.ImplementsInterface(this.GetScopedMediatorInterfaceTemplateName());
                    @class.AddConstructor(ctor =>
                    {
                        ctor.AddParameter(this.GetScopedExecutorInterfaceTemplateName(), "scopedExecutor", param =>
                        {
                            param.IntroduceReadonlyField();
                        });
                    });
                    @class.AddMethod("Task<TResponse>", "Send", method =>
                    {
                        method
                            .Async()
                            .AddGenericParameter("TResponse");
                        method.AddParameter("IRequest<TResponse>", "request");
                        method.AddParameter("CancellationToken", "cancellationToken", p => p.WithDefaultValue("default"));

                        method.AddStatement(@"return await _scopedExecutor.ExecuteAsync(
                async provider => 
                {
                    var mediator = provider.GetRequiredService<IMediator>();
                    return await mediator.Send(request, cancellationToken);
                });");
                    });

                    @class.AddMethod("Task", "Send", method =>
                    {
                        method
                            .Async()
                            .AddGenericParameter("TRequest", out var generic)
                            .AddGenericTypeConstraint(generic, c => c.AddType("IRequest"));
                        method.AddParameter("TRequest", "request");
                        method.AddParameter("CancellationToken", "cancellationToken", p => p.WithDefaultValue("default"));

                        method.AddStatement(@"await _scopedExecutor.ExecuteAsync(
                async provider => 
                {
                    var mediator = provider.GetRequiredService<IMediator>();
                    await mediator.Send(request, cancellationToken);
                });");
                    });

                    @class.AddMethod("Task<object?>", "Send", method =>
                    {
                        method
                            .Async();
                        method.AddParameter("object", "request");
                        method.AddParameter("CancellationToken", "cancellationToken", p => p.WithDefaultValue("default"));

                        method.AddStatement(@"return await _scopedExecutor.ExecuteAsync(
                async provider => 
                {
                    var mediator = provider.GetRequiredService<IMediator>();
                    return await mediator.Send(request, cancellationToken);
                });");
                    });

                    @class.AddMethod("IAsyncEnumerable<TResponse>", "CreateStream", method =>
                    {
                        method
                            .AddGenericParameter("TResponse");
                        method.AddParameter("IStreamRequest<TResponse>", "request");
                        method.AddParameter("CancellationToken", "cancellationToken", p => p.WithDefaultValue("default"));

                        method.AddStatement(@"return _scopedExecutor.ExecuteStreamAsync(
                provider =>
                {
                    var mediator = provider.GetRequiredService<IMediator>();
                    return mediator.CreateStream(request, cancellationToken);
                });");
                    });

                    @class.AddMethod("IAsyncEnumerable<object?>", "CreateStream", method =>
                    {
                        method.AddParameter("object", "request");
                        method.AddParameter("CancellationToken", "cancellationToken", p => p.WithDefaultValue("default"));

                        method.AddStatement(@"return _scopedExecutor.ExecuteStreamAsync(
                provider =>
                {
                    var mediator = provider.GetRequiredService<IMediator>();
                    return mediator.CreateStream(request, cancellationToken);
                });");
                    });

                });
        }

        public override bool CanRunTemplate()
        {
            return base.CanRunTemplate() 
                && ExecutionContext.GetSettings().GetBlazor().RenderMode().IsInteractiveServer()
                && ExecutionContext.InstalledModules.Any(m => m.ModuleId == "Intent.Application.MediatR");
        }

        public override void BeforeTemplateExecution()
        {
            if (!CanRunTemplate()) return;

            ExecutionContext.EventDispatcher.Publish(ContainerRegistrationRequest.ToRegister(this)
                .ForConcern("Infrastructure")
                .WithPerServiceCallLifeTime()
                .ForInterface(this.GetScopedMediatorInterfaceTemplateName()));
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