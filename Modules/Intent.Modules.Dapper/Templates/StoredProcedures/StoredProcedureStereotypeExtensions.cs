using System;
using Intent.Metadata.Models;
using Intent.Modelers.Domain.Api;
using Intent.Modules.Common;

namespace Intent.Modules.Dapper.Templates.StoredProcedures
{
    /// <summary>
    /// Written by hand since these stereotypes use a function to determine applicability and so no
    /// Api is code-generated for them. The stereotypes themselves are defined in the
    /// "Intent.Modules.Modelers.Domain.StoredProcedures" module.
    /// </summary>
    internal static class StoredProcedureStereotypeExtensions
    {
        public static bool TryGetStoredProcedure(this OperationModel operation, out StoredProcedureStereotype storedProcedureStereotype)
        {
            var stereotype = operation?.GetStereotype(StoredProcedureStereotype.Id);
            if (stereotype == null)
            {
                storedProcedureStereotype = default;
                return false;
            }

            storedProcedureStereotype = new StoredProcedureStereotype(stereotype);
            return true;
        }

        public static bool TryGetStoredProcedureParameter(this ParameterModel parameter, out StoredProcedureParameterStereotype storedProcedureParameterStereotype)
        {
            var stereotype = parameter?.GetStereotype(StoredProcedureParameterStereotype.Id);
            if (stereotype == null)
            {
                storedProcedureParameterStereotype = default;
                return false;
            }

            storedProcedureParameterStereotype = new StoredProcedureParameterStereotype(stereotype);
            return true;
        }
    }

    internal class StoredProcedureStereotype
    {
        public const string Id = "f40ff84c-68ad-405f-bda0-1237dd15fc92";

        private readonly IStereotype _stereotype;

        public StoredProcedureStereotype(IStereotype stereotype)
        {
            _stereotype = stereotype;
        }

        public string GetName() => _stereotype.GetProperty("4e2a3f58-6b6e-43c5-9398-f9c3fde593f6")?.Value;
    }

    internal class StoredProcedureParameterStereotype
    {
        public const string Id = "6ac91fd5-206c-49da-b4a2-b6ea2cad11f7";

        private readonly IStereotype _stereotype;

        public StoredProcedureParameterStereotype(IStereotype stereotype)
        {
            _stereotype = stereotype;
        }

        public string GetName() => _stereotype.GetProperty("714a95a6-c3ef-4117-a66c-24876c675cd5")?.Value;

        public StoredProcedureParameterDirection GetDirection()
        {
            var value = _stereotype.GetProperty("39491728-8327-4b94-b9a2-9851dd4b4a01")?.Value;

            return value switch
            {
                null or "" or "In" => StoredProcedureParameterDirection.In,
                "Out" => StoredProcedureParameterDirection.Out,
                "Both" => StoredProcedureParameterDirection.Both,
                _ => throw new Exception($"Unknown [Stored Procedure Parameter] direction value: {value}")
            };
        }
    }

    internal enum StoredProcedureParameterDirection
    {
        In,
        Out,
        Both
    }
}
