using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.Common.Plugins;
using Intent.Modules.Common.Templates;
using Intent.Modules.Entities.Keys.Templates.IdentityGenerator;
using Intent.Modules.Entities.Templates;
using Intent.Plugins.FactoryExtensions;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.FactoryExtension", Version = "1.0")]

namespace Intent.Modules.Entities.Keys.FactoryExtensions
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class DomainEntityKeysExtension : FactoryExtensionBase
    {
        public override string Id => "Intent.Entities.Keys.DomainEntityKeysExtension";

        [IntentManaged(Mode.Ignore)]
        public override int Order => 0;

        protected override void OnBeforeTemplateExecution(IApplication application)
        {
            var entityTemplates = application.FindTemplateInstances<ICSharpFileBuilderTemplate>(TemplateDependency.OnTemplate("Domain.Entity"));
            foreach (var template in entityTemplates)
            {
                var @class = template.Output.Classes.First();
                var explicitKeys = @class.Properties.Where(x => x.Name.Equals("Id", StringComparison.InvariantCultureIgnoreCase)).ToArray();
                string surrogateKeyType = null;
                bool requiresImplicitKey = true;
                if (!explicitKeys.Any())
                {
                    surrogateKeyType = template.GetSurrogateKeyType();
                    requiresImplicitKey = @class.BaseType == null;
                    template.FileMetadata.CustomMetadata.TryAdd("Surrogate Key Type", surrogateKeyType);
                }
                else if (explicitKeys.Length == 1)
                {
                    surrogateKeyType = explicitKeys.Single().Type;
                    requiresImplicitKey = false;
                    template.FileMetadata.CustomMetadata.TryAdd("Surrogate Key Type", surrogateKeyType);
                }

                if (requiresImplicitKey)
                {
                    var property = @class.InsertProperty(0, surrogateKeyType, "Id")
                        .Virtual()
                        .WithBackingField(field =>
                        {
                            field.CanBeNull();
                        })
                        .WithComments(@"
/// <summary>
/// Get the persistent object's identifier
/// </summary>");
                    property.Getter.WithExpressionImplementation($"_id ??= {template.GetTypeName(IdentityGeneratorTemplate.Identifier)}.NewSequentialId().Value;");
                    property.Setter.WithExpressionImplementation($"_id = value");
                }
            }
        }
    }
}