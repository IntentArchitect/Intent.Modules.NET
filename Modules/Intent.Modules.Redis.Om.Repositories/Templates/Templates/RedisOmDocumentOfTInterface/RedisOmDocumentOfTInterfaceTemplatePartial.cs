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

namespace Intent.Modules.Redis.Om.Repositories.Templates.Templates.RedisOmDocumentOfTInterface
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class RedisOmDocumentOfTInterfaceTemplate : CSharpTemplateBase<object>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.Redis.Om.Repositories.Templates.RedisOmDocumentOfTInterface";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public RedisOmDocumentOfTInterfaceTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            var createEntityInterfaces = ExecutionContext.Settings.GetDomainSettings().CreateEntityInterfaces();

            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddUsing("System")
                .AddInterface($"IRedisOmDocument", @interface =>
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
                        .AddGenericParameter("TDocument", out var tDocument, g => g.Covariant())
                        .AddGenericTypeConstraint(tDomain, c => c
                            .AddType("class"));
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
                            .AddType($"{this.GetRedisOmDocumentOfTInterfaceName()}<{tDomain}{tDomainStateConstraint}, {tDocument}>"));

                    @interface.AddMethod(tDocument, "PopulateFromEntity", c => c
                        .AddParameter(tDomain, "entity"));

                    @interface.AddMethod(tDomainState, "ToEntity", c => c
                        .AddParameter($"{tDomainState}?", "entity", p => p.WithDefaultValue("null")));
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