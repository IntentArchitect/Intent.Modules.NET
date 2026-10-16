using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Services.Guardrails.Contracts.Api;
using Intent.Exceptions;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;

namespace Intent.Modules.Services.Guardrails.Contracts.Templates
{
    /// <summary>
    /// An operation's async shape: its return type, the async keyword and the CancellationToken parameter. With the Async
    /// Configuration stereotype it comes from Async Mode, Task Type and the Cancellation Token properties. Without it, an
    /// operation whose name ends in Async is async: the async keyword, a Task return type and
    /// <c>CancellationToken cancellationToken = default</c>. Method signatures are fully managed under Merge, so all of it
    /// must match the code.
    /// Same rules as the Domain module's ContractAsync; keep the two in step.
    /// </summary>
    internal sealed class ContractAsync
    {
        private const string DefaultCancellationTokenName = "cancellationToken";

        // Read raw: a stereotype applied before this property existed has no stored value, which must keep "= default".
        private const string CancellationTokenDefaultProperty = "Cancellation Token Default";

        private static readonly ContractAsync Sync = new(isAwaitable: false, hasAsyncKeyword: false, asValueTask: null, includeCancellationToken: false, DefaultCancellationTokenName, cancellationTokenHasDefault: true);

        private ContractAsync(bool isAwaitable, bool hasAsyncKeyword, bool? asValueTask, bool includeCancellationToken, string cancellationTokenName, bool cancellationTokenHasDefault)
        {
            IsAwaitable = isAwaitable;
            HasAsyncKeyword = hasAsyncKeyword;
            AsValueTask = asValueTask;
            IncludeCancellationToken = includeCancellationToken;
            CancellationTokenName = cancellationTokenName;
            CancellationTokenHasDefault = cancellationTokenHasDefault;
        }

        /// <summary>A Task or ValueTask return type: Async Mode async or no async keyword.</summary>
        public bool IsAwaitable { get; }

        /// <summary>The async modifier: Async Mode async.</summary>
        public bool HasAsyncKeyword { get; }

        /// <summary>Task Type: true for ValueTask, false for Task, null to keep a modelled ValueTask and otherwise use Task.</summary>
        public bool? AsValueTask { get; }

        public bool IncludeCancellationToken { get; }

        public string CancellationTokenName { get; }

        public bool CancellationTokenHasDefault { get; }

        /// <summary>
        /// The shape for <paramref name="operation"/>. <paramref name="configuration"/> is its Async Configuration, or null when
        /// the stereotype isn't applied. <paramref name="parameterNames"/> are its modelled parameters, which the
        /// CancellationToken parameter's name mustn't repeat.
        /// </summary>
        public static ContractAsync For(IElement operation, OperationModelStereotypeExtensions.AsyncConfiguration configuration, IEnumerable<string> parameterNames)
        {
            ContractAsync result;
            if (configuration == null)
            {
                var isAsync = operation.Name.EndsWith("Async", StringComparison.Ordinal);
                result = isAsync
                    ? new ContractAsync(isAwaitable: true, hasAsyncKeyword: true, asValueTask: null, includeCancellationToken: true, DefaultCancellationTokenName, cancellationTokenHasDefault: true)
                    : Sync;
            }
            else
            {
                result = FromConfiguration(operation, configuration);
            }

            if (result.IncludeCancellationToken && parameterNames.Contains(result.CancellationTokenName, StringComparer.Ordinal))
            {
                throw new ElementException(operation,
                    $"Operation '{operation.Name}' already has a parameter named '{result.CancellationTokenName}', the name of its CancellationToken parameter. " +
                    "Set Async Configuration Cancellation Token Name to the name the code declares, or untick Include Cancellation Token if the code has no token.");
            }

            return result;
        }

        private static ContractAsync FromConfiguration(IElement operation, OperationModelStereotypeExtensions.AsyncConfiguration configuration)
        {
            var mode = configuration.AsyncMode();
            bool hasAsyncKeyword;
            // An empty Async Mode is a stereotype applied before the property replaced Is Async (user decision: no migration);
            // async, the property's default, keeps that output.
            if (string.IsNullOrWhiteSpace(mode.Value) || mode.IsAsync())
            {
                hasAsyncKeyword = true;
            }
            else if (mode.IsNoAsyncKeyword())
            {
                hasAsyncKeyword = false;
            }
            else if (mode.IsSync())
            {
                return Sync;
            }
            else
            {
                throw new ElementException(operation,
                    $"Operation '{operation.Name}' has an Async Configuration Async Mode of '{mode.Value}', which isn't valid. Choose async, no async keyword or sync.");
            }

            var taskType = configuration.TaskType();
            bool? asValueTask;
            if (string.IsNullOrWhiteSpace(taskType.Value))
            {
                asValueTask = null;
            }
            else if (taskType.IsTask() || taskType.IsValueTask())
            {
                asValueTask = taskType.IsValueTask();
            }
            else
            {
                throw new ElementException(operation,
                    $"Operation '{operation.Name}' has an Async Configuration Task Type of '{taskType.Value}', which isn't valid. Choose Task or ValueTask, or clear it.");
            }

            var configuredName = configuration.CancellationTokenName();
            var tokenName = string.IsNullOrWhiteSpace(configuredName) ? DefaultCancellationTokenName : configuredName.Trim();
            if (!IsIdentifier(tokenName))
            {
                throw new ElementException(operation,
                    $"Operation '{operation.Name}' has an Async Configuration Cancellation Token Name of '{configuredName}', which isn't a valid C# parameter name. " +
                    "Use the name the code declares, for example ct, or clear it to use cancellationToken.");
            }

            var storedDefault = operation.GetStereotype(OperationModelStereotypeExtensions.AsyncConfiguration.DefinitionId)?.GetProperty<string>(CancellationTokenDefaultProperty);
            var hasDefault = !string.Equals(storedDefault?.Trim(), "false", StringComparison.OrdinalIgnoreCase);

            return new ContractAsync(isAwaitable: true, hasAsyncKeyword, asValueTask, configuration.IncludeCancellationToken(), tokenName, hasDefault);
        }

        /// <summary>
        /// The method's return type: <paramref name="modelled"/> (void when none is modelled) for a sync operation, otherwise
        /// wrapped in Task or ValueTask. A modelled Task or ValueTask isn't wrapped again, and an explicit Task Type converts one
        /// to the other. A modelled IAsyncEnumerable (an async iterator) is left as it is.
        /// </summary>
        public CSharpType GetReturnType(CSharpType modelled, ICSharpTemplate template)
        {
            if (!IsAwaitable)
            {
                return modelled;
            }

            var isTask = (modelled is CSharpTypeName name && name.IsTask()) || (modelled is CSharpTypeGeneric generic && generic.IsTask());
            var isValueTask = modelled.IsValueTask();
            if (!isTask && !isValueTask && modelled.IsTask())
            {
                // CSharpType.IsTask() also matches IAsyncEnumerable<T>.
                return modelled;
            }

            if (AsValueTask ?? isValueTask)
            {
                if (isValueTask)
                {
                    return modelled;
                }

                if (isTask)
                {
                    var resultType = modelled.GetTaskGenericType();
                    return resultType == null ? CSharpType.CreateValueTask(template) : CSharpType.CreateValueTask(resultType, template);
                }

                return modelled.WrapInValueTask(template);
            }

            if (isTask)
            {
                return modelled;
            }

            if (isValueTask)
            {
                var resultType = modelled.GetValueTaskGenericType();
                return resultType == null ? CSharpType.CreateTask(template) : CSharpType.CreateTask(resultType, template);
            }

            return modelled.WrapInTask(template);
        }

        /// <summary>Appends the CancellationToken parameter, when the operation takes one.</summary>
        public void AddCancellationTokenParameter(CSharpClassMethod method, ICSharpTemplate template)
        {
            if (!IncludeCancellationToken)
            {
                return;
            }

            method.AddParameter(template.UseType("System.Threading.CancellationToken"), CancellationTokenName, param =>
            {
                if (CancellationTokenHasDefault)
                {
                    param.WithDefaultValue("default");
                }
            });
        }

        /// <summary>
        /// The async modifier, for Async Mode async. Under Merge the existing method's modifiers are replaced with these, so it
        /// must be emitted to keep async/await implementations compiling, and left off a method that returns its task directly.
        /// Call it after the return type is set: Async(asValueTask) then leaves a Task or ValueTask return type as it is.
        /// </summary>
        public void ApplyAsyncKeyword(CSharpClassMethod method)
        {
            if (HasAsyncKeyword)
            {
                method.Async(asValueTask: method.ReturnTypeInfo.IsValueTask());
            }
        }

        private static bool IsIdentifier(string value)
        {
            var identifier = value.StartsWith("@", StringComparison.Ordinal) ? value.Substring(1) : value;
            return identifier.Length > 0 &&
                (char.IsLetter(identifier[0]) || identifier[0] == '_') &&
                identifier.All(c => char.IsLetterOrDigit(c) || c == '_');
        }
    }
}
