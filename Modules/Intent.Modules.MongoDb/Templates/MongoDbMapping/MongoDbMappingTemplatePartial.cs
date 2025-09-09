using System;
using System.Collections.Generic;
using Intent.Engine;
using Intent.Modelers.Domain.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.MongoDb.Templates.MongoDbMapping
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class MongoDbMappingTemplate : CSharpTemplateBase<ClassModel>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.MongoDb.MongoDbMapping";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public MongoDbMappingTemplate(IOutputTarget outputTarget, ClassModel model) : base(TemplateId, outputTarget, model)
        {
            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddUsing("MongoDB.Bson.Serialization")
                .AddUsing("MongoDB.Bson.Serialization.IdGenerators")
                .AddUsing("MongoDB.Bson.Serialization.Serializers")
                .AddClass($"{Model.Name}Mapping", @class =>
                {
                    @class.ImplementsInterface($"{GetTypeName(MongoMappingConfigurationInterface.MongoMappingConfigurationInterfaceTemplate.TemplateId)}<{Model.Name}>");

                    @class.AddProperty("string", "CollectionName", p => p.WithoutSetter().Getter.WithExpressionImplementation($"\"{Model.Name.Pluralize()}\""));
                    @class.AddMethod("void", "RegisterCollectionMap", registerCollectionMap =>
                    {
                        GetTypeName("Intent.Entities.DomainEntity", model.Id);
                        registerCollectionMap.AddIfStatement($"!BsonClassMap.IsClassMapRegistered(typeof({Model.Name}))", @if =>
                        {
                            @if.AddInvocationStatement($"BsonClassMap.RegisterClassMap<{Model.Name}>", invocation =>
                            {
                                invocation.AddLambdaBlock("mapping", block =>
                                {
                                    block.AddStatement("mapping.AutoMap();");
                                    block.AddStatement($"mapping.SetDiscriminator(nameof({Model.Name}));");

                                    var pkAttribute = Model.GetPrimaryKeyAttribute();

                                    block.AddInvocationStatement("mapping.MapIdMember", s => s.AddArgument($"x => x.{pkAttribute.Name}")
                                        .AddInvocation("SetIdGenerator", si => si.AddArgument("StringObjectIdGenerator.Instance"))
                                        .AddInvocation("SetSerializer", si => si.AddArgument("new StringSerializer(MongoDB.Bson.BsonType.ObjectId)")));
                                });
                            });
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