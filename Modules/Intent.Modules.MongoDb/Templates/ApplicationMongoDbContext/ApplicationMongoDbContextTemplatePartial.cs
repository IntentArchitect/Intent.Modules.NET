using System.Collections.Generic;
using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.DependencyInjection;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.MongoDb.Templates.MongoDbUnitOfWorkInterface;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.MongoDb.Templates.ApplicationMongoDbContext
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge, Signature = Mode.Merge)]
    public partial class ApplicationMongoDbContextTemplate : CSharpTemplateBase<object>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.MongoDb.ApplicationMongoDbContext";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public ApplicationMongoDbContextTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            AddNugetDependency(NugetPackages.MongoDbDataUnitOfWork);

            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddUsing("System.Threading")
                .AddUsing("System.Threading.Tasks")
                .AddUsing("MongoDB.Driver")
                .AddClass($"ApplicationMongoDbContext", @class =>
                {
                    @class.WithBaseType("MongoDbContext");
                    @class.ImplementsInterface(GetTypeName(MongoDbUnitOfWorkInterfaceTemplate.TemplateId));
                    @class.AddConstructor(ctor =>
                    {
                        ctor.Static();
                        ctor.AddStatement($"ApplyConfigurationsFromAssembly(typeof({@class.Name}).Assembly);");
                    });
                    @class.AddConstructor(ctor =>
                    {
                        ctor.AddParameter("string", "connectionString");
                        ctor.AddParameter("string", "databaseName");
                        ctor.AddParameter("MongoDatabaseSettings", "databaseSettings", param => param.WithDefaultValue("null"));
                        ctor.CallsBase(b =>
                        {
                            b.AddArgument("connectionString");
                            b.AddArgument("databaseName");
                            b.AddArgument("databaseSettings");
                        });
                        ctor.AddStatement("AcceptAllChangesOnSave = true;");
                        ctor.AddStatement("AddCommand(() => null);");
                    });
                    @class.AddMethod("Task<int>", $"SaveChangesAsync", method =>
                    {
                        method.Async();
                        method.AddParameter("CancellationToken", "cancellationToken");
                        method.AddStatement("return (await base.SaveChangesAsync(cancellationToken)).Results.Count;");
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

        public override void BeforeTemplateExecution()
        {
            ExecutionContext.EventDispatcher.Publish(ServiceConfigurationRequest
                .ToRegister("AddMongoDbUnitOfWork")
                .ForConcern("Infrastructure")
                .RequiresUsingNamespaces("MongoDB.UnitOfWork.Abstractions.Extensions"));
            ExecutionContext.EventDispatcher.Publish(ServiceConfigurationRequest
                .ToRegister($"AddMongoDbUnitOfWork<{ClassName}>")
                .ForConcern("Infrastructure")
                .RequiresUsingNamespaces("MongoDB.UnitOfWork.Abstractions.Extensions"));
            ExecutionContext.EventDispatcher.Publish(ContainerRegistrationRequest
                .ToRegister(this)
                .ForInterface(this.GetTemplate<IClassProvider>(MongoDbUnitOfWorkInterfaceTemplate.TemplateId))
                .ForConcern("Infrastructure")
                .WithResolveFromContainer());
            ExecutionContext.EventDispatcher.Publish(ContainerRegistrationRequest
                .ToRegister(this)
                .ForInterface("IMongoDbContext")
                .RequiresUsingNamespaces("MongoDB.Infrastructure")
                .ForConcern("Infrastructure")
                .WithResolveFromContainer());
        }
    }
}