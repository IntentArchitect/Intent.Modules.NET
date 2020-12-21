using System.Collections.Generic;
using System.Text;
using Intent.Metadata.Models;
using Intent.Modelers.Services.Api;
using Intent.Modules.Application.Dtos.Templates.DtoModel;
using Intent.Modules.Common;
using Intent.Modules.Common.Types.Api;
using Intent.RoslynWeaver.Attributes;
using DtoModelTemplate = Intent.Modules.Application.Dtos.Templates.DtoModel.DtoModelTemplate;

[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.TemplateDecorator", Version = "1.0")]
[assembly: DefaultIntentManaged(Mode.Merge)]

namespace Intent.Modules.Application.Dtos.Decorators
{
    [IntentManaged(Mode.Merge)]
    public class DataContractDTOAttributeDecorator : DtoModelDecorator, IDeclareUsings
    {
        [IntentManaged(Mode.Fully)]
        public const string DecoratorId = "Intent.Application.Dtos.DataContractDTOAttributeDecorator";
        private readonly DtoModelTemplate _template;

        public DataContractDTOAttributeDecorator(DtoModelTemplate template)
        {
            _template = template;
        }

        public override string ClassAttributes(DTOModel dto)
        {
            return $"[DataContract{ GetDataContractPropertiesFormatted(dto) }]";
        }

        public override string PropertyAttributes(DTOModel dto, DTOFieldModel field)
        {
            return "[DataMember]";
        }

        public IEnumerable<string> DeclareUsings()
        {
            yield return "System.Runtime.Serialization";
        }

        private string GetDataContractPropertiesFormatted(DTOModel dto)
        {
            var dataContractStereotype = GetDataContractStereotype(dto);
            if (dataContractStereotype != null)
            {
                var sb = new StringBuilder();

                var @namespace = dataContractStereotype.GetProperty<string>("Namespace");
                if (!string.IsNullOrEmpty(@namespace))
                {
                    sb.Append($@"Namespace=""{ @namespace }""");
                }

                var isReference = dataContractStereotype.GetProperty<bool>("IsReference");
                if (isReference)
                {
                    if (sb.Length > 0)
                    {
                        sb.Append(" ");
                    }

                    sb.Append($@"IsReference=""{ isReference }""");
                }

                if (sb.Length > 0)
                {
                    sb.Insert(0, "( ");
                    sb.Append(" )");

                    return sb.ToString();
                }
            }
            return string.Empty;
        }

        private IStereotype GetDataContractStereotype(DTOModel dto)
        {
            IStereotype stereotype;
            stereotype = dto.GetStereotype("DataContract");
            if (stereotype != null)
            {
                return stereotype;
            }

            stereotype = dto.GetStereotypeInFolders("DataContract");
            if (stereotype != null)
            {
                return stereotype;
            }

            return null;
        }
    }
}
