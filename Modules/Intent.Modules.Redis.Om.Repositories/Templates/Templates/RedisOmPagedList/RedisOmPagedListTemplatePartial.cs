using System;
using System.Collections.Generic;
using System.Linq;
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

namespace Intent.Modules.Redis.Om.Repositories.Templates.Templates.RedisOmPagedList
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class RedisOmPagedListTemplate : CSharpTemplateBase<object>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.Redis.Om.Repositories.Templates.RedisOmPagedList";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public RedisOmPagedListTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            var createEntityInterfaces = ExecutionContext.Settings.GetDomainSettings().CreateEntityInterfaces();

            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddUsing("System.Collections.Generic")
                .AddClass("RedisOmPagedList", @class =>
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
                        .AddGenericParameter("TDocument", out var tDocument)
                        .WithBaseType($"List<{tDomain}>")
                        .ImplementsInterface($"{this.GetPagedListInterfaceName()}<{tDomain}>")
                        .AddGenericTypeConstraint(tDomain, c => c
                            .AddType("class"));

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
                    @class
                        .AddGenericTypeConstraint(tDocument, c => c
                            .AddType($"{this.GetRedisOmDocumentOfTInterfaceName()}<{tDomain}{tDomainStateConstraint}, {tDocument}>"))
                        ;

                    @class.AddConstructor();
                    var ctor = @class.Constructors.First();
                    ctor.AddParameter("int", "totalCount")
                        .AddParameter("int", "pageNo")
                        .AddParameter("int", "pageSize")
                        .AddParameter($"IEnumerable<{tDomain}>", "results");

                    ctor.AddStatement("TotalCount = totalCount;");
                    ctor.AddStatement("PageCount = GetPageCount(pageSize, totalCount);");
                    ctor.AddStatement("PageNo = pageNo;");
                    ctor.AddStatement("PageSize = pageSize;");

                    ctor.AddForEachStatement("result", "results", stmt => { stmt.AddStatement("Add(result);"); });

                    @class.AddProperty("int", "TotalCount", prop => prop.WithoutSetter());
                    @class.AddProperty("int", "PageCount", prop => prop.WithoutSetter());
                    @class.AddProperty("int", "PageNo", prop => prop.WithoutSetter());
                    @class.AddProperty("int", "PageSize", prop => prop.WithoutSetter());

                    @class.AddMethod("int", "GetPageCount", method =>
                    {
                        method.Private().Static();
                        method.AddParameter("int", "pageSize");
                        method.AddParameter("int", "totalCount");
                        method.AddStatements(
                            """
                            if (pageSize == 0)
                            {
                                return 0;
                            }

                            var remainder = totalCount % pageSize;
                            return (totalCount / pageSize) + (remainder == 0 ? 0 : 1);
                            """);
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