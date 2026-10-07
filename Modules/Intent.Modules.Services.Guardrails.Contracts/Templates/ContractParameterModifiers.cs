using Intent.Modelers.Services.Api;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Services.Guardrails.Contracts.Api;

namespace Intent.Modules.Services.Guardrails.Contracts.Templates
{
    /// <summary>
    /// Applies the Parameter Modifier stereotype (ref, out, in, ref readonly, params). The builder supports ref, out and
    /// params, but its WithInParameterModifier() emits nothing, so in and ref readonly are written as a prefix of the
    /// parameter's type text instead.
    /// </summary>
    public static class ContractParameterModifiers
    {
        public static string GetModifier(this ParameterModel parameter)
        {
            if (!parameter.TryGetParameterModifier(out var stereotype))
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
        public static string WithModifierPrefix(this string type, string modifier)
        {
            return modifier != null && !IsSupportedByMethodBuilder(modifier)
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
