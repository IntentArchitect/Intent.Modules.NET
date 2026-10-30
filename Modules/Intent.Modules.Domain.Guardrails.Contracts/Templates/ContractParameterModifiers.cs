using Intent.Domain.Guardrails.Contracts.Api;
using Intent.Modelers.Domain.Api;
using Intent.Modules.Common.CSharp.Builder;
using CommonTypesParameterModel = Intent.Modules.Common.Types.Api.ParameterModel;

namespace Intent.Modules.Domain.Guardrails.Contracts.Templates
{
    /// <summary>
    /// Applies the Parameter Modifier stereotype (ref, out, in, ref readonly, params). The builder supports ref, out and
    /// params on method parameters, but its WithInParameterModifier() emits nothing and constructor parameters have no
    /// modifier API at all, so those cases write the modifier as a prefix of the parameter's type text instead.
    /// </summary>
    internal static class ContractParameterModifiers
    {
        // The stereotype accessor is generated against the Common.Types parameter model; both wrap the same designer element.
        public static string GetModifier(this ParameterModel parameter)
        {
            if (!new CommonTypesParameterModel(parameter.InternalElement).TryGetParameterModifier(out var stereotype))
            {
                return null;
            }

            var modifier = stereotype.Modifier();
            if (string.IsNullOrWhiteSpace(modifier.Value))
            {
                return null;
            }

            if (modifier.IsRef()) return "ref";
            if (modifier.IsOut()) return "out";
            if (modifier.IsIn()) return "in";
            if (modifier.IsRefReadonly()) return "ref readonly";
            if (modifier.IsParams()) return "params";
            throw new System.InvalidOperationException($"Unsupported Parameter Modifier option: {modifier.Value}");
        }

        /// <summary>
        /// The type text to pass to AddParameter: prefixed with the modifier when the builder can't emit it itself.
        /// </summary>
        public static string WithModifierPrefix(this string type, string modifier, bool isConstructorParameter)
        {
            return modifier != null && (isConstructorParameter || !IsSupportedByMethodBuilder(modifier))
                ? $"{modifier} {type}"
                : type;
        }

        public static void ApplyModifier(this CSharpParameter parameter, string modifier)
        {
            switch (modifier)
            {
                case "ref":
                    parameter.WithRefParameterModifier();
                    break;
                case "out":
                    parameter.WithOutParameterModifier();
                    break;
                case "params":
                    parameter.WithParamsParameterModifier();
                    break;
            }
        }

        private static bool IsSupportedByMethodBuilder(string modifier) => modifier is "ref" or "out" or "params";
    }
}
