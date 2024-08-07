using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Modelers.Domain.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.Redis.Om.Repositories.Templates.Templates.RedisOmDocument;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.Redis.Om.Repositories.Templates.Templates.IndexCreationService
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class IndexCreationServiceTemplate : CSharpTemplateBase<IList<ClassModel>>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.Redis.Om.Repositories.Templates.IndexCreationService";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public IndexCreationServiceTemplate(IOutputTarget outputTarget, IList<ClassModel> model) : base(TemplateId, outputTarget, model)
        {
            AddNugetDependency(NugetPackages.MicrosoftExtensionsHostingAbstractions(OutputTarget));

            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddUsing("System.Threading")
                .AddUsing("System.Threading.Tasks")
                .AddUsing("Microsoft.Extensions.Hosting")
                .AddUsing("Redis.OM")
                .AddClass($"IndexCreationService", @class =>
                {
                    @class.ImplementsInterface("IHostedService");
                    @class.AddConstructor(ctor => { ctor.AddParameter("RedisConnectionProvider", "provider", param => param.IntroduceReadonlyField()); });
                    @class.AddMethod("Task", "StartAsync", method =>
                    {
                        method.Async();
                        method.AddOptionalCancellationTokenParameter(this);
                        foreach (var entity in Model.Where(p => p.IsAggregateRoot() && !p.GenericTypes.Any()))
                        {
                            if (!TryGetTemplate<RedisOmDocumentTemplate>(RedisOmDocumentTemplate.TemplateId, entity, out var documentTemplate))
                            {
                                continue;
                            }

                            method.AddStatement($"await _provider.Connection.CreateIndexAsync(typeof({GetTypeName(documentTemplate)}));");
                        }
                    });
                    @class.AddMethod("Task", "StopAsync", method =>
                    {
                        method.AddOptionalCancellationTokenParameter(this);
                        method.AddStatement("return Task.CompletedTask;");
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