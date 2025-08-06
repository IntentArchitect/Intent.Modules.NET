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

namespace Intent.Modules.Entities.SoftDelete.Templates.SoftDeleteEFCoreInterceptor
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class SoftDeleteEFCoreInterceptorTemplate : CSharpTemplateBase<object>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.Entities.SoftDelete.SoftDeleteEFCoreInterceptor";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public SoftDeleteEFCoreInterceptorTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddClass($"SoftDeleteInterceptor", @class =>
                {
                    @class.Sealed();
                    @class.WithBaseType(UseType("Microsoft.EntityFrameworkCore.Diagnostics.SaveChangesInterceptor"));
                    @class.AddMethod("ValueTask<InterceptionResult<int>>", "SavingChangesAsync", method =>
                    {
                        method.Override();
                        method.AddParameter("DbContextEventData ", "eventData")
                            .AddParameter("InterceptionResult<int>", "result")
                            .AddParameter(UseType("System.Threading.CancellationToken"), "cancellationToken", p => p.WithDefaultValue("default"));

                        method.AddIfStatement("eventData.Context is null", block =>
                        {
                            block.AddStatement("return base.SavingChangesAsync(eventData, result, cancellationToken);");
                        });

                        // TODO: Fix obsolete method:
                        method.AddMethodChainStatement("var entries = eventData.Context.ChangeTracker", stmt => stmt
                            .AddChainStatement($"Entries<{this.GetSoftDeleteInterfaceName()}>()")
                            .AddChainStatement($"Where(e => e.State == {UseType("Microsoft.EntityFrameworkCore.EntityState")}.Deleted)")
                            .SeparatedFromPrevious());

                        method.AddForEachStatement("softDeletable", "entries", stmt =>
                        {
                            stmt.SeparatedFromPrevious();
                            stmt.AddStatement("softDeletable.State = EntityState.Modified;");
                            stmt.AddStatement("softDeletable.Entity.SetDeleted(true);");
                            stmt.AddStatement("HandleDependencies(eventData.Context, softDeletable);");
                        });

                        method.AddStatement("return base.SavingChangesAsync(eventData, result, cancellationToken);", s => s.SeparatedFromPrevious());
                    });

                    @class.AddMethod("void", "HandleDependencies", l =>
                    {
                        l.Private();
                        l.AddParameter(UseType("Microsoft.EntityFrameworkCore.DbContext"), "context");
                        l.AddParameter(UseType("Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry"), "entry");

                        l.AddStatement(new CSharpAssignmentStatement(
                            new CSharpVariableDeclaration("ownedReferencedEntries"),
                            new CSharpStatement("entry.References")
                                .AddInvocation("Where", i => i.AddArgument(new CSharpLambdaBlock("x"), a => a.WithExpressionBody("x.TargetEntry != null")).OnNewLine())
                                .AddInvocation("Select", i => i.AddArgument(new CSharpLambdaBlock("x"), a => a.WithExpressionBody("x.TargetEntry!")).OnNewLine())
                                .AddInvocation("Where", i => i.AddArgument(new CSharpLambdaBlock("x"), a => a.WithExpressionBody("x.State == EntityState.Deleted && x.Metadata.IsOwned()")).OnNewLine())
                        ));

                        l.AddForEachStatement("ownedEntry", "ownedReferencedEntries",
                            @for =>
                            {
                                @for.AddStatement("ownedEntry.State = EntityState.Unchanged;");
                                @for.AddStatement("HandleDependencies(context, ownedEntry);");
                            });

                        l.AddStatement(new CSharpAssignmentStatement(
                            new CSharpVariableDeclaration("ownedCollectionEntries"),
                            new CSharpStatement("entry.Collections")
                                .AddInvocation("Where", i => i.AddArgument(new CSharpLambdaBlock("x"), a => a.WithExpressionBody("x.IsLoaded && x.CurrentValue != null")).OnNewLine())
                                .AddInvocation("SelectMany", i => i.AddArgument(new CSharpLambdaBlock("x"), a => a.WithExpressionBody("x.CurrentValue!.Cast<object>().Select(context.Entry)")).OnNewLine())
                                .AddInvocation("Where", i => i.AddArgument(new CSharpLambdaBlock("x"), a => a.WithExpressionBody("x.State == EntityState.Deleted && x.Metadata.IsOwned()")).OnNewLine())
                        ), s => s.SeparatedFromPrevious());

                        l.AddForEachStatement("ownedEntry", "ownedCollectionEntries",
                            @for =>
                            {
                                @for.AddStatement("ownedEntry.State = EntityState.Unchanged;");
                                @for.AddStatement("HandleDependencies(context, ownedEntry);");
                            });
                    });
                });
        }

        public override bool CanRunTemplate()
        {
            return ExecutionContext.FindTemplateInstances<IIntentTemplate>(TemplateRoles.Infrastructure.Data.DbContext).Any();
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