using System.Linq;
using Intent.Engine;
using Intent.Modules.Bugsnag.Templates;
using Intent.Modules.Bugsnag.Templates.BugSnagQuartzJobListener;
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
    public class BugSnagQuartzJobListenerExtension : FactoryExtensionBase
    {
        public override string Id => "Intent.Bugsnag.BugSnagQuartzJobListenerExtension";

        [IntentManaged(Mode.Ignore)]
        public override int Order => 0;

        /// <summary>
        /// This is an example override which would extend the
        /// <see cref="ExecutionLifeCycleSteps.AfterTemplateRegistrations"/> phase of the Software Factory execution.
        /// See <see cref="FactoryExtensionBase"/> for all available overrides.
        /// </summary>
        /// <remarks>
        /// It is safe to update or delete this method.
        /// </remarks>
        protected override void OnAfterTemplateRegistrations(IApplication application)
        {
            var configTemplate = application.FindTemplateInstance<ICSharpFileBuilderTemplate>("Distribution.DependencyInjection.Quartz");
            if (configTemplate is null)
            {
                return;
            }

            configTemplate.CSharpFile.OnBuild(file =>
            {
                var @class = file.Classes.First();
                var method = @class.FindMethod("ConfigureQuartz");
                if (method is null)
                {
                    return;
                }

                // Quartz 4 takes the matchers as a C# 13 params collection. Under an older LangVersion that is an
                // ordinary parameter, so neither a bare matcher nor no argument compiles: the array is passed
                // explicitly. GroupMatcher also moved from Quartz.Impl.Matchers to Quartz.
                var usesQuartz4 = QuartzVersion.UsesQuartz4(configTemplate);
                if (!usesQuartz4)
                {
                    file.AddUsing("Quartz.Impl.Matchers");
                }
                method.InsertStatement(0, $"services.AddSingleton<{configTemplate.GetBugSnagQuartzJobListenerName()}>();");
                var addQuartzChain = method.Statements.OfType<CSharpMethodChainStatement>().FirstOrDefault();
                var addQuartzInv = addQuartzChain?.Statements.FirstOrDefault() as CSharpInvocationStatement;
                var addQuartzLambda = addQuartzInv?.Statements.FirstOrDefault() as CSharpLambdaBlock;
                addQuartzLambda?.AddStatement(usesQuartz4
                    ? $"q.AddJobListener<{configTemplate.GetBugSnagQuartzJobListenerName()}>(new IMatcher<JobKey>[] {{ GroupMatcher<JobKey>.AnyGroup() }});"
                    : $"q.AddJobListener<{configTemplate.GetBugSnagQuartzJobListenerName()}>(GroupMatcher<JobKey>.AnyGroup());");
            });
        }
    }
}
