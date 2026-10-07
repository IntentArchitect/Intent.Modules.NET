using System;
using Intent.Metadata.Models;
using Intent.Modelers.Services.Api;
using Intent.Modelers.Services.CQRS.Api;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Types.Api;

namespace Intent.Modules.Services.Guardrails.Contracts.Templates
{
    /// <summary>
    /// The one place each contract kind's C# type is built. A template calls its own kind's method
    /// for its own type; the owner of a multi-type file calls <see cref="AddContract"/> for every
    /// other element that shares its file. <paramref name="template"/> is always the template whose
    /// file is being built, so types and usings resolve into that file. Names are used verbatim.
    /// </summary>
    public static class ContractTypeBuilders
    {
        /// <summary>
        /// Dispatches on the element's type to the matching builder.
        /// </summary>
        public static void AddContract(CSharpFile file, ICSharpTemplate template, IElement element)
        {
            if (element.IsServiceModel())
            {
                AddService(file, template, element.AsServiceModel());
            }
            else if (element.IsCommandModel())
            {
                AddCommand(file, template, element.AsCommandModel());
            }
            else if (element.IsQueryModel())
            {
                AddQuery(file, template, element.AsQueryModel());
            }
            else if (element.IsDTOModel())
            {
                AddDto(file, template, element.AsDTOModel());
            }
            else if (element.IsEnumModel())
            {
                AddEnum(file, element.AsEnumModel());
            }
            else
            {
                throw new InvalidOperationException(
                    $"Unsupported element type '{element.SpecializationType}' ({element.Id}) for a Services guardrail contract.");
            }
        }

        public static void AddService(CSharpFile file, ICSharpTemplate template, ServiceModel model)
        {
            file.AddContractType(model.GetContractTypeKind(), model.Name, @class =>
            {
                @class.RepresentsModel(model);
                @class.AddStubbedOperations(template, model.Operations);
            });
        }

        public static void AddCommand(CSharpFile file, ICSharpTemplate template, CommandModel model)
        {
            file.AddContractType(model.GetContractTypeKind(), model.Name, @class =>
            {
                @class.RepresentsModel(model);
                @class.AddContractProperties(template, model.Properties);
            });
        }

        public static void AddQuery(CSharpFile file, ICSharpTemplate template, QueryModel model)
        {
            file.AddContractType(model.GetContractTypeKind(), model.Name, @class =>
            {
                @class.RepresentsModel(model);
                @class.AddContractProperties(template, model.Properties);
            });
        }

        public static void AddDto(CSharpFile file, ICSharpTemplate template, DTOModel model)
        {
            file.AddContractType(model.GetContractTypeKind(), model.Name, @class =>
            {
                @class.RepresentsModel(model);

                if (model.IsAbstract)
                {
                    @class.Abstract();
                }

                foreach (var genericType in model.GenericTypes)
                {
                    @class.AddGenericParameter(genericType);
                }

                if (model.ParentDtoTypeReference != null)
                {
                    @class.WithBaseType(template.GetTypeName(model.ParentDtoTypeReference));
                }

                @class.AddContractProperties(template, model.Fields);
            });
        }

        public static void AddEnum(CSharpFile file, EnumModel model)
        {
            file.AddEnum(model.Name, @enum =>
            {
                // No RepresentsModel here: Intent.Modules.Common.CSharp (3.10.5-3.10.11) throws for CSharpEnum and CSharpEnumLiteral.
                foreach (var literal in model.Literals)
                {
                    @enum.AddLiteral(literal.Name, string.IsNullOrWhiteSpace(literal.Value) ? null : literal.Value);
                }
            });
        }
    }
}
