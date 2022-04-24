using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.RDBMS.Api;
using Intent.Modelers.Domain.Api;
using Intent.Modules.Entities.Templates.DomainEntityState;
using Intent.Modules.Metadata.RDBMS.Settings;
using IdentityGeneratorTemplate = Intent.Modules.Entities.Keys.Templates.IdentityGenerator.IdentityGeneratorTemplate;

namespace Intent.Modules.Entities.Keys.Decorators
{
    public class SurrogatePrimaryKeyEntityStateDecorator : DomainEntityStateDecoratorBase
    {
        private readonly string _surrogateKeyType;
        private readonly bool _requiresImplicitKey;
        public const string Identifier = "Intent.Entities.Keys.SurrogatePrimaryKeyEntityStateDecorator";
        public const string SurrogateKeyType = "Surrogate Key Type";

        public SurrogatePrimaryKeyEntityStateDecorator(DomainEntityStateTemplate template) : base(template)
        {
            var explicitKeys = template.Model.Attributes.Where(x => x.Name.Equals("Id", StringComparison.InvariantCultureIgnoreCase) || x.HasPrimaryKey()).ToArray();
            if (!explicitKeys.Any())
            {
                _surrogateKeyType = template.ExecutionContext.Settings.GetDatabaseSettings()?.KeyType().Value ?? "System.Guid";
                _requiresImplicitKey = template.Model.ParentClass == null;
                template.FileMetadata.CustomMetadata.TryAdd("Surrogate Key Type", _surrogateKeyType);
            } else if (explicitKeys.Length == 1)
            {
                _surrogateKeyType = template.GetTypeInfo(explicitKeys.Single().TypeReference).Name;
                _requiresImplicitKey = false;
                template.FileMetadata.CustomMetadata.TryAdd("Surrogate Key Type", _surrogateKeyType);
            }
        }

        public override string BeforeProperties(ClassModel @class)
        {
            if (!_requiresImplicitKey)
            {
                return base.BeforeProperties(@class);
            }

            if (SurrogateKeyTypeIsGuid())
            {
                return $@"
        private Guid? _id = null;

        /// <summary>
        /// Get the persistent object's identifier
        /// </summary>
        public virtual Guid Id
        {{
            get {{ return _id ?? (_id = {Template.GetTypeName(IdentityGeneratorTemplate.Identifier)}.NewSequentialId()).Value; }}
            set {{ _id = value; }}
        }}";
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
