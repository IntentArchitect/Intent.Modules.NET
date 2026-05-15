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

namespace Intent.Modules.Eventing.NServiceBus.Templates.NServiceBusHostedService
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class NServiceBusHostedServiceTemplate : CSharpTemplateBase<object>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.Eventing.NServiceBus.NServiceBusHostedService";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public NServiceBusHostedServiceTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddUsing("System.Threading")
                .AddUsing("System.Threading.Tasks")
                .AddUsing("Microsoft.Extensions.Hosting")
                .AddUsing("NServiceBus")
                .AddClass("NServiceBusHostedService", @class =>
                {
                    @class.ImplementsInterface("IHostedService");

                    @class.AddField("IStartableEndpointWithExternallyManagedContainer", "_startableEndpoint", f => f.PrivateReadOnly());
                    @class.AddField("IServiceProvider", "_serviceProvider", f => f.PrivateReadOnly());
                    @class.AddField("IEndpointInstance?", "_endpointInstance", f => f.Private());

                    @class.AddConstructor(ctor =>
                    {
                        ctor.AddParameter("IStartableEndpointWithExternallyManagedContainer", "startableEndpoint", p => p.IntroduceReadonlyField());
                        ctor.AddParameter("IServiceProvider", "serviceProvider", p => p.IntroduceReadonlyField());
                    });

                    @class.AddMethod("Task", "StartAsync", method =>
                    {
                        method.Async();
                        method.AddParameter("CancellationToken", "cancellationToken");
                        method.AddStatement("_endpointInstance = await _startableEndpoint.Start(_serviceProvider, cancellationToken);");
                    });

                    @class.AddMethod("Task", "StopAsync", method =>
                    {
                        method.Async();
                        method.AddParameter("CancellationToken", "cancellationToken");
                        method.AddIfStatement("_endpointInstance is not null", b =>
                            b.AddStatement("await _endpointInstance.Stop(cancellationToken);"));
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
