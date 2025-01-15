using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.Modules.Common.Types.Api;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiElementModelExtensions", Version = "1.0")]

namespace Intent.EntityFrameworkCore.DataMasking.Api
{
    // This is for disambiguating the extension method
    using Intent.Modelers.Domain.Api;

    public static class AttributeModelStereotypeExtensions
    {
        public static DataMasking GetDataMasking(this AttributeModel model)
        {
            var stereotype = model.GetStereotype(DataMasking.DefinitionId);
            return stereotype != null ? new DataMasking(stereotype) : null;
        }


        public static bool HasDataMasking(this AttributeModel model)
        {
            return model.HasStereotype(DataMasking.DefinitionId);
        }

        public static bool TryGetDataMasking(this AttributeModel model, out DataMasking stereotype)
        {
            if (!HasDataMasking(model))
            {
                stereotype = null;
                return false;
            }

            stereotype = new DataMasking(model.GetStereotype(DataMasking.DefinitionId));
            return true;
        }

        public class DataMasking
        {
            private IStereotype _stereotype;
            public const string DefinitionId = "2c878051-d640-47d6-98bf-243fda0e60fb";

            public DataMasking(IStereotype stereotype)
            {
                _stereotype = stereotype;
            }

            public string Name => _stereotype.Name;

            public DataMaskTypeOptions DataMaskType()
            {
                return new DataMaskTypeOptions(_stereotype.GetProperty<string>("Data Mask Type"));
            }

            public string MaskCharacter()
            {
                return _stereotype.GetProperty<string>("Mask Character");
            }

            public int? SetLength()
            {
                return _stereotype.GetProperty<int?>("Set Length");
            }

            public int? UnmaskedPrefixLength()
            {
                return _stereotype.GetProperty<int?>("Unmasked Prefix Length");
            }

            public int? UnmaskedSuffixLength()
            {
                return _stereotype.GetProperty<int?>("Unmasked Suffix Length");
            }

            public string Roles()
            {
                return _stereotype.GetProperty<string>("Roles");
            }

            public string Policy()
            {
                return _stereotype.GetProperty<string>("Policy");
            }

            public IElement[] SecurityRoles()
            {
                return _stereotype.GetProperty<IElement[]>("Security Roles") ?? new IElement[0];
            }

            public IElement[] SecurityPolicies()
            {
                return _stereotype.GetProperty<IElement[]>("Security Policies") ?? new IElement[0];
            }

            public class DataMaskTypeOptions
            {
                public readonly string Value;

                public DataMaskTypeOptions(string value)
                {
                    Value = value;
                }

                public DataMaskTypeOptionsEnum AsEnum()
                {
                    switch (Value)
                    {
                        case "Set Length":
                            return DataMaskTypeOptionsEnum.SetLength;
                        case "Variable Length":
                            return DataMaskTypeOptionsEnum.VariableLength;
                        case "Partial Mask":
                            return DataMaskTypeOptionsEnum.PartialMask;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }

                public bool IsSetLength()
                {
                    return Value == "Set Length";
                }
                public bool IsVariableLength()
                {
                    return Value == "Variable Length";
                }
                public bool IsPartialMask()
                {
                    return Value == "Partial Mask";
                }
            }

            public enum DataMaskTypeOptionsEnum
            {
                SetLength,
                VariableLength,
                PartialMask
            }
        }

    }
}