using System;
using System.Collections.Generic;
using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.Application.Wolverine.Templates.UnitOfWorkMiddleware
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class UnitOfWorkMiddlewareTemplate : CSharpTemplateBase<object>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.Application.Wolverine.UnitOfWorkMiddleware";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public UnitOfWorkMiddlewareTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            var unitOfWork = GetTypeName("Intent.Entities.Repositories.Api.UnitOfWorkInterface");

            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddUsing("System.Transactions")
                .AddClass("UnitOfWorkMiddleware", @class =>
                {
                    @class.AddMethod($"{UseType("System.Transactions.TransactionScope")}?", "Before", method =>
                    {
                        method.Static();
                        method.AddParameter(unitOfWork, "dataSource");
                        method.AddStatement($"return new {UseType("System.Transactions.TransactionScope")}(");
                        method.AddStatement($"    {UseType("System.Transactions.TransactionScopeOption")}.Required,");
                        method.AddStatement($"    new {UseType("System.Transactions.TransactionOptions")} {{ IsolationLevel = {UseType("System.Transactions.IsolationLevel")}.ReadCommitted }},");
                        method.AddStatement($"    {UseType("System.Transactions.TransactionScopeAsyncFlowOption")}.Enabled);");
                    });

                    @class.AddMethod(UseType("System.Threading.Tasks.Task"), "AfterAsync", method =>
                    {
                        method.Static().Async();
                        method.AddParameter($"{UseType("System.Transactions.TransactionScope")}?", "tx");
                        method.AddParameter(unitOfWork, "dataSource");
                        method.AddParameter(UseType("System.Threading.CancellationToken"), "cancellationToken");

                        method.AddTryBlock(tryBlock =>
                        {
                            tryBlock.AddStatement("await dataSource.SaveChangesAsync(cancellationToken);");
                            tryBlock.AddStatement("tx?.Complete();");
                        });
                        method.AddFinallyBlock(finallyBlock =>
                        {
                            finallyBlock.AddStatement("tx?.Dispose();");
                        });
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