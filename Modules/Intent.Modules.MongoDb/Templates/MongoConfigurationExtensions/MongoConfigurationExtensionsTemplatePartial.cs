using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.Constants;
using Intent.Modules.Entities.Repositories.Api.Templates.EntityRepositoryInterface;
using Intent.Modules.MongoDb.Templates.MongoDbMapping;
using Intent.Modules.MongoDb.Templates.MongoDbRepository;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;
using System;
using System.Collections.Generic;
using System.Linq;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.MongoDb.Templates.MongoConfigurationExtensions
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class MongoConfigurationExtensionsTemplate : CSharpTemplateBase<object>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.MongoDb.MongoConfigurationExtensions";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public MongoConfigurationExtensionsTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddClass($"MongoConfigurationExtensions", @class =>
                {
                    @class.Static().Internal();

                    @class.AddMethod("IServiceCollection", "AddMongoCollection<T>", addMongoCollection =>
                    {
                        addMongoCollection.Static();
                        addMongoCollection.AddParameter("IServiceCollection", "services", p => p.WithThisModifier());
                        addMongoCollection.AddParameter("IMongoMappingConfiguration<T>", "mongoConfiguration");

                        addMongoCollection.AddStatement("mongoConfiguration.RegisterCollectionMap();");

                        addMongoCollection.AddStatements($@"services.AddSingleton(sp =>
                        {{
                            var database = sp.GetRequiredService<IMongoDatabase>();
                            return database.GetCollection<T>(mongoConfiguration.CollectionName);
                        }});".ConvertToStatements());

                        addMongoCollection.AddReturn("services");
                    });

                    @class.AddMethod("IServiceCollection", "RegisterMongoCollections", registerMongoCollections =>
                    {
                        registerMongoCollections.Static();
                        registerMongoCollections.AddParameter("IServiceCollection", "services", p => p.WithThisModifier());

                        // Foreach model mapping
                        foreach (var model in ExecutionContext.FindTemplateInstances<ICSharpFileBuilderTemplate>(MongoDbMappingTemplate.TemplateId))
                        {
                            GetTypeName(model);
                            if (!model.CSharpFile.Classes.First().IsAbstract)
                            {
                                registerMongoCollections.AddStatement($"services.AddMongoCollection(new {model.ClassName}());");
                            }
                        }

                        registerMongoCollections.AddReturn("services");
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