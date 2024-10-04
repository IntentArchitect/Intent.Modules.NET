using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiElementModelExtensions", Version = "1.0")]

namespace Intent.Modules.Hangfire.Api
{
    public static class HangfireConfigurationModelStereotypeExtensions
    {
        public static HangfireOptions GetHangfireOptions(this HangfireConfigurationModel model)
        {
            var stereotype = model.GetStereotype(HangfireOptions.DefinitionId);
            return stereotype != null ? new HangfireOptions(stereotype) : null;
        }


        public static bool HasHangfireOptions(this HangfireConfigurationModel model)
        {
            return model.HasStereotype(HangfireOptions.DefinitionId);
        }

        public static bool TryGetHangfireOptions(this HangfireConfigurationModel model, out HangfireOptions stereotype)
        {
            if (!HasHangfireOptions(model))
            {
                stereotype = null;
                return false;
            }

            stereotype = new HangfireOptions(model.GetStereotype(HangfireOptions.DefinitionId));
            return true;
        }

        public class HangfireOptions
        {
            private IStereotype _stereotype;
            public const string DefinitionId = "fb655689-92b4-49b5-838e-537c3768a2f9";

            public HangfireOptions(IStereotype stereotype)
            {
                _stereotype = stereotype;
            }

            public string Name => _stereotype.Name;

            public StorageOptions Storage()
            {
                return new StorageOptions(_stereotype.GetProperty<string>("Storage"));
            }

            public bool ShowDashboard()
            {
                return _stereotype.GetProperty<bool>("Show Dashboard");
            }

            public string DashboardURL()
            {
                return _stereotype.GetProperty<string>("Dashboard URL");
            }

            public string DashboardTitle()
            {
                return _stereotype.GetProperty<string>("Dashboard Title");
            }

            public bool ReadOnlyDashboard()
            {
                return _stereotype.GetProperty<bool>("Read Only Dashboard");
            }

            public bool ConfigureAsHangfireServer()
            {
                return _stereotype.GetProperty<bool>("Configure as Hangfire server");
            }

            public int? WorkerCount()
            {
                return _stereotype.GetProperty<int?>("Worker Count");
            }

            public int? JobRetentionHours()
            {
                return _stereotype.GetProperty<int?>("Job Retention Hours");
            }

            public class StorageOptions
            {
                public readonly string Value;

                public StorageOptions(string value)
                {
                    Value = value;
                }

                public StorageOptionsEnum AsEnum()
                {
                    switch (Value)
                    {
                        case "None":
                            return StorageOptionsEnum.None;
                        case "InMemory":
                            return StorageOptionsEnum.InMemory;
                        case "SQLServer":
                            return StorageOptionsEnum.SQLServer;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }

                public bool IsNone()
                {
                    return Value == "None";
                }
                public bool IsInMemory()
                {
                    return Value == "InMemory";
                }
                public bool IsSQLServer()
                {
                    return Value == "SQLServer";
                }
            }

            public enum StorageOptionsEnum
            {
                None,
                InMemory,
                SQLServer
            }

        }

    }
}