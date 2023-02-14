using System.Collections.Generic;
using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.Entities.Repositories.Api.Templates.PagedResultInterface;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.MongoDb.Repositories.Templates.MongoPagedList
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    partial class MongoPagedListTemplate : CSharpTemplateBase<object>
    {
        public const string TemplateId = "Intent.MongoDb.Repositories.MongoPagedList";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public MongoPagedListTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            AddNugetDependency(NugetPackages.MongoDbDataUnitOfWork);
        }
        
        public string PagedResultInterfaceName => GetTypeName(PagedResultInterfaceTemplate.TemplateId);

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        protected override CSharpFileConfig DefineFileConfig()
        {
            return new CSharpFileConfig(
                className: $"MongoPagedList",
                @namespace: $"{this.GetNamespace()}",
                relativeLocation: $"{this.GetFolderPath()}");
        }
    }
}