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

namespace Intent.Modules.Dapr.AspNetCore.StateManagement.Templates.DaprStateStoreGenericRepositoryInterface
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class DaprStateStoreGenericRepositoryInterfaceTemplate : CSharpTemplateBase<object>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.Dapr.AspNetCore.StateManagement.DaprStateStoreGenericRepositoryInterface";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public DaprStateStoreGenericRepositoryInterfaceTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddUsing("System.Threading")
                .AddUsing("System.Threading.Tasks")
                .AddInterface("IDaprStateStoreGenericRepository", @interface => @interface
                    .WithComments(new[]
                    {
                        "/// <summary>",
                        "/// A generic repository for working with values by key against an underlying key/value state store.",
                        "/// </summary>"
                    })
                    .AddMethod("void", "Upsert", method => method
                        .WithComments(new[]
                        {
                            "/// <summary>",
                            "/// Upserts the provided <paramref name=\"value\" /> associated with the provided <paramref name=\"key\" /> to the state",
                            "/// store.",
                            "/// </summary>",
                            "/// <remarks>",
                            "/// The implementation of this interface follows a unit of work pattern. Calling this",
                            "/// method internally queues up a work action which is only executed when",
                            $"/// <see cref=\"UnitOfWork\"/>'s <see cref=\"{UnitOfWorkTypeName}.SaveChangesAsync\"/> is called.",
                            "/// </remarks>",
                            "/// <typeparam name=\"TValue\">The type of the data that will be JSON serialized and stored in the state store.</typeparam>",
                            "/// <param name=\"key\">The state key.</param>",
                            "/// <param name=\"value\">The data that will be JSON serialized and stored in the state store.</param>"
                        })
                        .AddGenericParameter("TValue")
                        .AddParameter("string", "key")
                        .AddParameter("TValue", "value")
                    )
                    .AddMethod("Task<TValue>", "GetAsync", method => method
                        .WithComments(new[]
                        {
                            "",
                            "/// <summary>",
                            "/// Gets the current value associated with the <paramref name=\"key\" /> from the state store.",
                            "/// </summary>",
                            "/// <typeparam name=\"TValue\">The data type of the value to read.</typeparam>",
                            "/// <param name=\"key\">The state key.</param>",
                            "/// <param name=\"cancellationToken\">A <see cref=\"CancellationToken\" /> that can be used to cancel the operation.</param>",
                            "/// <returns>A <see cref=\"Task{T}\" /> that will return the value when the operation has completed.</returns>"
                        })
                        .AddGenericParameter("TValue")
                        .AddParameter("string", "key")
                        .AddParameter("CancellationToken", "cancellationToken", p => p.WithDefaultValue("default"))
                    )
                    .AddMethod("void", "Delete", method => method
                        .WithComments(new[]
                        {
                            "",
                            "/// <summary>",
                            "/// Deletes the value associated with the provided <paramref name=\"key\" /> in the state store.",
                            "/// </summary>",
                            "/// <remarks>",
                            "/// The implementation of this interface follows a unit of work pattern. Calling this",
                            "/// method internally queues up a work action which is only executed when",
                            $"/// <see cref=\"UnitOfWork\"/>'s <see cref=\"{UnitOfWorkTypeName}.SaveChangesAsync\"/> is called.",
                            "/// </remarks>",
                            "/// <param name=\"key\">The state key.</param>",
                            "/// <returns>A <see cref=\"Task\" /> that will complete when the operation has completed.</returns>"
                        })
                        .AddParameter("string", "key")
                    )
                    .AddProperty(UnitOfWorkTypeName, "UnitOfWork", property => property
                        .WithComments(new[]
                        {
                            "",
                            "/// <summary>",
                            $"/// Provides access to the <see cref=\"{UnitOfWorkTypeName}\"/>.",
                            "/// </summary>",
                        })
                        .WithoutSetter()
                    )
                );
        }

        private string UnitOfWorkTypeName => this.GetDaprStateStoreUnitOfWorkInterfaceName();

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