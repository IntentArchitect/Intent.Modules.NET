using System.Linq;
using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.CSharp.VisualStudio;
using Intent.Modules.Common.Plugins;
using Intent.Plugins.FactoryExtensions;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.FactoryExtension", Version = "1.0")]

namespace Intent.Modules.EntityFrameworkCore.BulkOperations.FactoryExtensions
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class BulkOperationExtension : FactoryExtensionBase
    {
        public override string Id => "Intent.EntityFrameworkCore.BulkOperations.BulkOperationExtension";

        [IntentManaged(Mode.Ignore)]
        public override int Order => 0;

        protected override void OnAfterTemplateRegistrations(IApplication application)
        {
            var baseEFRepositoryTemplate = application.FindTemplateInstance<ICSharpFileBuilderTemplate>("Intent.EntityFrameworkCore.Repositories.RepositoryBase");
            if (baseEFRepositoryTemplate == null)
                return;

            AppendEFRepository(application, baseEFRepositoryTemplate);

            var baseEFRepositoryInterfaceTemplate = application.FindTemplateInstance<ICSharpFileBuilderTemplate>("Intent.EntityFrameworkCore.Repositories.EFRepositoryInterface");
            if (baseEFRepositoryInterfaceTemplate == null)
                return;

            AppendEFRepositoryInterface(application, baseEFRepositoryInterfaceTemplate);
        }

        private void AppendEFRepositoryInterface(IApplication application, ICSharpFileBuilderTemplate template)
        {
            template.CSharpFile.OnBuild(file =>
            {
                string nullableChar = template.OutputTarget.GetProject().NullableEnabled ? "?" : "";
                var @interface = file.Interfaces.First();
                @interface.AddMethod("Task", "BulkInsertAsync", method =>
                {
                    method
                        .AddParameter($"IEnumerable<TDomain>", "entities")
                        .AddParameter("CancellationToken", "cancellationToken", p => p.WithDefaultValue("default"));
                });
                @interface.AddMethod("Task", "BulkUpdateAsync", method =>
                {
                    method
                        .AddParameter($"IEnumerable<TDomain>", "entities")
                        .AddParameter("CancellationToken", "cancellationToken", p => p.WithDefaultValue("default"));
                });
                @interface.AddMethod("Task", "BulkMergeAsync", method =>
                {
                    method
                        .AddParameter($"IEnumerable<TDomain>", "entities")
                        .AddParameter("CancellationToken", "cancellationToken", p => p.WithDefaultValue("default"));
                });
                if (template.ExecutionContext.Settings.GetDatabaseSettings().AddSynchronousMethodsToRepositories())
                {
                    @interface.AddMethod("void", "BulkInsert", method =>
                    {
                        method
                            .AddParameter($"IEnumerable<TDomain>", "entities")
                            ;
                    });
                    @interface.AddMethod("void", "BulkUpdate", method =>
                    {
                        method
                            .AddParameter($"IEnumerable<TDomain>", "entities")
                            ;
                    });
                    @interface.AddMethod("void", "BulkMerge", method =>
                    {
                        method
                            .AddParameter($"IEnumerable<TDomain>", "entities")
                            ;
                    });
                }
            });
        }

        private void AppendEFRepository(IApplication application, ICSharpFileBuilderTemplate template)
        {
            template.CSharpFile.OnBuild(file =>
            {
                template.AddUsing("Z.BulkOperations");

                template.AddNugetDependency(NugetPackages.ZEntityFrameworkExtensionsEFCore(template.OutputTarget));
                var @class = file.Classes.First();

                string nullableChar = template.OutputTarget.GetProject().NullableEnabled ? "?" : "";

                @class.AddMethod("Task", "BulkInsertAsync", method =>
                {
                    method
                        .Async()
                        .AddParameter($"IEnumerable<TDomain>", "entities")
                        .AddParameter($"Action<BulkOperation<TDomain>>{nullableChar}", "configure", p => p.WithDefaultValue("default"))
                        .AddParameter("CancellationToken", "cancellationToken", p => p.WithDefaultValue("default"));
                    method.AddInvocationStatement("await _dbContext.BulkInsertAsync", invocation =>
                    {
                        invocation.AddArgument("entities");
                        invocation.AddArgument(new CSharpLambdaBlock("options"), a =>
                        {
                            var options = (CSharpLambdaBlock)a;
                            options.AddStatement("configure?.Invoke(options);");
                        });
                        invocation.AddArgument("cancellationToken");
                    });
                });
                @class.AddMethod("Task", "BulkInsertAsync", method =>
                {
                    method
                        .Async()
                        .AddParameter($"IEnumerable<TDomain>", "entities")
                        .AddParameter("CancellationToken", "cancellationToken", p => p.WithDefaultValue("default"));
                    method.AddStatement("await BulkInsertAsync(entities, null, cancellationToken);");
                });
                @class.AddMethod("Task", "BulkUpdateAsync", method =>
                {
                    method
                        .Async()
                        .AddParameter($"IEnumerable<TDomain>", "entities")
                        .AddParameter($"Action<BulkOperation<TDomain>>{nullableChar}", "configure", p => p.WithDefaultValue("default"))
                        .AddParameter("CancellationToken", "cancellationToken", p => p.WithDefaultValue("default"));
                    method.AddInvocationStatement("await _dbContext.BulkUpdateAsync", invocation =>
                    {
                        invocation.AddArgument("entities");
                        invocation.AddArgument(new CSharpLambdaBlock("options"), a =>
                        {
                            var options = (CSharpLambdaBlock)a;
                            options.AddStatement("configure?.Invoke(options);");
                        });
                        invocation.AddArgument("cancellationToken");
                    });
                });
                @class.AddMethod("Task", "BulkUpdateAsync", method =>
                {
                    method
                        .Async()
                        .AddParameter($"IEnumerable<TDomain>", "entities")
                        .AddParameter("CancellationToken", "cancellationToken", p => p.WithDefaultValue("default"));
                    method.AddStatement("await BulkUpdateAsync(entities, null, cancellationToken);");
                });
                @class.AddMethod("Task", "BulkMergeAsync", method =>
                {
                    method
                        .Async()
                        .AddParameter($"IEnumerable<TDomain>", "entities")
                        .AddParameter($"Action<BulkOperation<TDomain>>{nullableChar}", "configure", p => p.WithDefaultValue("default"))
                        .AddParameter("CancellationToken", "cancellationToken", p => p.WithDefaultValue("default"));
                    method.AddInvocationStatement("await _dbContext.BulkUpdateAsync", invocation =>
                    {
                        invocation.AddArgument("entities");
                        invocation.AddArgument(new CSharpLambdaBlock("options"), a =>
                        {
                            var options = (CSharpLambdaBlock)a;
                            options.AddStatement("configure?.Invoke(options);");
                        });
                        invocation.AddArgument("cancellationToken");
                    });
                });
                @class.AddMethod("Task", "BulkMergeAsync", method =>
                {
                    method
                        .Async()
                        .AddParameter($"IEnumerable<TDomain>", "entities")
                        .AddParameter("CancellationToken", "cancellationToken", p => p.WithDefaultValue("default"));
                    method.AddStatement("await BulkMergeAsync(entities, null, cancellationToken);");
                });


                if (template.ExecutionContext.Settings.GetDatabaseSettings().AddSynchronousMethodsToRepositories())
                {
                    @class.AddMethod("void", "BulkInsert", method =>
                    {
                        method
                            .AddParameter($"IEnumerable<TDomain>", "entities")
                            .AddParameter($"Action<BulkOperation<TDomain>>{nullableChar}", "configure", p => p.WithDefaultValue("default"));
                        method.AddInvocationStatement("_dbContext.BulkInsert", invocation =>
                        {
                            invocation.AddArgument("entities");
                            invocation.AddArgument(new CSharpLambdaBlock("options"), a =>
                            {
                                var options = (CSharpLambdaBlock)a;
                                options.AddStatement("configure?.Invoke(options);");

                            });
                        });
                    });
                    @class.AddMethod("void", "BulkInsert", method =>
                    {
                        method
                            .AddParameter($"IEnumerable<TDomain>", "entities");
                        method.AddStatement("BulkInsert(entities);");
                    });
                    @class.AddMethod("void", "BulkUpdate", method =>
                    {
                        method
                            .AddParameter($"IEnumerable<TDomain>", "entities")
                            .AddParameter($"Action<BulkOperation<TDomain>>{nullableChar}", "configure", p => p.WithDefaultValue("default"));
                        method.AddInvocationStatement("_dbContext.BulkUpdate", invocation =>
                        {
                            invocation.AddArgument("entities");
                            invocation.AddArgument(new CSharpLambdaBlock("options"), a =>
                            {
                                var options = (CSharpLambdaBlock)a;
                                options.AddStatement("configure?.Invoke(options);");
                            });
                        });
                    });
                    @class.AddMethod("void", "BulkUpdate", method =>
                    {
                        method
                            .AddParameter($"IEnumerable<TDomain>", "entities");
                        method.AddStatement("BulkUpdate(entities);");
                    });
                    @class.AddMethod("void", "BulkMerge", method =>
                    {
                        method
                            .AddParameter($"IEnumerable<TDomain>", "entities")
                            .AddParameter($"Action<BulkOperation<TDomain>>{nullableChar}", "configure", p => p.WithDefaultValue("default"));
                        method.AddInvocationStatement("_dbContext.BulkUpdate", invocation =>
                        {
                            invocation.AddArgument("entities");
                            invocation.AddArgument(new CSharpLambdaBlock("options"), a =>
                            {
                                var options = (CSharpLambdaBlock)a;
                                options.AddStatement("configure?.Invoke(options);");
                            });
                        });
                    });
                    @class.AddMethod("void", "BulkMerge", method =>
                    {
                        method
                            .AddParameter($"IEnumerable<TDomain>", "entities");
                        method.AddStatement("BulkMerge(entities);");
                    });

                }
            });
        }

    }
}