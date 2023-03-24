using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
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
                .AddUsing("MongoFramework")
                .AddUsing("MongoFramework.Infrastructure.Mapping")
                .AddClass($"ApplicationMongoDbContext", @class =>
                {
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

                    @class.AddMethod("void", "OnConfigureMapping", method =>
                    {
                        method.Protected().Override().AddParameter("MappingBuilder", "mappingBuilder");

                        foreach (var aggregate in Model)
                        {
                            method.AddStatement(GetEntityRegistrationStatements(aggregate));
                        }

                        method.AddStatement("base.OnConfigureMapping(mappingBuilder);");
                    });

                    @class.AddMethod("void", "Dispose", method =>
                    {
                        method.Protected().Override().AddParameter("bool", "disposing");
                        method.AddStatement("//we don't want to dispose the connection which the base class does");
                    });
                });
        }

        private CSharpStatement GetEntityRegistrationStatements(ClassModel aggregate)
        {
            var result = new CSharpMethodChainStatement($"mappingBuilder.Entity<{GetTypeName(TemplateFulfillingRoles.Domain.Entity.Primary, aggregate)}>()");
            var pk = aggregate.GetExplicitPrimaryKey().Single();

            if (pk.Type.Element.IsStringType())
            {
                result.AddChainStatement($"HasKey(e => e.{pk.Name.ToPascalCase()}, d => d.HasKeyGenerator(EntityKeyGenerators.ObjectIdKeyGenerator))");
            }
            else if (pk.Type.Element.IsGuidType())
            {
                result.AddChainStatement($"HasKey(e => e.{pk.Name.ToPascalCase()}, d => d.HasKeyGenerator(EntityKeyGenerators.GuidKeyGenerator))");
            }
#warning what do we need here
            else if (pk.Type.Element.IsIntType())
            {
                result.AddChainStatement($"HasKey(e => e.{pk.Name.ToPascalCase()}, d => new Int32KeyGenerator<MyIntity>(this.Connection))");
            }
            else if (pk.Type.Element.IsLongType())
            {
                result.AddChainStatement($"HasKey(e => e.{pk.Name.ToPascalCase()}, d => new Int64KeyGenerator<MyIntity>(this.Connection))");
            }
            else
            {
                throw new InvalidOperationException($"Given Type [{pk.Type.Element.Name}] is not valid for an Id for Element {aggregate.Name} [{aggregate.Id}].");
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