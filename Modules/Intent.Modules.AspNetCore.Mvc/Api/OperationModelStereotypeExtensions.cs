using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modelers.Services.Api;
using Intent.Modules.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiElementModelExtensions", Version = "1.0")]

namespace Intent.AspNetCore.Mvc.Api
{
    public static class OperationModelStereotypeExtensions
    {
        public static MVCSettings GetMVCSettings(this OperationModel model)
        {
            var stereotype = model.GetStereotype(MVCSettings.DefinitionId);
            return stereotype != null ? new MVCSettings(stereotype) : null;
        }


        public static bool HasMVCSettings(this OperationModel model)
        {
            return model.HasStereotype(MVCSettings.DefinitionId);
        }

        public static bool TryGetMVCSettings(this OperationModel model, out MVCSettings stereotype)
        {
            if (!HasMVCSettings(model))
            {
                stereotype = null;
                return false;
            }

            stereotype = new MVCSettings(model.GetStereotype(MVCSettings.DefinitionId));
            return true;
        }

        public class MVCSettings
        {
            private IStereotype _stereotype;
            public const string DefinitionId = "647586e0-09df-444a-9dc4-f13637b0c3ac";

            public MVCSettings(IStereotype stereotype)
            {
                _stereotype = stereotype;
            }

            public string StereotypeName => _stereotype.Name;

            public string Name()
            {
                return _stereotype.GetProperty<string>("Name");
            }

            public ReturnTypeOptions ReturnType()
            {
                return new ReturnTypeOptions(_stereotype.GetProperty<string>("Return Type"));
            }

            public string Route()
            {
                return _stereotype.GetProperty<string>("Route");
            }

            public VerbOptions Verb()
            {
                return new VerbOptions(_stereotype.GetProperty<string>("Verb"));
            }

            public string ViewName()
            {
                return _stereotype.GetProperty<string>("View Name");
            }

            public IElement RedirectToController()
            {
                return _stereotype.GetProperty<IElement>("RedirectTo Controller");
            }

            public IElement RedirectToAction()
            {
                return _stereotype.GetProperty<IElement>("RedirectTo Action");
            }

            public string RedirectToRouteValues()
            {
                return _stereotype.GetProperty<string>("RedirectTo Route Values");
            }

            public string RedirectToFragment()
            {
                return _stereotype.GetProperty<string>("RedirectTo Fragment");
            }

            public class VerbOptions
            {
                public readonly string Value;

                public VerbOptions(string value)
                {
                    Value = value;
                }

                public VerbOptionsEnum AsEnum()
                {
                    switch (Value)
                    {
                        case "DELETE":
                            return VerbOptionsEnum.DELETE;
                        case "GET":
                            return VerbOptionsEnum.GET;
                        case "PATCH":
                            return VerbOptionsEnum.PATCH;
                        case "POST":
                            return VerbOptionsEnum.POST;
                        case "PUT":
                            return VerbOptionsEnum.PUT;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }

                public bool IsDELETE()
                {
                    return Value == "DELETE";
                }
                public bool IsGET()
                {
                    return Value == "GET";
                }
                public bool IsPATCH()
                {
                    return Value == "PATCH";
                }
                public bool IsPOST()
                {
                    return Value == "POST";
                }
                public bool IsPUT()
                {
                    return Value == "PUT";
                }
            }

            public enum VerbOptionsEnum
            {
                DELETE,
                GET,
                PATCH,
                POST,
                PUT
            }
            public class ReturnTypeOptions
            {
                public readonly string Value;

                public ReturnTypeOptions(string value)
                {
                    Value = value;
                }

                public ReturnTypeOptionsEnum AsEnum()
                {
                    switch (Value)
                    {
                        case "RedirectToAction":
                            return ReturnTypeOptionsEnum.RedirectToAction;
                        case "View":
                            return ReturnTypeOptionsEnum.View;
                        case "Ok":
                            return ReturnTypeOptionsEnum.Ok;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
                public bool IsRedirectToAction()
                {
                    return Value == "RedirectToAction";
                }
                public bool IsView()
                {
                    return Value == "View";
                }
                public bool IsOk()
                {
                    return Value == "Ok";
                }
            }

            public enum ReturnTypeOptionsEnum
            {
                RedirectToAction,
                View,
                Ok
            }
        }

    }
}