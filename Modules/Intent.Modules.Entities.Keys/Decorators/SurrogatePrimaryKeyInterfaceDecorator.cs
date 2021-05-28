using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Modelers.Domain.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Entities.Templates.DomainEntityInterface;

namespace Intent.Modules.Entities.Keys.Decorators
{
    public static class EntitiesKeysSettingsExtensions
    {
        public static string GetSurrogateKeyType<T>(this CSharpTemplateBase<T> template)
        {
            return template.ExecutionContext.Settings.GetGroup("intent.modules.entities.keys")?.GetSetting("use-partials-for-api-select")?.Value;
        }
    }

    public class SurrogatePrimaryKeyInterfaceDecorator : DomainEntityInterfaceDecoratorBase
    {
        private string _surrogateKeyType;

        public const string Identifier = "Intent.Entities.Keys.SurrogatePrimaryKeyInterfaceDecorator";

        public string SurrogateKeyType => "Surrogate Key Type";

        public SurrogatePrimaryKeyInterfaceDecorator(DomainEntityInterfaceTemplate template) : base(template)
        {
            _surrogateKeyType = template.GetSurrogateKeyType() ?? "System.Guid";
        }

        public override string BeforeProperties(ClassModel @class)
        {
            if (@class.ParentClass != null || @class.Attributes.Any(x => x.Name.Equals("Id", StringComparison.InvariantCultureIgnoreCase) || x.HasStereotype("Primary Key")))
            {
                return base.BeforeProperties(@class);
            }

            return $@"
        /// <summary>
        /// Get the persistent object's identifier
        /// </summary>
        {Template.UseType(_surrogateKeyType)} Id {{ get; }}";
        }

        public override void Configure(IDictionary<string, string> settings)
        {
            base.Configure(settings);
            if (settings.ContainsKey(SurrogateKeyType) && !string.IsNullOrWhiteSpace(settings[SurrogateKeyType]))
            {
                _surrogateKeyType = settings[SurrogateKeyType];
            }
        }
    }
}
