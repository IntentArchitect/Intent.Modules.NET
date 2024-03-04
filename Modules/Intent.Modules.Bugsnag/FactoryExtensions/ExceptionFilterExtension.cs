using System.Linq;
using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Plugins;
using Intent.Plugins.FactoryExtensions;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.FactoryExtension", Version = "1.0")]

namespace Intent.Modules.Bugsnag.FactoryExtensions
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class ExceptionFilterExtension : FactoryExtensionBase
    {
        public override string Id => "Intent.Bugsnag.ExceptionFilterExtension";

        [IntentManaged(Mode.Ignore)]
        public override int Order => 0;

        protected override void OnAfterTemplateRegistrations(IApplication application)
        {
            var filterTemplate = application.FindTemplateInstance<ICSharpFileBuilderTemplate>("Distribution.ExceptionFilter");
            filterTemplate?.CSharpFile.OnBuild(file =>
            {
                filterTemplate.AddUsing("System.Collections.Generic");

                var @class = file.Classes.First();
                var ctor = @class.Constructors.FirstOrDefault();
                if (ctor is null)
                {
                    @class.AddConstructor();
                    ctor = @class.Constructors.First();
                }

                ctor.AddParameter("Bugsnag.IClient", "client");
                ctor.AddInvocationStatement("client.BeforeNotify", inv => inv
                    .AddArgument(new CSharpLambdaBlock("report")
                        .AddStatement("var activityId = System.Diagnostics.Activity.Current?.Id;")
                        .AddIfStatement("!string.IsNullOrEmpty(activityId)", stmt => stmt
                            .AddStatement("// Add the Activity ID to the report's metadata under a 'Trace' tab")
                            .AddInvocationStatement("report.Event.Metadata.Add", addInv => addInv
                                .AddObjectInitializerBlock("\"Trace\", new Dictionary<string, object>", block => block
                                    .AddKeyAndValue(@"""ActivityId""", "activityId"))
                            ))));
            });
        }
    }
}