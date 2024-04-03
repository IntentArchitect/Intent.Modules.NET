using System.Linq;
using Intent.Engine;
using Intent.Modules.AspNetCore.Controllers.Templates;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Plugins;
using Intent.Modules.Common.Templates;
using Intent.Plugins.FactoryExtensions;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.FactoryExtension", Version = "1.0")]

namespace Intent.Modules.AspNetCore.Controllers.FactoryExtentions
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class BinaryContentFilterExtension : FactoryExtensionBase
    {
        public override string Id => "Intent.AspNetCore.Controllers.BinaryContentFilterExtension";
        private readonly IMetadataManager _metadataManager;

        public BinaryContentFilterExtension(IMetadataManager metadataManager)
        {
            _metadataManager = metadataManager;
        }

        [IntentManaged(Mode.Ignore)]
        public override int Order => 0;

        protected override void OnBeforeTemplateExecution(IApplication application)
        {
            var template = application.FindTemplateInstance<ICSharpFileBuilderTemplate>(TemplateDependency.OnTemplate("Distribution.SwashbuckleConfiguration"));
            if (template == null)
            {
                return;
            }

            if (!FileTransferHelper.NeedsFileUploadInfrastructure(_metadataManager, application.Id))
            {
                return;
            }

            var @class = template.CSharpFile.Classes.First();

            var configureSwaggerOptionsBlock = GetConfigureSwaggerOptionsBlock(@class);
            if (configureSwaggerOptionsBlock == null)
            {
                return;
            }

            configureSwaggerOptionsBlock.AddStatement($@"options.OperationFilter<{template.GetBinaryContentFilterName()}>();");
        }
        private static CSharpLambdaBlock GetConfigureSwaggerOptionsBlock(CSharpClass @class)
        {
            var configureSwaggerMethod = @class.FindMethod("ConfigureSwagger");
            var addSwaggerGen = configureSwaggerMethod?.FindStatement(p => p.HasMetadata("AddSwaggerGen")) as CSharpInvocationStatement;
            var cSharpLambdaBlock = addSwaggerGen?.Statements.First() as CSharpLambdaBlock;
            return cSharpLambdaBlock;
        }
    }
}