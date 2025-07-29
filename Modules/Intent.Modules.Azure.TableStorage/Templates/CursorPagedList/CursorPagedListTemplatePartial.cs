using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.Constants;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.Azure.TableStorage.Templates.CursorPagedList
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class CursorPagedListTemplate : CSharpTemplateBase<object>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.Azure.TableStorage.CursorPagedList";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public CursorPagedListTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            FulfillsRole(TemplateRoles.Application.Common.CursorPagedList);
            AddUsing("System.Collections.Generic");

            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddClass($"CursorPagedList")
                .OnBuild(file =>
                {
                    var @class = file.Classes.First();
                    @class.AddGenericParameter("T", out var T);
                    @class.ExtendsClass($"List<{T}>");
                    if (!string.IsNullOrWhiteSpace(CursorPagedResultInterfaceName))
                    {
                        @class.ImplementsInterface($"{CursorPagedResultInterfaceName}<{T}>");
                    }
                    @class.AddProperty("string?", "CursorToken", prop => prop.PrivateSetter());
                    @class.AddProperty("int", "PageSize", prop => prop.PrivateSetter());

                    @class.AddProperty("bool", "HasMoreResults", prop =>
                    {
                        prop.Getter.WithExpressionImplementation("!string.IsNullOrEmpty(CursorToken)");
                        prop.WithoutSetter();
                    });

                    @class.AddConstructor(ctor =>
                    {
                        ctor.AddParameter("string?", "cursorToken")
                            .AddParameter("int", "pageSize")
                            .AddParameter($"IEnumerable<{T}>", "results");
                        ctor.AddStatements($@"
                            CursorToken = cursorToken;
                            PageSize = pageSize;
                            AddRange(results);");
                    });
                });
        }

        public string? CursorPagedResultInterfaceName => TryGetTypeName(TemplateRoles.Repository.Interface.CursorPagedList, out var interfaceName) ? interfaceName : null;

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