using System.Linq;
using System.Threading;
using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Plugins;
using Intent.Modules.Common.Templates;
using Intent.Modules.Constants;
using Intent.Modules.Entities.Repositories.Api.Templates.PagedResultInterface;
using Intent.Modules.EntityFrameworkCore.Repositories.Settings;
using Intent.Modules.Metadata.RDBMS.Settings;
using Intent.Plugins.FactoryExtensions;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.FactoryExtension", Version = "1.0")]

namespace Intent.Modules.EntityFrameworkCore.Repositories.FactoryExtensions
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class PagedListFactoryExtension : FactoryExtensionBase
    {
        public override string Id => "Intent.EntityFrameworkCore.Repositories.PagedListFactoryExtension";

        [IntentManaged(Mode.Ignore)]
        public override int Order => 0;

        protected override void OnAfterTemplateRegistrations(IApplication application)
        {
            //var templates = application.FindTemplateInstances<ICSharpFileBuilderTemplate>(TemplateDependency.OnTemplate(TemplateRoles.Application.Common.PagedList)).ToArray();
            //if (!templates.Any())
            //{
            //    return;
            //}
            //var template = templates.Single(); // Registration should guarantee one template instance
            //template.CSharpFile.AfterBuild(file =>
            //{
            //    //file.AddUsing("Microsoft.EntityFrameworkCore");
            //    var @class = file.Classes.First();
            //    var T = @class.GenericParameters.First();
            //    var pagedResultInterfaceName = template.GetTypeName(TemplateRoles.Repository.Interface.PagedList);
            //    // This has been moved to to the actual template itself:
            //    @class.AddMethod($"Task<{pagedResultInterfaceName}<{T}>>", "CreateAsync", method =>
            //    {
            //        method.Static();
            //        method.Async();
            //        method.AddParameter($"IQueryable<{T}>", "source")
            //            .AddParameter("int", "pageNo")
            //            .AddParameter("int", "pageSize")
            //            .AddParameter("CancellationToken", "cancellationToken", parm => parm.WithDefaultValue("default"));
            //        method.AddStatement("var count = await source.CountAsync(cancellationToken);");
            //        method.AddStatement("var skip = ((pageNo - 1) * pageSize);");
            //        method.AddStatement(new CSharpMethodChainStatement("var results = await source")
            //            .AddChainStatement("Skip(skip)")
            //            .AddChainStatement("Take(pageSize)")
            //            .AddChainStatement("ToListAsync(cancellationToken)"));
            //        method.AddStatement($"return new {@class.Name}<{T}>(count, pageNo, pageSize, results);");
            //    });
            //    // This has not been moved to to the actual template itself because it requires this setting (consider changing this):
            //    if (template.ExecutionContext.Settings.GetDatabaseSettings().AddSynchronousMethodsToRepositories())
            //    {
            //        @class.AddMethod($"{pagedResultInterfaceName}<{T}>", "Create", method =>
            //        {
            //            method.Static();
            //            method.AddParameter($"IQueryable<{T}>", "source")
            //                .AddParameter("int", "pageNo")
            //                .AddParameter("int", "pageSize");
            //            method.AddStatement("var count = source.Count();");
            //            method.AddStatement("var skip = ((pageNo - 1) * pageSize);");
            //            method.AddStatement(new CSharpMethodChainStatement("var results = source")
            //                .AddChainStatement("Skip(skip)")
            //                .AddChainStatement("Take(pageSize)")
            //                .AddChainStatement("ToList()"));
            //            method.AddStatement($"return new {@class.Name}<{T}>(count, pageNo, pageSize, results);");
            //        });
            //    }
            //});
        }
    }
}