using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.Constants;
using Intent.Modules.Entities.Repositories.Api.Templates.PagedResultInterface;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.CosmosDb.Repositories.Templates.PagedList
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    partial class PagedListTemplate : CSharpTemplateBase<object>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.CosmosDb.Repositories.PagedList";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public PagedListTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
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
                    @class.ImplementsInterface($"{PagedResultInterfaceName}<{T}>");
                    @class.AddProperty("int", "TotalCount", prop => prop.PrivateSetter());
                    @class.AddProperty("int", "PageCount", prop => prop.PrivateSetter());
                    @class.AddProperty("int", "PageNo", prop => prop.PrivateSetter());
                    @class.AddProperty("int", "PageSize", prop => prop.PrivateSetter());
                    @class.AddConstructor(ctor =>
                    {
                        ctor.AddParameter($"ICosmosQueryable<{T}>", "source")
                            .AddParameter("int", "pageNo")
                            .AddParameter("int", "pageSize");

                        var aggregator = new CSharpStatementAggregator();
                        aggregator.Add($"TotalCount = source.Count();");
                        aggregator.Add($"PageCount = GetPageCount(pageSize, TotalCount);");
                        aggregator.Add($"PageNo = pageNo;");
                        aggregator.Add($"PageSize = pageSize;");
                        aggregator.Add($"var skip = ((PageNo - 1) * PageSize);");
                        aggregator.Add($"");

                        aggregator.Add(new CSharpInvocationStatement("AddRange")
                            .AddArgument(new CSharpMethodChainStatement("source")
                                .AddChainStatement("Skip(skip)")
                                .AddChainStatement("Take(PageSize)")
                                .AddChainStatement("ToList()")
                                .WithoutSemicolon(), arg => arg.BeforeSeparator = CSharpCodeSeparatorType.EmptyLines));
                        ctor.AddStatements(aggregator.ToList());
                    });

                    @class.AddConstructor(ctor =>
                    {
                        ctor.AddParameter("int", "totalCount")
                            .AddParameter("int", "pageNo")
                            .AddParameter("int", "pageSize")
                            .AddParameter($"List<{T}>", "results");
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
                });
        }

        public string PagedResultInterfaceName => GetTypeName(PagedResultInterfaceTemplate.TemplateId);

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