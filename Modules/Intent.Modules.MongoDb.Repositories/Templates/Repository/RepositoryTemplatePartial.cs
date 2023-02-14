using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Modelers.Domain.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.DependencyInjection;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.Entities.Repositories.Api.Templates.EntityRepositoryInterface;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.MongoDb.Repositories.Templates.Repository
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    partial class RepositoryTemplate : CSharpTemplateBase<ClassModel>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.MongoDb.Repositories.Repository";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public RepositoryTemplate(IOutputTarget outputTarget, ClassModel model) : base(TemplateId, outputTarget, model)
        {
            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddUsing("System")
                .AddUsing("System.Collections.Generic")
                .AddUsing("System.Linq")
                .AddUsing("System.Threading")
                .AddUsing("System.Threading.Tasks")
                .AddClass($"{Model.Name}Repository", @class =>
                {
                    @class.WithBaseType($"{this.GetMongoRepositoryBaseName()}<{EntityInterfaceName}, {EntityName}>");
                    @class.ImplementsInterface(RepositoryContractName);
                    @class.AddConstructor(ctor =>
                    {
                        ctor.AddParameter(DbContextName, "context");
                        ctor.CallsBase(b => b.AddArgument("context"));
                    });

                    if (TryGetTemplate<ICSharpFileBuilderTemplate>("Domain.Entity", Model, out var entityTemplate))
                    {
                        entityTemplate.CSharpFile.AfterBuild(file =>
                        {
                            var rootEntity = file.Classes.First().GetRootEntity();
                            if (!rootEntity.HasSinglePrimaryKey())
                            {
                                @class.AddMethod("void", "Remove", method =>
                                {
                                    method.Override();
                                    method.AddParameter(EntityInterfaceName, "entity");
                                    method.AddStatement($@"throw NotImplementedException(""Composite Keys not supported and needs to be implemented manually"");");
                                });
                            }
                            else
                            {
                                @class.AddMethod($"Task<{GetTypeName("Domain.Entity.Interface", Model)}>", "FindByIdAsync", method =>
                                {
                                    var pk = rootEntity.GetPropertyWithPrimaryKey();
                                    method.Async();
                                    method.AddParameter(entityTemplate.UseType(pk.Type), pk.Name.ToCamelCase());
                                    method.AddParameter("CancellationToken", "cancellationToken", param => param.WithDefaultValue("default"));

                                    method.AddStatement($"return await FindAsync(x => x.{pk.Name} == {pk.Name.ToCamelCase()}, cancellationToken);");
                                });
                                @class.AddMethod($"Task<List<{GetTypeName("Domain.Entity.Interface", Model)}>>", "FindByIdsAsync", method =>
                                {
                                    var pk = rootEntity.GetPropertyWithPrimaryKey();
                                    method.Async();
                                    method.AddParameter($"{entityTemplate.UseType(pk.Type)}[]", pk.Name.ToCamelCase().Pluralize());
                                    method.AddParameter("CancellationToken", "cancellationToken", param => param.WithDefaultValue("default"));

                                    method.AddStatement($"return await FindAllAsync(x => {pk.Name.ToCamelCase().Pluralize()}.Contains(x.{pk.Name}), cancellationToken);");
                                });

                                @class.AddMethod("void", "Remove", method =>
                                {
                                    var pk = rootEntity.GetPropertyWithPrimaryKey();
                                    method.Override();
                                    method.AddParameter(EntityInterfaceName, "entity");
                                    method.AddStatement($"base.DeleteOne(p => p.{pk.Name} == entity.{pk.Name});");
                                });
                            }
                        });
                    }
                });
        }
        
        public string EntityName => GetTypeName("Domain.Entity", Model);

        public string EntityInterfaceName => GetTypeName("Domain.Entity.Interface", Model);

        public string RepositoryContractName => TryGetTypeName(EntityRepositoryInterfaceTemplate.TemplateId, Model) ?? $"I{ClassName}";
        public string DbContextName => TryGetTypeName("Infrastructure.Data.DbContext.MongoDb", out var dbContextName) ? dbContextName : $"{Model.Application.Name}MongoDbContext";


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
            var contractTemplate = Project.FindTemplateInstance<IClassProvider>(EntityRepositoryInterfaceTemplate.TemplateId, Model);
            if (contractTemplate == null)
            {
                return;
            }

            ExecutionContext.EventDispatcher.Publish(ContainerRegistrationRequest.ToRegister(this)
                .ForConcern("Infrastructure")
                .ForInterface(contractTemplate));
        }
    }
}