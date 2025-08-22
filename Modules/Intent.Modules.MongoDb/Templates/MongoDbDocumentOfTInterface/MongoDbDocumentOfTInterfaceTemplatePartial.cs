using System;
using System.Collections.Generic;
using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.Modelers.Domain.Settings;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.MongoDb.Templates.MongoDbDocumentOfTInterface
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class MongoDbDocumentOfTInterfaceTemplate : CSharpTemplateBase<object>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.MongoDb.MongoDbDocumentOfTInterface";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public MongoDbDocumentOfTInterfaceTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            var createEntityInterfaces = ExecutionContext.Settings.GetDomainSettings().CreateEntityInterfaces();
            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddInterface($"IMongoDbDocument", @interface =>
                {
                    @interface
                        .Internal()
                        .AddGenericParameter("TDomain", out var tDomain, genericParameter =>
                        {
                            if (createEntityInterfaces)
                            {
                                genericParameter.Contravariant();
                            }
                        });

                    var tDomainState = tDomain;
                    if (createEntityInterfaces)
                    {
                        @interface
                            .AddGenericParameter("TDomainState", out tDomainState);
                    }

                    @interface
                        .AddGenericParameter("TDocument", out var tDocument)
                        .AddGenericTypeConstraint(tDomain, c => c
                            .AddType("class"));

                    @interface
                        .AddGenericParameter("TIdentifier", out var tIdentifier);

                    if (createEntityInterfaces)
                    {
                        @interface
                            .AddGenericTypeConstraint(tDomainState, c => c
                                .AddType("class")
                                .AddType(tDomain));
                    }

                    var tDomainStateConstraint = createEntityInterfaces
                        ? $", {tDomainState}"
                        : string.Empty;
                    @interface
                        .AddGenericTypeConstraint(tDocument, c => c
                            .AddType($"{this.GetMongoDbDocumentOfTInterfaceName()}<{tDomain}{tDomainStateConstraint}, {tDocument}, {tIdentifier}>"));

                    @interface.AddMethod(tDocument, "PopulateFromEntity", c => c
                    .AddParameter(tDomain, "entity"));

                    @interface.AddMethod(tDomainState, "ToEntity", c => c
                        .AddParameter($"{tDomainState}?", "entity", p => p.WithDefaultValue("null")));

                    @interface.AddMethod($"FilterDefinition<{tDocument}>", "GetIdFilter", c => c.Static().Abstract()
                        .AddParameter($"{tIdentifier}", "id"));
                    @interface.AddMethod($"FilterDefinition<{tDocument}>", "GetIdsFilter", c => c.Static().Abstract()
                        .AddParameter($"{tIdentifier}[]", "ids"));
                    @interface.AddMethod($"FilterDefinition<{tDocument}>", "GetIdFilter");
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