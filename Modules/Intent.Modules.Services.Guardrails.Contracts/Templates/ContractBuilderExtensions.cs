using System;
using System.Collections.Generic;
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
        /// Adds a <c>{ get; set; }</c> property (no initializer) for each field, name used verbatim.
        /// Must be called from within a builder callback so types resolve at build time.
        /// </summary>
        public static void AddContractProperties(this CSharpClass @class, ICSharpTemplate template, IEnumerable<DTOFieldModel> fields)
        {
            foreach (var field in fields)
            {
                @class.AddProperty(template.GetContractTypeName(field), field.Name, property =>
                {
                    property.RepresentsModel(field);
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
                var isAsync = operation.IsAsync();
                var returnType = CSharpTypeParser.Parse(operation.TypeReference?.Element != null
                    ? template.GetContractTypeName(operation)
                    : "void");

                if (isAsync && !returnType.IsTask() && !returnType.IsValueTask())
                {
                    returnType = returnType.WrapInTask(template);
                }

                @class.AddMethod(returnType, operation.Name, method =>
                {
                    method.RepresentsModel(operation);

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
                        });
                    }

                    if (isAsync && operation.IncludeCancellationToken())
                    {
                        method.AddParameter(template.UseType("System.Threading.CancellationToken"), "cancellationToken", param =>
                        {
                            param.WithDefaultValue("default");
                        });
                    }

                    // Under Mode.Merge the existing method's modifiers are replaced with these, so async must be emitted
                    // to keep async/await implementations compiling.
                    if (isAsync)
                    {
                        // Async() with no argument converts a ValueTask return type to Task; keep a modelled ValueTask as is.
                        method.Async(asValueTask: returnType.IsValueTask());
                    }
                });
            }
        }

        /// <summary>
        /// An explicit Async Configuration stereotype wins; otherwise an operation is async when its
        /// name ends with "Async" (the same rule as the Domain guardrail contracts).
        /// </summary>
        public static bool IsAsync(this OperationModel operation)
        {
            return operation.GetAsyncConfiguration()?.IsAsync() ?? operation.Name.EndsWith("Async", StringComparison.Ordinal);
        }

        /// <summary>
        /// Async operations get a CancellationToken parameter unless the Async Configuration stereotype turns it off.
        /// </summary>
        public static bool IncludeCancellationToken(this OperationModel operation)
        {
            return operation.GetAsyncConfiguration()?.IncludeCancellationToken() ?? true;
        }
    }
}
