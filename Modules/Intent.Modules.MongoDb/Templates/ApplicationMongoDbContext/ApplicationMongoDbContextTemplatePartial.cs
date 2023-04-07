using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Metadata.RDBMS.Api;
using Intent.Modelers.Domain.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.DependencyInjection;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.Constants;
using Intent.Modules.MongoDb.Templates.MongoDbUnitOfWorkInterface;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.MongoDb.Templates.ApplicationMongoDbContext
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge, Signature = Mode.Merge)]
    public partial class ApplicationMongoDbContextTemplate : CSharpTemplateBase<IList<ClassModel>>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.MongoDb.ApplicationMongoDbContext";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public ApplicationMongoDbContextTemplate(IOutputTarget outputTarget, IList<ClassModel> model) : base(TemplateId, outputTarget, model)
        {
            AddNugetDependency(NugetPackages.MongoFramework);

            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddUsing("System.Threading")
                .AddUsing("System.Threading.Tasks")
                .AddUsing("MongoFramework")
                .AddUsing("MongoFramework.Infrastructure.Mapping")
                .AddClass($"ApplicationMongoDbContext")
                .OnBuild(file =>
                {
                    var @class = file.Classes.First();
                    @class.WithBaseType("MongoDbContext");
                    @class.ImplementsInterface(GetTypeName(MongoDbUnitOfWorkInterfaceTemplate.TemplateId));
                    @class.AddConstructor(ctor =>
                    {
                        ctor.AddParameter("IMongoDbConnection", "connection");
                        ctor.CallsBase(b =>
                        {
                            b.AddArgument("connection");
                        });
                    });

                    foreach (var aggregate in Model)
                    {
                        @class.AddProperty($"MongoDbSet<{GetTypeName(TemplateFulfillingRoles.Domain.Entity.Primary, aggregate)}>", aggregate.Name.Pluralize().ToPascalCase());
                    }

                    @class.InsertMethod(0, "Task<int>", "SaveChangesAsync", method =>
                    {
                        method.Override().Async()
                            .AddParameter($"CancellationToken", "cancellationToken",
                                param =>
                                {
                                    param.WithDefaultValue("default");
                                })
                            .AddStatement("await base.SaveChangesAsync(cancellationToken);")
                            .AddStatement("return default;");
                    });

                    /*
                    @class.AddMethod("Task<int>", $"{GetTypeName(MongoDbUnitOfWorkInterfaceTemplate.TemplateId)}.SaveChangesAsync", method =>
                    {
                        method.WithoutAccessModifier().Async().AddParameter("CancellationToken", "cancellationToken", param => param.WithDefaultValue("default"));
                        method.AddStatement("await SaveChangesAsync(cancellationToken);");
                        method.AddStatement("return default;");
                    });

                    var originalUnitOfWorkInterfaceName = this.GetTypeName(TemplateFulfillingRoles.Domain.UnitOfWork, TemplateDiscoveryOptions.DoNotThrow);
                    if (!string.IsNullOrEmpty(originalUnitOfWorkInterfaceName))
                    {
                        @class.AddMethod("Task<int>", $"{originalUnitOfWorkInterfaceName}.SaveChangesAsync", method =>
                        {
                            method.WithoutAccessModifier().Async().AddParameter("CancellationToken", "cancellationToken", param => param.WithDefaultValue("default"));
                            method.AddStatement("await SaveChangesAsync(cancellationToken);");
                            method.AddStatement("return default;");
                        });
                    }*/

                    @class.AddMethod("void", "Dispose", method =>
                    {
                        method.Protected().Override().AddParameter("bool", "disposing");
                        method.AddStatement("// Don't call the base's dispose as it disposes the connection which is not recommended as per https://www.mongodb.com/docs/manual/administration/connection-pool-overview/");
                    });
                })
                .AfterBuild(file =>
                {
                    var @class = file.Classes.First();
                    @class.AddMethod("void", "OnConfigureMapping", method =>
                    {
                        method.Protected().Override().AddParameter("MappingBuilder", "mappingBuilder");

                        foreach (var aggregate in Model)
                        {
                            method.AddStatement(GetEntityRegistrationStatements(aggregate));
                        }

                        method.AddStatement("base.OnConfigureMapping(mappingBuilder);");
                    });

                });
        }

        private CSharpStatement GetEntityRegistrationStatements(ClassModel aggregate)
        {

            var result = new CSharpMethodChainStatement($"mappingBuilder.Entity<{GetTypeName(TemplateFulfillingRoles.Domain.Entity.Primary, aggregate)}>()");
            var pk = aggregate.GetExplicitPrimaryKey().Single();

            if (pk.Type.Element.IsStringType())
            {
                result.AddChainStatement($"HasKey(e => e.{pk.Name.ToPascalCase()}, d => d.HasKeyGenerator(EntityKeyGenerators.StringKeyGenerator))");
            }
            else if (pk.Type.Element.IsGuidType())
            {
                result.AddChainStatement($"HasKey(e => e.{pk.Name.ToPascalCase()}, d => d.HasKeyGenerator(EntityKeyGenerators.GuidKeyGenerator))");
            }
            //result.AddChainStatement($"HasKey(e => e.{pk.Name.ToPascalCase()}, d => d.HasKeyGenerator(EntityKeyGenerators.ObjectIdKeyGenerator))");
#warning what do we need here
            /*
            else if (pk.Type.Element.IsIntType())
            {
                result.AddChainStatement($"HasKey(e => e.{pk.Name.ToPascalCase()}, d => new Int32KeyGenerator<MyIntity>(this.Connection))");
            }
            else if (pk.Type.Element.IsLongType())
            {
                result.AddChainStatement($"HasKey(e => e.{pk.Name.ToPascalCase()}, d => new Int64KeyGenerator<MyIntity>(this.Connection))");
            }*/
            else
            {
                throw new InvalidOperationException($"Given Type [{pk.Type.Element.Name}] is not valid for an Id for Element {aggregate.Name} [{aggregate.Id}].");
            }

            CSharpClass? entityClass = null;
            if ((TryGetTemplate("Domain.Entity.State", aggregate, out ICSharpFileBuilderTemplate entityTemplate) ||
                 TryGetTemplate("Domain.Entity", aggregate, out entityTemplate)))
            {
                entityClass = entityTemplate.CSharpFile.Classes.First();
                foreach (var property in entityClass.GetAllProperties())
                {
                    if (property.TryGetMetadata("non-persistent", out bool nonPersistent))
                    {
                        result.AddChainStatement($"Ignore(e => e.{property.Name})");
                    }
                }
            }

            return result;
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