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

namespace Intent.Modules.Eventing.Kafka.Templates.KafkaConsumerBackgroundService
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class KafkaConsumerBackgroundServiceTemplate : CSharpTemplateBase<object>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.Eventing.Kafka.KafkaConsumerBackgroundService";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public KafkaConsumerBackgroundServiceTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            AddNugetDependency(NugetPackages.MicrosoftExtensionsHosting(OutputTarget));

            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddUsing("System.Collections.Generic")
                .AddUsing("System.Linq")
                .AddUsing("System.Threading")
                .AddUsing("System.Threading.Tasks")
                .AddUsing("Microsoft.Extensions.Hosting")
                .AddClass($"KafkaConsumerBackgroundService", @class =>
                {
                    @class.WithBaseType("BackgroundService");

                    @class.AddConstructor(ctor =>
                    {
                        ctor.AddParameter($"IEnumerable<{this.GetKafkaConsumerInterfaceName()}>", "consumers", param =>
                        {
                            param.IntroduceReadonlyField();
                        });
                    });

                    @class.AddMethod("Task", "ExecuteAsync", method =>
                    {
                        method.Protected().Override();
                        method.AddParameter("CancellationToken", "stoppingToken");
                        method.AddStatement("return Task.WhenAll(_consumers.Select(x => Task.Run(() => x.DoWork(stoppingToken), stoppingToken)));");
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