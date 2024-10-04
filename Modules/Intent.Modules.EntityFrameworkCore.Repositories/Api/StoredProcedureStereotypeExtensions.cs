using System;
using Intent.Metadata.Models;
using Intent.Modelers.Domain.Api;
using Intent.Modules.Common;

namespace Intent.Modules.EntityFrameworkCore.Repositories.Api
{
    /// <summary>
    /// This file was created manually since these stereotypes use a function to determine applicability.
    /// Stereotypes defined in the "Intent.Modelers.Domain.StoredProcedures" module.
    /// </summary>
    internal static class StoredProcedureStereotypeExtensions
    {
        public static bool TryGetStoredProcedure(this OperationModel operation, out StoredProcedureStereotype storedProcedureStereotype)
        {
            var stereotype = operation.GetStereotype(StoredProcedureStereotype.Id);
            if (stereotype == null)
            {
                storedProcedureStereotype = default;
                return false;
            }

            storedProcedureStereotype = new StoredProcedureStereotype(stereotype);
            return true;
        }

        public static bool TryGetStoredProcedureParameter(this ParameterModel parameter,
            out StoredProcedureParameterStereotype storedProcedureParameterStereotype)
        {
            var stereotype = parameter.GetStereotype(StoredProcedureParameterStereotype.Id);
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
        private readonly IStereotype _stereotype;

        public StoredProcedureStereotype(IStereotype stereotype)
        {
            _stereotype = stereotype;
        }

        public const string Id = "f40ff84c-68ad-405f-bda0-1237dd15fc92";

        public string GetName() => _stereotype.GetProperty("4e2a3f58-6b6e-43c5-9398-f9c3fde593f6").Value;
    }

    internal class StoredProcedureParameterStereotype
    {
        private readonly IStereotype _stereotype;

        public StoredProcedureParameterStereotype(IStereotype stereotype)
        {
            _stereotype = stereotype;
        }

        public const string Id = "6ac91fd5-206c-49da-b4a2-b6ea2cad11f7";

        public string GetName() => _stereotype.GetProperty("714a95a6-c3ef-4117-a66c-24876c675cd5").Value;
        public StoredProcedureParameterDirection GetDirection()
        {
            var value = _stereotype.GetProperty("39491728-8327-4b94-b9a2-9851dd4b4a01").Value;

            return value switch
            {
                "In" => StoredProcedureParameterDirection.In,
                "Out" => StoredProcedureParameterDirection.Out,
                "Both" => StoredProcedureParameterDirection.Both,
                _ => throw new Exception($"Unknown value: {value}")
            };
        }

        public string GetSqlStringType() => _stereotype.GetProperty<string>("8ba486d3-853c-42b8-acfb-bafb1e2cdb6e");
        public int? GetSize() => _stereotype.GetProperty<int?>("a2df34af-2fb9-49e3-ab6e-caff7a27bf99");
        public int? GetPrecision() => _stereotype.GetProperty<int?>("ed35ae5c-a708-457d-a22a-145b3b2f1148");
        public int? GetScale() => _stereotype.GetProperty<int?>("38d3c607-ac3b-41ea-86b2-b43fa81e101c");
    }

    internal enum StoredProcedureParameterDirection { In, Out, Both }
}
