using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Modelers.Domain.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.Templates;
using Intent.Modules.Entities.Keys.Settings;
using Intent.Modules.Entities.Templates.DomainEntityState;
using Intent.Templates;
using IdentityGeneratorTemplate = Intent.Modules.Entities.Keys.Templates.IdentityGenerator.IdentityGeneratorTemplate;

namespace Intent.Modules.Entities.Keys.Decorators
{
    public class SurrogatePrimaryKeyEntityStateDecorator : DomainEntityStateDecoratorBase, IHasTemplateDependencies
    {
        private string _surrogateKeyType;
        public const string Identifier = "Intent.Entities.Keys.SurrogatePrimaryKeyEntityStateDecorator";
        public const string SurrogateKeyType = "Surrogate Key Type";

        public SurrogatePrimaryKeyEntityStateDecorator(DomainEntityStateTemplate template) : base(template)
        {
            _surrogateKeyType = template.ExecutionContext.Settings.GetEntityKeySettings()?.KeyType().Value ?? "System.Guid";
        }

        public override string BeforeProperties(ClassModel @class)
        {
            if (@class.ParentClass != null || @class.Attributes.Any(x => x.Name.Equals("Id", StringComparison.InvariantCultureIgnoreCase) || x.HasStereotype("Primary Key")))
            {
                return base.BeforeProperties(@class);
            }

            if (SurrogateKeyTypeIsGuid())
            {
                return @"
        private Guid? _id = null;

        /// <summary>
        /// Get the persistent object's identifier
        /// </summary>
        public virtual Guid Id
        {
            get { return _id ?? (_id = IdentityGenerator.NewSequentialId()).Value; }
            set { _id = value; }
        }";
            }

            return $@"
        /// <summary>
        /// Get the persistent object's identifier
        /// </summary>
        public virtual {Template.UseType(_surrogateKeyType)} Id {{ get; set; }}";
        }

        private bool SurrogateKeyTypeIsGuid()
        {
            return _surrogateKeyType.Trim().Equals("System.Guid", StringComparison.InvariantCultureIgnoreCase);
        }

        public IEnumerable<ITemplateDependency> GetTemplateDependencies()
        {
            if (!SurrogateKeyTypeIsGuid())
            {
                return new List<ITemplateDependency>();
            }
            return new[] { TemplateDependency.OnTemplate(IdentityGeneratorTemplate.Identifier) };
        }

        //public override void Configure(IDictionary<string, string> settings)
        //{
        //    base.Configure(settings);
        //    if (settings.ContainsKey(SurrogateKeyType) && !string.IsNullOrWhiteSpace(settings[SurrogateKeyType]))
        //    {
        //        _surrogateKeyType = settings[SurrogateKeyType];
        //    }
        //}

    }
}
