using System;
using System.Collections.Generic;
using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.Entities.Repositories.Api.Templates;
using Intent.Modules.Modelers.Domain.Settings;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.MongoDb.Templates.MongoDbPagedList
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class MongoDbPagedListTemplate : CSharpTemplateBase<object>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.MongoDb.MongoDbPagedList";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public MongoDbPagedListTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            var createEntityInterfaces = ExecutionContext.Settings.GetDomainSettings().CreateEntityInterfaces();
            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddUsing("MongoDB.Driver.Linq")
                .AddUsing("System.Collections.Generic")
                .AddUsing("System.Linq")
                .AddUsing("System.Threading")
                .AddClass($"MongoPagedList", @class =>
                {
                    @class
                        .Internal()
                        .AddGenericParameter("TDomain", out var tDomain);

                    var tDomainState = tDomain;
                    if (createEntityInterfaces)
                    {
                        @class
                            .AddGenericParameter("TDomainState", out tDomainState);
                    }

                    @class
                        .WithBaseType($"List<{tDomain}>")
                        .ImplementsInterface($"{this.GetPagedResultInterfaceName()}<{tDomain}>")
                        .AddGenericTypeConstraint(tDomain, c => c
                            .AddType("class"));

                    @class
                        .Internal()
                        .AddGenericParameter("TIdentifier", out var tIdentifier);

                    if (createEntityInterfaces)
                    {
                        @class
                            .AddGenericTypeConstraint(tDomainState, c => c
                                .AddType("class")
                                .AddType(tDomain));
                    }

                    var tDomainStateConstraint = createEntityInterfaces
                        ? $", {tDomainState}"
                        : string.Empty;

                    @class.AddProperty("int", "TotalCount", prop => prop.PrivateSetter());
                    @class.AddProperty("int", "PageCount", prop => prop.PrivateSetter());
                    @class.AddProperty("int", "PageNo", prop => prop.PrivateSetter());
                    @class.AddProperty("int", "PageSize", prop => prop.PrivateSetter());
                    @class.AddConstructor(ctor =>
                    {
                        ctor.AddParameter($"IQueryable<{tDomain}>", "source")
                            .AddParameter("int", "pageNo")
                            .AddParameter("int", "pageSize");

                        ctor.AddStatement("TotalCount = source.Count();");
                        ctor.AddStatement("PageCount = GetPageCount(pageSize, TotalCount);");
                        ctor.AddStatement("PageNo = pageNo;");
                        ctor.AddStatement("PageSize = pageSize;");
                        ctor.AddStatement("var skip = ((PageNo - 1) * PageSize);");

                        ctor.AddStatement("AddRange(source.Skip(skip).Take(PageSize).ToList());");
                    });

                    @class.AddConstructor(ctor =>
                    {
                        ctor.AddParameter($"int", "totalCount")
                            .AddParameter("int", "pageNo")
                            .AddParameter("int", "pageSize")
                            .AddParameter($"List<{tDomain}>", "results");

                        ctor.AddStatement("TotalCount = totalCount;");
                        ctor.AddStatement("PageCount = GetPageCount(pageSize, TotalCount);");
                        ctor.AddStatement("PageNo = pageNo;");
                        ctor.AddStatement("PageSize = pageSize;");

                        ctor.AddStatement("AddRange(results);");
                    });

                    @class.AddMethod($"IPagedList<{tDomain}>", "CreateAsync", method =>
                    {
                        method.Static().Async();

                        method.AddParameter($"IQueryable<{tDomain}>", "source")
                            .AddParameter("int", "pageNo")
                            .AddParameter("int", "pageSize")
                            .AddParameter("CancellationToken", "cancellationToken", c => c.WithDefaultValue("default"));

                        method.AddStatement("var count = await source.CountAsync(cancellationToken);");
                        method.AddStatement("var skip = ((pageNo - 1) * pageSize);");
                        method.AddStatement("var results = await source.Skip(skip).Take(pageSize).ToListAsync(cancellationToken);");
                        method.AddStatement($"return new MongoPagedList<{tDomain}, {tIdentifier}>(count, pageNo, pageSize, results);");
                    });

                    @class.AddMethod($"int", "GetPageCount", method =>
                    {
                        method.Private();

                        method.AddParameter("int", "pageSize")
                            .AddParameter("int", "totalCount");

                        method.AddIfStatement("pageSize == 0", @if => @if.AddReturn("0"));

                        method.AddStatement("var remainder = totalCount % pageSize;");
                        method.AddStatement("return (totalCount / pageSize) + (remainder == 0 ? 0 : 1);");
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