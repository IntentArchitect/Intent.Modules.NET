using System;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.Modules.Common.Types.Api;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("ModuleBuilder.Templates.Api.ApiElementModelExtensions", Version = "1.0")]

namespace Intent.Modules.Application.Contracts.Api
{
    public static class FolderModelExtensions
    {
        public static DataContract GetDataContract(this FolderModel model)
        {
            var stereotype = model.GetStereotype("DataContract");
            return stereotype != null ? new DataContract(stereotype) : null;
        }

        public static bool HasDataContract(this FolderModel model)
        {
            return model.HasStereotype("DataContract");
        }


        public class DataContract
        {
            private IStereotype _stereotype;

            public DataContract(IStereotype stereotype)
            {
                _stereotype = stereotype;
            }

            public string Name => _stereotype.Name;

            public string Namespace()
            {
                return _stereotype.GetProperty<string>("Namespace");
            }

            public bool IsReference()
            {
                return _stereotype.GetProperty<bool>("IsReference");
            }

        }

    }
}