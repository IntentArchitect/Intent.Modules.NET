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

        // The stereotype's property names and options, for a filter-targeted Async Configuration read by name. They must stay
        // the same as the typed stereotype's.
        private const string IncludeCancellationTokenProperty = "Include Cancellation Token";
        private const string AsyncModeProperty = "Async Mode";
        private const string TaskTypeProperty = "Task Type";
        private const string CancellationTokenNameProperty = "Cancellation Token Name";
        private const string AsyncModeAsync = "async";
        private const string AsyncModeNoAsyncKeyword = "no async keyword";
        private const string AsyncModeSync = "sync";
        private const string TaskTypeTask = "Task";
        private const string TaskTypeValueTask = "ValueTask";

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
            var settings = configuration == null
                ? null
                : new Settings(
                    configuration.AsyncMode().Value,
                    configuration.TaskType().Value,
                    configuration.IncludeCancellationToken(),
                    configuration.CancellationTokenName(),
                    operation.GetStereotype(OperationModelStereotypeExtensions.AsyncConfiguration.DefinitionId)?.GetProperty<string>(CancellationTokenDefaultProperty));
            return For(operation, settings, parameterNames);
        }

        /// <summary>
        /// The same for an operation whose Async Configuration is a filter-targeted stereotype with no typed accessor, read by
        /// property name. <paramref name="stereotype"/> is that stereotype, or null when it isn't applied. Its properties and
        /// options must be the same as the typed stereotype's.
        /// </summary>
        public static ContractAsync For(IElement operation, IStereotype stereotype, IEnumerable<string> parameterNames)
        {
            var settings = stereotype == null
                ? null
                : new Settings(
                    stereotype.GetProperty<string>(AsyncModeProperty),
                    stereotype.GetProperty<string>(TaskTypeProperty),
                    stereotype.GetProperty<bool>(IncludeCancellationTokenProperty),
                    stereotype.GetProperty<string>(CancellationTokenNameProperty),
                    stereotype.GetProperty<string>(CancellationTokenDefaultProperty));
            return For(operation, settings, parameterNames);
        }

        private static ContractAsync For(IElement operation, Settings settings, IEnumerable<string> parameterNames)
        {
            ContractAsync result;
            if (settings == null)
            {
                var isAsync = operation.Name.EndsWith("Async", StringComparison.Ordinal);
                result = isAsync
                    ? new ContractAsync(isAwaitable: true, hasAsyncKeyword: true, asValueTask: null, includeCancellationToken: true, DefaultCancellationTokenName, cancellationTokenHasDefault: true)
                    : Sync;
            }
            else
            {
                result = FromSettings(operation, settings);
            }

            if (result.IncludeCancellationToken && parameterNames.Contains(result.CancellationTokenName, StringComparer.Ordinal))
            {
                throw new ElementException(operation,
                    $"Operation '{operation.Name}' already has a parameter named '{result.CancellationTokenName}', the name of its CancellationToken parameter. " +
                    "Set Async Configuration Cancellation Token Name to the name the code declares, or untick Include Cancellation Token if the code has no token.");
            }

            return result;
        }

        private static ContractAsync FromSettings(IElement operation, Settings settings)
        {
            var mode = settings.AsyncMode;
            bool hasAsyncKeyword;
            // An empty Async Mode is a stereotype applied before the property replaced Is Async (user decision: no migration);
            // async, the property's default, keeps that output.
            if (string.IsNullOrWhiteSpace(mode) || mode == AsyncModeAsync)
            {
                hasAsyncKeyword = true;
            }
            else if (mode == AsyncModeNoAsyncKeyword)
            {
                hasAsyncKeyword = false;
            }
            else if (mode == AsyncModeSync)
            {
                return Sync;
            }
            else
            {
                throw new ElementException(operation,
                    $"Operation '{operation.Name}' has an Async Configuration Async Mode of '{mode}', which isn't valid. Choose async, no async keyword or sync.");
            }

            var taskType = settings.TaskType;
            bool? asValueTask;
            if (string.IsNullOrWhiteSpace(taskType))
            {
                asValueTask = null;
            }
            else if (taskType == TaskTypeTask || taskType == TaskTypeValueTask)
            {
                asValueTask = taskType == TaskTypeValueTask;
            }
            else
            {
                throw new ElementException(operation,
                    $"Operation '{operation.Name}' has an Async Configuration Task Type of '{taskType}', which isn't valid. Choose Task or ValueTask, or clear it.");
            }

            var configuredName = settings.CancellationTokenName;
            var tokenName = string.IsNullOrWhiteSpace(configuredName) ? DefaultCancellationTokenName : configuredName.Trim();
            if (!IsIdentifier(tokenName))
            {
                throw new ElementException(operation,
                    $"Operation '{operation.Name}' has an Async Configuration Cancellation Token Name of '{configuredName}', which isn't a valid C# parameter name. " +
                    "Use the name the code declares, for example ct, or clear it to use cancellationToken.");
            }

            var hasDefault = !string.Equals(settings.StoredCancellationTokenDefault?.Trim(), "false", StringComparison.OrdinalIgnoreCase);

            return new ContractAsync(isAwaitable: true, hasAsyncKeyword, asValueTask, settings.IncludeCancellationToken, tokenName, hasDefault);
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
            if (IncludeCancellationToken)
            {
                method.AddParameter(template.UseType("System.Threading.CancellationToken"), CancellationTokenName, ConfigureCancellationTokenParameter);
            }
        }

        /// <summary>The same for an interface method.</summary>
        public void AddCancellationTokenParameter(CSharpInterfaceMethod method, ICSharpTemplate template)
        {
            if (IncludeCancellationToken)
            {
                method.AddParameter(template.UseType("System.Threading.CancellationToken"), CancellationTokenName, ConfigureCancellationTokenParameter);
            }
        }

        /// <summary>
        /// The async modifier, for Async Mode async. Under Merge the existing method's modifiers are replaced with these, so it
        /// must be emitted to keep async/await implementations compiling, and left off a method that returns its task directly.
        /// Call it after the return type is set: Async(asValueTask) then leaves a Task or ValueTask return type as it is.
        /// An interface method has no body, so it never gets the keyword and has no overload here.
        /// </summary>
        public void ApplyAsyncKeyword(CSharpClassMethod method)
        {
            if (HasAsyncKeyword)
            {
                method.Async(asValueTask: method.ReturnTypeInfo.IsValueTask());
            }
        }

        private void ConfigureCancellationTokenParameter(CSharpParameter parameter)
        {
            if (CancellationTokenHasDefault)
            {
                parameter.WithDefaultValue("default");
            }
        }

        private static bool IsIdentifier(string value)
        {
            var identifier = value.StartsWith("@", StringComparison.Ordinal) ? value.Substring(1) : value;
            return identifier.Length > 0 &&
                (char.IsLetter(identifier[0]) || identifier[0] == '_') &&
                identifier.All(c => char.IsLetterOrDigit(c) || c == '_');
        }

        /// <summary>An Async Configuration's values, read through the typed accessor or by property name.</summary>
        private sealed record Settings(string AsyncMode, string TaskType, bool IncludeCancellationToken, string CancellationTokenName, string StoredCancellationTokenDefault);
    }
}
