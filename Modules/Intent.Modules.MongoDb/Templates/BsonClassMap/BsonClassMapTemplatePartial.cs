using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Metadata.RDBMS.Api;
using Intent.Modelers.Domain.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.Constants;
using Intent.Modules.MongoDb.Settings;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.MongoDb.Templates.BsonClassMap
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    partial class BsonClassMapTemplate : CSharpTemplateBase<ClassModel>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.MongoDb.BsonClassMap";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public BsonClassMapTemplate(IOutputTarget outputTarget, ClassModel model) : base(TemplateId, outputTarget, model)
        {
            AddNugetDependency(NugetPackages.MongoDbDataUnitOfWork);
            AddTypeSource(TemplateFulfillingRoles.Domain.Entity.Primary);

            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddClass($"{Model.Name}ClassMap")
                .OnBuild(file =>
                {
                    file.AddUsing("MongoDB.Bson");
                    file.AddUsing("MongoDB.Bson.Serialization");
                    file.AddUsing("MongoDB.Bson.Serialization.IdGenerators");
                    file.AddUsing("MongoDB.Bson.Serialization.Serializers");
                    file.AddUsing("MongoDB.Infrastructure");
                    var priClass = file.Classes.First();
                    priClass.ImplementsInterface("IMongoDbFluentConfiguration");
                    priClass.AddMethod("void", "Configure", method =>
                    {
                        method.AddStatement(new CSharpStatementBlock($"if (BsonClassMap.IsClassMapRegistered(typeof({GetTypeName(model.InternalElement)})))")
                            .AddStatement("return;"));
                        method.AddStatement(new CSharpInvocationStatement($"BsonClassMap.RegisterClassMap<{GetTypeName(model.InternalElement)}>")
                            .AddArgument(new CSharpLambdaBlock("build")
                                .AddStatement("build.AutoMap();")
                                .AddStatement(GetIdRegistrationStatements()))
                            .WithArgumentsOnNewLines());
                    });
                });
        }

        private CSharpStatement GetIdRegistrationStatements()
        {
            var pk = Model.GetExplicitPrimaryKey().Single();

            if (pk.Type.Element.IsStringType())
            {
                return new CSharpMethodChainStatement("build.MapIdProperty(c => c.Id)")
                    .AddChainStatement("SetIdGenerator(StringObjectIdGenerator.Instance)")
                    .AddChainStatement("SetSerializer(new StringSerializer(BsonType.ObjectId))");
            }
            if (pk.Type.Element.IsGuidType())
            {
                return new CSharpMethodChainStatement("build.MapIdProperty(c => c.Id)")
                    .AddChainStatement("SetIdGenerator(GuidGenerator.Instance)");
            }
            if (pk.Type.Element.IsIntType())
            {
                AddNugetDependency(NugetPackages.MongoDBDataGenerators);
                AddUsing("MongoDB.Generators");
                return new CSharpMethodChainStatement("build.MapIdProperty(c => c.Id)")
                    .AddChainStatement($"SetIdGenerator(Int32IdGenerator<{GetTypeName(Model.InternalElement)}>.Instance)");
            }
            if (pk.Type.Element.IsLongType())
            {
                AddNugetDependency(NugetPackages.MongoDBDataGenerators);
                AddUsing("MongoDB.Generators");
                return new CSharpMethodChainStatement("build.MapIdProperty(c => c.Id)")
                    .AddChainStatement($"SetIdGenerator(Int64IdGenerator<{GetTypeName(Model.InternalElement)}>.Instance)");
            }

            throw new InvalidOperationException($"Given Type [{pk.Type.Element.Name}] is not valid for an Id for Element {Model.Name} [{Model.Id}].");
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