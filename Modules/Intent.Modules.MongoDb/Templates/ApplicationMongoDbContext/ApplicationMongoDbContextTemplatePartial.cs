using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Metadata.DocumentDB.Api;
using Intent.Metadata.Models;
using Intent.Modelers.Domain.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.Constants;
using Intent.Modules.MongoDb.Templates.MongoDbUnitOfWorkInterface;
using Intent.MongoDb.Api;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;
using ClassExtensionModel = Intent.MongoDb.Api.ClassExtensionModel;

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
            AddNugetDependency(NugetPackages.MongoFramework(OutputTarget));

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
                        // MongoFramework doesn't seem to support entities with generic type parameters
                        if (aggregate.GenericTypes.Any())
                        {
                            continue;
                        }

                        @class.AddProperty($"MongoDbSet<{GetTypeName(TemplateRoles.Domain.Entity.Primary, aggregate)}>", aggregate.Name.Pluralize().ToPascalCase());
                    }

                    @class.InsertMethod(0, "Task<int>", "SaveChangesAsync", method =>
                    {
                        method.Override().Async()
                            .AddParameter("CancellationToken", "cancellationToken",
                                param =>
                                {
                                    param.WithDefaultValue("default");
                                })
                            .AddStatement("await base.SaveChangesAsync(cancellationToken);")
                            .AddStatement("return default;");
                    });

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
                            if (aggregate.GenericTypes.Any())
                            {
                                // MongoFramework doesn't seem to support entities with generic type parameters
                                continue;
                            }

                            method.AddStatement(GetEntityRegistrationStatements(aggregate));
                        }

                        method.AddStatement("base.OnConfigureMapping(mappingBuilder);");
                    });

                });
        }

        private CSharpStatement GetEntityRegistrationStatements(ClassModel aggregate)
        {

            var result = new CSharpMethodChainStatement($"mappingBuilder.Entity<{GetTypeName(TemplateRoles.Domain.Entity.Primary, aggregate)}>()");

            var pk = aggregate.Attributes.SingleOrDefault(x => x.HasPrimaryKey());
            if (pk != null)
            {
                var invocation = new CSharpInvocationStatement("HasKey")
                    .WithoutSemicolon()
                    .AddArgument(new CSharpLambdaBlock("entity").WithExpressionBody($"entity.{pk.Name.ToPascalCase()}"));
                if (!pk.GetPrimaryKey().DataSource().IsUserSupplied())
                {
                    invocation.AddArgument(new CSharpLambdaBlock("build").WithExpressionBody(GetKeyGeneratorExpression(pk.Type.Element, aggregate)));
                }
                result.AddChainStatement(invocation);
            }

            if (aggregate.HasCollection() || aggregate.Folder?.HasCollection() == true)
            {
                var collectionName = aggregate.GetCollection()?.Name() ??
                                     aggregate.Folder?.GetCollection()?.Name();
                result.AddChainStatement($@"ToCollection(""{collectionName}"")");
            }

            foreach (var index in new ClassExtensionModel(aggregate.InternalElement).MongoDbIndices)
            {
                if (index.Fields.Count == 0)
                {
                    continue;
                }

                var build = new CSharpMethodChainStatement("build")
                    .WithoutSemicolon()
                    .AddChainStatement($@"HasName(""{index.Name}"")");

                build.AddChainStatement("HasType(IndexType.Standard)");

                if (index.GetSettings().SortOrder().IsDescending())
                {
                    build.AddChainStatement("IsDescending()");
                }

                result.AddChainStatement(new CSharpInvocationStatement("HasIndex")
                    .WithoutSemicolon()
                    .AddArgument(new CSharpLambdaBlock("entity").WithExpressionBody(GetEntityIndexExpression(index)))
                    .AddArgument(new CSharpLambdaBlock("build").WithExpressionBody(build)));
            }

            if (!TryGetTemplate("Domain.Entity.State", aggregate, out ICSharpFileBuilderTemplate entityTemplate) &&
                !TryGetTemplate("Domain.Entity", aggregate, out entityTemplate))
            {
                return result;
            }

            var entityClass = entityTemplate.CSharpFile.Classes.First();
            foreach (var property in entityClass.GetAllProperties())
            {
                if (property.TryGetMetadata("non-persistent", out _))
                {
                    result.AddChainStatement($"Ignore(entity => entity.{property.Name})");
                }
            }

            return result;
        }

        private CSharpStatement GetEntityIndexExpression(DocumentStoreIndexModel index)
        {
            if (index.Fields.Count != 1)
            {
                var block = new CSharpObjectInitializerBlock("new ");
                foreach (var field in index.Fields)
                {
                    block.AddStatement($"entity.{GetMapPathExpression(field)}");
                }
                return block;
            }

            return $"entity.{GetMapPathExpression(index)}";
        }

        private static string GetMapPathExpression(DocumentStoreIndexModel index)
        {
            return string.Join(".", GetMapPathExpression(index.Fields[0]));
        }

        private static string GetMapPathExpression(IndexFieldModel field)
        {
            return string.Join(".", field.InternalElement.MappedElement.Path
                .Select(x => $"{x.Name}{(x.Element.AsAssociationEndModel()?.IsCollection == true ? ".First()" : string.Empty)}"));
        }

        private static string GetKeyGeneratorExpression(ICanBeReferencedType typeElement, ClassModel aggregate)
        {

            return typeElement switch
            {
                _ when typeElement.IsStringType() => "build.HasKeyGenerator(EntityKeyGenerators.StringKeyGenerator)",
                _ when typeElement.IsGuidType() => "build.HasKeyGenerator(EntityKeyGenerators.GuidKeyGenerator)",
                _ => throw new InvalidOperationException($"Given Type [{typeElement.Name}] is not valid for an Id for Element {aggregate.Name} [{aggregate.Id}].")
            };
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