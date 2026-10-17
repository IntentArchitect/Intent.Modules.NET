using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Exceptions;
using Intent.Metadata.Models;
using Intent.Modelers.Services.Api;
using Intent.Modules.Common;
using Intent.Services.Guardrails.Contracts.Api;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.CSharp.TypeResolvers;
using Intent.Modules.Common.Templates;

namespace Intent.Modules.Services.Guardrails.Contracts.Templates
{
    /// <summary>
    /// Shared builder logic for the Services guardrail contract templates. Contracts only ensure
    /// that modelled types and members exist in the developer's existing files; they never
    /// implement or rewrite behaviour.
    /// </summary>
    public static class ContractBuilderExtensions
    {
        /// <summary>
        /// The single dispatch point for the C# kind of a contract type. Class and record both
        /// populate a <see cref="CSharpClass"/> (<see cref="CSharpRecord"/> derives from it), so
        /// <see cref="AddContractProperties"/> and <see cref="AddStubbedOperations"/> work unchanged.
        /// Name is used verbatim.
        /// </summary>
        public static CSharpFile AddContractType(this CSharpFile file, ContractTypeKind kind, string name, Action<CSharpClass> configure)
        {
            switch (kind)
            {
                case ContractTypeKind.Class:
                    return file.AddClass(name, configure);
                case ContractTypeKind.Record:
                case ContractTypeKind.PositionalRecord:
                    // A positional record is a record whose fields go on its primary constructor (see AddContractFields).
                    return file.AddRecord(name, record => configure(record));
                default:
                    throw new InvalidOperationException($"Unsupported contract type kind: {kind}");
            }
        }

        /// <summary>
        /// The merge directives every contract file carries (user decision): the file-wide
        /// <c>[assembly: DefaultIntentManaged(Mode.Merge)]</c>, plus a default for constructors and methods,
        /// <c>[assembly: DefaultIntentManaged(Mode.Merge, Signature = Mode.Fully, Attributes = Mode.Merge, Comments = Mode.Merge, Targets = Targets.Constructors | Targets.Methods)]</c>.
        /// Same as the Domain module's; keep the two in step.
        /// </summary>
        public static CSharpFile IntentManagedContractMerge(this CSharpFile file)
        {
            return file
                .IntentManagedMerge()
                .AddAssemblyAttribute(CSharpDefaultIntentManagedAttribute.Merge()
                    .WithSignatureFully()
                    .AddArgument("Attributes = Mode.Merge")
                    .AddArgument("Comments = Mode.Merge")
                    .AddArgument("Targets = Targets.Constructors | Targets.Methods"));
        }

        /// <summary>
        /// Registers every Services and Domain guardrail contract role as a type source, with
        /// collections formatted as <c>List&lt;T&gt;</c>.
        /// </summary>
        public static void AddContractTypeSources(this IntentTemplateBase template)
        {
            template.SetDefaultCollectionFormatter(CSharpCollectionFormatter.CreateList());
            foreach (var role in GuardrailsContractsServicesRoles.TypeSources)
            {
                template.AddTypeSource(role).WithCollectionFormatter(CSharpCollectionFormatter.CreateList());
            }
        }

        /// <summary>
        /// Adds a <c>{ get; set; }</c> property for each field, name used verbatim, with an initializer when the field has
        /// a Value (see <see cref="ContractDefaultValues"/>), and <c>required</c> when its Property Configuration says so.
        /// Must be called from within a builder callback so types resolve at build time.
        /// </summary>
        public static void AddContractProperties(this CSharpClass @class, ICSharpTemplate template, IEnumerable<DTOFieldModel> fields)
        {
            foreach (var field in fields)
            {
                @class.AddProperty(template.GetContractTypeName(field), field.Name, property =>
                {
                    property.RepresentsModel(field);
                    property.WithContractInitialValue(template, field.Value, field.TypeReference);
                    property.WithContractRequired(field.GetPropertyConfiguration()?.Required() == true);
                });
            }
        }

        /// <summary>
        /// Property Configuration Required: the property is declared <c>required</c>. Under Merge a property's own modifiers follow
        /// the generated ones, and on its first merge into an existing file every existing modifier is replaced, so a
        /// <c>required</c> in the code survives only if it is modelled. Positional parameters never get here, so the stereotype
        /// is ignored on a positional record's fields (user decision). Same rule as the Domain module's.
        /// </summary>
        private static void WithContractRequired(this CSharpProperty property, bool isRequired)
        {
            if (isRequired)
            {
                property.Required();
            }
        }

        /// <summary>
        /// The fields as properties, or for a <see cref="ContractTypeKind.PositionalRecord"/> as the positional parameters of
        /// its primary constructor. Returns that primary constructor, or null when the fields are properties.
        /// </summary>
        public static CSharpConstructor AddContractFields(this CSharpClass @class, ICSharpTemplate template, ContractTypeKind kind, IEnumerable<DTOFieldModel> fields)
        {
            if (kind != ContractTypeKind.PositionalRecord)
            {
                @class.AddContractProperties(template, fields);
                return null;
            }

            return @class.AddContractPositionalParameters(fields, (ctor, field) =>
                ctor.AddParameter(template.GetContractTypeName(field), field.Name, parameter =>
                {
                    parameter.RepresentsModel(field);
                    parameter.WithContractDefaultValue(template, field.InternalElement, field.Value, field.TypeReference, modifier: null);
                }));
        }

        /// <summary>
        /// The same for the raw Properties of a Message or Integration Command, or the Fields of an Eventing DTO.
        /// </summary>
        public static CSharpConstructor AddContractFields(this CSharpClass @class, ICSharpTemplate template, ContractTypeKind kind, IEnumerable<IElement> fields)
        {
            if (kind != ContractTypeKind.PositionalRecord)
            {
                @class.AddContractProperties(template, fields);
                return null;
            }

            return @class.AddContractPositionalParameters(fields, (ctor, field) =>
            {
                var collectionType = field.GetOptionalModelerCollectionType();
                var typeName = template.GetContractTypeName(field.TypeReference, field, collectionType?.Kind, collectionType?.CustomFormat);
                ctor.AddParameter(typeName, field.Name, parameter =>
                {
                    parameter.RepresentsModel(field);
                    parameter.WithContractDefaultValue(template, field, field.Value, field.TypeReference, modifier: null);
                });
            });
        }

        /// <summary>
        /// A positional record's primary constructor, e.g. <c>record CancelOrderCommand(int OrderNumber)</c>: one parameter per
        /// field, in model order and named verbatim (they are the record's properties, so they're usually PascalCase). A field's
        /// Value becomes the parameter's default.
        /// </summary>
        private static CSharpConstructor AddContractPositionalParameters<TField>(this CSharpClass @class, IEnumerable<TField> fields, Action<CSharpConstructor, TField> addParameter)
        {
            CSharpConstructor primaryConstructor = null;
            @class.AddPrimaryConstructor(ctor =>
            {
                primaryConstructor = ctor;
                foreach (var field in fields)
                {
                    addParameter(ctor, field);
                }
            });
            return primaryConstructor;
        }

        /// <summary>
        /// A positional record that inherits from a positional record passes the base's positional parameters on by name,
        /// e.g. <c>record ChildDto(int Id, string Name) : BaseDto(Id)</c>. The derived record must itself be positional and
        /// declare a parameter for each of the base's, which is how the code has to be written.
        /// </summary>
        public static void PassBasePositionalParameters(this CSharpConstructor primaryConstructor, IElement element, string elementDescription, IReadOnlyCollection<string> ownParameters, string baseDescription, IReadOnlyList<string> baseParameters)
        {
            if (baseParameters.Count == 0)
            {
                return;
            }

            if (primaryConstructor == null)
            {
                throw new ElementException(element,
                    $"{elementDescription} inherits from {baseDescription}, which is a positional record, so it has to pass {baseDescription}'s positional parameters ({string.Join(", ", baseParameters)}) to the base constructor. " +
                    $"Set {element.Name}'s Type Configuration Type to positional record, with fields for those parameters, as the code declares it.");
            }

            var missing = baseParameters.Where(x => !ownParameters.Contains(x)).ToList();
            if (missing.Count > 0)
            {
                throw new ElementException(element,
                    $"{elementDescription} is a positional record that inherits from {baseDescription}, also a positional record, so its own positional parameters must include {baseDescription}'s to pass on to the base constructor. " +
                    $"Add the missing fields to {element.Name}, as the code declares them: {string.Join(", ", missing)}.");
            }

            primaryConstructor.CallsBase(call =>
            {
                foreach (var name in baseParameters)
                {
                    call.AddArgument(name);
                }
            });
        }

        /// <summary>
        /// The same for the raw Properties of a Message or Integration Command, or the Fields of an Eventing DTO (see
        /// <see cref="OptionalModelerMetadata"/>), honouring this module's filter-targeted Collection Type. Only an Eventing
        /// DTO-Field can have a Value; the Eventing designer offers none on a Property, so those never get an initializer.
        /// Must be called from within a builder callback so types resolve at build time.
        /// </summary>
        public static void AddContractProperties(this CSharpClass @class, ICSharpTemplate template, IEnumerable<IElement> fields)
        {
            foreach (var field in fields)
            {
                var collectionType = field.GetOptionalModelerCollectionType();
                var typeName = template.GetContractTypeName(field.TypeReference, field, collectionType?.Kind, collectionType?.CustomFormat);
                @class.AddProperty(typeName, field.Name, property =>
                {
                    property.RepresentsModel(field);
                    property.WithContractInitialValue(template, field.Value, field.TypeReference);
                    property.WithContractRequired(field.IsOptionalModelerPropertyRequired());
                });
            }
        }

        /// <summary>
        /// Adds a method with an empty body for each operation, name used verbatim, so Merge mode
        /// keeps whatever body is already on disk. Must be called from within a builder callback.
        /// </summary>
        public static void AddStubbedOperations(this CSharpClass @class, ICSharpTemplate template, IEnumerable<OperationModel> operations)
        {
            foreach (var operation in operations)
            {
                var async = ContractAsync.For(operation.InternalElement, operation.GetAsyncConfiguration(), operation.Parameters.Select(x => x.Name));
                var modelledReturnType = CSharpTypeParser.Parse(operation.TypeReference?.Element != null
                    ? template.GetContractTypeName(operation)
                    : "void");

                @class.AddMethod(async.GetReturnType(modelledReturnType, template), operation.Name, method =>
                {
                    method.RepresentsModel(operation);

                    // Service operations have no Is Static or Is Abstract of their own, so static comes from the stereotype.
                    var configuration = operation.GetOperationConfiguration();
                    method.ApplyContractModifiers(operation.InternalElement, configuration?.AccessModifier().Value,
                        isStatic: configuration?.Static() == true,
                        isAbstract: false,
                        isVirtual: configuration?.Virtual() == true,
                        isOverride: configuration?.Override() == true,
                        isSealed: configuration?.Sealed() == true,
                        isNew: configuration?.New() == true);


                    foreach (var genericType in operation.GenericTypes)
                    {
                        method.AddGenericParameter(genericType);
                    }

                    foreach (var parameter in operation.Parameters)
                    {
                        var modifier = parameter.GetModifier();
                        method.AddParameter(template.GetContractTypeName(parameter).WithModifierPrefix(modifier), parameter.Name, param =>
                        {
                            param.RepresentsModel(parameter);
                            param.ApplyModifier(modifier);
                            param.WithContractDefaultValue(template, parameter.InternalElement, parameter.Value, parameter.TypeReference, modifier);
                        });
                    }

                    async.AddCancellationTokenParameter(method, template);
                    async.ApplyAsyncKeyword(method);
                });
            }
        }

        /// <summary>
        /// A concrete Service Proxy's Operations (raw elements, see <see cref="OptionalModelerMetadata"/>): a public method with
        /// an empty body for each, name used verbatim, plus the async keyword for Async Mode async. Method modifiers other than
        /// async aren't modelled for proxies (a documented limitation). Must be called from within a builder callback.
        /// </summary>
        public static void AddStubbedProxyOperations(this CSharpClass @class, ICSharpTemplate template, IEnumerable<IElement> operations)
        {
            foreach (var operation in operations)
            {
                var parameters = operation.GetServiceProxyParameters();
                var async = ContractAsync.For(operation, operation.GetServiceProxyAsyncConfiguration(), parameters.Select(x => x.Name));
                @class.AddMethod(async.GetReturnType(template.GetProxyOperationReturnType(operation), template), operation.Name, method =>
                {
                    method.RepresentsModel(operation);
                    method.Public();
                    AddProxyParameters(template, parameters, representsModel: true, (type, name, configure) => method.AddParameter(type, name, configure));
                    async.AddCancellationTokenParameter(method, template);
                    async.ApplyAsyncKeyword(method);
                });
            }
        }

        /// <summary>
        /// The same for a Service Proxy whose Type is interface: interface methods have no access modifier and no body, so they
        /// never get the async keyword; the return type and CancellationToken follow the same async rules.
        /// </summary>
        public static void AddStubbedProxyOperations(this CSharpInterface @interface, ICSharpTemplate template, IEnumerable<IElement> operations)
        {
            foreach (var operation in operations)
            {
                var parameters = operation.GetServiceProxyParameters();
                var async = ContractAsync.For(operation, operation.GetServiceProxyAsyncConfiguration(), parameters.Select(x => x.Name));
                @interface.AddMethod(async.GetReturnType(template.GetProxyOperationReturnType(operation), template), operation.Name, method =>
                {
                    method.RepresentsModel(operation);
                    // Intent.Modules.Common.CSharp (up to 3.10.12-pre.2) never sets an interface method parameter's Parent, so
                    // RepresentsModel throws on one; the parameters are written without it.
                    AddProxyParameters(template, parameters, representsModel: false, (type, name, configure) => method.AddParameter(type, name, configure));
                    async.AddCancellationTokenParameter(method, template);
                });
            }
        }

        /// <summary>
        /// The Operation's return type before any async wrapping (void when none is modelled), honouring this module's
        /// filter-targeted Collection Type.
        /// </summary>
        private static CSharpType GetProxyOperationReturnType(this ICSharpTemplate template, IElement operation)
        {
            if (operation.TypeReference?.Element == null)
            {
                return CSharpTypeParser.Parse("void");
            }

            var collectionType = operation.GetOptionalModelerCollectionType();
            return CSharpTypeParser.Parse(template.GetContractTypeName(operation.TypeReference, operation, collectionType?.Kind, collectionType?.CustomFormat));
        }

        /// <summary>
        /// Each Parameter by name and type, verbatim, with its Collection Type and its Default Value stereotype as the default
        /// (formatted like a service operation parameter's Value, see <see cref="ContractDefaultValues"/>). Proxy Parameters
        /// have no Parameter Modifier.
        /// </summary>
        private static void AddProxyParameters(ICSharpTemplate template, IEnumerable<IElement> parameters, bool representsModel, Action<string, string, Action<CSharpParameter>> addParameter)
        {
            foreach (var parameter in parameters)
            {
                var collectionType = parameter.GetOptionalModelerCollectionType();
                var typeName = template.GetContractTypeName(parameter.TypeReference, parameter, collectionType?.Kind, collectionType?.CustomFormat);
                addParameter(typeName, parameter.Name, param =>
                {
                    if (representsModel)
                    {
                        param.RepresentsModel(parameter);
                    }

                    param.WithContractDefaultValue(template, parameter, parameter.GetServiceProxyDefaultValue(), parameter.TypeReference, modifier: null);
                });
            }
        }
    }
}
