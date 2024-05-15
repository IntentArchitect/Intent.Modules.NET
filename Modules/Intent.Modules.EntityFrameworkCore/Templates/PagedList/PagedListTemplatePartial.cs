using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.Constants;
using Intent.Modules.EntityFrameworkCore.Templates.DbContextInterface;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.EntityFrameworkCore.Templates.PagedList
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge, Signature = Mode.Merge)]
    public partial class PagedListTemplate : CSharpTemplateBase<object>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.EntityFrameworkCore.PagedList";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public PagedListTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            // For backward compatibility after being moved from the EFCore.Repositories module:
            FulfillsRole(TemplateRoles.Repository.PagedList);

            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddUsing("System.Collections.Generic")
                .AddUsing("System.Linq")
                .AddUsing("System.Threading")
                .AddUsing("System.Threading.Tasks")
                .AddClass($"PagedList")
                .OnBuild(file =>
                {
                    var @class = file.Classes.First();
                    @class.AddGenericParameter("T", out var T);
                    @class.ExtendsClass($"List<{T}>");
                    if (!string.IsNullOrWhiteSpace(PagedResultInterfaceName))
                    {
                        @class.ImplementsInterface($"{PagedResultInterfaceName}<{T}>");
                    }
                    @class.AddProperty("int", "TotalCount", prop => prop.PrivateSetter());
                    @class.AddProperty("int", "PageCount", prop => prop.PrivateSetter());
                    @class.AddProperty("int", "PageNo", prop => prop.PrivateSetter());
                    @class.AddProperty("int", "PageSize", prop => prop.PrivateSetter());

                    @class.AddConstructor(ctor =>
                    {
                        ctor.AddParameter("int", "totalCount")
                            .AddParameter("int", "pageNo")
                            .AddParameter("int", "pageSize")
                            .AddParameter($"IEnumerable<{T}>", "results");
                        ctor.AddStatements($@"
                            TotalCount = totalCount;
                            PageCount = GetPageCount(pageSize, TotalCount);
                            PageNo = pageNo;
                            PageSize = pageSize;
                            AddRange(results);");
                    });

                    @class.AddMethod("int", "GetPageCount", method =>
                    {
                        method.Private();
                        method.AddParameter("int", "pageSize")
                            .AddParameter("int", "totalCount");
                        method.AddStatement(new CSharpStatementBlock("if (pageSize == 0)")
                            .AddStatement("return 0;"));
                        method.AddStatements($@"
                            var remainder = totalCount % pageSize;
                            return (totalCount / pageSize) + (remainder == 0 ? 0 : 1);");
                    });
                })
                .AfterBuild(file =>
                {
                    var dbContextInterface = GetTemplate<DbContextInterfaceTemplate>(TemplateRoles.Application.Common.DbContextInterface, TemplateDiscoveryOptions.DoNotThrow);
                    if (dbContextInterface?.IsEnabled != true)
                    {
                        return;
                    }

                    file.AddUsing("Microsoft.EntityFrameworkCore");
                    file.AddClass("QueryablePaginationExtension", @class =>
                    {
                        @class.Static();
                        @class.AddMethod($"{ClassName}<T>", "ToPagedListAsync<T>", method =>
                        {
                            method.Static().Async();
                            method.AddParameter($"IQueryable<T>", "queryable", p => p.WithThisModifier());
                            method.AddParameter($"int", "pageNo");
                            method.AddParameter($"int", "pageSize");
                            method.AddParameter($"CancellationToken", "cancellationToken", p => p.WithDefaultValue("default"));
                            method.AddStatement("var count = await queryable.CountAsync(cancellationToken);");
                            method.AddStatement("var skip = ((pageNo - 1) * pageSize);");
                            method.AddStatement(new CSharpMethodChainStatement("var results = await queryable")
                                .AddChainStatement("Skip(skip)")
                                .AddChainStatement("Take(pageSize)")
                                .AddChainStatement("ToListAsync(cancellationToken)"));
                            method.AddStatement($"return new {ClassName}<T>(count, pageNo, pageSize, results);");
                        });
                    });
                });
        }

        public string PagedResultInterfaceName => TryGetTypeName(TemplateRoles.Repository.Interface.PagedList, out var interfaceName) ? interfaceName : null;

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
