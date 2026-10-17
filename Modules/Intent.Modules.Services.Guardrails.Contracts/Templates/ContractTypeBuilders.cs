using System;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modelers.Services.Api;
using Intent.Modelers.Services.CQRS.Api;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Types.Api;
using Intent.Services.Guardrails.Contracts.Api;

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
        private const string ServiceProxyConfigurationName = "Service Proxy Configuration";

        /// <summary>
        /// Dispatches on the element's type to the matching builder.
        /// </summary>
        public static void AddContract(CSharpFile file, ICSharpTemplate template, IElement element)
        {
            // Raw optional modeler elements first (see OptionalModelerMetadata): they have no typed Is*Model() check.
            if (element.IsDomainEventHandler())
            {
                AddDomainEventHandler(file, template, element);
            }
            else if (element.IsIntegrationEventHandler())
            {
                AddIntegrationEventHandler(file, template, element);
            }
            else if (element.IsIntegrationEvent())
            {
                AddIntegrationEvent(file, template, element);
            }
            else if (element.IsIntegrationCommand())
            {
                AddIntegrationCommand(file, template, element);
            }
            else if (element.IsEventingDto())
            {
                AddEventingDto(file, template, element);
            }
            else if (element.IsServiceProxy())
            {
                AddServiceProxy(file, template, element);
            }
            else if (element.IsServiceModel())
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
                AddEnum(file, template, element.AsEnumModel());
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
                @class.ApplyContractModifiers(template, model.InternalElement, model.GetTypeConfiguration()?.Modifiers(), isInherited: false);
            });
        }

        /// <summary>
        /// A command is a class or record with its fields as properties, or a positional record with them as positional
        /// parameters (<c>record CancelOrderCommand(int OrderNumber)</c>). No IRequest or other interfaces; Merge keeps the code's.
        /// </summary>
        public static void AddCommand(CSharpFile file, ICSharpTemplate template, CommandModel model)
        {
            var kind = model.GetContractTypeKind();
            file.AddContractType(kind, model.Name, @class =>
            {
                @class.RepresentsModel(model);
                @class.AddContractFields(template, kind, model.Properties);
                @class.ApplyContractModifiers(template, model.InternalElement, model.GetTypeConfiguration()?.Modifiers(), isInherited: false);
            });
        }

        /// <summary>
        /// A query is built like a command (<see cref="AddCommand"/>).
        /// </summary>
        public static void AddQuery(CSharpFile file, ICSharpTemplate template, QueryModel model)
        {
            var kind = model.GetContractTypeKind();
            file.AddContractType(kind, model.Name, @class =>
            {
                @class.RepresentsModel(model);
                @class.AddContractFields(template, kind, model.Properties);
                @class.ApplyContractModifiers(template, model.InternalElement, model.GetTypeConfiguration()?.Modifiers(), isInherited: false);
            });
        }

        public static void AddDto(CSharpFile file, ICSharpTemplate template, DTOModel model)
        {
            var kind = model.GetContractTypeKind();
            file.AddContractType(kind, model.Name, @class =>
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

                var primaryConstructor = @class.AddContractFields(template, kind, model.Fields);

                var parentDto = model.ParentDtoTypeReference?.Element?.AsDTOModel();
                if (parentDto != null && parentDto.GetDeclaredContractTypeKind() == ContractTypeKind.PositionalRecord)
                {
                    primaryConstructor.PassBasePositionalParameters(model.InternalElement, $"DTO '{model.Name}'",
                        model.Fields.Select(x => x.Name).ToList(), $"DTO '{parentDto.Name}'", parentDto.Fields.Select(x => x.Name).ToList());
                }

                @class.ApplyContractModifiers(template, model.InternalElement, model.GetTypeConfiguration()?.Modifiers(), template.IsInheritedByAnyDto(model));
            });
        }

        /// <summary>
        /// A Message, shown as "Integration Event" (raw element, see <see cref="OptionalModelerMetadata"/>), is guarded like a
        /// command: a class or record with its Properties and no base type or interfaces.
        /// </summary>
        public static void AddIntegrationEvent(CSharpFile file, ICSharpTemplate template, IElement element)
        {
            AddEventingMessage(file, template, element);
        }

        /// <summary>
        /// An Integration Command (raw element, see <see cref="OptionalModelerMetadata"/>) is guarded like a command: a class or
        /// record with its Properties and no base type or interfaces.
        /// </summary>
        public static void AddIntegrationCommand(CSharpFile file, ICSharpTemplate template, IElement element)
        {
            AddEventingMessage(file, template, element);
        }

        /// <summary>
        /// An Eventing DTO (raw element, see <see cref="OptionalModelerMetadata"/>) is guarded like a DTO: Is Abstract, generic
        /// types, the base type from its Generalization, and its Fields.
        /// </summary>
        public static void AddEventingDto(CSharpFile file, ICSharpTemplate template, IElement element)
        {
            var kind = element.GetContractTypeKind();
            file.AddContractType(kind, element.Name, @class =>
            {
                @class.RepresentsModel(element);

                if (element.IsAbstract)
                {
                    @class.Abstract();
                }

                foreach (var genericType in element.GenericTypes)
                {
                    @class.AddGenericParameter(genericType.Name);
                }

                var parentDtoTypeReference = element.GetEventingParentDtoTypeReference();
                if (parentDtoTypeReference != null)
                {
                    @class.WithBaseType(template.GetTypeName(parentDtoTypeReference));
                }

                var fields = element.GetEventingDtoFields();
                var primaryConstructor = @class.AddContractFields(template, kind, fields);

                var parentDto = parentDtoTypeReference?.Element as IElement;
                if (parentDto != null && parentDto.IsEventingDto() && parentDto.GetDeclaredContractTypeKind() == ContractTypeKind.PositionalRecord)
                {
                    primaryConstructor.PassBasePositionalParameters(element, $"Eventing DTO '{element.Name}'",
                        fields.Select(x => x.Name).ToList(), $"Eventing DTO '{parentDto.Name}'", parentDto.GetEventingDtoFields().Select(x => x.Name).ToList());
                }

                @class.ApplyContractModifiers(template, element, element.GetOptionalModelerTypeConfiguration()?.Modifiers(), element.IsInheritedByAnyEventingDto());
            });
        }

        /// <summary>
        /// A Domain Event Handler (raw element, see <see cref="OptionalModelerMetadata"/>) is guarded as a class only:
        /// its Handle method signatures and base interfaces are framework-specific, and Merge keeps the existing ones.
        /// Its modifiers come from the handler Type Configuration stereotype (Modifiers only) or the module setting.
        /// </summary>
        public static void AddDomainEventHandler(CSharpFile file, ICSharpTemplate template, IElement element)
        {
            AddHandler(file, template, element);
        }

        /// <summary>
        /// An Integration Event Handler (raw element, see <see cref="OptionalModelerMetadata"/>) is guarded as a class only, like a
        /// Domain Event Handler: its handle methods and handler interfaces depend on the message broker (e.g. MassTransit's
        /// <c>IConsumer&lt;T&gt;</c>, NServiceBus's <c>IHandleMessages&lt;T&gt;</c>), and Merge keeps the existing ones. Its
        /// Subscribe Integration Event and Subscribe Integration Command associations are documentation only and are not read.
        /// </summary>
        public static void AddIntegrationEventHandler(CSharpFile file, ICSharpTemplate template, IElement element)
        {
            AddHandler(file, template, element);
        }

        /// <summary>
        /// A Service Proxy (raw element, see <see cref="OptionalModelerMetadata"/>) is guarded as the one type its client code
        /// declares (user decision): the concrete client class by default, or with Service Proxy Configuration Type interface,
        /// the client's interface, for code whose implementation is generated at runtime (e.g. Refit). Each Operation becomes a
        /// method with an empty body. No base types or attributes are generated, so Merge keeps the code's own (a class's
        /// <c>: IPaymentsClient</c>, Refit's <c>[Get(...)]</c>). Mappings are never read: a proxy represents the client code only
        /// (user decision).
        /// </summary>
        public static void AddServiceProxy(CSharpFile file, ICSharpTemplate template, IElement element)
        {
            var configuration = element.GetServiceProxyConfiguration();
            if (configuration?.IsInterface() == true)
            {
                file.AddInterface(element.Name, @interface =>
                {
                    @interface.RepresentsModel(element);
                    @interface.AddStubbedProxyOperations(template, element.GetServiceProxyOperations());
                    @interface.ApplyContractModifiers(template, element, configuration.Modifiers(), ServiceProxyConfigurationName);
                });
                return;
            }

            file.AddClass(element.Name, @class =>
            {
                @class.RepresentsModel(element);
                @class.AddStubbedProxyOperations(template, element.GetServiceProxyOperations());
                @class.ApplyContractModifiers(template, element, configuration?.Modifiers(), isInherited: false, ServiceProxyConfigurationName);
            });
        }

        private static void AddEventingMessage(CSharpFile file, ICSharpTemplate template, IElement element)
        {
            var kind = element.GetContractTypeKind();
            file.AddContractType(kind, element.Name, @class =>
            {
                @class.RepresentsModel(element);
                @class.AddContractFields(template, kind, element.GetEventingProperties());
                @class.ApplyContractModifiers(template, element, element.GetOptionalModelerTypeConfiguration()?.Modifiers(), isInherited: false);
            });
        }

        private static void AddHandler(CSharpFile file, ICSharpTemplate template, IElement element)
        {
            file.AddClass(element.Name, @class =>
            {
                @class.RepresentsModel(element);
                @class.ApplyContractModifiers(template, element, element.GetHandlerModifiers(), isInherited: false);
            });
        }

        public static void AddEnum(CSharpFile file, ICSharpTemplate template, EnumModel model)
        {
            file.AddEnum(model.Name, @enum =>
            {
                // No RepresentsModel here: Intent.Modules.Common.CSharp (3.10.5-3.10.11) throws for CSharpEnum and CSharpEnumLiteral.
                foreach (var literal in model.Literals)
                {
                    @enum.AddLiteral(literal.Name, string.IsNullOrWhiteSpace(literal.Value) ? null : literal.Value);
                }

                @enum.ApplyContractAccessModifier(template, model.InternalElement, model.GetEnumConfiguration()?.AccessModifier().Value);
            });
        }
    }
}
