using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.Constants;
using Intent.Modules.Entities.Repositories.Api.Templates.PagedResultInterface;
using Intent.Modules.Modelers.Domain.Settings;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.MongoDb.Repositories.Templates.PagedList
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class PagedListTemplate : CSharpTemplateBase<object>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.MongoDb.Repositories.PagedList";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public PagedListTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddUsing("System.Collections.Generic")
                .AddUsing("System.Linq")
                .AddUsing("System.Threading")
                .AddUsing("System.Threading.Tasks")
                .AddUsing("MongoFramework")
                .AddUsing("MongoFramework.Linq")
                .AddClass($"MongoPagedList")
                .OnBuild(file =>
                {
                    var createEntityInterfaces = ExecutionContext.Settings.GetDomainSettings().CreateEntityInterfaces();

                    var @class = file.Classes.First();
                    @class.AddGenericParameter("TDomain", out var tDomain);
                    CSharpGenericParameter tPersistence = tDomain;

                    if (createEntityInterfaces)
                    {
                        @class.AddGenericParameter("TPersistence", out tPersistence);
                        @class.AddGenericTypeConstraint(tPersistence, p => p.AddType("class").AddType(tDomain))
                            .AddGenericTypeConstraint(tDomain, p => p.AddType("class"));
                    }
                    else
                    {
                        @class.AddGenericTypeConstraint(tDomain, p => p.AddType("class"));
                    }


                    @class.ExtendsClass($"List<{tDomain}>");
                    @class.ImplementsInterface($"{PagedResultInterfaceName}<{tDomain}>");
                    @class.AddProperty("int", "TotalCount", prop => prop.PrivateSetter());
                    @class.AddProperty("int", "PageCount", prop => prop.PrivateSetter());
                    @class.AddProperty("int", "PageNo", prop => prop.PrivateSetter());
                    @class.AddProperty("int", "PageSize", prop => prop.PrivateSetter());
                    @class.AddConstructor(ctor =>
                    {
                        ctor.AddParameter($"IQueryable<{tPersistence}>", "source")
                            .AddParameter("int", "pageNo")
                            .AddParameter("int", "pageSize");

                        var aggregator = new CSharpStatementAggregator();
                        aggregator.Add($"TotalCount = source.Count();");
                        aggregator.Add($"PageCount = GetPageCount(pageSize, TotalCount);");
                        aggregator.Add($"PageNo = pageNo;");
                        aggregator.Add($"PageSize = pageSize;");
                        aggregator.Add($"var skip = ((PageNo - 1) * PageSize);");
                        aggregator.Add($"");

                        if (createEntityInterfaces)
                        {
                            aggregator.Add(new CSharpInvocationStatement("AddRange")
                                .AddArgument(new CSharpMethodChainStatement("source")
                                    .AddChainStatement("Skip(skip)")
                                    .AddChainStatement("Take(PageSize)")
                                    .AddChainStatement($"Cast<{tDomain}>()")
                                    .AddChainStatement("ToList()")
                                    .WithoutSemicolon(), arg => arg.BeforeSeparator = CSharpCodeSeparatorType.EmptyLines));
                            ctor.AddStatements(aggregator.ToList());
                        }
                        else
                        {
                            aggregator.Add(new CSharpInvocationStatement("AddRange")
                                .AddArgument(new CSharpMethodChainStatement("source")
                                    .AddChainStatement("Skip(skip)")
                                    .AddChainStatement("Take(PageSize)")
                                    .AddChainStatement("ToList()")
                                    .WithoutSemicolon(), arg => arg.BeforeSeparator = CSharpCodeSeparatorType.EmptyLines));
                            ctor.AddStatements(aggregator.ToList());

                        }
                    });

                    @class.AddConstructor(ctor =>
                    {
                        ctor.AddParameter("int", "totalCount")
                            .AddParameter("int", "pageNo")
                            .AddParameter("int", "pageSize")
                            .AddParameter($"List<{tDomain}>", "results");
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

                    @class.AddMethod($"Task<{PagedResultInterfaceName}<{tDomain}>>", "CreateAsync", method =>
                    {
                        method.Static();
                        method.Async();
                        method.AddParameter($"IQueryable<{tPersistence}>", "source")
                            .AddParameter("int", "pageNo")
                            .AddParameter("int", "pageSize")
                            .AddParameter("CancellationToken", "cancellationToken", parm => parm.WithDefaultValue("default"));
                        method.AddStatement("var count = await source.CountAsync(cancellationToken);");
                        method.AddStatement("var skip = ((pageNo - 1) * pageSize);");
                        method.AddStatement(new CSharpMethodChainStatement("var results = await source")
                            .AddChainStatement("Skip(skip)")
                            .AddChainStatement("Take(pageSize)")
                            .AddChainStatement("ToListAsync(cancellationToken)"));
                        if (createEntityInterfaces)
                        {
                            method.AddStatement($"return new {@class.Name}<{tDomain}, {tPersistence}>(count, pageNo, pageSize, results.Cast<{tDomain}>().ToList());");
                        }
                        else
                        {
                            method.AddStatement($"return new {@class.Name}<{tDomain}>(count, pageNo, pageSize, results);");
                        }
                    });

                });
        }

        public string PagedResultInterfaceName => TryGetTypeName(TemplateRoles.Repository.Interface.PagedList, out var name)
            ? name : GetTypeName(TemplateRoles.Repository.Interface.PagedResult); // for backward compatibility

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