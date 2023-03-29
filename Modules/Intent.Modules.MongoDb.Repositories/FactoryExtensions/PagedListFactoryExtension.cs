using System.Linq;
using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Plugins;
using Intent.Modules.Common.Templates;
using Intent.Modules.Constants;
using Intent.Modules.Entities.Repositories.Api.Templates.PagedResultInterface;
using Intent.Plugins.FactoryExtensions;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.FactoryExtension", Version = "1.0")]

namespace Intent.Modules.MongoDb.Repositories.FactoryExtensions
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class PagedListFactoryExtension : FactoryExtensionBase
    {
        public override string Id => "Intent.MongoDb.Repositories.PagedListFactoryExtension";

        [IntentManaged(Mode.Ignore)]
        public override int Order => 0;

        protected override void OnAfterTemplateRegistrations(IApplication application)
        {
#warning remove
            /*
            var templates = application.FindTemplateInstances<ICSharpFileBuilderTemplate>(TemplateDependency.OnTemplate(TemplateFulfillingRoles.Repository.PagedList)).ToArray();
            if (!templates.Any())
            {
                return;
            }
            var template = templates.Single(); // Registration should guarantee one template instance

            template.CSharpFile.AfterBuild(file =>
            {
                file.AddUsing("MongoDB.Driver");
                file.AddUsing("MongoDB.Driver.Linq");
                var @class = file.Classes.First();
                var T = @class.GenericParameters.First();
                var pagedResultInterfaceName = template.GetTypeName(PagedResultInterfaceTemplate.TemplateId);
                @class.AddMethod($"Task<{pagedResultInterfaceName}<{T}>>", "CreateAsync", method =>
                {
                    method.Static();
                    method.Async();
                    method.AddParameter($"IMongoQueryable<{T}>", "source")
                        .AddParameter("int", "pageNo")
                        .AddParameter("int", "pageSize")
                        .AddParameter("CancellationToken", "cancellationToken", parm => parm.WithDefaultValue("default"));
                    method.AddStatement("var count = await source.CountAsync(cancellationToken);");
                    method.AddStatement("var skip = ((pageNo - 1) * pageSize);");
                    method.AddStatement("IAsyncCursorSource<T> cursorSource = source.Skip(skip).Take(pageSize);");
                    method.AddStatement("var results = await cursorSource.ToListAsync(cancellationToken);");
                    method.AddStatement($"return new {@class.Name}<{T}>(count, pageNo, pageSize, results);");
                });
            });*/
        }
    }
}